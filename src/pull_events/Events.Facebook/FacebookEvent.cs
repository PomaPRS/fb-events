using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Facebook
{
    public class FacebookEvent
    {
        public string Id { get; set; }
        public string SmallPictureLink { get; set; }
        public string NormalPictureLink { get; set; }
        public string LargePictureLink { get; set; }
        public string SquarePictureLink { get; set; }
        public string Description { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsDateOnly { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string ParentGroupId { get; set; }
        public string Privacy { get; set; }
        public DateTime StartTime { get; set; }
        public string TicketUri { get; set; }
        public string Timezone { get; set; }
        public DateTime? UpdatedTime { get; set; }

        // Location
        public string Country { get; set; }
        public string City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string VenueId { get; set; }
    }
}
