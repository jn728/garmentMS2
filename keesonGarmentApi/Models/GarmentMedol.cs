namespace keesonGarmentApi.Models
{
    public class GarmentListModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        //public List<string>? Colors { get; set; }
        public List<Dictionary<string, string>> Colors { get; set; }
    }
    public class AddGarmentModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public decimal Price { get; set; }

        public bool IsClothes { get; set; }

        //public List<string>? Colors { get; set; }
        public string? Colors { get; set; }

        public DateTime CreateTime => DateTime.Now;
    }
    public class UpdateGarmentModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public decimal Price { get; set; }

        //public List<string>? Colors { get; set; }
        public string? Colors { get; set; }
    }
}
