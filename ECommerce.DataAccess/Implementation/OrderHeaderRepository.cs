using ECommerce.DataAccess.Data;
using ECommerce.Entities.Models;
using ECommerce.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Implementation
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
            => _context.OrderHeaders.Update(orderHeader);

        public async Task UpdateStatus(int id, string? orderStatus, string? paymentStatus)
        {
            var orderFromDB = await _context.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);

            if(orderFromDB is not null)
            {
                orderFromDB.OrderStatus = orderStatus;
                orderFromDB.PaymentDate = DateTime.Now;

                if(paymentStatus is not null)
                    orderFromDB.PaymentStatus = paymentStatus;
            }
        }
    }
}
