namespace keesonGarmentApi.Entities
{
    public class GEmployee
    {
        public Guid Id { get; set; }

        public string Code { get; set; }
        
        public string Name { get; set; }

        public string Department { get; set; }

        public string Postion { get; set; }

        public DateTime Induction { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string? UpdateUser { get; set; }
    }
}
