using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
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
    public class BonusController : ControllerBase
    {
        private readonly ApplicationSettings _settings;
        private readonly TheRoomContext _context;
        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public BonusController(TheRoomContext context, UnitOfWork.UnitOfWork unitOfWork, IOptionsSnapshot<ApplicationSettings> settings)
        {
            _settings = settings.Value;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Promos")]
        public async Task<ActionResult<PaginatedItemsViewModel<BonusResponse>>> Get([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Authentication).Value;
            var userActivePromos = await _unitOfWork.PromoUsersRepository.Get(p => p.UserId == userId, includeProperties: "Promo");
            var items = userActivePromos.Select(items => new BonusResponse
            {
                Promo = items.Promo.Code,
                Expiry = items.Promo.ExpiresOn
            });
            return new PaginatedItemsViewModel<BonusResponse>(pageIndex, pageSize, userActivePromos.Count(), items);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return Ok();
        }
        /// <summary>
        /// Active Promos
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("ActivePromos")]
        public async Task<ActionResult<PaginatedItemsViewModel<PromoUsers>>> ActivePromos(int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Authentication).Value;
            var promoUsers = await _unitOfWork.PromoUsersRepository.Get(p => p.UserId == userId);

            return new PaginatedItemsViewModel<PromoUsers>(pageIndex, pageSize, promoUsers.Count(), promoUsers.ToList());
        }

        /// <summary>
        /// Activate a Bonus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Activate")]
        public async Task<IActionResult> Post([FromBody] BonusRequests model)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Authentication).Value;
            var serviceListPromos = await _unitOfWork.ServiceLisPromoRepository.Get(promo => promo.Code == model.Promocode, includeProperties:"ActivatedUsers");
            var serviceListPromoItem = serviceListPromos.FirstOrDefault();

            if(serviceListPromos.Count() == 0)
            {
                return BadRequest("Invalid Promo Code");
            }

            if(serviceListPromoItem.ActivatedUsers.Any(user => user.UserId == userId))
            {
                return BadRequest("Promo already Activated");
            }
            else
            {
                var promoUsers = new PromoUsers
                {
                    UserId = userId
                };
                serviceListPromoItem.ActivatedUsers.Add(promoUsers);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
        }
    }
}
