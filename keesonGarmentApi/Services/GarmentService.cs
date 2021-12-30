using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services;

public class GarmentService : BaseService
{
    public GarmentService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper) : base(garmentContext, cache, mapper)
    {
    }

    public async Task<PageViewModel<GarmentListModel>> GetGarmentsAsync(int pageIndex, int pageSize, string code, string name)
    {
        pageIndex = pageIndex == 0 ? 1 : pageIndex;
        pageSize = pageSize == 0 ? 20 : pageSize;
        
        var ret = new PageViewModel<GarmentListModel>();
        var query = GarmentContext.Garments.AsQueryable();
        
        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(x => x.Code == code);
        }
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(x => x.Name == name);
        }

        var count = await query.CountAsync();
        var list = await query.OrderBy(x => x.Code).Skip((pageIndex-1) * pageSize).Take(pageSize).ToListAsync();

        var data = Mapper.Map<List<GarmentListModel>>(list);

        for (int i = 0; i < data.Count; i++)
        {
            if (string.IsNullOrEmpty(list[i].Colors))
            {
                data[i].Colors = null;
            }
            else
            {
                var clist = list[i].Colors.Split('|').ToList();
                data[i].Colors = new List<Dictionary<string, string>>();
                foreach (var color in clist)
                {
                    var dic = new Dictionary<string, string>();
                    dic.Add("code", color);
                    dic.Add("name", color);
                    data[i].Colors.Add(dic);
                }
            }
        }
        ret.Data = data;
        ret.CurrentPage = pageIndex;
        ret.PageSize = pageSize;
        ret.TotalCount = count;
        ret.TotalPage = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
        ret.Code = HttpStatus.Success;
        ret.Message = "获取数据成功.";

        return ret;
    }

    public async Task<PageViewModel> AddGarmentAsync(AddGarmentModel model)
    {
        var ret = new PageViewModel();
        var isEx = await GarmentContext.Garments.AnyAsync(x => x.Code == model.Code);
        if (isEx)
        {
            ret.Code = HttpStatus.BadRequest;
            ret.Message = "工衣编号重复";
            return ret;
        }

        var garment = Mapper.Map<Garment>(model);
        garment.CreateUser = UserId;
        //garment.Colors = string.Join('|',model.Colors.ToArray());
        //garment.Colors = model.Colors.Replace('、','|');

        var colorList = new List<string>();
        foreach(var item in model.Colors)
        {
            colorList.Add(item.GetValueOrDefault("value",null));
        }
        garment.Colors = string.Join('|', colorList.ToArray());

        await GarmentContext.Garments.AddAsync(garment);
        await GarmentContext.SaveChangesAsync();

        ret.Code = HttpStatus.Success;
        ret.Message = "工衣添加成功";
        return ret;
    }

    public async Task<ResultViewModel> UpdateGarmentAsync(UpdateGarmentModel model)
    {
        var ret = new ResultViewModel();
        var garment = GarmentContext.Garments.FirstOrDefaultAsync(x => x.Code == model.Code).Result;

        if (garment == null)
        {
            ret.Code = HttpStatus.BadRequest;
            ret.Message = "工衣不存在";
            return ret;
        }

        garment.Name = model.Name;
        garment.Price = model.Price;
        garment.Remark = model.Remark;
        //garment.Colors = string.Join('|', model.Colors.ToArray());
        //garment.Colors = model.Colors.Replace('、', '|');
        var colorList = new List<string>();
        foreach (var item in model.Colors)
        {
            colorList.Add(item.GetValueOrDefault("value", null));
        }
        garment.Colors = string.Join('|', colorList.ToArray());
        garment.UpdateTime = DateTime.Now;
        garment.UpdateUser = UserId;

        await GarmentContext.SaveChangesAsync();

        ret.Code = HttpStatus.Success;
        ret.Message = "工衣修改成功";

        return ret;
    }

    public async Task<ResultViewModel> DeleteGarmentAsync(string code)
    {
        var ret = new ResultViewModel();

        var garment = await GarmentContext.Garments.SingleAsync(x => x.Code == code);
        if (garment == null)
        {
            ret.Code = HttpStatus.BadRequest;
            ret.Message = "工衣不存在";
            return ret;
        }

        GarmentContext.Garments.Remove(garment);

        await GarmentContext.SaveChangesAsync();

        ret.Code = HttpStatus.Success;
        ret.Message = "工衣删除成功";
        
        return ret;
    }
}