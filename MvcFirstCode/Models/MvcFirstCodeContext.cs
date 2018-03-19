using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcFirstCode.Models
{
    public class MvcFirstCodeContext : DbContext
    {
        public MvcFirstCodeContext() : base("name=MvcFirstCodeContext")
        {

        }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
    }
}