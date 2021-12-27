using keesonGarmentApi.Filters;
using keesonGarmentApi.Models;
using keesonGarmentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keesonGarmentApi.Controllers;

//[Authorize]
[ApiController]
[Route("api/garment")]
public class GarmentController : Controller
{
    private readonly GarmentService _garmentService;

    public GarmentController(GarmentService garmentService)
    {
        _garmentService = garmentService;
    }

    /// <summary>
    /// 获取工衣列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页面尺寸</param>
    /// <param name="code">工服编号</param>
    /// <param name="name">工服名称</param>
    /// <returns></returns>
    [HttpGet]
    [Permission("Get-Garment")]
    //[Permission("Garment-low")]
    public async Task<IActionResult> GetGarmentList(int pageIndex, int pageSize, string? code, string? name)
    {
        var ret = await _garmentService.GetGarmentsAsync(pageIndex, pageSize, code, name);
        return Ok(ret);
    }

    /// <summary>
    /// 添加工衣
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission("Post-Garment")]
    //[Permission("Garment-mid")]
    public async Task<IActionResult> AddGarment(AddGarmentModel model)
    {
        var ret = await _garmentService.AddGarmentAsync(model);
        return Ok(ret);
    }

    /// <summary>
    /// 修改工衣
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Permission("Update-Garment")]
    //[Permission("Garment-mid")]
    public async Task<IActionResult> UpdateGarment(UpdateGarmentModel model)
    {
        var ret = await _garmentService.UpdateGarmentAsync(model);
        return Ok(ret);
    }

    /// <summary>
    /// 删除工衣
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpDelete]
    [Permission("Delete-Garment")]
    //[Permission("Garment-mid")]
    public async Task<IActionResult> DeleteGarment(string code)
    {
        var ret = await _garmentService.DeleteGarmentAsync(code);
        return Ok(ret);
    }
}

