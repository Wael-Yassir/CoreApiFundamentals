using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength (20)]
        public string Moniker { get; set; }

        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public LocationModel Location { get; set; }

        [Range(1, 25)]
        public int Length { get; set; } = 1;
        public ICollection<TalkModel> Talks { get; set; }
    }
}
