namespace keesonGarmentApi.Entities;

//工衣领取记录
public class GarmentAssignedLog
{
    public Guid Id { get; set; }

    //工衣编号
    public string GarmentId { get; set; }

    //员工号
    public string UserId { get; set; }

    //实际申领尺码
    public string? Size { get; set; }

    //数量
    public int Number { get; set; }

    //状态(0:未提交 1:未领取 2:已领取 3:删除)
    public int State { get; set; }

    //领取时间
    public DateTime? AssignedTime { get; set; }

    //退还时间
    public DateTime? RefundTime { get; set; }

    //方式(1:领取 0:退还)
    public bool Type { get; set; }

    //领子颜色(红、黄、蓝、null)
    public string? Color { get; set; }

    //操作时间
    public DateTime OperationTime { get; set; }

    //备注
    public string? Remark { get; set; }

    public DateTime CreateTime { get; set; }

    public string CreateUser { get; set; }
        
    public DateTime? UpdateTime { get; set; }

    public string? UpdateUser { get; set; }
    
}