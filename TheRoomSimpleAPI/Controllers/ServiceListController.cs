using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model;
using TheRoomSimpleAPI.Models;

namespace TheRoomSimpleAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class ServiceListController : ControllerBase
    {
        private readonly TheRoomContext _theRoomContext;
        private readonly ApplicationSettings _settings;
        public ServiceListController(TheRoomContext context, IOptionsSnapshot<ApplicationSettings> settings)
        {
            _theRoomContext = context ?? throw new ArgumentNullException(nameof(context));
            _settings = settings.Value;

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<ServiceListItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<ServiceListItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);

                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-separated list of numbers");
                }

                return Ok(items);
            }

            var totalItems = await _theRoomContext.tblServiceListItems
                .LongCountAsync();

            var itemsOnPage = await _theRoomContext.tblServiceListItems
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginatedItemsViewModel<ServiceListItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
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

            var items = await _theRoomContext.tblServiceListItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            return items;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ServiceListItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ServiceListItem>> ItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = await _theRoomContext.tblServiceListItems.SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<ServiceListItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<ServiceListItem>>> ItemsWithNameAsync(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _theRoomContext.tblServiceListItems
                .Where(c => c.Name.Contains(name))
                .LongCountAsync();

            var itemsOnPage = await _theRoomContext.tblServiceListItems
                .Where(c => c.Name.Contains(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedItemsViewModel<ServiceListItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        }

        //PUT api/v1/[controller]/items
        [Route("items")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateserviceListItemAsync([FromBody] ServiceListItem serviceListItemToUpdate)
        {
            var ServiceListItem = await _theRoomContext.tblServiceListItems.SingleOrDefaultAsync(i => i.Id == serviceListItemToUpdate.Id);

            if (ServiceListItem == null)
            {
                return NotFound(new { Message = $"Item with id {serviceListItemToUpdate.Id} not found." });
            }

            var oldPrice = ServiceListItem.Price;
            var raiseserviceListItemPriceChangedEvent = oldPrice != serviceListItemToUpdate.Price;

            ServiceListItem = serviceListItemToUpdate;
            _theRoomContext.tblServiceListItems.Update(ServiceListItem);

            await _theRoomContext.SaveChangesAsync();
            return CreatedAtAction(nameof(ItemByIdAsync), new { id = serviceListItemToUpdate.Id }, null);
        }

        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateserviceListItemAsync([FromBody] ServiceListItem serviceListItem)
        {
            var item = new ServiceListItem
            {
                Description = serviceListItem.Description,
                Name = serviceListItem.Name,
                Price = serviceListItem.Price
            };

            _theRoomContext.tblServiceListItems.Add(item);
            await _theRoomContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
        }

        //DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteserviceListItemAsync(int id)
        {
            var serviceListItem = _theRoomContext.tblServiceListItems.SingleOrDefault(x => x.Id == id);

            if (serviceListItem == null)
            {
                return NotFound();
            }

            _theRoomContext.tblServiceListItems.Remove(serviceListItem);
            await _theRoomContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
