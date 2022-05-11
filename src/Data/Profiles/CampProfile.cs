using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data.Profiles
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>();
        }
    }
}
