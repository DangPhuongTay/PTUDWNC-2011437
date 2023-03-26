
using TatBlog.Core.DTO;
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
    public class CategoriesController: Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IBlogRepository _categoryRepository;
        private readonly IMapper _mapper;
      

        public CategoriesController(ILogger<PostsController> logger, IBlogRepository categoryRepository, IMapper mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
          
        }

        public async Task<IActionResult> Index(CategoryFilterModel model,
                                              [FromQuery(Name = "p")] int pageNumber = 1,
                                              [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var categoryQuery = _mapper.Map<CategoryQuery>(model);

            // _logger.LogInformation("Lấy danh sách bài viết từ CSDL");

            ViewData["CategoriesList"] = await _categoryRepository.GetCategoryByQueryAsync(categoryQuery, pageNumber, pageSize);

            ViewData["PagerQuery"] = new PagerQuery
            {
                Area = "Admin",
                Controller = "Categories",
                Action = "Index",
            };

            return View(model);
        }
    }
}
