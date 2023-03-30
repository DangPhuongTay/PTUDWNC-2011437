using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using FluentValidation.Results;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;
using System;

namespace TatBlog.WebApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static WebApplication MapAuthorEndpoints(
            this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/posts");

            // Nested Map with defined specific route
            routeGroupBuilder.MapGet("/", GetPosts)
                             .WithName("GetPosts")
                             .Produces<PaginationResult<PostItem>>();

            routeGroupBuilder.MapGet("/featured/{limit:int}", GetFeaturedPost)
                             .WithName("GetFeaturedPost")
                             .Produces<IList<Post>>();

          

            routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
                             .WithName("GetPostById")
                             .Produces<PostDto>()
                             .Produces(404);

            routeGroupBuilder.MapGet("/byslug/{slug::regex(^[a-z0-9_-]+$)}", GetPostBySlug)
                             .WithName("GetPostBySlug")
                             .Produces<PaginationResult<PostDto>>();

            routeGroupBuilder.MapPost("/", AddPost)
                             .WithName("AddNewPost")
                             .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                             .Produces(201)
                             .Produces(409);



            return app;
        }

        private static async Task<IResult> GetPosts([AsParameters] PostFilterModel model, IBlogRepository blogRepository, IMapper mapper)
        {
            var postQuery = mapper.Map<PostQuery>(model);
            var postList = await blogRepository.GetPostByQueryAsync(postQuery, model, post => post.ProjectToType<PostItem>());

            var paginationResult = new PaginationResult<PostItem>(postList);

            return Results.Ok(paginationResult);
        }

        private static async Task<IResult> GetFeaturedPost(int limit, IBlogRepository blogRepository)
        {
            var posts = await blogRepository.GetPopularArticlesAsync(limit);

            return Results.Ok(posts);
        }



        private static async Task<IResult> GetPostDetails(int id, IBlogRepository blogRepository, IMapper mapper)
        {
            var post = await blogRepository.GetCachedPostByIdAsync(id);

            return post == null ? Results.NotFound($"Không tìm thấy bài có mã số {id}") : Results.Ok(mapper.Map<PostItem>(post));
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

            return post == null ? Results.NotFound($"Không tìm thấy bài có slug {slug}") : Results.Ok(post);
        }

        private static async Task<IResult> AddPost(PostEditModel model, IBlogRepository blogRepository, IMapper mapper)
        {
            if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var post = mapper.Map<Post>(model);
            await blogRepository.CreateOrUpdatePostAsync(post, model.GetSelectedTags());

            return Results.CreatedAtRoute("GetPostById", new { post.Id }, mapper.Map<PostItem>(post));
        }

      


     

       
    }
}
