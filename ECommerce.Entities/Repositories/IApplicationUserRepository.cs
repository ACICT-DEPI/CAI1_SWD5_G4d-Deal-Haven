using ECommerce.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Entities.Repositories
{
    public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
    {
        Task UpdateAsync(ApplicationUser user); // Define the asynchronous update method
    }
}
