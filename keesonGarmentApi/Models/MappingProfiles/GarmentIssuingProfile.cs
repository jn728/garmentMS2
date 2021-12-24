using AutoMapper;
using keesonGarmentApi.Entities;

namespace keesonGarmentApi.Models.MappingProfiles
{
    public class GarmentIssuingProfile : Profile
    {
        public GarmentIssuingProfile()
        {
            CreateMap<AddGarmentIssuingModel, GarmentIssuing>();
            CreateMap<GarmentIssuing, GarmentIssuingListModel>();
        }
    }
}
