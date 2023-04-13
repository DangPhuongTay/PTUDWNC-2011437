using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        //routeGroupBuilder.MapPost("/", AddPost)
        //.WithName("AddNewPost")
        //.Accepts<PostEditModel>("multipart/form-data")
        //.Produces(401)
        //.Produces<ApiResponse<PostItem>>();
        routeGroupBuilder.MapPost("/", AddPost)
                .WithName("AddNewPost")
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                .Produces(401)
                .Produces<ApiResponse<PostDetail>>();

        routeGroupBuilder.MapGet("/{id:int}", GetPostById)
                 .WithName("GetPostById")
                 .Produces<ApiResponse<PostDetail>>();

        routeGroupBuilder.MapGet(
               "/byslug/{slug:regex(^[a-z0-9 -]+$)}", GetPostBySlug)
               .WithName("GetPostBySlug")
               .Produces<ApiResponse<PostDetail>>();
        routeGroupBuilder.MapGet("/get-posts-filter", GetFilteredPosts)
        .WithName("GetFilteredPost")
            .Produces<ApiResponse<PostDto>>();
        routeGroupBuilder.MapGet("/get-filter", GetFilter)
        .WithName("GetFilter")
        .Produces<ApiResponse<PostFilterModel>>();

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
    public static async Task<IResult> GetPostById(
              int id,
              IMapper mapper,
              IBlogRepository blogRepository)
    {
        var posts = await blogRepository.GetPostByIdAsync(id);

        return posts != null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}"))
           : Results.Ok(ApiResponse.Success(mapper.Map<PostDetail>(posts)));
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
            KeyWord = model.Keyword,
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

    public static async Task<IResult> GetPosts(
      [AsParameters] PagingModel model,
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

    private static async Task<IResult> AddPost(
            HttpContext context,
            IBlogRepository blogRepository,
            IMapper mapper,
            IMediaManager mediaManager)
    {
        var model = await PostEditModel.BindAsync(context);
        var slug = model.Title.GenerateSlug();
        if (await blogRepository.IsPostSlugExistedAsync(model.Id, slug))
        {
            return Results.Ok(ApiResponse.Fail(
            HttpStatusCode.Conflict, $"Slug '{slug}' đã được sử dụng cho bài viết khác"));
        }
        var post = model.Id > 0 ? await
            blogRepository.GetPostByIdAsync(model.Id) : null;
        if (post == null)
        {
            post = new Post()
            {
                PostedDate = DateTime.Now
            };
        }
        post.Title = model.Title;
        post.AuthorId = model.AuthorId;
        post.CategoryId = model.CategoryId;
        post.ShortDescription = model.ShortDescription;
        post.Description = model.Description;
        post.Meta = model.Meta;
        post.Published = model.Published;
        post.ModifiedDate = DateTime.Now;
        post.UrlSlug = model.Title.GenerateSlug();
        if (model.ImageFile?.Length > 0)
        {
            string hostname =
           $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/",
            uploadedPath = await
           mediaManager.SaveFileAsync(model.ImageFile.OpenReadStream(),
            model.ImageFile.FileName,
            model.ImageFile.ContentType);
            if (!string.IsNullOrWhiteSpace(uploadedPath))
            {
                post.ImageUrl = uploadedPath;
            }
        }
        await blogRepository.CreateOrUpdatePostAsync(post,
       model.GetSelectedTags());
        return Results.Ok(ApiResponse.Success(
        mapper.Map<PostItem>(post), HttpStatusCode.Created));
    }

}
