using keesonGarmentApi.Filters;
using keesonGarmentApi.Models;
using keesonGarmentApi.Services;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keesonGarmentApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/employee")]
    public class GEmployeeController : Controller
    {
        private readonly GEmployeeService _gemployeeService;

        public GEmployeeController(GEmployeeService gemployeeService)
        {
            _gemployeeService = gemployeeService;
        }

        /// <summary>
        /// 获取员工信息(包含员工工衣尺码)
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面尺寸</param>
        /// <param name="code">员工号</param>
        /// <param name="name">员工姓名</param>
        /// <param name="department">部门</param>
        /// <param name="postion">工段</param>
        /// <returns></returns>
        [HttpGet]
        [Permission("Get-Employee")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> GetGEmployee(int pageIndex, int pageSize, string? code, string? name, string? department, string? postion, bool isDelete = false)
        {
            var ret = await _gemployeeService.GetGEmployeeAsync(pageIndex, pageSize, code, name, department, postion, isDelete);
            return Ok(ret);
        }

        /// <summary>
        /// 添加员工(工衣尺码可为空)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Permission("Post-Employee")]
        //[Permission("Garment-mid")]
        public async Task<IActionResult> AddGEmployee(AddGEmployeeModel model)
        {
            var ret = await _gemployeeService.AddGEmployeeAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 批量添加员工
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost("list")]
        //[Permission("Garment-mid")]
        public async Task<IActionResult> AddCEmployeeList(List<AddGEmployeeModel> models)
        {
            var ret = await _gemployeeService.AddGEmployeeListAsync(models);
            return Ok(ret);
        }

        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Permission("Update-Employee")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> UpdateGEmployee(UpdateGEmployeeModel model)
        {
            var ret = await _gemployeeService.UpdateGEmployeeAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 删除员工(仅将员工IsDelete字段标记为True)
        /// </summary>
        /// <param name="code">员工号</param>
        /// <returns></returns>
        [HttpDelete]
        [Permission("Delete-Employee")]
        //[Permission("Garment-mid")]
        public async Task<IActionResult> DeleteGEmployee(string code)
        {
            var ret = await _gemployeeService.DeleteGEmployeeAsync(code);
            return Ok(ret);
        }

        /// <summary>
        /// 快速删除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpDelete("list")]
        [Permission("Delete-EmployeeList")]
        //[Permission("Garment-mid")]
        public async Task<IActionResult> DeleteGEmployeeList(List<string> list)
        {
            var ret = await _gemployeeService.DeleteGEmployeeListAsync(list);
            return Ok(ret);
        }


        [HttpPost("import")]
        public async Task<IActionResult> ImportEmployees(IFormFile file)
        {

            var ret = await _gemployeeService.ImportEmployeeAsync(file);
            return Ok(ret);
        }
    }
}
