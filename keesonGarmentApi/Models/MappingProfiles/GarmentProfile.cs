using AutoMapper;
using keesonGarmentApi.Entities;

namespace keesonGarmentApi.Models.MappingProfiles
{
    public class GarmentProfile : Profile
    {
        public GarmentProfile()
        {
            CreateMap<AddGarmentModel, Garment>();
            CreateMap<Garment, GarmentListModel>().ForMember(dest => dest.Colors, opt => opt.Ignore());
        }
    }
}
