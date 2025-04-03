using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class Statistic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateFinish { get; set; }
    }
}
