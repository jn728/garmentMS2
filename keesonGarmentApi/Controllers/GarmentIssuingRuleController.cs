using keesonGarmentApi.Filters;
using keesonGarmentApi.Models;
using keesonGarmentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keesonGarmentApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/rule")]
    public class GarmentIssuingRuleController : Controller
    {
        private readonly GarmentIssuingRuleService _garmentIssuingRuleService;

        public GarmentIssuingRuleController(GarmentIssuingRuleService garmentIssuingRuleService)
        {
            _garmentIssuingRuleService = garmentIssuingRuleService;
        }

        /// <summary>
        /// 获取工服发放标准
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Permission("Get-Rule")]
        //[Permission("Garment-hig")]
        public async Task<IActionResult> GetGarmentIssuingRule()
        {
            var ret = await _garmentIssuingRuleService.GetGarmentIssuingRulesAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 添加工服发放标准
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Permission("Get-Rule")]
        //[Permission("Garment-hig")]
        public async Task<IActionResult> AddGarmentIssuingRule(AddGarmentIssuingRuleModel model)
        {
            var ret = await _garmentIssuingRuleService.AddGarmentIssuingRuleAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 导入发放标准
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("importRules")]
        public async Task<IActionResult> ImportGarmentIssuingRule(IFormFile file)
        {
            var ret = await _garmentIssuingRuleService.ImportRules(file);
            return Ok(ret);
        }
    }
}
