using keesonGarmentApi.Filters;
using keesonGarmentApi.Models;
using keesonGarmentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keesonGarmentApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/garmentissuing")]
    public class GarmentIssuingController : Controller
    {
        private readonly GarmentIssuingService _garmentIssuingService;
        
        public GarmentIssuingController(GarmentIssuingService garmentIssuingService)
        {
            _garmentIssuingService = garmentIssuingService;
        }

        /// <summary>
        /// 获取正在发放工服信息(返回距离截止日期最近的记录)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Permission("Get-Issuing")]
        public async Task<IActionResult> GetGarmentIssuing()
        {
            var ret = await _garmentIssuingService.GetGarmentIssuingAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns></returns>
        [HttpGet("files")]
        public async Task<IActionResult> GetFIlesName()
        {
            var ret = await _garmentIssuingService.GetFIlesNameAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("file")]
        //[Permission("Post-Issuing")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var ret = await _garmentIssuingService.UploadFileAsync(file);
            return Ok(ret);
        }

        /// <summary>
        /// 提交发放工服、截止时间
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddGarmentIssuing(AddGarmentIssuingModel model)
        {
            var ret = await _garmentIssuingService.AddGarmentIssuingAsync(model);
            return Ok(ret);
        }
    }
}
