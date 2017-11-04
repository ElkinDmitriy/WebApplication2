using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation
{
    public class BlogContext : DbContext
    {
        public DbSet<Data.User> Users { get; set; }
    }
}
