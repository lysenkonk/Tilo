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
        public IActionResult AddToCart(Product product, string s, string returnUrl, int quantity)
        {
            Product prodCurrent;
            string size = product.Sizes != null ? product.Sizes[0].Name : null;

            if (product.Category.Name == "Подарочный сертификат")
            {
                prodCurrent = productRepository.Products.FirstOrDefault(p => p.Category.Name == product.Category.Name && p.Price == product.Price);
                //int price = product.Price;
            } else if(product.Products != null)
            {
                Product item = productRepository.Products.FirstOrDefault(p => p.Id == product.Id);
                prodCurrent = product;
                prodCurrent.Images = item.Images;
            }
            else prodCurrent = productRepository.Products.FirstOrDefault(p => p.Id == product.Id);

            if(product.Products != null)
            {
                SaveCart(GetCart().AddItem(product, "suit size ", quantity));
                foreach (var currentProduct in product.Products)
                {
                    if(currentProduct.Sizes != null)
                        {
                            SaveCart(GetCart().AddItem(currentProduct, currentProduct.Sizes[0].Name, quantity));
                        }
                }
            } else SaveCart(GetCart().AddItem(prodCurrent, size , quantity));

            return RedirectToAction(nameof(Index), new { returnUrl, size});
        }

        [HttpPost]
        [Route("Cart/RemoveFromCart")]
        public IActionResult RemoveFromCart(long productId, string returnUrl)
        {
           //Product product = productRepository.Products.FirstOrDefault(p => p.Id == productId);

           // if (product.Products != null && product.Products.Count > 0)
           // {
           //    IQueryable<Product> productsChild = productRepository.Products.Where(p => p.Id == productId);
           // }
            
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
                    Quantity = s.Quantity
                    //Product = productRepository.Products.FirstOrDefault(p => p.Id == s.ProductId);

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
                try
                {
                    if (orderLine.Product.Sizes != null && orderLine.Product.Sizes.Count > 0)
                    {
                        foreach (var s in orderLine.Product.Sizes)
                        {
                            sizes += s + "; ";
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }
                orderLinesJoinAll += orderLine.Product.Name + " " + "(" + sizes + ")" + "x" + orderLine.Quantity + "=" + orderLine.Quantity* orderLine.Product.Price + ";" + "\n"; 
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