using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Entities.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> CompleteAsync();
    }
}
