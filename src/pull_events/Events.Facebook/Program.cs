using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Facebook;

namespace Events.Facebook
{
    class ApplicationDbContext : DbContext
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

    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
                var dbContext = ApplicationDbContext.Create();

                var events = Facebook.SearchEvents("ижевск")
                    .Where(e => !dbContext.FacebookEvents.Any(s => s.Id == e.Id))
                    .ToList();

                //dbContext.FacebookEvents.AddRange(events);
                //dbContext.SaveChanges();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }
    }
}
