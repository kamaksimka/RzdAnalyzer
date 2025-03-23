using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Database.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? LastSingInDate { get; set; }

        public virtual List<Role> Roles { get; set; }

    }
}
