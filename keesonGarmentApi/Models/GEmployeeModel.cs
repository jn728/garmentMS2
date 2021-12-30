using Magicodes.ExporterAndImporter.Core;

namespace keesonGarmentApi.Models;
public class GEmployeeListModel
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public DateTime Induction { get; set; }

    public string ClothesSize { get; set; }

    public string ShoesSize { get; set; }
}

public class AddGEmployeeModel
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public string Induction { get; set; }

    public string? ClothesSize { get; set; }

    public string? ShoesSize { get; set; }

    public DateTime CreateTime => DateTime.Now;
}

public class UpdateGEmployeeModel
{
    public string Code { get; set; }

    public string Postion { get; set; }

    public string? ClothesSize { get; set; }

    public string? ShoesSize { get; set; }
}

public class ImportEmployeeModel
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Department { get; set; }

    public string Postion { get; set; }

    public DateTime Induction { get; set; }

    public string? ClothesSize { get; set; }

    public string? ShoesSize { get; set; }

    public int Num1 { get; set; } = 0;
    public DateTime? Date1 { get; set; }
    public int Num2 { get; set; } = 0;
    public DateTime? Date2 { get; set; }
    public int Num3 { get; set; } = 0;
    public DateTime? Date3 { get; set; }
    public int Num4 { get; set; } = 0;
    public DateTime? Date4 { get; set; }
    public int Num5 { get; set; } = 0;
    public DateTime? Date5 { get; set; }
    public int Num6 { get; set; } = 0;
    public DateTime? Date6 { get; set; }
    public int Num7 { get; set; } = 0;
    public DateTime? Date7 { get; set; }
}