using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class Discount
    {
        public long Id { get; set; }
        public string DiscountType { get; set; }

        public long CarId { get; set; }
        public virtual Car Car { get; set; }
    }
}
