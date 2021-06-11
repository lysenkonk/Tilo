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
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MailKit;
using MimeKit.Utils;
using Tilo.Models.ViewModels;

namespace Tilo.Controllers
{
    [ViewComponent(Name = "Cart")]
    public class CartController : Controller
    {
        private IProductRepository productRepository;
        private IOrdersRepository ordersRepository;
        private ITemplateHelper _templateHelper;
        private readonly IHostingEnvironment _appEnvironment;

        private const string SmallGalleryFolder2 = "/Files/Sm2/";



        public CartController(IProductRepository prepo, IOrdersRepository orepo, IHostingEnvironment appEnvironment, ITemplateHelper helper)
        {
            _templateHelper = helper;
            productRepository = prepo;
            ordersRepository = orepo;
            _appEnvironment = appEnvironment;

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
            }
            else if (product.Products != null)
            {
                Product item = productRepository.Products.FirstOrDefault(p => p.Id == product.Id);
                prodCurrent = product;
                prodCurrent.Images = item.Images;
            }
            else {
                prodCurrent = productRepository.Products.FirstOrDefault(p => p.Id == product.Id);
                prodCurrent.Sizes = null;
                prodCurrent.Sizes = new List<Size> { new Size(size) };
            }

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
        public IActionResult RemoveFromCart(Product product, string returnUrl)
        {
            // Product productCurrent = productRepository.Products.FirstOrDefault(p => p.Id == product.Id);

            if (product.Category.Name == "Подарочный сертификат")
            {
                SaveCart(GetCart().RemoveGiftCardItem(product.Id, product.Price));
            }

            if (product.Products != null && product.Products.Count > 0)
            {
                if (product.Products[0].Sizes != null)
                {
                    SaveCart(GetCart().RemoveSubItem(product));
                }
                return RedirectToAction(nameof(Index), new { returnUrl });

            }
            if (product.Sizes != null)
            {
                SaveCart(GetCart().RemoveItem(product.Id, product.Sizes));
            }

            return RedirectToAction(nameof(Index));
        }
        [Route("Cart/Clear")]
        public IActionResult Clear(string returnUrl)
        {
           SaveCart(GetCart().Clear());

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
                    //Product = productRepository.Products.FirstOrDefault(p => p.Id == s.ProductId)
                    Product = s.Product

                }).Where(p => p.ProductId != 0).ToArray();

                IEnumerable<OrderLine> ordersForMessage = GetCart().Selections.Select(s => new OrderLine
                {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity,
                    //Product = productRepository.Products.FirstOrDefault(p => p.Id == s.ProductId)
                    Product = s.Product

                }).Where(p => p.ProductId != 0).ToArray();

                ordersRepository.AddOrder(order);

                SaveCart(new Cart());
                await SendMessage(order, ordersForMessage);
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
        
        public async Task<IActionResult> SendMessage(Order order, IEnumerable<OrderLine> ordersForMessage)
        {

            long numberOrder = ordersRepository.Orders.Last<Order>().Id;
            //var headerImagePath = "";
            foreach (var currentOrder in order.Lines)
            {
                if(currentOrder.Product != null)
                {
                    Product item = productRepository.Products.FirstOrDefault(p => p.Id == currentOrder.Product.Id);
                    if(item.Images != null && item.Images.Count > 0)
                    {
                        currentOrder.Product.Images[0].Name = item.Images[0].Name;
                        //headerImagePath = string.Format("{0}/{1}", _appEnvironment.ContentRootPath, "wwwroot/Files/Sm2/tiloLogo.png");
                    }
                }
            }
            //var textMessage = infoAboutOrder(order, ordersForMessage);
            EmailService emailService = new EmailService();
            string subject = "Order №" + numberOrder + " is processed. With love your Tiloshowroom";
            try
            {
                var model = new MailViewModel();

                model.HeaderImage = new List<Models.ViewModels.LinkedResource>();
                //headerImagePath = string.Format("{0}/{1}", _appEnvironment.ContentRootPath, "wwwroot/Files/Sm2/tiloLogo.png");

                //model.HeaderImage.Add(new Models.ViewModels.LinkedResource
                //{
                //    ContentId = "BraLayla4.jpg",
                //    ContentPath = headerImagePath,
                //    ContentType = "image/png"
                //});
                foreach (var currentOrder in order.Lines)
                {
                    if (currentOrder.Product != null)
                    {
                        Product item = productRepository.Products.FirstOrDefault(p => p.Id == currentOrder.Product.Id);
                        if (item.Images != null && item.Images.Count > 0)
                        {
                            currentOrder.Product.Images[0].Name = item.Images[0].Name;
                            var path = string.Format("{0}/{1}", _appEnvironment.ContentRootPath, Url.Content("wwwroot/files/Sm2/" + item.Images[0].Name));
                            string name = currentOrder.Product.Images[0].Name.Trim();
                            model.HeaderImage.Add(new Models.ViewModels.LinkedResource
                            {

                                ContentId = currentOrder.Product.Id.ToString(),
                                ContentPath = path,
                                ContentType = "image/jpeg"
                            });
                           
                        }
                    }
                }
                order.Lines = ordersForMessage;
                var response = await _templateHelper.GetTemplateHtmlAsStringAsync<Order>("Orders/Content", order);

                model.Content = response;
               
                await emailService.SendEmailAsync(order.Email, subject, model);
                return RedirectToAction("Completed");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());            
            }
        }

        private string infoAboutOrder(Order order, IEnumerable<OrderLine> ordersForMessage)
        {

            string orderLinesJoinAll = "";
           
            int priceAllOrder = 0;
            string infoAboutProduct = "";
            var builder = new BodyBuilder();

            string orderHtml = String.Format(@"<p class='text-align:center; font-weight: bold; font-size: 22px'>TILOSHOWROOM <br> </p>
                                                <p class='text-align:center; font-align: center; font-size: 14px'>Шановний(-a) {0}, <br>
                                                   Ваше замовлення № {1}, наш менеджер передзвонить Вам найближчим часом.<br>
                                                   З подякою, <a href='tiloshowroom.com' target='_blank'>Tiloshowroom</a></p>", order.CustomerName, order.Id);
            foreach (var orderLine in ordersForMessage)
            {
                string sizesAndNames = "";
                try
                {
                    if (orderLine.Product.Sizes != null && orderLine.Product.Sizes.Count > 0)
                    {
                        foreach (var s in orderLine.Product.Sizes)
                        {
                            sizesAndNames += s.Name + "; ";
                        }
                    }
                    if (orderLine.Product.Products != null && orderLine.Product.Products.Count > 0)
                    {
                        foreach (var p in orderLine.Product.Products)
                        {
                            if (p.Sizes != null)
                            {
                                string name;
                                if (p.Name == "Bottom")
                                    name = "Трусики";
                                else if (p.Name == "Top")
                                    name = "Бра";
                                else name = p.Name;

                                

                                if(p.Price > 0)
                                {
                                    priceAllOrder += p.Price;
                                    sizesAndNames += name + ": " + p.Sizes[0].Name;
                                    sizesAndNames += " (+ " + p.Price + "грн)" + "; ";
                                }else sizesAndNames += name + ": " + p.Sizes[0].Name + "; ";
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }

                string size = "";
                if (orderLine.Product.Category.Name != "Подарочный сертификат")
                {
                    size = sizesAndNames;
                }
                //if(orderLine.Product.Images[0])
                //{ 
                string path = _appEnvironment.WebRootPath + SmallGalleryFolder2 + orderLine.Product.Images[0].Name;
                //LinkedResource res = new LinkedResource(path, "image/png"); 
                //res.ContentId = Guid.NewGuid().ToString();C:\Users\Nadiia\source\repos\Tilo\Tilo\wwwroot\Files\Sm2\1sertivicate1000.jpg
                //var image = builder.LinkedResources.Add(@"C:\Users\Nadiia\source\repos\Tilo\Tilo\wwwroot\Files\Sm2\1sertivicate1000.jpg");
                //image.ContentId = MimeUtils.GenerateMessageId();

                orderLinesJoinAll += orderLine.Product.Name   + " x " + orderLine.Quantity + " = " + orderLine.Quantity* orderLine.Product.Price + "грн; " + size + "\n";
                //builder.HtmlBody = String.Format(@"<div class='row'>                      
                //                                                    <div class='col-sm-4'>
                //                                                       <img src=""cid:{0}"">
                //                                                     </div>
                //                                                      <div class='col-sm-4'>
                //                                                       <h3>{0}</h3>
                //                                                        <p>Розмір: {1} </p>
                //                                                       <p>Кількість: {2}</p>
                //                                                        </div>
                //                                                      <div class='col-sm-2'>
                //                                                        <h3> {3} грн </h3>
                //                                                        </div>
                                                                     
                //                                </div>", image.ContentId, orderLine.Product.Name, size, orderLine.Quantity, orderLine.Quantity * orderLine.Product.Price);

                priceAllOrder += orderLine.Quantity * orderLine.Product.Price;
            }
            var priceOrder = String.Format(@"<div class='row'>                      
                                                                    <div class='col-sm-12'>
                                                                Всего к оплате: {0} грн
                                                                   </div> 
                                                </div>", priceAllOrder);

            string resultHtml = orderHtml + infoAboutProduct + priceOrder;
            //builder.HtmlBody = infoAboutProduct;
            return builder.HtmlBody;

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