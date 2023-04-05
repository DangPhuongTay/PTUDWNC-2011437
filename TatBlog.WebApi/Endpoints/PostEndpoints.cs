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

        routeGroupBuilder.MapGet("/byslug/{slug::regex(^[a-z0-9_-]+$)}", GetPostBySlug)
                         .WithName("GetPostBySlug")
                         .Produces<ApiResponse<PaginationResult<PostDto>>>();

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


        return app;
    }

    private static async Task<IResult> GetPosts([AsParameters] PostFilterModel model, IBlogRepository blogRepository, IMapper mapper)
    {
        var postQuery = mapper.Map<PostQuery>(model);
        var postList = await blogRepository.GetPostByQueryAsync(postQuery, model, post => post.ProjectToType<PostItem>());

        var paginationResult = new PaginationResult<PostItem>(postList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetFeaturedPost(int limit, IBlogRepository blogRepository)
    {
        var posts = await blogRepository.GetPopularArticlesAsync(limit);

        return Results.Ok(ApiResponse.Success(posts));
    }



    private static async Task<IResult> GetPostDetails(int id, IBlogRepository blogRepository, IMapper mapper)
    {
        var post = await blogRepository.GetCachedPostByIdAsync(id);

        return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post)));
    }

    private static async Task<IResult> GetPostBySlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery
        {
            PostSlug = slug,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

        var post = postsList.FirstOrDefault();

        return Results.Ok(ApiResponse.Success(post));
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
public static WebApplication MapPostEndpoints(this WebApplication app)
{

 routeGroupBuilder.MapGet("/get-posts-filter", GetFilteredPosts)
 .WithName("GetFilteredPost")
 .Produces<ApiResponse<PostDto>>();
    routeGroupBuilder.MapGet("/get-filter", GetFilter)
    .WithName("GetFilter")
    .Produces<ApiResponse<PostFilterModel>>();
    return app;
}

 private static async Task<IResult> GetFilter(
 IAuthorRepository authorRepository,
 IBlogRepository blogRepository)
{
    var model = new PostFilterModel()
    {
        AuthorList = (await authorRepository.GetAuthorsAsync())
    .Select(a => new SelectListItem()
    {
        Text = a.FullName,
        Value = a.Id.ToString()
    }),
        CategoryList = (await blogRepository.GetCategoriesAsync())
    .Select(c => new SelectListItem()
    {
        Text = c.Name,
        Value = c.Id.ToString()
    })
    };
    return Results.Ok(ApiResponse.Success(model));
}
private static async Task<IResult> GetFilteredPosts(
[AsParameters] PostFilterModel model,
[AsParameters] PagingModel pagingModel,
IBlogRepository blogRepository)
{
    var postQuery = new PostQuery()
    {
        Keyword = model.Keyword,

    CategoryId = model.CategoryId,
        AuthorId = model.AuthorId,
        Year = model.Year,
        Month = model.Month,
    };
    var postsList = await blogRepository.GetPagedPostsAsync(
    postQuery, pagingModel, posts =>
   posts.ProjectToType<PostDto>());
    var paginationResult = new PaginationResult<PostDto>(postsList);
    return Results.Ok(ApiResponse.Success(paginationResult));
}
//using FluentValidation;
//using Mapster;
//using MapsterMapper;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using TatBlog.Core.Collections;
//using FluentValidation.Results;
//using TatBlog.Core.DTO;
//using TatBlog.Core.Entities;
//using TatBlog.Services.Blogs;
//using TatBlog.Services.Media;
//using TatBlog.WebApi.Filters;
//using TatBlog.WebApi.Models;
//using System;

//namespace TatBlog.WebApi.Endpoints
//{
//    public static class PostEndpoints
//    {
//        public static WebApplication MapPostEndpoints(
//            this WebApplication app)
//        {
//            var routeGroupBuilder = app.MapGroup("/api/posts");

//            // Nested Map with defined specific route
//            routeGroupBuilder.MapGet("/", GetPosts)
//                             .WithName("GetPosts")
//                             .Produces<PaginationResult<PostItem>>();

//            routeGroupBuilder.MapGet("/featured/{limit:int}", GetFeaturedPost)
//                             .WithName("GetFeaturedPost")
//                             .Produces<IList<Post>>();



//            routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
//                             .WithName("GetPostById")
//                             .Produces<PostDto>()
//                             .Produces(404);

//            routeGroupBuilder.MapGet("/byslug/{slug::regex(^[a-z0-9_-]+$)}", GetPostBySlug)
//                             .WithName("GetPostBySlug")
//                             .Produces<PaginationResult<PostDto>>();

//            routeGroupBuilder.MapPost("/", AddPost)
//                             .WithName("AddNewPost")
//                             .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
//                             .Produces(201)
//                             .Produces(409);



//            return app;
//        }

//        private static async Task<IResult> GetPosts([AsParameters] PostFilterModel model, IBlogRepository blogRepository, IMapper mapper)
//        {
//            var postQuery = mapper.Map<PostQuery>(model);
//            var postList = await blogRepository.GetPostByQueryAsync(postQuery, model, post => post.ProjectToType<PostItem>());

//            var paginationResult = new PaginationResult<PostItem>(postList);

//            return Results.Ok(paginationResult);
//        }

//        private static async Task<IResult> GetFeaturedPost(int limit, IBlogRepository blogRepository)
//        {
//            var posts = await blogRepository.GetPopularArticlesAsync(limit);

//            return Results.Ok(posts);
//        }



//        private static async Task<IResult> GetPostDetails(int id, IBlogRepository blogRepository, IMapper mapper)
//        {
//            var post = await blogRepository.GetCachedPostByIdAsync(id);

//            return post == null ? Results.NotFound($"Không tìm thấy bài có mã số {id}") : Results.Ok(mapper.Map<PostItem>(post));
//        }

//        private static async Task<IResult> GetPostBySlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
//        {
//            var postQuery = new PostQuery
//            {
//                PostSlug = slug,
//                PublishedOnly = true
//            };

//            var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

//            var post = postsList.FirstOrDefault();

//            return post == null ? Results.NotFound($"Không tìm thấy bài có slug {slug}") : Results.Ok(post);
//        }

//        private static async Task<IResult> AddPost(PostEditModel model, IBlogRepository blogRepository, IMapper mapper)
//        {
//            if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
//            {
//                return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng");
//            }

//            var post = mapper.Map<Post>(model);
//            await blogRepository.CreateOrUpdatePostAsync(post, model.GetSelectedTags());

//            return Results.CreatedAtRoute("GetPostById", new { post.Id }, mapper.Map<PostItem>(post));
//        }







//    }
//}
