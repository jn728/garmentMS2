using AutoMapper;
using keesonGarmentApi.Entities;

namespace keesonGarmentApi.Models.MappingProfiles
{
    public class GarmentAssignedLogProfile : Profile
    {
        public GarmentAssignedLogProfile()
        {
            CreateMap<AddGarmentAssignedLogBeforeCommitModel, GarmentAssignedLog>();
            CreateMap<GarmentAssignedLog, GarmentAssignedLogListModel>();
            CreateMap<AddSingleGarmentAssignedLogModel, AddGarmentAssignedLogBeforeCommitModel>();
            CreateMap<GEmployee, GarmentAllEmployeeLogListModel>();
        }
    }
}
