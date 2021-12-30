namespace keesonGarmentApi.Entities
{
    public class GFile
    {
        public int Id { get; set; }

        //文件名
        public string FileName { get; set; }

        //绝对路径
        public string APath { get; set; }

        //相对路径(/Garment/fileName)
        public string RPath { get; set; }

        public DateTime UpdateTime { get; set; }

        public string UpdateUser { get; set; }
    }
}
