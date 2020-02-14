using System.Collections.Generic;
using System.Linq;


namespace Tilo.Models
{
    public class Cart
    {
        private List<OrderLine> selections = new List<OrderLine>();

        public Cart AddItem(Product p, int quantity)
        {
            OrderLine line = selections
                .Where(l => l.ProdId == p.ID).FirstOrDefault();
            if (line != null)
            {
                line.Quantity += quantity;
            }
            else
            {
                selections.Add(new OrderLine
                {
                    ProdId = p.ID,
                    Product = p,
                    Quantity = quantity
                });
            }
            return this;
        }

        public Cart RemoveItem(long productId)
        {
            selections.RemoveAll(l => l.ProdId == productId);
            return this;
        }

        public void Clear() => selections.Clear();

        public IEnumerable<OrderLine> Selections { get => selections; }
    }
}
