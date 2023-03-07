using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TatBlog.Core.Entities;
using System.Threading.Tasks;
using TatBlog.Core.DTO;
using TatBlog.Core.Contracts;

namespace TatBlog.Services.Blogs
{
    public interface IBlogRepository
    {
        Task<Post> GetPostAsync(
            int year,
            int month,
            string slug,
            CancellationToken cancellationToken = default);
        Task<IList<Post>> GetPopularArticlesAsync(
            int numPosts,
            CancellationToken cancellationToken = default);
        Task<bool> IsPostSlugExistedAsync(
            int postId, string slug,
            CancellationToken cancellationToken = default);
        Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken = default);
        Task<IList<CategoryItem>> GetCategoriesAsync(
           bool showOnMenu = false,
           CancellationToken cancellationToken = default);
        Task<IPagedList<TagItem>> GetPagedTagAsync(
         IPagingParams pagingParams,
         CancellationToken cancellationToken = default);
            }

   
}
