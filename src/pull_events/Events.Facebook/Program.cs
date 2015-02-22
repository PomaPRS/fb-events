using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Facebook;

namespace Events.Facebook
{
    class Program
    {
        private static ApplicationDbContext dbContext;

        static IEnumerable<string> ReadCities()
        {
            string fileName = "cities.txt";
            return File.ReadAllLines(fileName);
        }

        static IEnumerable<FacebookEvent> GetFacebookEvents(string query)
        {
            return Facebook.SearchEvents(query)
                    .Where(e => !dbContext.FacebookEvents.Any(s => s.Id == e.Id))
                    .ToList();
        }

        static void Main(string[] args)
        {
            try
            {
                dbContext = ApplicationDbContext.Create();
                var cities = ReadCities();

                foreach (var city in cities)
                {
                    var events = GetFacebookEvents(city);
                    dbContext.FacebookEvents.AddRange(events);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
