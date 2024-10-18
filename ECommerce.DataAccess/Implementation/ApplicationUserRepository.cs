using ECommerce.DataAccess.Data;
using ECommerce.Entities.Models;
using ECommerce.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Implementation
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.ApplicationUsers.Update(user); // Update the entity in the context
            await _context.SaveChangesAsync();      // Save changes asynchronously
        }
    }
}
