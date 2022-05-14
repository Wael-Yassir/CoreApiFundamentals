using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data.Profiles
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ReverseMap();

            this.CreateMap<Location, LocationModel>()
                .ForMember(lm => lm.Venue, opt => opt.MapFrom(l => l.VenueName))
                .ReverseMap();

            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                // when mapping form talkmodel to talk camp object is not define on talkmodel so,
                // don't overrite it on talk and don't make it null (the same for speaker)
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());

            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
        }
    }
}
