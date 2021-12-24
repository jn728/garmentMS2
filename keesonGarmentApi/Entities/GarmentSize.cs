namespace keesonGarmentApi.Entities;

//工衣尺码
public class GarmentSize
{
    public Guid Id { get; set; }

    //员工号
    public string UserCode { get; set; }

    //工衣尺码
    public string? ClothesSize { get; set; }

    //工鞋尺码
    public string? ShoesSize { get; set; }
    
    public DateTime CreateTime { get; set; }

    public string CreateUser { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UpdateUser { get; set; }
}