using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Tilo.Models;
using Tilo.Infrastructure;
using Tilo.Services;
using System.Threading.Tasks;

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

        public IActionResult Index(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View(GetCart());
        }

        [HttpPost]
        public IActionResult AddToCart(Product product, string returnUrl)
        {
            Product prodCurrent = productRepository.Products.FirstOrDefault(p => p.Size == product.Size && p.Name == product.Name && p.Color == product.Color);

            SaveCart(GetCart().AddItem(prodCurrent, 1));
            return RedirectToAction(nameof(Index), new { returnUrl });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(long productId, string returnUrl)
        {
            SaveCart(GetCart().RemoveItem(productId));
            return RedirectToAction(nameof(Index), new { returnUrl });
        }

        public IActionResult CreateOrder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            order.Lines = GetCart().Selections.Select(s => new OrderLine
            {
                ProdId = s.ProdId,
                //Quantity = s.Quantity,
                //Product = s.Product,
                //Order = order
            }).ToArray();

            ordersRepository.AddOrder(order);
            SaveCart(new Cart());
            return RedirectToAction("Completed");
            //await SendMessage(order);
            //if (isSuccess)
            //{
            //    ordersRepository.AddOrder(order);
            //    SaveCart(new Cart());
            //    Completed();
            //}else
            //{
            //    NotCompleted();
            //}
        }

        public IActionResult AddAndSaveOrder(Order order)
        {
            
            SaveCart(new Cart());
            return RedirectToAction("Completed");
        }

        public IActionResult NotCompleted()
        {
            return View();
        }
        public IActionResult Completed()
        {
            return View();
        }

        //public async Task<IActionResult> SendMessage(Order order)
        //{
        //    var textMessage = "Deer " + order.CustomerName + ", your order №" + order.Id + " is processed. Our manager will contact you";
        //    EmailService emailService = new EmailService();
        //    string subject = "Order №" + order.Id + " is processed. With love your Tiloshowroom";
        //    try
        //    {
        //        await emailService.SendEmailAsync(order.Address, subject, textMessage);
        //        return AddAndSaveOrder(order);
        //    }
        //    catch
        //    {
        //        return NotCompleted();
        //    }
            
        //}

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