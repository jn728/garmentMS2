using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services
{
    public class SelectorService : BaseService
    {
        public SelectorService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper) : base(garmentContext, cache, mapper)
        {
        }

        public async Task<ResultsModel<SelectorGarmentModel>> GetGarmentSelectorAsync()
        {
            var ret = new ResultsModel<SelectorGarmentModel>();

            var qeury = await GarmentContext.Garments.Select(g => new SelectorGarmentModel
            {
                Id = g.Code,
                Name = g.Name
            }).ToListAsync();

            ret.Data = qeury;
            ret.Code = HttpStatus.Success;
            ret.Message = "数据获取成功";
            return ret;
        }
    }
}
