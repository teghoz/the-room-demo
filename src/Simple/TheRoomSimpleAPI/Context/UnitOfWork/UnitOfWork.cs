using System;
using System.Threading.Tasks;
using TheRoomSimpleAPI.Context;

namespace TheRoomSimpleAPI.UnitOfWork
{
    public partial class UnitOfWork : IDisposable
    {
        private TheRoomContext context;
        public UnitOfWork()
        {

        }
        public UnitOfWork(TheRoomContext _context)
        {
            context = _context;
        }

        public TheRoomContext Context()
        {
            return context;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
