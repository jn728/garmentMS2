namespace keesonGarmentApi.Entities;

//工衣发放标准
public class GarmentIssuingRole
{
    public Guid Id { get; set; }

    //工衣编号
    public string GarmentId { get; set; }

    //部门Id
    public string DepartmentId { get; set; }

    //职能Id
    public string PositionId { get; set; }

    //新员工发放标准年限
    public int NewEmpAssignedYear { get; set; }

    //新员工发放标准数量
    public int NewEmpAssignedNumber { get; set; }

    //发放标准年限
    public int EmpAssignedYear { get; set; }

    //发放标准数量
    public int EmpAssignedNumber { get; set; }
    
    //备注
    public string Remark { get; set; }

    public DateTime CreateTime { get; set; }

    public string CreateUser { get; set; }
        
    public DateTime UpdateTime { get; set; }

    public string UpdateUser { get; set; }

}