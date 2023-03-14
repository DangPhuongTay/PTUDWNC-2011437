using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class PostsController : Controller
    {



        //public async Task<IActionResult> Index(PostFilterModel model)
        //{
        //    var postQuery = new PostQuery()
        //    {
        //        KeyWord = model.Keyword,
        //        CategoryId = model.CategoryId,
        //        AuthorId = model.AuthorId,
        //        Year = model.Year,
        //        Month = model.Mouth
        //    };
        //    ViewBag.PostsList = await _blogRepository
        //        .GetPagedPostsAsync(postQuery,1,5);
        //    await PopulatePostFilterModelAsync(model);
        //    return View(model);
        //}
        private readonly IBlogRepository _blogRepository;
        //public PostsController(IBlogRepository blogRepository)
        //{
        //    _blogRepository = blogRepository;
        //}
        
        private readonly IMapper _mapper;
        public PostsController(IBlogRepository blogRepository, IMapper mapper)
        {
            _blogRepository = blogRepository;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index (PostFilterModel model)
        {
            var postQuery = _mapper.Map<PostQuery>(model);
            ViewBag.PostQuery = await _blogRepository
                .GetPagedPostsAsync(postQuery,1,5);
            await PopulatePostFilterModelAsync(model);
            return View();
        }
        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
        {
            var authors = await _blogRepository.GetAuthorItemsAsync();
            var categories = await _blogRepository.GetCategoryItemsAsync();
            model.AuthorList = authors.Select(a => new SelectListItem()
            {
                Text = a.FullName,
                Value = a.Id.ToString()
            });
            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

        }
    }
}
