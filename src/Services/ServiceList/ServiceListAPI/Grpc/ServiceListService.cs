using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceListApi;
using ServiceListAPI.Infrastructure;
using ServiceListAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ServiceListApi.ServiceList;

namespace ServiceListAPI.Grpc
{
    public class ServiceListService : ServiceListBase
    {
        private readonly ServiceListContext _catalogContext;
        private readonly ServiceListSettings _settings;
        private readonly ILogger _logger;

        public ServiceListService(ServiceListContext dbContext, IOptions<ServiceListSettings> settings, ILogger<ServiceListService> logger)
        {
            _settings = settings.Value;
            _catalogContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }

        public override async Task<ServiceListItemResponse> GetItemById(ServiceListItemRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call CatalogService.GetItemById for product id {Id}", request.Id);
            if (request.Id <= 0)
            {
                context.Status = new Status(StatusCode.FailedPrecondition, $"Id must be > 0 (received {request.Id})");
                return null;
            }

            var item = await _catalogContext.ServiceListItems.SingleOrDefaultAsync(ci => ci.Id == request.Id);
            var baseUri = _settings.PicBaseUrl;
            var azureStorageEnabled = _settings.AzureStorageEnabled;

            if (item != null)
            {
                return new ServiceListItemResponse()
                {
                    Description = item.Description,
                    Id = item.Id,
                    Name = item.Name,
                    Price = (double)item.Price,
                };
            }

            context.Status = new Status(StatusCode.NotFound, $"Product with id {request.Id} do not exist");
            return null;
        }

        public override async Task<PaginatedItemsResponse> GetItemsByIds(ServiceListItemsRequest request, ServerCallContext context)
        {
            if (!string.IsNullOrEmpty(request.Ids))
            {
                var items = await GetItemsByIdsAsync(request.Ids);

                context.Status = !items.Any() ?
                    new Status(StatusCode.NotFound, $"ids value invalid. Must be comma-separated list of numbers") :
                    new Status(StatusCode.OK, string.Empty);

                return MapToResponse(items);
            }

            var totalItems = await _catalogContext.ServiceListItems
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.ServiceListItems
                .OrderBy(c => c.Name)
                .Skip(request.PageSize * request.PageIndex)
                .Take(request.PageSize)
                .ToListAsync();

            var model = MapToResponse(itemsOnPage, totalItems, request.PageIndex, request.PageSize);
            context.Status = new Status(StatusCode.OK, string.Empty);

            return model;
        }

        private PaginatedItemsResponse MapToResponse(List<ServiceListItem> items)
        {
            return MapToResponse(items, items.Count, 1, items.Count);
        }

        private PaginatedItemsResponse MapToResponse(List<ServiceListItem> items, long count, int pageIndex, int pageSize)
        {
            var result = new PaginatedItemsResponse()
            {
                Count = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            items.ForEach(i =>
            {
                result.Data.Add(new ServiceListItemResponse()
                {
                    Description = i.Description,
                    Id = i.Id,
                    Name = i.Name,
                    Price = (double)i.Price,
                });
            });

            return result;
        }


        private async Task<List<ServiceListItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.Ok))
            {
                return new List<ServiceListItem>();
            }

            var idsToSelect = numIds
                .Select(id => id.Value);

            var items = await _catalogContext.ServiceListItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            return items;
        }

        private List<ServiceListItem> ChangeUriPlaceholder(List<ServiceListItem> items)
        {
            var baseUri = _settings.PicBaseUrl;
            var azureStorageEnabled = _settings.AzureStorageEnabled;

            return items;
        }
    }
}
