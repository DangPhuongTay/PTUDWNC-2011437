using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class PostEndpoints
{
    public static WebApplication MapPostEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/posts");

        // Nested Map with defined specific route
        routeGroupBuilder.MapGet("/", GetPosts)
                         .WithName("GetPosts")
                         .Produces<ApiResponse<PaginationResult<PostItem>>>();

        routeGroupBuilder.MapGet("/featured/{limit:int}", GetFeaturedPost)
                         .WithName("GetFeaturedPost")
                         .Produces<ApiResponse<IList<Post>>>();


        routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
                         .WithName("GetPostById")
                         .Produces<ApiResponse<PostDto>>();

        routeGroupBuilder.MapGet(
               "/byslug/{slug:regex(^[a-z0-9 -]+$)}", GetPostBySlug)
               .WithName("GetPostBySlug")
               .Produces<ApiResponse<PostDetail>>();

        routeGroupBuilder.MapPost("/", AddPost)
                         .WithName("AddNewPost")
                         .Accepts<PostEditModel>("multipart/form-data")
                         .Produces(401)
                         .Produces<ApiResponse<PostItem>>();


        routeGroupBuilder.MapPost("/{id:int}/picture", SetPostPicture)
                         .WithName("SetPostPicture")
                         .Accepts<IFormFile>("multipart/formdata")
                         .Produces(401)
                         .Produces<string>();
        routeGroupBuilder.MapGet("/random/{limit:int}", GetPostRandom)
              .WithName("GetPostRandom")
              .Produces<ApiResponse<IList<PostDto>>>();

        return app;
    }

   
    public static async Task<IResult> GetPosts(
      [AsParameters] PostFilterModel model,
      IBlogRepository bolgRepository,
      IMapper mapper)
    {
        var postQuery = mapper.Map<PostQuery>(model);
        var postList = await bolgRepository
            .GetPagedPostsAsync(postQuery, model, posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postList);
        return Results.Ok(ApiResponse.Success(paginationResult));
    }
 public static async Task<IResult> GetFeaturedPost(
           int limit,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetPopularArticlesAsync(limit);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết được nhiều người đọc nhất"))
                   : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostDto>>(post)));
        }



    private static async Task<IResult> GetPostDetails(int id, IBlogRepository blogRepository, IMapper mapper)
    {
        var post = await blogRepository.GetCachedPostByIdAsync(id);

        return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post)));
    }

    private static async Task<IResult> GetPostBySlug(
              [FromRoute] string slug,
              [AsParameters] PagingModel pagingModel,
              IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery()
        {
            PostSlug = slug,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPagedPostsAsync(
            postQuery, pagingModel,
            postsList => postsList.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);
        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    public static async Task<IResult> GetPostRandom(
       int limit,
       IBlogRepository blogRepository,
        IMapper mapper)
    {
        var post = await blogRepository.GetRandomPostsAsync(limit);
        return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy ngẫu nhiên bài viết nào"))
            : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostDto>>(post)));
    }
    private static async Task<IResult> AddPost(PostEditModel model, IBlogRepository blogRepository, IMapper mapper)
    {
        if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

        var post = mapper.Map<Post>(model);
        await blogRepository.CreateOrUpdatePostAsync(post, model.GetSelectedTags());

        return Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post), HttpStatusCode.Created));
    }



    private static async Task<IResult> SetPostPicture(int id, IFormFile imageFile, IBlogRepository blogRepository, IMediaManager mediaManager)
    {
        var post = await blogRepository.GetCachedPostByIdAsync(id);
        string newImagePath = string.Empty;

        // Nếu người dùng có upload hình ảnh minh họa cho bài viết
        if (imageFile?.Length > 0)
        {
            // Thực hiện việc lưu tập tin vào thư mực uploads
            newImagePath = await mediaManager.SaveFileAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);

            if (string.IsNullOrWhiteSpace(newImagePath))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "Không lưu được tập tin"));
            }

            // Nếu lưu thành công, xóa tập tin hình ảnh cũ (nếu có)
            await mediaManager.DeleteFileAsync(post.ImageUrl);
            post.ImageUrl = newImagePath;
        }

        return Results.Ok(ApiResponse.Success(newImagePath));
    }

}
