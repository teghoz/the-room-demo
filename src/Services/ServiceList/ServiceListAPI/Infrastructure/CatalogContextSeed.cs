namespace ServiceListAPI.Infrastructure
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Model;
    using Polly;
    using Polly.Retry;
    using ServiceListAPI.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class ServiceListContextSeed
    {
        public async Task SeedAsync(ServiceListContext context, IWebHostEnvironment env, IOptions<ServiceListSettings> settings, ILogger<ServiceListContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ServiceListContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = settings.Value.UseCustomizationData;
                var contentRootPath = env.ContentRootPath;
                var picturePath = env.WebRootPath;

                if (!context.ServiceListItems.Any())
                {
                    await context.ServiceListItems.AddRangeAsync(useCustomizationData
                        ? GetServiceListItemsFromFile(contentRootPath, context, logger)
                        : GetPreconfiguredItems());

                    await context.SaveChangesAsync();

                    GetServiceListItemPictures(contentRootPath, picturePath);
                }
            });
        }

        private IEnumerable<ServiceListItem> GetServiceListItemsFromFile(string contentRootPath, ServiceListContext context, ILogger<ServiceListContextSeed> logger)
        {
            string csvFileServiceListItems = Path.Combine(contentRootPath, "Setup", "ServiceListItems.csv");

            if (!File.Exists(csvFileServiceListItems))
            {
                return GetPreconfiguredItems();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "catalogtypename", "catalogbrandname", "description", "name", "price", "picturefilename" };
                string[] optionalheaders = { "availablestock", "restockthreshold", "maxstockthreshold", "onreorder" };
                csvheaders = GetHeaders(csvFileServiceListItems, requiredHeaders, optionalheaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return GetPreconfiguredItems();
            }

            return File.ReadAllLines(csvFileServiceListItems)
                        .Skip(1) // skip header row
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry(column => CreateServiceListItem(column, csvheaders))
                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                        .Where(x => x != null);
        }

        private ServiceListItem CreateServiceListItem(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            string priceString = column[Array.IndexOf(headers, "price")].Trim('"').Trim();
            if (!Decimal.TryParse(priceString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Decimal price))
            {
                throw new Exception($"price={priceString}is not a valid decimal number");
            }

            var ServiceListItem = new ServiceListItem()
            {
                Description = column[Array.IndexOf(headers, "description")].Trim('"').Trim(),
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
                Price = price,
            };

            int availableStockIndex = Array.IndexOf(headers, "availablestock");
            if (availableStockIndex != -1)
            {
                string availableStockString = column[availableStockIndex].Trim('"').Trim();
                if (!String.IsNullOrEmpty(availableStockString))
                {
                }
            }

            int restockThresholdIndex = Array.IndexOf(headers, "restockthreshold");
            if (restockThresholdIndex != -1)
            {
                string restockThresholdString = column[restockThresholdIndex].Trim('"').Trim();
                if (!String.IsNullOrEmpty(restockThresholdString))
                {
                    if (int.TryParse(restockThresholdString, out int restockThreshold))
                    {
                    }
                    else
                    {
                        throw new Exception($"restockThreshold={restockThreshold} is not a valid integer");
                    }
                }
            }

            int maxStockThresholdIndex = Array.IndexOf(headers, "maxstockthreshold");
            if (maxStockThresholdIndex != -1)
            {
                string maxStockThresholdString = column[maxStockThresholdIndex].Trim('"').Trim();
                if (!String.IsNullOrEmpty(maxStockThresholdString))
                {
                    if (int.TryParse(maxStockThresholdString, out int maxStockThreshold))
                    {
                    }
                    else
                    {
                        throw new Exception($"maxStockThreshold={maxStockThreshold} is not a valid integer");
                    }
                }
            }

            int onReorderIndex = Array.IndexOf(headers, "onreorder");
            if (onReorderIndex != -1)
            {
                string onReorderString = column[onReorderIndex].Trim('"').Trim();
                if (!String.IsNullOrEmpty(onReorderString))
                {
                    if (bool.TryParse(onReorderString, out bool onReorder))
                    {
                    }
                    else
                    {
                        throw new Exception($"onReorder={onReorderString} is not a valid boolean");
                    }
                }
            }

            return ServiceListItem;
        }

        private IEnumerable<ServiceListItem> GetPreconfiguredItems()
        {
            return new List<ServiceListItem>()
            {
                new ServiceListItem { Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M },
                new ServiceListItem { Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M },
                new ServiceListItem { Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12 },
            };
        }

        private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }

            if (optionalHeaders != null)
            {
                if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
                {
                    throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
                }
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        private void GetServiceListItemPictures(string contentRootPath, string picturePath)
        {
            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileServiceListItemPictures = Path.Combine(contentRootPath, "Setup", "ServiceListItems.zip");
                ZipFile.ExtractToDirectory(zipFileServiceListItemPictures, picturePath);
            }
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<ServiceListContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
