using MapsterMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
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
        //        CategoryId =model.CategoryId,
        //        AuthorId = model.AuthorId,
        //        Year = model.Year,
        //        Month = model.Mouth
        //    };
        //    ViewBag.PostsList = await _blogRepository
        //        .GetPagedPostsAsync(postQuery, 1, 5);
        //    await PopulatePostFilterModelAsync(model);
        //    return View(model);
        //}
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        
        private readonly IMapper _mapper;
        public PostsController(IBlogRepository blogRepository,IMediaManager mediaManager, IMapper mapper)
        {
            _blogRepository = blogRepository;
            _mediaManager = mediaManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(PostFilterModel model)
        {
            var postQuery = _mapper.Map<PostQuery>(model);

            ViewBag.PostsList = await _blogRepository

                .GetPagedPostsAsync(postQuery, 1, 5);

            await PopulatePostFilterModelAsync(model);

            return View(model);
        }
        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
        {
            var authors = await _blogRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();
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
        private async Task PopulatePostEditModelAsync(PostEditModel model)
        {
            var authors = await _blogRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();

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
        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var post = id > 0
                ? await _blogRepository.GetPostByIdAsync(id, true)
                : null;

            var model = post == null
                ? new PostEditModel()
                : _mapper.Map<PostEditModel>(post);

            await PopulatePostEditModelAsync(model);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PostEditModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePostEditModelAsync(model);
                return View(model);
            }
            var post = model.Id > 0
                ? await _blogRepository.GetPostByIdAsync(model.Id)
                : null;
            if (post == null)
            {
                post = _mapper.Map<Post>(model);

                post.Id = 0;
                post.PostedDate = DateTime.Now;
            }
            else
            {
                _mapper.Map(model, post);

                post.Category = null;
                post.ModifiedDate = DateTime.Now;

            }
            if(model.ImageFile?.Length > 0)
            {
                var newImagePath = await _mediaManager.SaveFileAsync(
                    model.ImageFile.OpenReadStream(),
                    model.ImageFile.FileName,
                    model.ImageFile.ContentType
                    );
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    await _mediaManager.DeleteFileAsync(post.ImageUrl);
                    post.ImageUrl = newImagePath;
                }
            }
            
            await _blogRepository.CreateOrUpdatePostAsync(
                post, model.GetSelectedTags());

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> VerifyPostSlug(
            int id, string urlSlug
            )
        {
            var slugExited = await _blogRepository
                .IsPostSlugExistedAsync(id,urlSlug);
            return slugExited
                    ? Json($"Slug '{urlSlug}' đã được sử dụng")
                    : Json(true);
        }

    }
}
