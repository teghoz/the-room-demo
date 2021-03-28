using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model;
using TheRoomSimpleAPI.Model.Requests;
using TheRoomSimpleAPI.Model.Responses;
using TheRoomSimpleAPI.Models;

namespace TheRoomSimpleAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ServiceListController : ControllerBase
    {
        private readonly TheRoomContext _context;
        private readonly ApplicationSettings _settings;
        private readonly UnitOfWork.UnitOfWork _unitOfWork;
        public ServiceListController(TheRoomContext context, IOptionsSnapshot<ApplicationSettings> settings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            var connectionstring = context.Database.GetDbConnection().ConnectionString;
            _settings = settings.Value;
            _unitOfWork = new UnitOfWork.UnitOfWork(_context);
        }

        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> Get([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
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

            var totalItems = await _context.tblServiceListItems
                .LongCountAsync();

            var itemsOnPage = await _context.tblServiceListItems.Include(c => c.PromoCodes)
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Select(i => new ServiceListResponse
                {
                    PriceAfterDiscount = i.PriceAfterDiscount,
                    Description = i.Description,
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    Promos = i.PromoCodes.Select(p => new PromoResponse
                    {
                        DiscountPercentage = p.DiscountPercentage,
                        Code = p.Code,
                        Discount = p.Discount,
                        ExpiresOn = p.ExpiresOn,
                        Id = p.Id,
                        Name = p.Name,
                        UsePercentage = p.UsePercentage
                    }).ToList()
                }).ToListAsync();

            var model = new PaginatedItemsViewModel<ServiceListResponse>(pageIndex, pageSize, totalItems, itemsOnPage);

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

            var items = await _context.tblServiceListItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            return items;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<ActionResult<ServiceListItem>> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = _unitOfWork.ServiceListRepository.GetByID(id);

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        /// <summary>
        /// Get Service List Items
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        public async Task<ActionResult<PaginatedItemsViewModel<ServiceListItem>>> ItemsWithNameAsync(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _context.tblServiceListItems
                .Where(c => c.Name.Contains(name))
                .LongCountAsync();

            var itemsOnPage = await _context.tblServiceListItems
                .Where(c => c.Name.Contains(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedItemsViewModel<ServiceListItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        }

        /// <summary>
        /// Update a Service List Item
        /// </summary>
        /// <param name="serviceListItemToUpdate"></param>
        /// <returns></returns>
        [Route("items")]
        [HttpPut]
        public async Task<ActionResult> UpdateserviceListItemAsync([FromBody] ServiceListItem serviceListItemToUpdate)
        {
            var serviceListItems = await _unitOfWork.ServiceListRepository.Get(i => i.Id == serviceListItemToUpdate.Id);
            var serviceListItem = serviceListItems.FirstOrDefault();

            if (serviceListItem == null)
            {
                return NotFound(new { Message = $"Item with id {serviceListItemToUpdate.Id} not found." });
            }

            serviceListItem = serviceListItemToUpdate;
            _unitOfWork.ServiceListRepository.Update(serviceListItem);

            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetByIdAsync), new { id = serviceListItemToUpdate.Id }, null);
        }

        /// <summary>
        /// Create a Service List Item
        /// </summary>
        /// <param name="serviceListItem"></param>
        /// <returns></returns>
        [Route("items")]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateServiceListItemAsync([FromBody] ServiceListRequests model)
        {
            var item = new ServiceListItem
            {
                Description = model.Description,
                Name = model.Name,
                Price = model.Price
            };

            _unitOfWork.ServiceListRepository.Insert(item);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, null);
        }

        /// <summary>
        /// ServiceListItem Promos
        /// </summary>
        /// <param name="serviceListItem"></param>
        /// <returns></returns>
        [Route("items/{id:int}/promocode")]
        [HttpGet]
        public async Task<ActionResult> ServiceListItemAsync([FromRoute] int id, int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var item = await _unitOfWork.ServiceListRepository.Get(s => s.Id == id, includeProperties: "PromoCodes");
            if (item.FirstOrDefault().PromoCodes == null)
            {
                item.FirstOrDefault().PromoCodes = new List<ServiceListItemPromo>();
            }

            var itemsOnPage = item.FirstOrDefault().PromoCodes
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Select(p => new PromoResponse
                {
                    DiscountPercentage = p.DiscountPercentage,
                    Code = p.Code,
                    Discount = p.Discount,
                    ExpiresOn = p.ExpiresOn,
                    Id = p.Id,
                    Name = p.Name,
                    UsePercentage = p.UsePercentage
                }).ToList();

            var model = new PaginatedItemsViewModel<PromoResponse>(pageIndex, pageSize, item.FirstOrDefault().PromoCodes.Count(), itemsOnPage);
            return Ok(model);
        }


        /// <summary>
        /// Add Promocode to ServiceListItem
        /// </summary>
        /// <param name="serviceListItem"></param>
        /// <returns></returns>
        [Route("items/{id:int}/promocode")]
        [HttpPost]
        public async Task<ActionResult> AddAPromoCodeToServiceListItemAsync([FromRoute] int id, [FromBody] PromoRequests model)
        {
            var item = _unitOfWork.ServiceListRepository.GetByID(id);
            if(item.PromoCodes == null)
            {
                item.PromoCodes = new List<ServiceListItemPromo>();
            }
            item.PromoCodes.Add(new ServiceListItemPromo
            {
                Code = model.Promocode,
                ExpiresOn = DateTime.Now.AddDays(7),
                Discount = model.Discount,
                DiscountPercentage = model.DiscountPercentage,
                UsePercentage = model.UsePercentage
            });
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        /// <summary>
        /// Delete a Service List Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteserviceListItemAsync(int id)
        {
            var serviceListItem = _context.tblServiceListItems.SingleOrDefault(x => x.Id == id);

            if (serviceListItem == null)
            {
                return NotFound();
            }

            _context.tblServiceListItems.Remove(serviceListItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
