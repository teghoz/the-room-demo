using System;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model;

namespace TheRoomSimpleAPI.UnitOfWork
{
    public partial class UnitOfWork : IDisposable
    {
        private TheRoomRepository<ServiceListItemPromo> serviceListItemPromoRepository;
        public TheRoomRepository<ServiceListItemPromo> ServiceListItemRepository
        {
            get
            {
                if (serviceListItemPromoRepository == null)
                {
                    serviceListItemPromoRepository = new TheRoomRepository<ServiceListItemPromo>(context);
                }
                return serviceListItemPromoRepository;
            }
        }
    }
}
