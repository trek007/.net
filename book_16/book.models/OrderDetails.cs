using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.models
{
   public class OrderDetails
    {
        public int Id { get; set; }
        public int OrderHederId { get; set; }
        [ForeignKey("OderHederId")]
        public OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
