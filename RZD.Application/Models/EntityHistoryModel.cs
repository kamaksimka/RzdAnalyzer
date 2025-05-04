using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Models
{
    public class EntityHistoryModel
    {
        public DateTimeOffset ChangedAt { get; set; }
        public string FieldName { get; set; }
        public string? OldFieldValue { get; set; }
    }
}
