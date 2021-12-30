using keesonGarmentApi.Filters;
using keesonGarmentApi.Models;
using keesonGarmentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace keesonGarmentApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/log")]
    public class GarmentAssignedLogController : Controller
    {
        private readonly GarmentAssignedLogService _garmentAssignedLogService;

        public GarmentAssignedLogController(GarmentAssignedLogService garmentAssignedLogService)
        {
            _garmentAssignedLogService = garmentAssignedLogService;
        }

        /// <summary>
        /// 获取领取记录
        /// </summary>
        /// <param name="state">状态(0:未提交 1:提交未领取 2:已领取)</param>
        /// <param name="isDelete">是否离职</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页码尺寸</param>
        /// <param name="code">员工号</param>
        /// <param name="name">员工姓名</param>
        /// <param name="department">部门</param>
        /// <param name="postion">工段</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [HttpGet]
        [Permission("Get-Log")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> GetGarmentAssignedLog(int state, int pageIndex, int pageSize, string? code, string? name, string? department, string? postion, DateTime? date, bool isDelete = false)
        {
            var ret = await _garmentAssignedLogService.GetGarmentAssignedLogAsync(state, isDelete, pageIndex, pageSize, code, name,department, postion, date);
            return Ok(ret);
        }

        /// <summary>
        /// 申领工服->全部员工
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面尺寸</param>
        /// <param name="code">员工号</param>
        /// <param name="name">姓名</param>
        /// <param name="department">部门</param>
        /// <param name="postion">工段</param>
        /// <returns></returns>
        [HttpGet("allEmployee")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> GetAllEmployeeLog(int pageIndex, int pageSize, string? code, string? name, string? department, string? postion, DateTime? date)
        {
            var ret = await _garmentAssignedLogService.GetAllEmployeeLogAsync(pageIndex,pageSize,code,name,department,postion,date);
            return Ok(ret);
        }

        /// <summary>
        /// 申请工服->离职员工(暂不使用)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="department"></param>
        /// <param name="postion"></param>
        /// <returns></returns>
        [HttpGet("quitEmployee")]
        public async Task<IActionResult> GetQuitEmployeeLog(int pageIndex, int pageSize, string? code, string? name, string? department, string? postion)
        {
            var ret = await _garmentAssignedLogService.GetQuitEmployeeLogAsync(pageIndex, pageSize, code, name, department, postion);
            return Ok(ret);
        }

        /// <summary>
        /// 后台管理->台账报表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面尺寸</param>
        /// <param name="code">员工号</param>
        /// <param name="name">姓名</param>
        /// <param name="department">部门</param>
        /// <param name="date">入职时间</param>
        /// <returns></returns>
        [HttpGet("singleLogs")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> GetAllEmployeeLog(int pageIndex, int pageSize, string? code, string? name, string? department, DateTime? date)
        {
            var ret = await _garmentAssignedLogService.GetGarmentAssignedLogSingleAsync(pageIndex, pageSize, code, name, department, date);
            return Ok(ret);
        }

        /// <summary>
        /// 自动审核，批量添加记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Permission("Add-LogAll")]
        //[Permission("Garment-hig")]
        public async Task<IActionResult> AddGarmentAssignedLog()
        {
            var ret = await _garmentAssignedLogService.AddGarmentAssignedLogAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 单独添加记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("single")]
        [Permission("Add-LogSingle")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> AddSingleGarmentAssignedLog(AddSingleGarmentAssignedLogModel model)
        {
            var ret = await _garmentAssignedLogService.AddSingleGarmentAssignedLogAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 修改记录(维护)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("maintain")]
        [Permission("Update-LogInfo")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> UpdateGarmentAssignedLog(UpdateGarmentAssignedLogModel model)
        {
            var ret = await _garmentAssignedLogService.UpdateGarmentAssignedLogAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 更新记录状态
        /// </summary>
        /// <param name="model">state为更新后的状态</param>
        /// <returns></returns>
        [HttpPut("state")]
        [Permission("Update-LogState")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> UpdateGarmentAssignedLogState(UpdateGarmentAssignedLogStateModel model)
        {
            var ret = await _garmentAssignedLogService.UpdateGarmentAssignedLogStateAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 快速提交、领取
        /// </summary>
        /// <param name="list">员工号列表</param>
        /// <param name="state">修改后转态(1:提交 2:领取)</param>
        /// <param name="date">领取日期</param>
        /// <returns></returns>
        [HttpPut("fastAssigned")]
        [Permission("Update-LogFastAssigned")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> UpdateFastAssigned(UpdateFastCommitOrAssignedLogModel model)
        {
            var ret = await _garmentAssignedLogService.UpdateFastCommitOrAssignedAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 快速维护
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("fastMaintain")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> UpdateFastMaintainLogState(UpdateFastMaintainLogModel model)
        {
            var ret = await _garmentAssignedLogService.UpdateFastMaintainAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 退还工衣(新增退还记录)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("refund")]
        [Permission("Update-LogRefund")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> RefundGarmentAssignedLog(RefundGarmentAssignedLogModel model)
        {
            var ret = await _garmentAssignedLogService.RefundGarmentAssignedLogAsync(model);
            return Ok(ret);
        }

        /// <summary>
        /// 申请汇总
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        [Permission("Get-LogSummary")]
        //[Permission("Garment-low")]
        public async Task<IActionResult> GetCommitGarmentAssignedLogSummary()
        {
            var ret = await _garmentAssignedLogService.GetGarmentCommitLogSummaryAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 导出台账报表
        /// </summary>
        /// <returns></returns>
        [HttpPost("exportLog")]
        public async Task<IActionResult> ExportLog()
        {
            var ret = await _garmentAssignedLogService.ExportExcelLogAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 导出申请列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("exportApply")]
        public async Task<IActionResult> ExportApply()
        {
            var ret = await _garmentAssignedLogService.ExportExcelApplyAsync();
            return Ok(ret);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件绝对路径</param>
        /// <returns></returns>
        //[HttpGet("downloadFile")]
        //public ActionResult<dynamic> DownloadFiles()
        //{
        //    return File(new FileInfo("D:\\File\\Garment\\申请工服.xlsx").OpenRead(), "application/vnd.android.package-archive", "申请工服");
        //}

        
        //public HttpResponseMessage DownloadFile()
        //{
        //    try
        //    {
        //        var stream = new FileStream("D:\\File\\Garment\\申请工服.xlsx", FileMode.Open);
        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StreamContent(stream);
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = "Wep Api Demo File.zip"
        //        };
        //        return response;
        //    }
        //    catch
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NoContent);
        //    }
        //}
    }
}
