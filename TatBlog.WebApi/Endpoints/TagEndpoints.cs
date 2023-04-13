using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class TagEndpoints
{
    public static WebApplication MapTagEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/tags");

        // Nested Map with defined specific route
        routeGroupBuilder.MapGet("/", GetTags)
                 .WithName("GetTags")
                 .Produces<ApiResponse<PaginationResult<TagItem>>>();

        return app;
    }
    private static async Task<IResult> GetTags(
     IBlogRepository blogRepository)
    {
        var tags = await blogRepository.GetTagsAsync();
        return Results.Ok(ApiResponse.Success(tags));
    }
}