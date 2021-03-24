using System;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model;

namespace TheRoomSimpleAPI.UnitOfWork
{
    public partial class UnitOfWork : IDisposable
    {
        private TheRoomRepository<ServiceListItem> serviceListRepository;
        public TheRoomRepository<ServiceListItem> ServiceListRepository
        {
            get
            {
                if (serviceListRepository == null)
                {
                    serviceListRepository = new TheRoomRepository<ServiceListItem>(context);
                }
                return serviceListRepository;
            }
        }
    }
}
