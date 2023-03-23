using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
public class Featured : ViewComponent
{
    private readonly IBlogRepository _blogRepository;

    public Featured(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var posts = await _blogRepository.GetFeaturedAsync(3);

        return View(posts);
    }
}}