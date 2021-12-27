namespace keesonGarmentApi.Models;

public class GarmentAllEmployeeLogListModel
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public string ClothesSize { get; set; }

    public string ShoesSize { get; set; }

    public DateTime Induction { get; set; }

    //public string Number { get; set; }

    //public string Size { get; set; }

    public bool IsPass { get; set; }

    public List<SingleLog> Logs { get; set; }
}

public class GarmentQuitEmployeeLogListModel
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public string ClothesSize { get; set; }

    public string ShoesSize { get; set; }

    public DateTime Induction { get; set; }

    public string GarmentName { get; set; }

    public int Number { get; set; }

    public string Size { get; set; }
}

public class GarmentAssignedLogSingleListModel
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public string ClothesSize { get; set; }

    public string ShoesSize { get; set; }

    public DateTime Induction { get; set; }

    public int Number { get; set; }

    public decimal? Fare { get; set; } = 0;

    public DateTime? AssigedOrRefundTime { get; set; }

    public bool Type { get; set; }

    public string GarmentName { get; set; }

    public string Remark { get; set; }

    public DateTime OperationTime { get; set; }
}

public class GarmentAssignedLogListModel
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public string ClothesSize { get; set; }

    public string ShoesSize { get; set; }

    public DateTime Induction { get; set; }

    public decimal? AllFare { get; set; } = 0;

    public List<SingleLog> Logs { get; set; }
}

public class SingleLog
{
    public string Number { get; set; }

    public string Size { get; set; }

    public decimal? Fare { get; set; } = 0;

    public DateTime? AssigedOrRefundTime { get; set; }

    public bool Type { get; set; }

    public string GarmentName { get; set; }

    public string GarmentCode { get; set; }

    public string? Color { get; set; } = "none";

    public string Remark { get; set; }

    public DateTime OperationTime { get; set; }
}

public class GarmentSummaryConvertModel
{
    public List<Dictionary<string, string>> DataList { get; set; }

    public List<Dictionary<string, string>> TitleList { get; set; }
}

public class GarmentCommitLogSummaryListModel
{
    public string Department { get; set; }

    public List<SingleSummary> List { set; get; }
}

public class SingleSummary
{
    public string GarmentName { get; set; }

    public string Size { set; get; }

    public string Color { get; set; }

    public int Number { get; set; }
}

public class AddSingleGarmentAssignedLogModel
{
    public string GarmentId { get; set; }

    public string? Color { get; set; } = "none";

    public string UserId { get; set; }

    public int Number { get; set; }

    public string Size { get; set; }

    public string? Remark { get; set; }

    public DateTime OperationTime => DateTime.Now;

    public DateTime CreateTime => DateTime.Now;
}

public class AddGarmentAssignedLogBeforeCommitModel
{
    public string GarmentId { get; set; }

    public string UserId { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public int Number { get; set; }

    public bool Type { get; set; }

    public string? Size { get; set; }

    public int State { get; set; }

    public string? Color { get; set; } = "none";

    public DateTime OperationTime => DateTime.Now;

    public string CreateUser { get; set; }

    public DateTime CreateTime => DateTime.Now;

    public DateTime Induction { get; set; }
}

public class UpdateGarmentAssignedLogModel
{
    public string UserId { get; set; }

    public string GarmentId { get; set; }

    public int Number { get; set; }

    public string Size { get; set; }

    public string? Color { get; set; } = "none";

    public string? Remark { get; set; }
}

public class UpdateGarmentAssignedLogStateModel
{
    public string UserId { get; set; }

    public string GarmentId { get; set; }

    public int State { get; set; }

    public bool IsAssigned { get; set; } = true;
    
    public DateTime Date { get; set; } = DateTime.Today;
}

public class UpdateFastCommitOrAssignedLogModel
{
    public List<string> List { get; set; }

    public int State { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
}

public class UpdateFastMaintainLogModel
{
    public List<string> List { get; set; }

    public string GarmentCode { get; set; }

    public int Number { get; set; }

    public string? Color { get; set; }
}

public class RefundGarmentAssignedLogModel
{
    public string UserId { get; set; }

    public string GarmentId { get; set; }

    public DateTime AssignedTime { get; set; }

    public DateTime Date { get; set; }

    public int Number { get; set; }
}