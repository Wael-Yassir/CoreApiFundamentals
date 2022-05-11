using System;
using System.Collections.Generic;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public LocationModel Location { get; set; }
        public int Length { get; set; } = 1;
        public ICollection<TalkModel> Talks { get; set; }
    }
}
