using keesonGarmentApi.Common;

namespace keesonGarmentApi.Models
{
    public class ResultViewModel
    {
        public HttpStatus Code { get; set; }

        public string Message { get; set; }
    }

    public class ResultsModel<T> : ResultViewModel
    {
        public List<T> Data { get; set; }
    }

    public class ResultModel<T> : ResultViewModel
    {
        public T Data { get; set; }
    }

    public class PageViewModel
    {
        public HttpStatus Code { get; set; }

        public string Message { get; set; }
    }

    public class PageViewModel<T> : PageViewModel
    {
        public List<T> Data { get; set; }

        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPage
        {
            get; set;
        }
    }
}
