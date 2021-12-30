namespace keesonGarmentApi.Models;

public class GarmentIssuingRuleListModel
{
    public string GarmentId { get; set; }

    public string GarmentName { get; set;}

    public string DepartmentId { get; set; }

    public string PositionId { get; set; }

    public int NewEmpAssignedYear { get; set; }

    public int NewEmpAssignedNumber { get; set; }

    public int EmpAssignedYear { get; set; }

    public int EmpAssignedNumber { get; set; }

    public string? Remark { get; set; }
}

public class AddGarmentIssuingRuleModel
{
    public string GarmentId { get; set; }

    public string DepartmentId { get; set; }

    public string PositionId { get; set; }

    public int NewEmpAssignedYear { get; set; }

    public int NewEmpAssignedNumber { get; set; }

    public int EmpAssignedYear { get; set; }

    public int EmpAssignedNumber { get; set; }

    public string? Remark { get; set; }

    public DateTime CreateTime => DateTime.Now;
}

public class IssuingRuleImportModel
{
    public string Gid { get; set; }
    public string Dep { get; set; }
    public string Pos { get; set; }
    public int Year1 { get; set; }
    public int Num1 { get; set; }
    public int Year2 { get; set; }
    public int Num2 { get; set; }
}