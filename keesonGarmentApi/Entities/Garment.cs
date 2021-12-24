using System.ComponentModel.DataAnnotations;

namespace keesonGarmentApi.Entities;

//工衣
public class Garment
{
    [Key]
    public int Id { get; set; }

    //编号
    public string Code { get; set; }

    //工衣名
    public string Name { get; set; }
        
    //价格
    public decimal Price { get; set; }

    //是否为衣服(1:衣服 0:鞋子)
    public bool IsClothes { get; set; }

    //颜色列表
    public string? Colors { get; set; }

    //备注
    public string? Remark { get; set; }

    public DateTime CreateTime { get; set; }

    public string CreateUser { get; set; }
        
    public DateTime? UpdateTime { get; set; }

    public string? UpdateUser { get; set; }
}