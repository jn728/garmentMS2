using AutoMapper;
using keesonGarmentApi.Entities;

namespace keesonGarmentApi.Models.MappingProfiles
{
    public class GarmentIssuingRuleProfile : Profile
    {
        public GarmentIssuingRuleProfile()
        {
            CreateMap<AddGarmentIssuingRuleModel, GarmentIssuingRule>();
            CreateMap<GarmentIssuingRule, GarmentIssuingRuleListModel>();
        }
    }
}
