using System.Collections.Generic;
using System.Linq;


namespace Tilo.Models
{
    public class Cart
    {
        private List<OrderLine> selections = new List<OrderLine>();
        //public Cart()
        //{
        //    selections = new List<OrderLine>();
        //}
        

        public Cart AddItem(Product p, string size, int quantity)
        {
            //if(size != null)
            //{
            //    p.Sizes.Clear();
            //    foreach (var s in size)
            //    {
            //        p.Sizes.Add(new Size(s));
            //    }
            //}
            if(p.Products != null && p.Products.Count > 1)
            {
               List<OrderLine> linesCurrent = selections
               .Where(l => l.ProductId == p.Id && l.Product.Name == p.Name).ToList();
                if (linesCurrent != null)
                {
                    foreach (var lineCurrent in linesCurrent)
                    {
                        int count = 0;
                        foreach (var prod in p.Products)
                        {
                            // Product currentProd = lineCurrent.Product.Products.Where(subProd => subProd.Name == prod.Name && subProd.Sizes == prod.Sizes).FirstOrDefault();
                            foreach (var current in lineCurrent.Product.Products)
                            {
                                if (current.Name == prod.Name && current.Sizes != null && prod.Sizes != null && current.Sizes.Count == prod.Sizes.Count)
                                {
                                    for (int i = 0; i < current.Sizes.Count; i++)
                                    {
                                        if (prod.Sizes.Where(s => s.Name == current.Sizes[i].Name).FirstOrDefault() != null)
                                            count++;
                                    }
                                }
                            }

                            //if (currentProd != null)
                            //{
                            //    count++;
                            //}
                        }

                        if (count == p.Products.Count)
                        {
                            lineCurrent.Quantity += quantity;
                            return this;
                        }
                    }
                   
                }
            }


            OrderLine line = null;
            if (p.Category != null && p.Category.Name != "Подарочный сертификат")
            {
                line = selections
                .Where(l => l.ProductId == p.Id && l.Product.Name == p.Name && l.Product.Sizes != null && p.Sizes != null && l.Product.Sizes[0].Name == p.Sizes[0].Name).FirstOrDefault();
            }else if (p.Category != null && p.Category.Name == "Подарочный сертификат")
            {
                line = selections
                .Where(l => l.ProductId == p.Id && l.Product.Name == p.Name && l.Product.Price == p.Price).FirstOrDefault();

            }

            if (line != null && line.Product.Products == null)
            {
                line.Quantity += quantity;
            }else if (line != null && p.Category != null  && p.Category.Name == "Подарочный сертификат"){
                line.Quantity += quantity;
            }
            else
            {
                selections.Add(new OrderLine
                {
                    ProductId = p.Id,
                    Product = p,
                    Quantity = quantity
                });
            }
            return this;
        }

        public Cart RemoveItem(long productId, List<Size> sizes)
        {

            OrderLine lineForDelete = selections
               .Where(l => l.ProductId == productId && l.Product.Sizes[0] == sizes[0]).FirstOrDefault();
            if (lineForDelete != null)
            {
                foreach (var productForDelete in lineForDelete.Product.Products)
                {
                    selections.RemoveAll(l => l.Product.Name == productForDelete.Name && l.Product.Price == productForDelete.Price);

                }
            }

            selections.RemoveAll(l => l.ProductId == productId && l.Product.Sizes[0].Name == sizes[0].Name);

            return this;
        }

        public Cart RemoveGiftCardItem(long productId, int  price)
        {

            //OrderLine lineForDelete = selections
            //   .Where(l => l.ProductId == productId && l.Product.Price == price).FirstOrDefault();
            //if (lineForDelete != null)
            //{
            //    foreach (var productForDelete in lineForDelete.Product.Products)
            //    {
            //        selections.RemoveAll(l => l.Product.Name == productForDelete.Name && l.Product.Price == productForDelete.Price);

            //    }
            //}

            selections.RemoveAll(l => l.ProductId == productId && l.Product.Price == price);

            return this;
        }



        public Cart RemoveSubItem(Product product)
        {

            List <OrderLine>linesForDelete = selections
               .Where(l => l.ProductId == product.Id).ToList();
            foreach(var line in linesForDelete)
            {
                if (line.Product.Products != null && product.Products != null && line.Product.Products.Count == product.Products.Count)
                {
                    int count = 0;
                    foreach (var prod in product.Products)
                    {

                        if (line.Product.Products.Where(l => l.Sizes[0].Name == prod.Sizes[0].Name) != null)
                            count++;
                    }

                    if (count == product.Products.Count)
                    {
                        selections.Remove(line);

                        return this;
                    }

                }
            }
            return this;
        }
        public Cart Clear() { selections.Clear(); return this; } 

        public IEnumerable<OrderLine> Selections { get => selections; }
    }
}
