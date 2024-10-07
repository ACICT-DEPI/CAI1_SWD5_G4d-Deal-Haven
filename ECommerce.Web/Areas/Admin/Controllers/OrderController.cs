﻿using AutoMapper;
using ECommerce.Entities.Models;
using ECommerce.Entities.Repositories;
using ECommerce.Entities.ViewModels;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ECommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitofwork = unitOfWork;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = await _unitofwork.OrderHeader.GetAllAsync(Includeword: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }

        public async Task<IActionResult> Details(int orderId)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = await _unitofwork.OrderHeader.GetFirstorDefaultAsync(u => u.Id == orderId, Includeword: "ApplicationUser"),
                OrderDetail = await _unitofwork.OrderDetail.GetAllAsync(x => x.OrderHeaderId == orderId, Includeword: "Product")
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderFromDb = await _unitofwork.OrderHeader.GetFirstorDefaultAsync(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderFromDb is not null)
            {
                orderFromDb = _mapper.Map<OrderHeader>(OrderVM.OrderHeader);
                //orderFromDb.Name = OrderVM.OrderHeader.Name;
                //orderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
                //orderFromDb.Address = OrderVM.OrderHeader.Address;
                //orderFromDb.City = OrderVM.OrderHeader.City;
            }

            if (OrderVM.OrderHeader.Carrier is not null)
            {
                orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if (OrderVM.OrderHeader.TrackingNumber is not null)
            {
                orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitofwork.OrderHeader.Update(orderFromDb);
            await _unitofwork.CompleteAsync();
            TempData["Update"] = "Item has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProccess()
        {
            await _unitofwork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
            await _unitofwork.CompleteAsync();

            TempData["Update"] = "Order Status has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShip()
        {
            var orderFromDb = await _unitofwork.OrderHeader.GetFirstorDefaultAsync(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderFromDb is not null)
            {
                orderFromDb = _mapper.Map<OrderHeader>(OrderVM.OrderHeader);
                //orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
                //orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
                orderFromDb.OrderStatus = SD.Shipped;
                orderFromDb.ShippingDate = DateTime.Now;
            }
            _unitofwork.OrderHeader.Update(orderFromDb);
            await _unitofwork.CompleteAsync();

            TempData["Update"] = "Order has Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var orderfromdb = await _unitofwork.OrderHeader.GetFirstorDefaultAsync(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderfromdb.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderfromdb.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(option);

                await _unitofwork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                await _unitofwork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
            }
            await _unitofwork.CompleteAsync();

            TempData["Update"] = "Order has Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }
    }
}