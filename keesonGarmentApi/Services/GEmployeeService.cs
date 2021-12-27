using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Models;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services
{
    public class GEmployeeService : BaseService
    {
        public  GEmployeeService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper) : base(garmentContext, cache, mapper)
        {
        }

        public async Task<PageViewModel<GEmployeeListModel>> GetGEmployeeAsync(int pageIndex, int pageSize, string code, string name, string department, string postion, bool isDelete)
        {
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 20 : pageSize;

            var ret = new PageViewModel<GEmployeeListModel>();

            var roles = GetRoleNames();
            string dep = "";
            if (roles.Contains("部门文员"))
            {
                dep = (from ge in GarmentContext.GEmployees
                       where ge.Code == UserId
                       select ge.Department).First();
            }
            else if (roles.Contains("人事文员") || roles.Contains("行政文员"))
            {
                dep = "ALL";
            }

            var query = from ge in GarmentContext.GEmployees
                        join gs in GarmentContext.GarmentsSizes
                        on ge.Code equals gs.UserCode
                        where ge.IsDeleted == isDelete && ( ge.Department == dep || dep == "ALL" )
                        select new GEmployeeListModel
                        {
                            Code = ge.Code,
                            Name = ge.Name,
                            Department = ge.Department,
                            Postion = ge.Postion,
                            Induction = ge.Induction,
                            ClothesSize = gs.ClothesSize,
                            ShoesSize = gs.ShoesSize
                        };

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(x => x.Code == code);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name == name);
            }
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(x => x.Department == department);
            }
            if (!string.IsNullOrEmpty(postion))
            {
                query = query.Where(x => x.Postion == postion);
            }

            var count = await query.CountAsync();
            var list = await query.OrderBy(x => x.Code).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            ret.Data = Mapper.Map<List<GEmployeeListModel>>(list);
            ret.CurrentPage = pageIndex;
            ret.PageSize = pageSize;
            ret.TotalCount = count;
            ret.TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功.";

            return ret;
        }

        public async Task<ResultViewModel> AddGEmployeeAsync(AddGEmployeeModel model)
        {
            var ret = new ResultViewModel();
            
            var code = await addEmployeeAsync(model);
            if (code != null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "员工添加失败";
                return ret;
            }

            ret.Code = HttpStatus.Success;
            ret.Message = "员工添加成功";

            return ret;
        }

        public async Task<ResultsModel<string>> AddGEmployeeListAsync(List<AddGEmployeeModel> models)
        {
            var ret = new ResultsModel<string>();
            var retList = new List<string>();

            foreach (var model in models)
            {
                var code = await addEmployeeAsync(model);
                if (code != null)
                {
                    retList.Add(code);
                }
            }

            if (retList.Count >= models.Count)
            {
                ret.Data = retList;
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "批量添加失败";
                return ret;
            }
            if (retList.Any())
            {
                ret.Data = retList;
                ret.Code = HttpStatus.Success;
                ret.Message = "部分添加成功";
                return ret;
            }
            ret.Data = null;
            ret.Code = HttpStatus.Success;
            ret.Message = "批量添加成功";
            return ret;
        }

        private async Task<string> addEmployeeAsync(AddGEmployeeModel model)
        {
            var isEx = await GarmentContext.GEmployees.AnyAsync(x => x.Code == model.Code);

            if (isEx)
            {
                return model.Code;
            }

            var gemployee = Mapper.Map<GEmployee>(model);
            gemployee.CreateUser = UserId;

            var garmentSize = await GarmentContext.GarmentsSizes.FirstOrDefaultAsync(x => x.UserCode == model.Code);

            if (garmentSize != null)
            {
                if (!string.IsNullOrEmpty(model.ClothesSize))
                {
                    garmentSize.ClothesSize = model.ClothesSize;
                }
                if (!string.IsNullOrEmpty(model.ShoesSize))
                {
                    garmentSize.ShoesSize = model.ShoesSize;
                }
                garmentSize.UpdateUser = UserId;
                garmentSize.UpdateTime = DateTime.Now;
            }
            else
            {
                garmentSize = new GarmentSize
                {
                    UserCode = model.Code,
                    ClothesSize = model.ClothesSize,
                    ShoesSize = model.ShoesSize,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId
                };
                await GarmentContext.GarmentsSizes.AddAsync(garmentSize);
            }

            await GarmentContext.GEmployees.AddAsync(gemployee);
            await GarmentContext.SaveChangesAsync();

            return null;
        }

        public async Task<ResultViewModel> UpdateGEmployeeAsync(UpdateGEmployeeModel model)
        {
            var ret = new ResultViewModel();
            var gemployee = await GarmentContext.GEmployees.FirstOrDefaultAsync(x => x.Code == model.Code && x.IsDeleted == false);

            if (gemployee == null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "该员工不存在";
                return ret;
            }
            var roles = GetRoleNames();
            if (roles.Contains("行政文员"))
            {
                gemployee.Postion = model.Postion;
            }
            
            gemployee.UpdateUser = UserId;
            gemployee.UpdateTime = DateTime.Now;

            var garmentSize = await GarmentContext.GarmentsSizes.FirstOrDefaultAsync(x => x.UserCode == model.Code);
            if (garmentSize != null)
            {
                garmentSize.ClothesSize = model.ClothesSize;
                garmentSize.ShoesSize = model.ShoesSize;
                garmentSize.UpdateUser = UserId;
                garmentSize.UpdateTime = DateTime.Now;
            }
            else
            {
                garmentSize = Mapper.Map<GarmentSize>(model);
                garmentSize.CreateUser = UserId;
                await GarmentContext.GarmentsSizes.AddAsync(garmentSize);
            }
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "修改成功";

            return ret;
        }

        public async Task<ResultViewModel> DeleteGEmployeeAsync(string code)
        {
            var ret = new ResultViewModel();
            var gemployee = await GarmentContext.GEmployees.FirstOrDefaultAsync(x => x.Code == code && x.IsDeleted == false);

            if (gemployee == null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "该员工不存在";
                return ret;
            }

            gemployee.IsDeleted = true;
            gemployee.UpdateUser = UserId;
            gemployee.UpdateTime = DateTime.Now;

            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "删除成功";

            return ret;
        }

        public async Task<ResultsModel<string>> DeleteGEmployeeListAsync(List<string> list)
        {
            var ret = new ResultsModel<string>();
            var retList = new List<string>();

            foreach (var item in list)
            {
                var gemployee = await GarmentContext.GEmployees.FirstOrDefaultAsync(x => x.Code == item && x.IsDeleted == false);
                if (gemployee == null)
                {
                    retList.Add(item);
                    continue;
                }
                gemployee.IsDeleted = true;
                gemployee.UpdateUser = UserId;
                gemployee.UpdateTime = DateTime.Now;
                await GarmentContext.SaveChangesAsync();
            }

            if (retList.Count >= list.Count)
            {
                ret.Data = retList;
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "删除失败";
                return ret;
            }
            if (retList.Any())
            {
                ret.Data = retList;
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "部分删除成功";
                return ret;
            }

            ret.Data = null;
            ret.Code = HttpStatus.Success;
            ret.Message = "删除成功";
            return ret;
        }

        public async Task<ResultViewModel> ImportEmployeeAsync(IFormFile file)
        {
            var ret = new ResultViewModel();
            var importer = new ExcelImporter();
            try
            {
                var importRet = await importer.Import<ImportEmployeeModel>(file.OpenReadStream());
                var list = importRet.Data;
                var addEmplList = list.Select(l => new GEmployee
                {
                    Code = l.Code,
                    Name = l.Name,
                    Department = l.Department,
                    Postion = l.Postion,
                    Induction = l.Induction,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    IsDeleted = false
                }).ToList();

                var addSizeList = list.Select(l => new GarmentSize
                {
                    UserCode = l.Code,
                    ClothesSize = l.ClothesSize,
                    ShoesSize = l.ShoesSize,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId
                });

                var addLogList1 = list.Where(l=>l.Num1!=null && l.Num1>0).Select(l => new GarmentAssignedLog
                {
                    UserId = l.Code,
                    GarmentId = "29001111",
                    Number = l.Num1,
                    AssignedTime = l.Date1,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId ,
                    State = 2,
                    Type = true,
                    OperationTime = DateTime.Today
                });

                var addLogList2 = list.Where(l => l.Num2 != null && l.Num2 > 0).Select(l => new GarmentAssignedLog
                {
                    UserId = l.Code,
                    GarmentId = "29002666",
                    Number = l.Num2,
                    AssignedTime = l.Date2,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    State = 2,
                    Type = true,
                    OperationTime = DateTime.Today
                });

                var addLogList3 = list.Where(l => l.Num3 != null && l.Num3 > 0).Select(l => new GarmentAssignedLog
                {
                    UserId = l.Code,
                    GarmentId = "29002665",
                    Number = l.Num3,
                    AssignedTime = l.Date3,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    State = 2,
                    Type = true,
                    OperationTime = DateTime.Today
                });
                var addLogList4 = list.Where(l => l.Num4 != null && l.Num4 > 0).Select(l => new GarmentAssignedLog
                {
                    UserId = l.Code,
                    GarmentId = "29001111",
                    Number = l.Num4,
                    AssignedTime = l.Date4,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    State = 2,
                    Type = true,
                    OperationTime = DateTime.Today
                });
                var addLogList5 = list.Where(l => l.Num5 != null && l.Num5 > 0).Select(l => new GarmentAssignedLog
                {
                    UserId = l.Code,
                    GarmentId = "29005847",
                    Number = l.Num5,
                    AssignedTime = l.Date5,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    State = 2,
                    Type = true,
                    OperationTime = DateTime.Today
                });
                var addLogList6 = list.Where(l => l.Num6 != null && l.Num6 > 0).Select(l => new GarmentAssignedLog
                {
                    UserId = l.Code,
                    GarmentId = "12345678",
                    Number = l.Num6,
                    AssignedTime = l.Date6,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    State = 2,
                    Type = true,
                    OperationTime = DateTime.Today
                });

                await GarmentContext.GEmployees.AddRangeAsync(addEmplList);
                await GarmentContext.GarmentsSizes.AddRangeAsync(addSizeList);
                await GarmentContext.GarmentsAssignedLogs.AddRangeAsync(addLogList1);
                await GarmentContext.GarmentsAssignedLogs.AddRangeAsync(addLogList2);
                await GarmentContext.GarmentsAssignedLogs.AddRangeAsync(addLogList3);
                await GarmentContext.GarmentsAssignedLogs.AddRangeAsync(addLogList4);
                await GarmentContext.GarmentsAssignedLogs.AddRangeAsync(addLogList5);
                await GarmentContext.GarmentsAssignedLogs.AddRangeAsync(addLogList6);
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
