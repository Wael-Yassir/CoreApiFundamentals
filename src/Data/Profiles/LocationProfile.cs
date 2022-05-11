using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data.Profiles
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            this.CreateMap<Location, LocationModel>()
                .ForMember(lm => lm.Venue, o => o.MapFrom(l => l.VenueName));
        }
    }
}
