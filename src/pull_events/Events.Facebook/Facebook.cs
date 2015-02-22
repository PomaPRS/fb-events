using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;

namespace Events.Facebook
{
    public static class Facebook
    {
        public static string GetAccessToken()
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = "",
                client_secret = "",
                grant_type = "client_credentials"
            });
            return result.access_token;
        }

        public static string ExtendAccessToken()
        {
            var fb = new FacebookClient();
            return fb.Get<string>("oauth/access_token", new
            {
                client_id = "",
                client_secret = "",
                grant_type = "fb_exchange_token",
                fb_exchange_token = "EXISTING_ACCESS_TOKEN"
            });
        }

        public static List<FacebookEvent> SearchEvents(string query)
        {
            if (String.IsNullOrEmpty(query))
                throw new ArgumentNullException();

            var events = new List<FacebookEvent>();
            var accessToken = GetAccessToken();
            var fb = new FacebookClient(accessToken);

            int offset = 0;
            bool empty = false;
            while (!empty)
            {
                var e = SearchEvents(fb, query, offset);
                events.AddRange(e);
                empty = e.Count == 0;
                offset += e.Count;
            }

            return events;
        }

        public static List<FacebookEvent> SearchEvents(FacebookClient fb, string query, int offset)
        {
            if (fb == null || String.IsNullOrEmpty(query))
                throw new ArgumentNullException();

            var link = "/search?q=" + query + "&type=event&offset=" + offset;
            dynamic result = fb.Get(link);
            var data = result.data as List<dynamic>;
            if (data == null)
                throw new Exception();
            return data.Select(e => GetEvent(fb, e.id as string)).ToList();
        }

        public static FacebookEvent GetEvent(FacebookClient fb, string id)
        {
            if (fb == null || id == null)
                throw new ArgumentNullException();

            dynamic result = fb.Get("/" + id);
            if (result == null)
                throw new Exception();

            ((IDictionary<string, object>)result)["picture"] = GetEventPictureLinks(fb, id);
            return ToEvent(result);
        }

        public static dynamic GetEventPictureLinks(FacebookClient fb, string id)
        {
            if (fb == null || id == null)
                throw new ArgumentNullException();

            dynamic answer = null;
            var types = new List<string> {"small", "normal", "large", "square"};
            var result = new Dictionary<string, object>();

            foreach (var type in types)
            {
                object value = null;
                dynamic ans = fb.Get("/" + id + "/picture?fields=url&redirect=false&type=" + type);
                if (ans != null && ans.data != null)
                    value = ans.data.url;
                result[type] = value;
            }
            return result;
        }

        public static FacebookEvent ToEvent(dynamic obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            DateTime? endTime = null;
            DateTime? startTime = null;
            DateTime? updatedTime = null;
            string ownerId = null;
            string ownerName = null;
            string parentGroupId = null;
            
            if (obj.end_time is string)
                endTime = DateTime.Parse((string) obj.end_time);

            if (obj.start_time is string)
                startTime = DateTime.Parse((string) obj.start_time);

            if (obj.updated_time is string)
                updatedTime = DateTime.Parse((string) obj.updated_time);

            if (obj.owner != null)
            {
                ownerId = obj.owner.id as string;
                ownerName = obj.owner.name as string;
            }

            if (obj.parent_group != null)
                parentGroupId = obj.parent_group.id as string;

            string small = null;
            string normal = null;
            string large = null;
            string square = null;

            var  picture = obj.picture as IDictionary<string, object>;
            if (picture != null)
            {
                small = picture["small"] as string;
                normal = picture["normal"] as string;
                large = picture["large"] as string;
                square = picture["square"] as string;
            }

            string country = null;
            string city = null;
            string zip = null;
            string state = null;
            string street = null;
            string venueId = null;
            double? latitude = null;
            double? longitude = null;

            dynamic venue = obj.venue;
            if (venue != null)
            {
                venueId = venue.id as string;
                country = venue.country as string;
                city = venue.city as string;
                zip = venue.zip as string;
                state = venue.state as string;
                street = venue.street as string;
                latitude = venue.latitude as double?;
                longitude = venue.longitude as double?;
            }

            return new FacebookEvent
            {
                Id = obj.id as string,
                SmallPictureLink = small,
                NormalPictureLink = normal,
                LargePictureLink = large,
                SquarePictureLink = square,
                Description = obj.description as string,
                EndTime = endTime,
                IsDateOnly = obj.is_date_only as bool?,
                Location = obj.location as string,
                Name = obj.name as string,
                OwnerId = ownerId,
                OwnerName = ownerName,
                ParentGroupId = parentGroupId,
                Privacy = obj.privacy as string,
                StartTime = startTime,
                TicketUri = obj.ticket_uri as string,
                Timezone = obj.timezone as string,
                UpdatedTime = updatedTime,

                Country = country,
                City = city,
                Latitude = latitude,
                Longitude = longitude,
                Zip = zip,
                State = state,
                Street = street,
                VenueId = venueId
            };
        }
    }
}
