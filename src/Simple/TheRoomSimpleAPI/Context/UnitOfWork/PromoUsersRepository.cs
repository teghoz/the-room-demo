using System;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model;

namespace TheRoomSimpleAPI.UnitOfWork
{
    public partial class UnitOfWork : IDisposable
    {
        private TheRoomRepository<PromoUsers> promoUsersRepository;
        public TheRoomRepository<PromoUsers> PromoUsersRepository
        {
            get
            {
                if (promoUsersRepository == null)
                {
                    promoUsersRepository = new TheRoomRepository<PromoUsers>(context);
                }
                return promoUsersRepository;
            }
        }
    }
}
