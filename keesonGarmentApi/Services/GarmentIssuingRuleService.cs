using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Models;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace keesonGarmentApi.Services
{
    public class GarmentIssuingRuleService : BaseService
    {
        public GarmentIssuingRuleService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper) : base(garmentContext, cache, mapper)
        {
        }

        public async Task<ResultsModel<GarmentIssuingRuleListModel>> GetGarmentIssuingRulesAsync()
        {
            var ret = new ResultsModel<GarmentIssuingRuleListModel>();
            var query = from val in GarmentContext.GarmentsIssuingRules
                        select val;

            ret.Data = Mapper.Map<List<GarmentIssuingRuleListModel>>(query);
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功";

            return ret;
        }

        public async Task<ResultViewModel> AddGarmentIssuingRuleAsync(AddGarmentIssuingRuleModel model)
        {
            var ret = new ResultViewModel();
           
            var rule = from gir in GarmentContext.GarmentsIssuingRules
                    where gir.GarmentId == model.GarmentId && gir.DepartmentId == model.DepartmentId && gir.PositionId == model.PositionId
                    select gir.Id;
            
            if (rule.Any())
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "该工衣发放标准已存在";
                return ret;
            }

            var garmentIssuingRule = Mapper.Map<GarmentIssuingRule>(model);
            garmentIssuingRule.CreateUser = UserId;

            await GarmentContext.GarmentsIssuingRules.AddAsync(garmentIssuingRule);
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "工衣发放标准添加成功";

            return ret;
        }

        public async Task<ResultViewModel> ImportRules(IFormFile file)
        {
            var ret = new ResultViewModel();
            var importer = new ExcelImporter();
            try
            {
                var importRet = await importer.Import<IssuingRuleImportModel>(file.OpenReadStream());
                var list = importRet.Data;
                var addList = list.Select(l => new GarmentIssuingRule
                {
                    GarmentId = l.Gid,
                    DepartmentId = l.Dep,
                    PositionId = l.Pos,
                    NewEmpAssignedYear = l.Year1,
                    NewEmpAssignedNumber = l.Num1,
                    EmpAssignedYear = l.Year2,
                    EmpAssignedNumber = l.Num2,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId
                }).ToList();

               
                await GarmentContext.GarmentsIssuingRules.AddRangeAsync(addList);
                await GarmentContext.SaveChangesAsync();
                ret.Code = HttpStatus.Success;
                ret.Message = "导入数据成功.";
            }
            catch (Exception e)
            {
                ret.Code = HttpStatus.ServerError;
                ret.Message = $"导入数据失败,原因:{e.Message}";
            }
            return ret;
        }
    }
}
