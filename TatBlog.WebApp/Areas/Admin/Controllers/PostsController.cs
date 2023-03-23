using FluentValidation;
using FluentValidation.AspNetCore;
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
        private readonly ILogger<PostsController> _logger;
        private readonly IMediaManager _mediaManager;
        
        private readonly IMapper _mapper;
        public PostsController(ILogger<PostsController> logger ,IBlogRepository blogRepository,IMediaManager mediaManager, IMapper mapper)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _mediaManager = mediaManager;
            _mapper = mapper;
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
        public async Task<IActionResult> Edit([FromServices] IValidator<PostEditModel> PostValidator, PostEditModel model)
        {
            var validationResult = await PostValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }
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
        public async Task<IActionResult> Index(PostFilterModel model)
        {
            _logger.LogInformation("Tạo điều kiện truy vấn");
            var postQuery = _mapper.Map<PostQuery>(model);
            _logger.LogInformation("lấy danh sách bài viết từ CSDL");
            ViewBag.PostsList = await _blogRepository.GetPagedPostsAsync(postQuery,1,5);
            _logger.LogInformation("Chuẩn bị dữ liệu cho ViewModel");
            await PopulatePostFilterModelAsync(model);
            return View(model);
        }
    }
}
