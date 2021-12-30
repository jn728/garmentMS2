using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Models;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services
{
    public class GarmentAssignedLogService : BaseService
    {
        public GarmentAssignedLogService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper) : base(garmentContext, cache, mapper)
        {
        }
        public async Task<PageViewModel<GarmentAssignedLogListModel>> GetGarmentAssignedLogAsync(int state, bool isDelete, int pageIndex, int pageSize, string? code, string? name, string? department, string? postion, DateTime? date)
        {
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 20 : pageSize;

            var ret = new PageViewModel<GarmentAssignedLogListModel>();

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

            var userList = from ge in GarmentContext.GEmployees
                           where ge.IsDeleted == isDelete
                           join log in GarmentContext.GarmentsAssignedLogs on ge.Code equals log.UserId
                           where log.State == state
                           select ge.Code;

            var userSet = new HashSet<string>(userList);
            var query = from u in userSet
                        join ge in GarmentContext.GEmployees on u equals ge.Code
                        join gs in GarmentContext.GarmentsSizes on u equals gs.UserCode
                        select new GarmentAssignedLogListModel
                        {
                            UserId = u,
                            UserName = ge.Name,
                            Department = ge.Department,
                            Postion = ge.Postion,
                            Induction = ge.Induction,
                            ClothesSize = gs.ClothesSize,
                            ShoesSize = gs.ShoesSize
                        };

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(x => x.UserId == code);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.UserName == name);
            }
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(x => x.Department == department);
            }
            if (!string.IsNullOrEmpty(postion))
            {
                query = query.Where(x => x.Postion == postion);
            }
            if (date != null)
            {
                query = query.Where(x => x.Induction.Date == ((DateTime)date).Date);
            }

            var count = query.Count();
            var list = query.OrderBy(x => x.UserId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            //asdasd
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var logs = from l in GarmentContext.GarmentsAssignedLogs
                           where l.UserId == item.UserId && l.State == state
                           join g in GarmentContext.Garments on l.GarmentId equals g.Code
                           select new SingleLog
                           {
                               Type = l.Type,
                               Number = l.Number == 0 ? null : "" + l.Number,
                               Size = l.Size,
                               AssigedOrRefundTime = l.Type ? l.AssignedTime : l.RefundTime,
                               OperationTime = l.OperationTime,
                               //GarmentName = g.Name,
                               GarmentName = string.IsNullOrEmpty(l.Size) && l.Number == 0 ? null : g.Name,
                               GarmentCode = g.Code,
                               Color = l.Color,
                               Fare = state <= 1 ? 0 : GetRefundFare(l, GarmentContext.GarmentsAssignedLogs.Where(x => x.Type == false && x.UserId == l.UserId && x.GarmentId == l.GarmentId && x.State == 2 && x.AssignedTime == l.AssignedTime).Sum(x => x.Number), g.Price),
                               Remark = l.Remark
                           };
                item.Logs = logs.ToList();
                foreach (var log in logs)
                {
                    item.AllFare += log.Fare;
                }
            }

            ret.Data = list;
            ret.CurrentPage = pageIndex;
            ret.PageSize = pageSize;
            ret.TotalCount = count;
            ret.TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功.";

            return ret;
        }
        private static decimal GetRefundFare(GarmentAssignedLog log, int rNum, decimal price)
        {
            int count = 12;
            DateTime refund = DateTime.Now;
            if(log.AssignedTime == null)
            {
                return -1;
            }
            DateTime assigned = (DateTime)log.AssignedTime;
            int num = log.Number;

            if (log.Type == true)
            {
                num -= rNum;
            }
            else
            {
                refund = (DateTime)log.RefundTime;
            }
            int t = refund.Day >= assigned.Day ? 0 : 1;
            count = count - (12 * (refund.Year - assigned.Year) + refund.Month - assigned.Month - t);
            count = count >= 0 ? count : 0;
            decimal ret = (int)(price * count / 12 * 100 + 0.5M);

            return ret / 100 * num;
        }
        //private static decimal GetRefundFare(DateTime assigned, DateTime refund, decimal price)
        //{
        //    int count = 12;
        //    if (refund > assigned && assigned != null && refund != null)
        //    {
        //        int t = refund.Day >= assigned.Day ? 0 : 1;
        //        count = 12 * (refund.Year - assigned.Year) + refund.Month - assigned.Month - t;
        //    }
        //    return price * count / 12;
        //}

        public async Task<PageViewModel<GarmentAssignedLogSingleListModel>> GetGarmentAssignedLogSingleAsync(int pageIndex, int pageSize, string? code, string? name, string? department, DateTime? date)
        {
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 20 : pageSize;

            var ret = new PageViewModel<GarmentAssignedLogSingleListModel>();

            var roles = GetRoleNames();
            string dep = GetRoleSelectDeparment();
            //if (roles.Contains("部门文员"))
            //{
            //    dep = (from ge in GarmentContext.GEmployees
            //           where ge.Code == UserId
            //           select ge.Department).First();
            //}
            //else if (roles.Contains("人事文员") || roles.Contains("行政文员"))
            //{
            //    dep = "ALL";
            //}

            var query = from log in GarmentContext.GarmentsAssignedLogs
                        join ge in GarmentContext.GEmployees on log.UserId equals ge.Code
                        join gs in GarmentContext.GarmentsSizes on log.UserId equals gs.UserCode
                        join g in GarmentContext.Garments on log.GarmentId equals g.Code
                        where ge.IsDeleted == false && log.State == 2 && (ge.Department == dep || dep == "ALL")
                        select new GarmentAssignedLogSingleListModel
                        {
                            UserId = log.UserId,
                            UserName = ge.Name,
                            Department = ge.Department,
                            Postion = ge.Postion,
                            Induction = ge.Induction,
                            ClothesSize = gs.ClothesSize,
                            ShoesSize = gs.ShoesSize,
                            Type = log.Type,
                            Number = log.Number,
                            AssigedOrRefundTime = log.Type ? log.AssignedTime : log.RefundTime,
                            OperationTime = log.OperationTime,
                            GarmentName = g.Name,
                            //Fare = log.Number * (log.Type == true ? g.Price : -1 * GetRefundFare((DateTime)log.AssignedTime, (DateTime)log.RefundTime, g.Price)),
                            Fare = GetRefundFare(log, GarmentContext.GarmentsAssignedLogs.Where(x => x.Type == false && x.UserId == log.UserId && x.GarmentId == log.GarmentId && x.State == 2 && x.AssignedTime == log.AssignedTime).Sum(x => x.Number), g.Price),
                            Remark = log.Remark
                        };

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(x => x.UserId == code);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.UserName == name);
            }
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(x => x.Department == department);
            }
            if (date != null)
            {
                query = query.Where(x => x.Induction.Date == date);
            }

            var count = query.Count();
            var list = query.OrderBy(x => x.UserId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            ret.Data = list;
            ret.CurrentPage = pageIndex;
            ret.PageSize = pageSize;
            ret.TotalCount = count;
            ret.TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功.";

            return ret;
        }

        public async Task<PageViewModel<GarmentAllEmployeeLogListModel>> GetAllEmployeeLogAsync(int pageIndex, int pageSize, string code, string name, string department, string postion, DateTime? date)
        {
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 20 : pageSize;

            var ret = new PageViewModel<GarmentAllEmployeeLogListModel>();

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
                        where ge.IsDeleted == false && (ge.Department == dep || dep == "ALL")
                        select new GarmentAllEmployeeLogListModel
                        {
                            UserId = ge.Code,
                            UserName = ge.Name,
                            Department = ge.Department,
                            Postion = ge.Postion,
                            Induction = ge.Induction,
                            ClothesSize = gs.ClothesSize,
                            ShoesSize = gs.ShoesSize,
                            IsPass = true,
                            Logs = new List<SingleLog>()
                            //Number = null,
                            //Size = null
                        };
            //select new GarmentAllEmployeeLogListModel
            //{
            //    UserId = ge.Code,
            //    UserName = ge.Name,
            //    Department = ge.Department,
            //    Postion = ge.Postion,
            //    Induction = ge.Induction,
            //    ClothesSize = gs.ClothesSize,
            //    ShoesSize = gs.ShoesSize,
            //    IsPass = true,
            //    Number = null,
            //    Size = null
            //};

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(x => x.UserId == code);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.UserName == name);
            }
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(x => x.Department == department);
            }
            if (!string.IsNullOrEmpty(postion))
            {
                query = query.Where(x => x.Postion == postion);
            }
            if (date != null)
            {
                query = query.Where(x => x.Induction.Date == ((DateTime)date).Date);
            }

            var count = await query.CountAsync();
            var list = await query.OrderBy(x => x.UserId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            if (roles.Contains("部门文员"))
            {
                var gid = await GarmentContext.GarmentsIssuings.Where(x => x.EndTime > DateTime.Now).Select(x => x.GarmentCode).FirstOrDefaultAsync();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (GetIssuingRuleNumber(gid, item.UserId).Result <= 0)
                    {
                        item.IsPass = false;
                    }
                }
            }

            var gTemp = (from issuingCode in GarmentContext.GarmentsIssuings
                         where issuingCode.EndTime > DateTime.Now
                         join garment in GarmentContext.Garments on issuingCode.GarmentCode equals garment.Code
                         select new
                         {
                             GarmentCode = garment.Code,
                             GarmentName = garment.Name
                         }).FirstOrDefault();

            for (int i = 0; i < list.Count; i++)
            {
                var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.State <= 1 && x.UserId == list[i].UserId);

                var singleLog = new SingleLog()
                {
                    Fare = 0,
                    AssigedOrRefundTime = null,
                    Type = true,
                    GarmentName = gTemp == null ? "" : gTemp.GarmentName,
                    GarmentCode = gTemp == null ? "" : gTemp.GarmentCode,
                    Remark = "",
                    OperationTime = DateTime.Now
                };
                if (log == null)
                {
                    singleLog.Number = "";
                    singleLog.Size = "";
                    singleLog.Color = "none";
                    singleLog.GarmentCode = null;
                    singleLog.GarmentName = null;
                }
                else
                {
                    singleLog.Number = log.Number == 0 ? null : "" + log.Number;
                    singleLog.Size = log.Size;
                    singleLog.Color = log.Color;
                }
                list[i].Logs.Add(singleLog);
            }

            ret.Data = Mapper.Map<List<GarmentAllEmployeeLogListModel>>(list);
            ret.CurrentPage = pageIndex;
            ret.PageSize = pageSize;
            ret.TotalCount = count;
            ret.TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功";

            return ret;
        }

        public async Task<PageViewModel<GarmentQuitEmployeeLogListModel>> GetQuitEmployeeLogAsync(int pageIndex, int pageSize, string code, string name, string department, string postion)
        {
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? 20 : pageSize;

            var ret = new PageViewModel<GarmentQuitEmployeeLogListModel>();

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
                        where ge.IsDeleted == true && (ge.Department == dep || dep == "ALL")
                        join gs in GarmentContext.GarmentsSizes on ge.Code equals gs.UserCode
                        join log in GarmentContext.GarmentsAssignedLogs on ge.Code equals log.UserId
                        where log.State == 1
                        join g in GarmentContext.Garments on log.GarmentId equals g.Code
                        select new GarmentQuitEmployeeLogListModel
                        {
                            UserId = ge.Code,
                            UserName = ge.Name,
                            Department = ge.Department,
                            Postion = ge.Postion,
                            Induction = ge.Induction,
                            ClothesSize = gs.ClothesSize,
                            ShoesSize = gs.ShoesSize,
                            GarmentName = g.Name,
                            Number = log.Number,
                            Size = log.Size
                        };

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(x => x.UserId == code);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.UserName == name);
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
            var list = await query.OrderBy(x => x.UserId).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            ret.Data = Mapper.Map<List<GarmentQuitEmployeeLogListModel>>(list);
            ret.CurrentPage = pageIndex;
            ret.PageSize = pageSize;
            ret.TotalCount = count;
            ret.TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功";

            return ret;
        }

        public async Task<ResultModel<GarmentSummaryConvertModel>> GetGarmentCommitLogSummaryAsync()
        {
            var ret = new ResultModel<GarmentSummaryConvertModel>();

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

            var query = from log in GarmentContext.GarmentsAssignedLogs
                        where log.State == 1
                        join e in GarmentContext.GEmployees on log.UserId equals e.Code
                        join g in GarmentContext.Garments on log.GarmentId equals g.Code
                        where (e.Department == dep || dep == "ALL") && e.IsDeleted == false
                        select new
                        {
                            Department = e.Department,
                            GarmentName = g.Name,
                            Color = log.Color,
                            Size = log.Size,
                            Number = log.Number
                        };
            var list = from l in query
                       group l by new { l.Department, l.GarmentName, l.Color, l.Size } into li
                       select new
                       {
                           Department = li.Select(x => x.Department).First(),
                           GarmentName = li.Select(x => x.GarmentName).First(),
                           Color = li.Select(x => x.Color).First(),
                           Size = li.Select(x => x.Size).First(),
                           Number = li.Sum(x => x.Number)
                       };

            var departmentList = list.Select(x => x.Department).ToList();
            var departmentSet = new HashSet<string>(departmentList);
            var tempDate = new List<GarmentCommitLogSummaryListModel>();
            var typeList = new List<string>();
            foreach (var department in departmentSet)
            {
                var singleSummaryList = from l in list
                                        where l.Department == department
                                        select new SingleSummary
                                        {
                                            GarmentName = l.GarmentName,
                                            Size = l.Size,
                                            Color = l.Color,
                                            Number = l.Number
                                        };
                typeList.AddRange(singleSummaryList.Select(x => x.GarmentName + (x.Color == null || x.Color == "none" ? "" : "(" + x.Color + ")") + x.Size).ToList());
                tempDate.Add(new GarmentCommitLogSummaryListModel
                {
                    Department = department,
                    List = singleSummaryList.ToList()
                });
            }

            //待完善，代码混乱
            var retDate2 = new GarmentSummaryConvertModel();
            retDate2.DataList = new List<Dictionary<string, string>>();
            retDate2.TitleList = new List<Dictionary<string, string>>();

            var propSet = new HashSet<string>(typeList);

            var dic = new Dictionary<string, string>();
            dic.Add("label", "部门");
            dic.Add("prop", "dep");
            retDate2.TitleList.Add(dic);

            var indexDic = new Dictionary<string, string>();
            int col = 1;
            foreach (var prop in propSet)
            {
                var dicT = new Dictionary<string, string>();
                dicT.Add("label", prop);
                dicT.Add("prop", "prop" + col);
                retDate2.TitleList.Add(dicT);
                indexDic.Add(prop, "prop" + col);
                col++;
            }
            foreach (var item in tempDate)
            {
                var dicT = new Dictionary<string, string>();
                dicT.Add("dep", item.Department);
                foreach (var key in item.List)
                {
                    string type = key.GarmentName + (key.Color == null || key.Color == "none" ? "" : "(" + key.Color + ")") + key.Size;
                    string index = indexDic.GetValueOrDefault(type);
                    dicT.Add(index, "" + key.Number);
                }
                retDate2.DataList.Add(dicT);
            }
            ret.Data = retDate2;
            //ret.Data = new List<GarmentCommitLogSummaryListModel>();
            //foreach (var department in departmentSet)
            //{
            //    var singleSummaryList = from l in list
            //                            where l.Department == department
            //                            select new SingleSummary
            //                            {
            //                                GarmentName = l.GarmentName,
            //                                Size = l.Size,
            //                                Color = l.Color,
            //                                Number = l.Number
            //                            };
            //    ret.Data.Add(new GarmentCommitLogSummaryListModel
            //    {
            //        Department = department,
            //        List = singleSummaryList.ToList()
            //    });
            //}

            ret.Code = HttpStatus.Success;
            ret.Message = "数据获取成功";
            return ret;
        }

        public async Task<ResultViewModel> AddGarmentAssignedLogAsync()
        {
            var ret = new ResultViewModel();

            //删除未领取记录
            var delList = GarmentContext.GarmentsAssignedLogs.Where(x => x.State <= 1);
            GarmentContext.GarmentsAssignedLogs.RemoveRange(delList);
            await GarmentContext.SaveChangesAsync();


            var garmentIdList = from gi in GarmentContext.GarmentsIssuings
                                where DateTime.Now < gi.EndTime
                                select gi.GarmentCode;

            var addLog = from user in GarmentContext.GEmployees
                         where user.IsDeleted == false
                         //join gs in GarmentContext.GarmentsSizes on user.Code equals gs.UserCode
                         from g in garmentIdList
                         select new AddGarmentAssignedLogBeforeCommitModel
                         {
                             GarmentId = g,
                             UserId = user.Code,
                             Induction = user.Induction,
                             Department = user.Department,
                             Postion = user.Postion,
                             //Size = GarmentContext.Garments.Where(x => x.Code == g).Select(x => x.IsClothes).First() == true ? gs.ClothesSize : gs.ShoesSize,
                             Size = null,
                             Number = 0,
                             State = 0,
                             Type = true,
                             Color = "none",
                             CreateUser = UserId
                         };

            foreach (var item in addLog.ToList())
            {
                await AddLogAsync(item);
            }
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "审核完毕";

            return ret;
        }

        public async Task<ResultViewModel> AddSingleGarmentAssignedLogAsync(AddSingleGarmentAssignedLogModel model)
        {
            var ret = new ResultViewModel();

            var user = await GarmentContext.GEmployees.FirstOrDefaultAsync(x => x.Code == model.UserId);
            if (user == null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "不存在该用户";
                return ret;
            }
            var isEx = await GarmentContext.Garments.AnyAsync(x => x.Code == model.GarmentId);
            if (!isEx)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "不存在该工衣";
                return ret;
            }

            var log = Mapper.Map<AddGarmentAssignedLogBeforeCommitModel>(model);
            log.Department = user.Department;
            log.Postion = user.Postion;
            log.Type = true;
            log.State = 1;
            log.CreateUser = UserId;
            log.Induction = user.Induction;

            var roles = GetRoleNames();
            if (roles.Contains("行政文员") || roles.Contains("人事文员"))
            {
                var log2 = Mapper.Map<GarmentAssignedLog>(log);
                await GarmentContext.GarmentsAssignedLogs.AddAsync(log2);
            }
            else
            {
                var success = await AddLogAsync(log);

                if (!success)
                {
                    ret.Code = HttpStatus.BadRequest;
                    ret.Message = "提交失败";
                    return ret;
                }
            }

            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "提交成功";
            return ret;

        }

        private async Task<bool> AddLogAsync(AddGarmentAssignedLogBeforeCommitModel item)
        {

            //var rule = (from gir in GarmentContext.GarmentsIssuingRules
            //            where gir.DepartmentId.Equals(item.Department) && gir.PositionId.Equals(item.Postion) && gir.GarmentId.Equals(item.GarmentId)
            //            select new
            //            {
            //                year_new = gir.NewEmpAssignedYear,
            //                year = gir.EmpAssignedYear,
            //                number_new = gir.NewEmpAssignedNumber,
            //                number = gir.EmpAssignedNumber
            //            }).FirstOrDefault();
            //if (rule == null)
            //{
            //    return false;
            //}

            //var logList = (from gal in GarmentContext.GarmentsAssignedLogs
            //               where gal.UserId == item.UserId && gal.GarmentId == item.GarmentId && gal.State == 2
            //               orderby gal.AssignedTime descending
            //               select gal).FirstOrDefault();

            //DateTime assigned = DateTime.Today;
            //if (logList != null)
            //{
            //    assigned = (DateTime)logList.AssignedTime;
            //}
            //DateTime induction = item.Induction;
            ////判断该员工是否领取过该工服
            //if (logList == null && item.Number <= rule.number_new)
            //{
            //    //item.Number = rule.number_new;
            //}
            ////判断该员工是否可以领取
            //else if (induction.AddYears(rule.year_new) < DateTime.Now && assigned.AddYears(rule.year) < DateTime.Now && item.Number <= rule.number)
            //{
            //    //item.Number = rule.number;
            //}
            //else
            //{
            //    return false;
            //}
            if (item.Number > GetIssuingRuleNumber(item.GarmentId, item.UserId).Result)
            {
                return false;
            }

            var log = Mapper.Map<GarmentAssignedLog>(item);
            await GarmentContext.GarmentsAssignedLogs.AddAsync(log);

            //if (item.Number != 0)
            //{
            //    var log = Mapper.Map<GarmentAssignedLog>(item);
            //    await GarmentContext.GarmentsAssignedLogs.AddAsync(log);
            //}
            return true;
        }

        private async Task<int> GetIssuingRuleNumber(string gid, string uid)
        {
            var temp = (from ge in GarmentContext.GEmployees
                        where ge.Code == uid
                        select new
                        {
                            dep = ge.Department,
                            pos = ge.Postion,
                            ind = ge.Induction
                        }).FirstOrDefault();
            if (temp == null)
            {
                return -1;
            }
            //???

            //获取该工服部门工段发放标准
            var rule = (from gir in GarmentContext.GarmentsIssuingRules
                            //where gir.DepartmentId.Equals(temp.dep) && gir.PositionId.Equals(temp.pos) && gir.GarmentId.Equals(gid)
                        where gir.DepartmentId.Equals(temp.dep) && gir.PositionId.Equals(CoventPosition(temp.pos)) && gir.GarmentId.Equals(gid)
                        select new
                        {
                            year_new = gir.NewEmpAssignedYear,
                            year = gir.EmpAssignedYear,
                            number_new = gir.NewEmpAssignedNumber,
                            number = gir.EmpAssignedNumber
                        }).FirstOrDefault();
            if (rule == null)
            {
                return -1;
            }
            //获取上次领取时间
            var logList = (from gal in GarmentContext.GarmentsAssignedLogs
                           where gal.UserId == uid && gal.GarmentId == gid && gal.State == 2
                           orderby gal.AssignedTime descending
                           select gal).FirstOrDefault();

            DateTime assigned = DateTime.Today;
            if (logList != null)
            {
                assigned = (DateTime)logList.AssignedTime;
            }
            DateTime induction = temp.ind;
            //判断该员工是否领取过该工服
            if (logList == null)
            {
                return rule.number_new;
            }
            //判断该员工是否可以领取
            else if (induction.AddYears(rule.year_new) < DateTime.Now && assigned.AddYears(rule.year) < DateTime.Now)
            {
                return rule.number;
            }
            else
            {
                return -1;
            }
        }

        //快速维护
        //行政文员：数量可以超出标准
        public async Task<ResultsModel<string>> UpdateFastMaintainAsync(UpdateFastMaintainLogModel model)
        {
            var ret = new ResultsModel<string>();
            ret.Data = new List<string>();

            //去除不可维护人员id
            if(GetRoleState() == 0)
            {
                model.List = (from l in model.List
                              join g in GarmentContext.GEmployees on l equals g.Code
                              where g.Department == GetRoleSelectDeparment()
                              select l).ToList();
            }


            var isClothes = await GarmentContext.Garments.Where(x => x.Code == model.GarmentCode).Select(x => x.IsClothes).FirstAsync();
            foreach (var item in model.List)
            {
                //???
                var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.UserId == item && x.State == 0);
                if (log == null)
                {
                    var isEx = await GarmentContext.GEmployees.AnyAsync(x => x.Code == item);
                    if (!isEx)
                    {
                        ret.Data.Add(item);
                        continue;
                    }
                    if (model.Number > GetIssuingRuleNumber(model.GarmentCode, item).Result && GetRoleState() != 2)
                    {
                        ret.Data.Add(item);
                        continue;
                    }

                    var gs = await GarmentContext.GarmentsSizes.FirstOrDefaultAsync(x => x.UserCode == item);
                    var logTemp = new GarmentAssignedLog()
                    {
                        GarmentId = model.GarmentCode,
                        UserId = item,
                        Size = gs == null ? null : (isClothes == true ? gs.ClothesSize : gs.ShoesSize),
                        Number = model.Number,
                        State = 0,
                        AssignedTime = null,
                        RefundTime = null,
                        Type = true,
                        Color = model.Color,
                        OperationTime = DateTime.Today,
                        Remark = null,
                        CreateTime = DateTime.Now,
                        CreateUser = UserId,
                        UpdateTime = null,
                        UpdateUser = null
                    };
                    await GarmentContext.GarmentsAssignedLogs.AddAsync(logTemp);
                }
                else
                {
                    log.GarmentId = model.GarmentCode;

                    if (model.Number > GetIssuingRuleNumber(model.GarmentCode, item).Result && GetRoleState() != 2)
                    {
                        ret.Data.Add(item);
                        continue;
                    }
                    log.Number = model.Number;

                    var size = (from gs in GarmentContext.GarmentsSizes
                                where gs.UserCode == item
                                select new
                                {
                                    csize = gs.ClothesSize,
                                    ssize = gs.ShoesSize
                                }).FirstOrDefault();
                    if (size == null)
                    {
                        ret.Data.Add(item);
                        continue;
                    }
                    log.Size = isClothes ? size.csize : size.ssize;
                    log.Color = model.Color;
                }
                await GarmentContext.SaveChangesAsync();
            }

            if (ret.Data.Count >= model.List.Count)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "快速维护失败";
                return ret;
            }
            if (ret.Data.Count != 0)
            {
                ret.Code = HttpStatus.Success;
                ret.Message = "快速维护部分成功，请注意申请数量";
                return ret;
            }

            ret.Code = HttpStatus.Success;
            ret.Message = "快速维护成功";
            return ret;
        }

        public async Task<ResultViewModel> UpdateGarmentAssignedLogAsync(UpdateGarmentAssignedLogModel model)
        {
            var ret = new ResultViewModel();

            var userId = UserId;
            var roles = GetRoleNames();
            int state = 0;
            if (roles.Contains("部门文员"))
            {
                state = 0;
            }
            else if (roles.Contains("人事文员") || roles.Contains("行政文员"))
            {
                state = 1;
            }

            //判断是否可以维护
            if(state == 0)
            {
                var isExT = await GarmentContext.GEmployees.AnyAsync(x => x.Code == model.UserId && x.Department == GetRoleSelectDeparment());
                if (!isExT)
                {
                    ret.Code = HttpStatus.BadRequest;
                    ret.Message = "不可维护其他部门人员";
                    return ret;
                }
            }

            //var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.GarmentId == model.GarmentId && x.State <= state);
            var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.State <= state);

            if (log == null)
            {
                var isExT = await GarmentContext.GEmployees.AnyAsync(x => x.Code == model.UserId);
                if (!isExT)
                {
                    ret.Code = HttpStatus.BadRequest;
                    ret.Message = "不存在该员工";
                    return ret;
                }
                if (model.Number > GetIssuingRuleNumber(model.GarmentId, model.UserId).Result && GetRoleState() != 2)
                {
                    ret.Code = HttpStatus.BadRequest;
                    ret.Message = "申请数量超出标准";
                    return ret;
                }
                var gs = await GarmentContext.GarmentsSizes.FirstOrDefaultAsync(x => x.UserCode == model.UserId);
                var logTemp = new GarmentAssignedLog()
                {
                    GarmentId = model.GarmentId,
                    UserId = model.UserId,
                    Size = model.Size,
                    Number = model.Number,
                    State = 0,
                    AssignedTime = null,
                    RefundTime = null,
                    Type = true,
                    Color = model.Color,
                    OperationTime = DateTime.Today,
                    Remark = model.Remark,
                    CreateTime = DateTime.Now,
                    CreateUser = UserId,
                    UpdateTime = null,
                    UpdateUser = null
                };
                await GarmentContext.GarmentsAssignedLogs.AddAsync(logTemp);
            }
            else
            {
                var isEx = await GarmentContext.Garments.AnyAsync(x => x.Code == model.GarmentId);
                if (!isEx)
                {
                    ret.Code = HttpStatus.BadRequest;
                    ret.Message = "不存在该工衣";
                    return ret;
                }

                log.GarmentId = model.GarmentId;
                if (state == 0)
                {
                    if (model.Number > GetIssuingRuleNumber(model.GarmentId, model.UserId).Result)
                    {
                        ret.Code = HttpStatus.BadRequest;
                        ret.Message = "超过可申请数量";
                        return ret;
                    }
                }
                log.Number = model.Number;
                log.Size = model.Size;
                log.Color = model.Color;
                log.Remark = model.Remark;
                log.UpdateUser = UserId;
                log.UpdateTime = DateTime.Now;
            }
            //qwe
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "修改成功";

            return ret;
        }

        public async Task<ResultViewModel> UpdateGarmentAssignedLogStateAsync(UpdateGarmentAssignedLogStateModel model)
        {
            var ret = new ResultViewModel();

            //判断是否可以维护
            if (GetRoleState() == 0)
            {
                var isExT = await GarmentContext.GEmployees.AnyAsync(x => x.Code == model.UserId && x.Department == GetRoleSelectDeparment());
                if (!isExT)
                {
                    ret.Code = HttpStatus.BadRequest;
                    ret.Message = "不可维护其他部门人员";
                    return ret;
                }
            }

            var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.GarmentId == model.GarmentId && x.State == model.State - 1);

            if (log == null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "不存在该记录";
                return ret;
            }

            if (model.IsAssigned == false)
            {
                GarmentContext.GarmentsAssignedLogs.Remove(log);
            }
            else
            {
                if (model.State == 2)
                {
                    log.AssignedTime = model.Date;
                }

                log.State = model.State;
                log.UpdateTime = DateTime.Now;
                log.OperationTime = DateTime.Today;
                log.UpdateUser = UserId;
            }
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "状态更新成功";
            return ret;
        }

        public async Task<ResultsModel<string>> UpdateFastCommitOrAssignedAsync(UpdateFastCommitOrAssignedLogModel model)
        {
            var ret = new ResultsModel<string>();
            var retList = new List<string>();

            //去除不可维护人员id
            if(GetRoleState() == 0)
            {
                model.List = (from l in model.List
                              join g in GarmentContext.GEmployees on l equals g.Code
                              where g.Department == GetRoleSelectDeparment()
                              select l).ToList();
            }

            foreach (var item in model.List)
            {
                var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.UserId == item && x.State == model.State - 1);
                if (log == null || log.Number <= 0 || string.IsNullOrEmpty(log.Size))
                {
                    retList.Add(item);
                    continue;
                }
                log.State = model.State;
                if (model.State == 2)
                {
                    log.AssignedTime = model.Date;
                }
                log.UpdateTime = DateTime.Now;
                log.UpdateUser = UserId;
                log.OperationTime = DateTime.Today;
                await GarmentContext.SaveChangesAsync();
            }

            if (retList.Count >= model.List.Count)
            {
                ret.Data = retList;
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "快速修改失败";
                return ret;
            }
            if (retList.Any())
            {
                ret.Data = retList;
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "快速修改部分成功";
                return ret;
            }

            ret.Data = null;
            ret.Code = HttpStatus.Success;
            ret.Message = "快速修改成功";
            return ret;
        }

        public async Task<ResultViewModel> RefundGarmentAssignedLogAsync(RefundGarmentAssignedLogModel model)
        {
            var ret = new ResultViewModel();
            var log = await GarmentContext.GarmentsAssignedLogs.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.GarmentId == model.GarmentId && x.State == 2 && x.AssignedTime == model.AssignedTime.Date);

            int max = (from l in GarmentContext.GarmentsAssignedLogs
                       where l.UserId == model.UserId && l.GarmentId == model.GarmentId && l.AssignedTime == model.AssignedTime && l.Type == false
                       select l.Number).Sum();

            if (log == null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "不存在该记录";
                return ret;
            }
            if (model.Number <= 0)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "退还数量不能小于1";
                return ret;
            }
            if (log.Number < model.Number + max)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "退还总数量不能大于领取数量";
                return ret;
            }

            var newLog = new GarmentAssignedLog
            {
                UserId = log.UserId,
                GarmentId = log.GarmentId,
                Number = model.Number,
                AssignedTime = log.AssignedTime,
                RefundTime = model.Date,
                Type = false,
                OperationTime = DateTime.Today,
                CreateTime = DateTime.Now,
                Size = log.Size,
                State = log.State,
                Color = log.Color,
                CreateUser = UserId,
                Remark = model.Remark
            };

            await GarmentContext.GarmentsAssignedLogs.AddAsync(newLog);
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "退还成功";
            return ret;
        }

        public async Task<ResultViewModel> ExportExcelLogAsync()
        {
            var ret = new ResultViewModel();

            string dep = GetRoleSelectDeparment();
            var list = from log in GarmentContext.GarmentsAssignedLogs
                       join ge in GarmentContext.GEmployees on log.UserId equals ge.Code
                       join gs in GarmentContext.GarmentsSizes on log.UserId equals gs.UserCode
                       join g in GarmentContext.Garments on log.GarmentId equals g.Code
                       where ge.IsDeleted == false && log.State == 2 && (ge.Department == dep || dep == "ALL")
                       select new ExportExcelLogModel
                       {
                           UserId = log.UserId,
                           UserName = ge.Name,
                           Department = ge.Department,
                           Postion = ge.Postion,
                           Induction = ge.Induction,
                           ClothesSize = gs.ClothesSize,
                           ShoesSize = gs.ShoesSize,
                           Type = log.Type == true ? "领取" : "退还",
                           Number = log.Number,
                           AssigedOrRefundTime = log.Type ? log.AssignedTime : log.RefundTime,
                           OperationTime = log.OperationTime,
                           GarmentName = g.Name,
                           Fare = GetRefundFare(log, GarmentContext.GarmentsAssignedLogs.Where(x => x.Type == false && x.UserId == log.UserId && x.GarmentId == log.GarmentId && x.State == 2 && x.AssignedTime == log.AssignedTime).Sum(x => x.Number), g.Price)
                       };

            var result = await new ExcelExporter().Export("D:\\File\\Garment\\台账报表.xlsx", list.ToList());

            ret.Code = HttpStatus.Success;
            ret.Message = "导出成功";
            return ret;

        }

        public async Task<ResultViewModel> ExportExcelApplyAsync()
        {
            var ret = new ResultViewModel();

            string dep = GetRoleSelectDeparment();
            var list = from log in GarmentContext.GarmentsAssignedLogs
                       where log.State == 1
                       join ge in GarmentContext.GEmployees on log.UserId equals ge.Code
                       where ge.IsDeleted == false && (ge.Department == dep || dep == "ALL")
                       join gs in GarmentContext.GarmentsSizes on ge.Code equals gs.UserCode
                       join g in GarmentContext.Garments on log.GarmentId equals g.Code
                       select new ExportExcelApplyModel
                       {
                           UserName = ge.Name,
                           UserId = ge.Code,
                           Department = ge.Department,
                           Postion = ge.Postion,
                           Induction = ge.Induction,
                           ClothesSize = gs.ClothesSize,
                           ShoesSize = gs.ShoesSize,
                           GarmentName = g.Name,
                           Number = log.Number,
                           Size = log.Size
                       };

            var result = await new ExcelExporter().Export("D:\\File\\Garment\\申请工服.xlsx", list.ToList());

            ret.Code = HttpStatus.Success;
            ret.Message = "导出成功";
            return ret;

        }
    }
}