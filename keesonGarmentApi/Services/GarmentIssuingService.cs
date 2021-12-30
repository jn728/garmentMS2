using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services
{
    public class GarmentIssuingService : BaseService
    {
        public GarmentIssuingService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper) : base(garmentContext, cache, mapper)
        {
        }

        public async Task<ResultsModel<GarmentIssuingListModel>> GetGarmentIssuingAsync()
        {
            var ret = new ResultsModel<GarmentIssuingListModel>();
            //查询距离截止日期最近
            var list = from gi in GarmentContext.GarmentsIssuings
                        join g in GarmentContext.Garments on gi.GarmentCode equals g.Code
                        where gi.EndTime > DateTime.Now
                        orderby gi.EndTime ascending
                        select new GarmentIssuingListModel
                        {
                            GarmentName = g.Name,
                            BeginTime = gi.BeginTime,
                            EndTime = gi.EndTime
                        };

            if (!list.Any())
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "暂未发放";
                return ret;
            }

            ret.Data = Mapper.Map<List<GarmentIssuingListModel>>(list);
            ret.Code = HttpStatus.Success;
            ret.Message = "获取数据成功";

            return ret;
        }

        public async Task<ResultsModel<FileListModel>> GetFIlesNameAsync()
        {
            var ret = new ResultsModel<FileListModel>();
            var query = from file in GarmentContext.GFiles
                        orderby file.UpdateTime descending
                        select new FileListModel
                        {
                            Name = file.FileName,
                            Path = "http://192.168.1.85:9000"+file.RPath,
                            UpdateTime = file.UpdateTime
                        };
            ret.Data = query.ToList();
            ret.Code = HttpStatus.Success;
            ret.Message = "文件名获取成功";
            return ret;
        }

        public async Task<ResultViewModel> AddGarmentIssuingAsync(AddGarmentIssuingModel model)
        {
            var ret = new ResultViewModel();

            var isEx = await GarmentContext.GarmentsIssuings.AnyAsync(x => x.EndTime >= DateTime.Now);
            
            if (isEx)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "工衣正在发放中";
                return ret;
            }

            var issuing = Mapper.Map<GarmentIssuing>(model);
            issuing.CreateUser = UserId;

            await GarmentContext.GarmentsIssuings.AddAsync(issuing);
            await GarmentContext.SaveChangesAsync();

            ret.Code = HttpStatus.Success;
            ret.Message = "添加成功";

            return ret;
        }

        public async Task<ResultViewModel> UploadFileAsync(IFormFile file)
        {
            var ret = new ResultViewModel();

            if (file == null)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "文件为空";
                return ret;
            }

            try
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                reader.ReadToEnd();
                string name = DateTime.Today.ToString("yyyy-MM-dd") + file.FileName;
                string filename = @"D:/File/Garment/" + name;
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }

                var f = await GarmentContext.GFiles.FirstOrDefaultAsync(x => x.FileName == name);
                if (f == null)
                {
                    f = new GFile()
                    {
                        FileName = name,
                        APath = filename,
                        RPath = @"/Garment/" + name,
                        UpdateTime = DateTime.Now,
                        UpdateUser = UserId
                    };
                    await GarmentContext.GFiles.AddAsync(f);
                }
                else
                {
                    f.UpdateTime = DateTime.Now;
                    f.UpdateUser = UserId;
                }
                await GarmentContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ret.Code = HttpStatus.BadRequest;
                ret.Message = "出现异常: "+ex.Message;
                return ret;
            }

            ret.Code = HttpStatus.Success;
            ret.Message = "添加成功";

            return ret;
        }
    }
}
