using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using EventProject.Data;
using EventProject.Entity;
using Facebook;


namespace Events.Facebook
{
    class Program
    {
        private static EventProject.Data.ApplicationDbContext dbContext;

        static IEnumerable<string> ReadCities()
        {
            string fileName = "cities.txt";
            return File.ReadAllLines(fileName);
        }

        static IEnumerable<FacebookEvent> GetFacebookEvents(string query)
        {
            return Facebook.SearchEvents(query)
                    .Where(e => !dbContext.FacebookEvents.Any(s => s.ExternalFacebookId == e.Id))
                    .ToList();
        }

        static void Main(string[] args)
        {
            try
            {
                dbContext = EventProject.Data.ApplicationDbContext.Create();
                ////var cities = ReadCities();

                var city = "Ижевск";

                var events = GetFacebookEvents(city)
                    .Select(e =>
                    {
                        CityVenue cityVenue = new CityVenue()
                        {
                            City = e.City ?? city,
                            //Country = e.Country
                        };

                        dbContext.CityVenues.AddOrUpdate(p => p.City, cityVenue);
                        dbContext.SaveChanges();

                        Venue venue = null;

                        if (e.VenueId != null)
                        {
                            venue = new Venue()
                            {
                                CityVenueId =  cityVenue.Id,
                                FacebookId = e.VenueId,
                                Latitude = e.Latitude,
                                Longitude = e.Longitude,
                                Street = e.Street
                            };

                            dbContext.Venues.AddOrUpdate(p => p.FacebookId, venue);
                            dbContext.SaveChanges();
                        }

                        var fbUser = new FacebookUser()
                        {
                            ExternalFacebookId = e.OwnerId,
                            FirstName = e.OwnerName
                        };

                        dbContext.FacebookUsers.AddOrUpdate(p => p.ExternalFacebookId, fbUser);
                        dbContext.SaveChanges();

                        return new EventProject.Entity.FacebookEvent()
                        {
                            CityVenueId = cityVenue.Id,
                            DateTimeBegin = e.StartTime,
                            DateTimeEnd = e.EndTime,
                            CategoryId = 1L,
                            Description = e.Description,
                            ExternalFacebookId = e.Id,
                            ExternalFacebookOwnerId = e.OwnerId,
                            Name = e.Name,
                            OwnerId = fbUser.Id,
                            VenueId = venue == null ? (long?) null : venue.Id
                        };

                    });


                dbContext.FacebookEvents.AddRange(events);
                dbContext.SaveChanges();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
