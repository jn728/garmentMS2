using Magicodes.ExporterAndImporter.Core;

namespace keesonGarmentApi.Models
{
    public class ExportExcelLogModel
    {
        [ExporterHeader(DisplayName = "姓名")]
        public string UserName { get; set; }

        [ExporterHeader(DisplayName = "工号")]
        public string UserId { get; set; }

        [ExporterHeader(DisplayName = "部门")]
        public string Department { get; set; }

        [ExporterHeader(DisplayName = "工段")]
        public string Postion { get; set; }

        [ExporterHeader(DisplayName = "入职时间", Format = "yyyy/MM/dd")]
        public DateTime Induction { get; set; }

        [ExporterHeader(DisplayName = "工衣尺码")]
        public string ClothesSize { get; set; }

        [ExporterHeader(DisplayName = "工鞋尺码")]
        public string ShoesSize { get; set; }

        [ExporterHeader(DisplayName = "方式")]
        public string Type { get; set; }

        [ExporterHeader(DisplayName = "申请种类")]
        public string GarmentName { get; set; }

        [ExporterHeader(DisplayName = "数量")]
        public int Number { get; set; }

        [ExporterHeader(DisplayName = "领取/退还时间", Format = "yyyy/MM/dd")]
        public DateTime? AssigedOrRefundTime { get; set; }

        [ExporterHeader(DisplayName = "操作时间", Format = "yyyy/MM/dd")]
        public DateTime OperationTime { get; set; }

        [ExporterHeader(DisplayName = "费用结算")]
        public decimal Fare { get; set; }
    }

    public class ExportExcelApplyModel
    {
        [ExporterHeader(DisplayName = "姓名")]
        public string UserName { get; set; }

        [ExporterHeader(DisplayName = "工号")]
        public string UserId { get; set; }

        [ExporterHeader(DisplayName = "部门")]
        public string Department { get; set; }

        [ExporterHeader(DisplayName = "工段")]
        public string Postion { get; set; }

        [ExporterHeader(DisplayName = "入职时间", Format = "yyyy/MM/dd")]
        public DateTime Induction { get; set; }

        [ExporterHeader(DisplayName = "工衣尺码")]
        public string ClothesSize { get; set; }

        [ExporterHeader(DisplayName = "工鞋尺码")]
        public string ShoesSize { get; set; }

        [ExporterHeader(DisplayName = "申请种类")]
        public string GarmentName { get; set; }

        [ExporterHeader(DisplayName = "数量")]
        public int Number { get; set; }

        [ExporterHeader(DisplayName = "尺码")]
        public string Size { get; set; }
    }
}
