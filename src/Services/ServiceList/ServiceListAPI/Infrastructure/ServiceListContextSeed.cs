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
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class ServiceListContextSeed
    {
        public async Task SeedAsync(ServiceListContext context, IWebHostEnvironment env, IOptions<ServiceListSettings> settings, ILogger<ServiceListContextSeed> logger)
        {
            context.Database.EnsureCreated();

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
                string[] requiredHeaders = { "name", "description",  "price" };
                csvheaders = GetHeaders(csvFileServiceListItems, requiredHeaders, null);
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

            return ServiceListItem;
        }
        private IEnumerable<ServiceListItem> GetPreconfiguredItems()
        {
            return new List<ServiceListItem>()
            {
                new ServiceListItem { Description = "Fox Sports", Name = "foxsport.com", Price = 19.5M },
                new ServiceListItem { Description = "Oracle things", Name = "oracle.io", Price= 8.50M },
                new ServiceListItem { Description = "Swagger things", Name = "swagger.io", Price = 12 },
                new ServiceListItem { Description = "Hangfire Sports", Name = "hangfire.com", Price = 19.5M },
                new ServiceListItem { Description = "Workkers things", Name = "worker.io", Price= 8.50M },
                new ServiceListItem { Description = "Test Host things", Name = "testhost.io", Price = 12 },
                new ServiceListItem { Description = "Fox Sports", Name = "foxsport.com", Price = 19.5M },
                new ServiceListItem { Description = "Java things", Name = "java.io", Price= 8.50M },
                new ServiceListItem { Description = "PHP things", Name = "php.com", Price = 12 },
                new ServiceListItem { Description = "Azure Service", Name = "azure.com", Price = 19.5M },
                new ServiceListItem { Description = "Kubernetes things", Name = "kubernetes.io", Price= 8.50M },
                new ServiceListItem { Description = "Razor things", Name = "razor.io", Price = 12 },
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
