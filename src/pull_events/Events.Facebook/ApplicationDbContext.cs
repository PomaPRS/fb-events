using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Facebook
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<FacebookEvent> FacebookEvents { get; set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public static ApplicationDbContext Create()
        {
            var cs = @"Data Source=(LocalDb)\v11.0;Initial Catalog=aspnet-EventProject.Web.App-20150125080753;Integrated Security=True";
            return new ApplicationDbContext(cs);
        }
    }
}
