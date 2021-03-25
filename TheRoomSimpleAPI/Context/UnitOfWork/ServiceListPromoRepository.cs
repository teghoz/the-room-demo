using System;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model;

namespace TheRoomSimpleAPI.UnitOfWork
{
    public partial class UnitOfWork : IDisposable
    {
        private TheRoomRepository<ServiceListItemPromo> serviceListPromoRepository;
        public TheRoomRepository<ServiceListItemPromo> ServiceLisPromoRepository
        {
            get
            {
                if (serviceListPromoRepository == null)
                {
                    serviceListPromoRepository = new TheRoomRepository<ServiceListItemPromo>(context);
                }
                return serviceListPromoRepository;
            }
        }
    }
}
