using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.Data.Contexts;
using TatBlog.Core.Contracts;
using TatBlog.Services.Extentions;

namespace TatBlog.Services.Blogs
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<Post> GetPostAsync(
            int year,
            int month,
            string slug,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postsQuery = _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author);

            if (year > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
            }
            if (year > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
            }
            if (!string.IsNullOrEmpty(slug))
            {
                postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
            }
            return await postsQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IList<Post>> GetPopularArticlesAsync(
            int numPost,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .OrderByDescending(x => x.ViewCount)
                .Take(numPost)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsPostSlugExistedAsync(
            int postId, string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
        }

        public async Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1),
                cancellationToken);
        }
        public async Task<IList<CategoryItem>> GetCategoryItemsAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> categories = _context.Set<Category>();
            if (showOnMenu)
            {
                categories = categories.Where(x => x.ShowMenu == showOnMenu);
            }
            return await categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    ShowOnMenu = showOnMenu,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<IPagedList<TagItem>> GetPagedTagAsync(
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>()
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            });
            return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
        }
        public async Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            IQueryable<Tag> tagQuery = _context.Set<Tag>().Include(i => i.Posts);

            if (!string.IsNullOrWhiteSpace(slug))
            {
                tagQuery = tagQuery.Where(x => x.UrlSlug == slug);
            }

            return await tagQuery.FirstOrDefaultAsync(cancellationToken);
        }
        public IQueryable<Post> FilterPosts(PostQuery postQuery)
        {
            IQueryable<Post> posts = _context.Set<Post>()
                .Include(x=>x.Tags)
                .Include(x => x.Category)
                .Include(x => x.Author);

            if (postQuery.CategoryId > 0)
            {
                posts = posts.Where(x => x.CategoryId == postQuery.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(postQuery.CategorySlug))
            {
                posts = posts.Where(x => x.Category.UrlSlug == postQuery.CategorySlug);
            }

            if (postQuery.AuthorId > 0)
            {
                posts = posts.Where(x => x.AuthorId == postQuery.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(postQuery.KeyWord))
            {
                posts = posts.Where(x => x.Title.Contains(postQuery.KeyWord) ||
                                         x.ShortDescription.Contains(postQuery.KeyWord) ||
                                         x.Description.Contains(postQuery.KeyWord) ||
                                         x.Category.Name.Contains(postQuery.KeyWord) ||
                                         x.Tags.Any(t => t.Name.Contains(postQuery.KeyWord)));
            }

            if (postQuery.Year > 0)
            {
                posts = posts.Where(x => x.PostedDate.Year == postQuery.Year);
            }

            if (postQuery.Month > 0)
            {
                posts = posts.Where(x => x.PostedDate.Month == postQuery.Month);
            }
            if (postQuery.PublishedOnly)
            {
                posts = posts.Where(x => x.Published);
            }
            if (!string.IsNullOrWhiteSpace(postQuery.AuthorSlug))
            {
                posts = posts.Where(x => x.Author.UrlSlug == postQuery.AuthorSlug);
            }

            if (!string.IsNullOrWhiteSpace(postQuery.TagSlug))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == postQuery.TagSlug));
            }
            if (!string.IsNullOrWhiteSpace(postQuery.CategorySlug))
            {
                posts = posts.Where(x => x.Category.UrlSlug == postQuery.CategorySlug);
            }
            return posts;
        }

        public async Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery postQuery,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        {
            return await FilterPosts(postQuery).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
        }

       
       
    }
}