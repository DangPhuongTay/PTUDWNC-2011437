namespace TatBlog.WebApi.Models
{
    public class PostDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDecription { get; set; }
        public string Description { get; set; }
        public string Meta { get; set; }
        public string UrlSlug { get; set; }
        public string ImmageUrl { get; set; }
        public int ViewCount { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ModifiedDate { get; set;}
        public CategoryDto category { get; set; }
        public AuthorDto author { get; set; }
        public IList<TagDto> tags { get; set; }
    }
}
