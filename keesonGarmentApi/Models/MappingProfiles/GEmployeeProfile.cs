using AutoMapper;
using keesonGarmentApi.Entities;

namespace keesonGarmentApi.Models.MappingProfiles
{
    public class GEmployeeProfile : Profile
    {
        public GEmployeeProfile()
        {
            CreateMap<AddGEmployeeModel, GEmployee>();
            CreateMap<GEmployee, GEmployeeListModel>();
            CreateMap<AddGarmentModel, GarmentSize>();
        }
    }
}
