namespace keesonGarmentApi.Entities;

public class GarmentIssuing
{
    public int Id { get; set; }

    //工衣编号
    public string GarmentCode { get; set; }

    //开始时间
    public DateTime BeginTime { get; set; }

    //截止时间
    public DateTime EndTime { get; set; }

    ////文件
    //public string FileName { get; set; }

    public DateTime CreateTime { get; set; }

    public string CreateUser { get; set; }
}

