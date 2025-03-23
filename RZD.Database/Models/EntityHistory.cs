using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class EntityHistory
    {
        public long Id { get; set; }

        public long EntityId { get; set; }
        public long EntityTypeId { get; set; }

        public DateTime ChangedAt { get; set; }
        public Dictionary<string, object> ChangedFields { get; set; }

        public virtual EntityType EntityType { get; set; }
    }
}
