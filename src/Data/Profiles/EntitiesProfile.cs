using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data.Profiles
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>().ReverseMap();
        }
    }

    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            this.CreateMap<Location, LocationModel>()
                .ForMember(lm => lm.Venue, o => o.MapFrom(l => l.VenueName)).ReverseMap();
        }
    }
    public class TalkProfile : Profile
    {
        public TalkProfile()
        {
            this.CreateMap<Talk, TalkModel>().ReverseMap();
        }
    }

    public class SpeakerProfile : Profile
    {
        public SpeakerProfile()
        {
            this.CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
