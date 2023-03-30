using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;
using TatBlog.Services.Extentions;

namespace TatBlog.Services.Blogs;

public class CategoryRepository : ICategoryRepository
{
    private readonly BlogDbContext _blogContext;
    private readonly IMemoryCache _memoryCache;

    public CategoryRepository(BlogDbContext dbContext, IMemoryCache memoryCache)
    {
        _blogContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Category> categories = _blogContext.Set<Category>().AsNoTracking();

        if (showOnMenu)
        {
            categories = categories.Where(x => x.ShowMenu);
        }

        return await categories.OrderByDescending(x => x.Name)
                              .Select(x => new CategoryItem()
                              {
                                  Id = x.Id,
                                  Name = x.Name,
                                  UrlSlug = x.UrlSlug,
                                  Description = x.Description,
                                  ShowOnMenu = x.ShowMenu,
                                  PostCount = x.Posts.Count(p => p.Published)
                              }).ToListAsync(cancellationToken);
    }

    public async Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _blogContext.Set<Category>()
                                .Where(c => c.UrlSlug.Equals(slug))
                                .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Category> GetCachedCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-slug.{slug}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetCategoryBySlugAsync(slug, cancellationToken);
            });
    }

    public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _blogContext.Set<Category>().FindAsync(id);
    }

    public async Task<Category> GetCachedCategoryByIdAsync(int categoryId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-id.{categoryId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetCategoryByIdAsync(categoryId);
            });
    }

    public async Task<IPagedList<Category>> GetCategoryByQueryAsync(CategoryQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await FilterCategories(query).ToPagedListAsync(
                                pageNumber,
                                pageSize,
                                nameof(Category.Name),
                                "DESC",
                                cancellationToken);
    }

    public async Task<IPagedList<Category>> GetCategoryByQueryAsync(CategoryQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        return await FilterCategories(query).ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<IPagedList<T>> GetCategoryByQueryAsync<T>(CategoryQuery query, IPagingParams pagingParams, Func<IQueryable<Category>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
    {
        IQueryable<T> result = mapper(FilterCategories(query));

        return await result.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<bool> AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category.Id > 0)
            _blogContext.Update(category);
        else
            _blogContext.Add(category);

        var result = await _blogContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> DeleteCategoryByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || _blogContext.Categories == null)
        {
            Console.WriteLine("Không có chuyên mục nào");
            return await Task.FromResult(false);
        }

        var category = await _blogContext.Set<Category>().FindAsync(id);

        if (category != null)
        {
            _blogContext.Categories.Remove(category);

            Console.WriteLine($"Đã xóa chuyên mục với id {id}");
        }

        var result = await _blogContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> CheckCategorySlugExisted(int id, string slug, CancellationToken cancellationToken = default)
    {
        return await _blogContext.Set<Category>().AnyAsync(x => x.Id != id && x.UrlSlug == slug, cancellationToken);
    }

    public async Task ChangeCategoryStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        await _blogContext.Set<Category>()
                          .Where(x => x.Id == id)
                          .ExecuteUpdateAsync(c => c.SetProperty(x => x.ShowMenu, x => !x.ShowMenu), cancellationToken);
    }

    private IQueryable<Category> FilterCategories(CategoryQuery query)
    {
        IQueryable<Category> categoryQuery = _blogContext.Set<Category>()
                                                         .Include(c => c.Posts);

        if (query.ShowOnMenu)
        {
            categoryQuery = categoryQuery.Where(x => x.ShowMenu);
        }

        if (!string.IsNullOrWhiteSpace(query.UrlSlug))
        {
            categoryQuery = categoryQuery.Where(x => x.UrlSlug == query.UrlSlug);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            categoryQuery = categoryQuery.Where(x => x.Name.Contains(query.Keyword) ||
                         x.Description.Contains(query.Keyword) ||
                         x.Posts.Any(p => p.Title.Contains(query.Keyword)));
        }

        return categoryQuery;
    }
}