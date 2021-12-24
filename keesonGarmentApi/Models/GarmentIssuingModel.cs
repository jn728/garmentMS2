namespace keesonGarmentApi.Models;

public class GarmentIssuingListModel
{
    public string GarmentName { get; set; }

    public DateTime BeginTime { get; set; }

    public DateTime EndTime { get; set; }
}

public class AddGarmentIssuingModel
{
    public string GarmentCode { get; set; }

    public DateTime BeginTime => DateTime.Now;

    public DateTime EndTime { get; set; }

    public DateTime CreateTime => DateTime.Now;
}