﻿namespace TatBlog.WebApp.Extensions
{
    public static class RouteExtensions
    {
        public static IEndpointRouteBuilder UseBlogRoutes(
            this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "posts-by-category",
                pattern: "blog/category/{slug}",
                defaults: new { controller = "Blog", action = "Category" });

            endpoints.MapControllerRoute(
                name: "posts-by-tag",
                pattern: "blog/tag/{slug}",
                defaults: new { controller = "Blog", action = "Tag" });

            endpoints.MapControllerRoute(
                name: "single-post",
                pattern: "blog/post/{year:int}/{month:int}/{day:int}/{slug}",
                defaults: new { controller = "Blog", action = "Post" });

            endpoints.MapControllerRoute(
                name: "admin-area",
                pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });
            endpoints.MapControllerRoute(
                name: "admin-area",
                pattern: "admin/{controller=Categories}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Blog}/{action=Index}/{id?}");

            return endpoints;

        }

    }
}