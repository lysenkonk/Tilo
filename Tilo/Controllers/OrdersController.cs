﻿using Microsoft.AspNetCore.Mvc;
using Tilo.Models;
using System.Collections.Generic;
using System.Linq;


namespace Tilo.Controllers
{
    public class OrdersController : Controller
    {
        private IProductRepository productRepository;
        private IOrdersRepository ordersRepository;
        public OrdersController(IProductRepository productRepo, IOrdersRepository orderRepo)
        {
            productRepository = productRepo;
            ordersRepository = orderRepo;
        }


        public IActionResult Index()
        {
            return View(ordersRepository.Orders);
        }

        public IActionResult EditOrder(long id)
        {
            var products = productRepository.Products;
            Order order = id == 0 ? new Order() : ordersRepository.GetOrder(id);
            IDictionary<long, OrderLine> linesMap = order.Lines?.ToDictionary(l => l.ProdId) ?? new Dictionary<long, OrderLine>();
            ViewBag.Lines = products.Select(p => linesMap.ContainsKey(p.ID)
                            ? linesMap[p.ID]
                            : new OrderLine { Product = p, ProdId = p.ID, Quantity = 0 });
            return View(order);
        }
        [HttpPost]
        public IActionResult AddOrUpdateOrder(Order order)
        {
            order.Lines = order.Lines
                .Where(l => l.Id > 0 || (l.Id == 0 && l.Quantity > 0)).ToArray();
            if (order.Id == 0)
            {
                ordersRepository.AddOrder(order);
            }
            else
            {
                ordersRepository.UpdateOrder(order);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteOrder(Order order)
        {
            ordersRepository.DeleteOrder(order);
            return RedirectToAction(nameof(Index));
        }
    }
}