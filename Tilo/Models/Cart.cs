using System.Collections.Generic;
using System.Linq;


namespace Tilo.Models
{
    public class Cart
    {
        private List<OrderLine> selections = new List<OrderLine>();

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
           
            OrderLine line = selections
                .Where(l => l.ProductId == p.Id && l.Product.Name == p.Name && l.Product.Sizes == p.Sizes).FirstOrDefault();
            if (line != null)
            {
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

        public Cart RemoveItem(long productId)
        {
            selections.RemoveAll(l => l.ProductId == productId);
            return this;
        }

        public void Clear() => selections.Clear();

        public IEnumerable<OrderLine> Selections { get => selections; }
    }
}
