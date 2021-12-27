using keesonGarmentApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace keesonGarmentApi.Controllers
{
    [ApiController]
    [Route("api/selector")]
    public class SelectorController : Controller
    {
        private readonly SelectorService _selectorService;

        public SelectorController(SelectorService selectorService) 
        { 
            _selectorService = selectorService;
        }

        /// <summary>
        /// 获取工衣种类(id,name)
        /// </summary>
        /// <returns></returns>
        [HttpGet("garment")]
        public async Task<IActionResult> GetGarmentSelector()
        {
            var ret = await _selectorService.GetGarmentSelectorAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 获取部门(name)
        /// </summary>
        /// <returns></returns>
        [HttpGet("department")]
        public async Task<IActionResult> GetDepartmentSelector()
        {
            var ret = await _selectorService.GetDepartmentSelectorAsync();
            return Ok(ret);
        }
    }
}
