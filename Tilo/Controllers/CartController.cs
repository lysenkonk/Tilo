using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Tilo.Models;
using Tilo.Infrastructure;
using Tilo.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Tilo.Controllers
{
    [ViewComponent(Name = "Cart")]
    public class CartController : Controller
    {
        private IProductRepository productRepository;
        private IOrdersRepository ordersRepository;

        public CartController(IProductRepository prepo, IOrdersRepository orepo)
        {
            productRepository = prepo;
            ordersRepository = orepo;
        }
        
        public IActionResult Index(string returnUrl, List<string> size)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.Size = size;
            return View(GetCart());
        }

        [HttpPost]
        [Route("Cart/AddToCart")]
        public IActionResult AddToCart(Product product, List<string> size, string returnUrl, int quantity)
        {
            Product prodCurrent;


            if (product.Category.Name == "Подарочный сертификат")
            {
                prodCurrent = productRepository.Products.FirstOrDefault(p => p.Category.Name == product.Category.Name && p.Price == product.Price);
                //int price = product.Price;
            } else prodCurrent = productRepository.Products.FirstOrDefault(p => p.Id == product.Id);


            SaveCart(GetCart().AddItem(prodCurrent,size, quantity));
            return RedirectToAction(nameof(Index), new { returnUrl, size});
        }

        [HttpPost]
        [Route("Cart/RemoveFromCart")]
        public IActionResult RemoveFromCart(long productId, string returnUrl)
        {
            SaveCart(GetCart().RemoveItem(productId));
            return RedirectToAction(nameof(Index), new { returnUrl });
        }
        [Route("Cart/CreateOrder")]
        public IActionResult CreateOrder()
        {
            return View();
        }

        [HttpPost]
        [Route("Cart/CreateOrder")]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            try
            {
                order.Lines = GetCart().Selections.Select(s => new OrderLine
                {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity,
                    Product = s.Product
                    
                }).ToArray();

                ordersRepository.AddOrder(order);

                SaveCart(new Cart());
                await SendMessage(order);
                return RedirectToAction("Completed");
            }
            catch (Exception ex)
            {
                return View("NotCompleted", ex.Message.ToString());
            }
        }
        [Route("Cart/NotCompleted")]
        public IActionResult NotCompleted(string ex)
        {
            return View("NotCompleted", ex);
        }
        [Route("Cart/Completed")]
        public IActionResult Completed()
        {
             return View();
        }
        
        public async Task<IActionResult> SendMessage(Order order)
        {
            long numberOrder = ordersRepository.Orders.Last<Order>().Id;
            var textMessage = "Здравствуйте, " + order.CustomerName + ",рады сообщить, что Ваш заказ №" + numberOrder + " будет обработан в ближайшее время!" + "\n"+ infoAboutOrder(order);
            EmailService emailService = new EmailService();
            string subject = "Order №" + numberOrder + " is processed. With love your Tiloshowroom";
            try
            {
                await emailService.SendEmailAsync(order.Email, subject, textMessage);
                return RedirectToAction("Completed");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());            
            }
        }


        private string infoAboutOrder(Order order)
        {

            string orderLinesJoinAll = "";
            int priceAllOrder = 0;
            

            foreach (var orderLine in order.Lines)
            {
                string sizes = "";
                if (orderLine.Product.Sizes != null || orderLine.Product.Sizes.Count > 0)
                {
                    foreach(var s in orderLine.Product.Sizes)
                    {
                        sizes += s + "; ";
                    }
                }
                orderLinesJoinAll += orderLine.Product.Name + "(" + sizes + ")" + "x" + orderLine.Quantity + "=" + orderLine.Quantity* orderLine.Product.Price + ";" + "\n"; 
                priceAllOrder += orderLine.Quantity * orderLine.Product.Price;
            }
            orderLinesJoinAll += "Всего к оплате: " + priceAllOrder + "; ";
            return orderLinesJoinAll;

        }

        private Cart GetCart() =>
            HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();

        private void SaveCart(Cart cart) =>
            HttpContext.Session.SetJson("Cart", cart);

        public IViewComponentResult Invoke(ISession session)
        {
            return new ViewViewComponentResult()
            {
                ViewData = new ViewDataDictionary<Cart>(ViewData,
                    session.GetJson<Cart>("Cart"))
            };
        }
    }
}