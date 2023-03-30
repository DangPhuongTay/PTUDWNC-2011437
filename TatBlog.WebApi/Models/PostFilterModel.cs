namespace TatBlog.WebApi.Models
{
    public class PostFilterModel:PagingModel

    {
        public string Keyword { get; set; }
        public bool PublishedOnly { get; set; }
        public bool NotPublished { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
    }
}
