using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extentions;
using TatBlog.Core.Contracts;
using System.Text.RegularExpressions;

namespace TatBlog.Services.Blogs
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogDbContext _context;

        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<Post> GetPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postsQuery = _context.Set<Post>()
                                                      .Include(x => x.Category)
                                                      .Include(x => x.Author)
                                                      .Include(x => x.Tags);

            if (year > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
            }

            if (month > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
            }

            if (day > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Day == day);
            }

            if (!string.IsNullOrWhiteSpace(slug))
            {
                postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
            }

            return await postsQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IList<Post>> GetFeaturedAsync(int numPosts, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                                     .Include(x => x.Author)
                                     .Include(x => x.Category)
                                     .OrderByDescending(p => p.ViewCount)
                                     .Take(numPosts)
                                     .ToListAsync(cancellationToken);
        }
        //    public async Task<bool> IsPostSlugExistedAsync(
        //        int postId, string slug,
        //        CancellationToken cancellationToken = default)
        //    {
        //        return await _context.Set<Post>()
        //            .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
        //    }

        //    public async Task IncreaseViewCountAsync(
        //        int postId,
        //        CancellationToken cancellationToken)
        //    {
        //        await _context.Set<Post>()
        //            .Where(x => x.Id == postId)
        //            .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1),
        //            cancellationToken);
        //    }
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
        public async Task<IList<AuthorItem>> GetAuthorItemsAsync(
            CancellationToken cancellationToken = default)
        {
            IQueryable<Author> author = _context.Set<Author>();
            return await author
            .OrderBy(x => x.FullName)
            .Select(x => new AuthorItem()
            {
                Id = x.Id,
                FullName = x.FullName,
                UrlSlug = x.UrlSlug,
                Email = x.Email,
                ImageUrl = x.ImageUrl,
                JoinedDate = x.JoinedDate,
                Notes = x.Notes,
                PostCount = x.Posts.Count(p => p.Published)
            }).ToListAsync(cancellationToken);

        }
        //    public async Task<Category> GetCategoryAsync(
        //          string slug, CancellationToken cancellationToken = default)
        //    {
        //        return await _context.Set<Category>()
        //            .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
        //    }
        //    public async Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default)
        //    {
        //        return await _context.Set<Author>()
        //            .OrderBy(a => a.FullName)
        //            .Select(a => new AuthorItem()
        //            {
        //                Id = a.Id,
        //                FullName = a.FullName,
        //                Email = a.ToString(),
        //                JoinedDate = a.JoinedDate,
        //                ImageUrl = a.ImageUrl,
        //                UrlSlug = a.UrlSlug,
        //                Notes = a.Notes,
        //                PostCount = a.Posts.Count(p => p.Published)
        //            })
        //            .ToListAsync(cancellationToken);
        //    }
        //    public async Task<IList<CategoryItem>> GetCategoriesAsync(
        //    bool showOnMenu = false,
        //    CancellationToken cancellationToken = default)
        //    {
        //        IQueryable<Category> categories = _context.Set<Category>();

        //        if (showOnMenu)
        //        {
        //            categories = categories.Where(x => x.ShowMenu);
        //        }

        //        return await categories
        //            .OrderBy(x => x.Name)
        //            .Select(x => new CategoryItem()
        //            {
        //                Id = x.Id,
        //                Name = x.Name,
        //                UrlSlug = x.UrlSlug,
        //                Description = x.Description,
        //                ShowOnMenu = x.ShowMenu,
        //                PostCount = x.Posts.Count(p => p.Published)
        //            })
        //            .ToListAsync(cancellationToken);
        //    }
        //    public async Task<Author> GetAuthorAsync(string slug, CancellationToken cancellationToken = default)
        //    {
        //        return await _context.Set<Author>()
        //            .FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
        //    }

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
        //    private IQueryable<Post> FilterPosts(PostQuery condition)
        //    {
        //        IQueryable<Post> posts = _context.Set<Post>()
        //            .Include(x => x.Category)
        //            .Include(x => x.Author)
        //            .Include(x => x.Tags);
        //        posts.ToList();

        //        if (condition.PublishedOnly)
        //        {
        //            posts = posts.Where(x => x.Published == true);
        //        }

        //        if (condition.NotPublished)
        //        {
        //            posts = posts.Where(x => !x.Published);
        //        }

        //        if (condition.CategoryId > 0)
        //        {
        //            posts = posts.Where(x => x.CategoryId == condition.CategoryId);
        //        }

        //        if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
        //        {
        //            posts = posts.Where(x => x.Category.UrlSlug == condition.CategorySlug);
        //        }

        //        if (condition.AuthorId > 0)
        //        {
        //            posts = posts.Where(x => x.AuthorId == condition.AuthorId);
        //        }

        //        if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
        //        {
        //            posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
        //        }

        //        if (!string.IsNullOrWhiteSpace(condition.TagSlug))
        //        {
        //            posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
        //        }

        //        if (!string.IsNullOrWhiteSpace(condition.Tag))
        //        {
        //            posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.Tag));
        //        }

        //        if (!string.IsNullOrWhiteSpace(condition.KeyWord))
        //        {
        //            posts = posts.Where(x => x.Title.Contains(condition.KeyWord) ||
        //                                     x.ShortDescription.Contains(condition.KeyWord) ||
        //                                     x.Description.Contains(condition.KeyWord) ||
        //                                     x.Category.Name.Contains(condition.KeyWord) ||
        //                                     x.Tags.Any(t => t.Name.Contains(condition.KeyWord)));
        //        }

        //        if (condition.Year > 0)
        //        {
        //            posts = posts.Where(x => x.PostedDate.Year == condition.Year);
        //        }

        //        if (condition.Month > 0)
        //        {
        //            posts = posts.Where(x => x.PostedDate.Month == condition.Month);
        //        }

        //        if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
        //        {
        //            posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
        //        }

        //        return posts;
        //    }
        //    public async Task<Post> GetPostByIdAsync(
        //      int postId, bool includeDetails = false,
        //      CancellationToken cancellationToken = default)
        //    {
        //        if (!includeDetails)
        //        {
        //            return await _context.Set<Post>().FindAsync(postId);
        //        }

        //        return await _context.Set<Post>()
        //            .Include(x => x.Category)
        //            .Include(x => x.Author)
        //            .Include(x => x.Tags)
        //            .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
        //    }

        //    public async Task<IPagedList<Post>> GetPagedPostsAsync(
        //    PostQuery postQuery,
        //    int pageNumber = 1,
        //    int pageSize = 5,
        //    CancellationToken cancellationToken = default)
        //    {
        //        return await FilterPosts(postQuery).ToPagedListAsync(
        //            pageNumber, pageSize,
        //            nameof(Post.PostedDate), "DESC",
        //            cancellationToken);
        //    }
        //    public async Task<Post> CreateOrUpdatePostAsync(
        //        Post post, IEnumerable<string> tags,
        //    CancellationToken cancellationToken = default)
        //    {
        //        if (post.Id > 0)
        //        {
        //            await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
        //        }
        //        else
        //        {
        //            post.Tags = new List<Tag>();
        //        }

        //        var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
        //            .Select(x => new
        //            {

        //                Name = x,
        //    Slug = x.GenerateSlug()
        //            })
        //            .GroupBy(x => x.Slug)
        //            .ToDictionary(g => g.Key, g => g.First().Name);


        //        foreach (var kv in validTags)
        //        {
        //            if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

        //            var tag = await GetTagAsync(kv.Key, cancellationToken) ?? new Tag()
        //            {
        //                Name = kv.Value,
        //                Description = kv.Value,
        //                UrlSlug = kv.Key
        //            };

        //            post.Tags.Add(tag);
        //        }

        //        post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

        //        if (post.Id > 0)
        //            _context.Update(post);
        //        else
        //            _context.Add(post);

        //        await _context.SaveChangesAsync(cancellationToken);

        //        return post;
        //    }

        //    public async Task<Tag> GetTagAsync(
        //string slug, CancellationToken cancellationToken = default)
        //    {
        //        return await _context.Set<Tag>()
        //            .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
        //    }

        //}

        // Tìm Top N bài viết phổ được nhiều người xem nhất
        public async Task<IList<Post>> GetPopularArticlesAsync(
            int numPosts, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .OrderByDescending(p => p.ViewCount)
                .Take(numPosts)
                .ToListAsync(cancellationToken);
        }

        // Tăng số lượt xem của một bài viết
        public async Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postId)
                .ExecuteUpdateAsync(p =>
                        p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1),
                    cancellationToken);
        }

        public async Task<Author> GetAuthorAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Set<Author>().FindAsync(authorId);
        }

        public async Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .OrderBy(a => a.FullName)
                .Select(a => new AuthorItem()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.ToString(),
                    JoinedDate = a.JoinedDate,
                    ImageUrl = a.ImageUrl,
                    UrlSlug = a.UrlSlug,
                    Notes = a.Notes,
                    PostCount = a.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<Post>> GetPostsAsync(
            PostQuery condition,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition)
                .OrderByDescending(x => x.PostedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<int> CountPostsAsync(
            PostQuery condition, CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition).CountAsync(cancellationToken: cancellationToken);
        }

        //public async Task<IList<MonthlyPostCountItem>> CountMonthlyPostsAsync(
        //    int numMonths, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<Post>()
        //        .GroupBy(x => new { x.PostedDate.Year, x.PostedDate.Month })
        //        .Select(g => new MonthlyPostCountItem()
        //        {
        //            Year = g.Key.Year,
        //            Month = g.Key.Month,
        //            PostCount = g.Count(x => x.Published)
        //        })
        //        .OrderByDescending(x => x.Year)
        //        .ThenByDescending(x => x.Month)
        //        .ToListAsync(cancellationToken);
        //}

        public async Task<Category> GetCategoryAsync(
            string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Set<Category>().FindAsync(categoryId);
        }

        public async Task<IList<CategoryItem>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> categories = _context.Set<Category>();

            if (showOnMenu)
            {
                categories = categories.Where(x => x.ShowMenu);
            }

            return await categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    ShowOnMenu = x.ShowMenu,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var categoriesQuery = _context.Set<Category>()
                .Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    ShowOnMenu = x.ShowMenu,
                    PostCount = x.Posts.Count(p => p.Published)
                });
            return await categoriesQuery.ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Category.Name), "DESC",
                cancellationToken);
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
        private IQueryable<Category> FilterCategories(CategoryQuery query)
        {
            IQueryable<Category> categoryQuery = _context.Set<Category>()
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
        public async Task<Category> CreateOrUpdateCategoryAsync(
            Category category, CancellationToken cancellationToken = default)
        {
            if (category.Id > 0)
            {
                _context.Set<Category>().Update(category);
            }
            else
            {
                _context.Set<Category>().Add(category);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return category;
        }

        public async Task<bool> IsCategorySlugExistedAsync(
            int categoryId, string categorySlug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                .AnyAsync(x => x.Id != categoryId && x.UrlSlug == categorySlug, cancellationToken);
        }

        public async Task<bool> DeleteCategoryAsync(
            int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _context.Set<Category>().FindAsync(categoryId);

            if (category is null) return false;

            _context.Set<Category>().Remove(category);
            var rowsCount = await _context.SaveChangesAsync(cancellationToken);

            return rowsCount > 0;
        }


        public async Task<Tag> GetTagAsync(
            string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
        }

        public async Task<IList<TagItem>> GetTagsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                .OrderBy(x => x.Name)
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
            IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>()
                .OrderBy(x => x.Name)
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

        public async Task<bool> DeleteTagAsync(
            int tagId, CancellationToken cancellationToken = default)
        {
            //var tag = await _context.Set<Tag>().FindAsync(tagId);

            //if (tag == null) return false;

            //_context.Set<Tag>().Remove(tag);
            //return await _context.SaveChangesAsync(cancellationToken) > 0;

            return await _context.Set<Tag>()
                .Where(x => x.Id == tagId)
                .ExecuteDeleteAsync(cancellationToken) > 0;
        }

        public async Task<bool> CreateOrUpdateTagAsync(
            Tag tag, CancellationToken cancellationToken = default)
        {
            if (tag.Id > 0)
            {
                _context.Set<Tag>().Update(tag);
            }
            else
            {
                _context.Set<Tag>().Add(tag);
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }


        public async Task<Post> GetPostAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = false,
                TitleSlug = slug
            };

            return await FilterPosts(postQuery).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Post> GetPostByIdAsync(
            int postId, bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            if (!includeDetails)
            {
                return await _context.Set<Post>().FindAsync(postId);
            }

            return await _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
        }

        public async Task<bool> TogglePublishedFlagAsync(
            int postId, CancellationToken cancellationToken = default)
        {
            var post = await _context.Set<Post>().FindAsync(postId);

            if (post is null) return false;

            post.Published = !post.Published;
            await _context.SaveChangesAsync(cancellationToken);

            return post.Published;
        }

        public async Task<IList<Post>> GetRandomArticlesAsync(
            int numPosts, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .OrderBy(x => Guid.NewGuid())
                .Take(numPosts)
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<Post>> GetPagedPostsAsync(
            PostQuery condition,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
        }

        public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
            PostQuery condition,
            IPagingParams pagingParams,
            Func<IQueryable<Post>, IQueryable<T>> mapper)
        {
            var posts = FilterPosts(condition);
            var projectedPosts = mapper(posts);

            return await projectedPosts.ToPagedListAsync(pagingParams);
        }

        public async Task<Post> CreateOrUpdatePostAsync(
            Post post, IEnumerable<string> tags,
            CancellationToken cancellationToken = default)
        {
            if (post.Id > 0)
            {
                await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
            }
            else
            {
                post.Tags = new List<Tag>();
            }

            var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new
                {
                    Name = x,
                    Slug = x.GenerateSlug()
                })
                .GroupBy(x => x.Slug)
                .ToDictionary(g => g.Key, g => g.First().Name);


            foreach (var kv in validTags)
            {
                if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

                var tag = await GetTagAsync(kv.Key, cancellationToken) ?? new Tag()
                {
                    Name = kv.Value,
                    Description = kv.Value,
                    UrlSlug = kv.Key
                };

                post.Tags.Add(tag);
            }

            post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

            if (post.Id > 0)
                _context.Update(post);
            else
                _context.Add(post);

            await _context.SaveChangesAsync(cancellationToken);

            return post;
        }

        public async Task<bool> IsPostSlugExistedAsync(
            int postId, string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
        }

        private IQueryable<Post> FilterPosts(PostQuery condition)
        {
            IQueryable<Post> posts = _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Tags);

            if (condition.PublishedOnly)
            {
                posts = posts.Where(x => x.Published);
            }

            if (condition.NotPublished)
            {
                posts = posts.Where(x => !x.Published);
            }

            if (condition.CategoryId > 0)
            {
                posts = posts.Where(x => x.CategoryId == condition.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
            {
                posts = posts.Where(x => x.Category.UrlSlug == condition.CategorySlug);
            }

            if (condition.AuthorId > 0)
            {
                posts = posts.Where(x => x.AuthorId == condition.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
            {
                posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
            }

            if (!string.IsNullOrWhiteSpace(condition.TagSlug))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
            }

            if (!string.IsNullOrWhiteSpace(condition.KeyWord))
            {
                posts = posts.Where(x => x.Title.Contains(condition.KeyWord) ||
                                         x.ShortDescription.Contains(condition.KeyWord) ||
                                         x.Description.Contains(condition.KeyWord) ||
                                         x.Category.Name.Contains(condition.KeyWord) ||
                                         x.Tags.Any(t => t.Name.Contains(condition.KeyWord)));
            }

            if (condition.Year > 0)
            {
                posts = posts.Where(x => x.PostedDate.Year == condition.Year);
            }

            if (condition.Month > 0)
            {
                posts = posts.Where(x => x.PostedDate.Month == condition.Month);
            }

            if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
            {
                posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
            }

            return posts;

        }
        public async Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await FilterPosts(query).ToPagedListAsync(
                                    pageNumber,
                                    pageSize,
                                    nameof(Post.PostedDate),
                                    "DESC",
                                    cancellationToken);
        }

        public async Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            return await FilterPosts(query).ToPagedListAsync(
                                            pagingParams,
                                            cancellationToken);
        }

        public async Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
        {
            IQueryable<T> result = mapper(FilterPosts(query));

            return await result.ToPagedListAsync(pagingParams, cancellationToken);
        }
    }
}