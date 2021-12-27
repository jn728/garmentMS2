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
    /// ��ȡ�����б�
    /// </summary>
    /// <param name="pageIndex">ҳ��</param>
    /// <param name="pageSize">ҳ��ߴ�</param>
    /// <param name="code">�������</param>
    /// <param name="name">��������</param>
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
    /// ��ӹ���
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
    /// �޸Ĺ���
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
    /// ɾ������
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

