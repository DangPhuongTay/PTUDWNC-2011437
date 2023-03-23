using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        public async Task<IActionResult> Index(
       [FromQuery(Name = "k")] string keyword = null,
       [FromQuery(Name = "p")] int pageNumber = 1,
       [FromQuery(Name = "ps")] int pageSize = 2)
        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
                KeyWord = keyword
            };
            var postsList = await _blogRepository
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            

            return View(postsList);
        }
  
        public IActionResult Contact() => View();
        public IActionResult About() => View();
        public async Task<IActionResult> Author(
                            string slug = null)
        {
            if (slug == null) return NotFound();

            var postQuery = new PostQuery
            {
                AuthorSlug = slug
            };

            var posts = await _blogRepository.GetPostByQueryAsync(postQuery);

            return View(posts);
        }
        public IActionResult Rss() => Content("Nội dung sẽ được cập nhật");
        public async Task<IActionResult> Post(
                            int year = 2023,
                            int month = 1,
                            int day = 1,
                            string slug = null)
        {
            if (slug == null) return NotFound();

            var post = await _blogRepository.GetPostAsync(year, month, day, slug);

            if (post == null) return Content("Không tìm thấy bài viết nào");

         

            return View(post);
        }

        
        public async Task<IActionResult> Category(
                           string slug = null)
        {
            if (slug == null) return NotFound();

            var postQuery = new PostQuery
            {
                CategorySlug = slug
            };

            var posts = await _blogRepository.GetPostByQueryAsync(postQuery);

            return View(posts);
        }
        public async Task<IActionResult> Tag(
                          string slug = null)
        {
            if (slug == null) return NotFound();

            var postQuery = new PostQuery
            {
                TagSlug = slug
            };

            var posts = await _blogRepository.GetPostByQueryAsync(postQuery);
            var tag = await _blogRepository.GetTagBySlugAsync(slug);

            ViewData["Tag"] = tag;

            return View(posts);
        }

    }
}
