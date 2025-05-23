﻿namespace RZD.Database.Models
{
    public class Role
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public virtual List<User> Users { get; set; }

    }
}
