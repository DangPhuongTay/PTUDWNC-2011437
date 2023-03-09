using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
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
            int numPost,
            CancellationToken cancellationToken = default);

        Task<bool> IsPostSlugExistedAsync(
            int postId, string slug,
            CancellationToken cancellationToken = default);

        Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken);
        Task<IList<CategoryItem>> GetCategoryItemsAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default);
        Task<IPagedList<TagItem>> GetPagedTagAsync(
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default);
        public Task<Tag> GetTagBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);
        Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery postQuery,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    }
}