// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;

internal class Program
{
    private static  void Main(string[] args)
    {
        var context = new BlogDbContext();

        var seeder = new DataSeeder(context);
        seeder.Initialize();
        var authors = context.Authors.ToList();
        Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12}",
            "ID","Full Name","Email","Joined Date");
        foreach(var author in authors)
        {
            Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12:MM/dd/yyyy}",
             author.Id,author.FullName,author.Email,author.JoinedDate);
        }
      
        //Run(context);
        //var categories = await blogRepo.GetCategoriesAsync();
        //Console.WriteLine("{0,-5}{1,-50}{2,10}",
        //    "ID", "Name", "Count");
        //foreach (var item in  categories)
        //{
        //    Console.WriteLine("{0,-5}{1,-50}{2,10}",
        //        item.Id, item.Name, item.PostCount);
        //}

        //IBlogRepository blogRepo = new BlogRepository(context);
        //var posts = await blogRepo.GetPopularArticlesAsync(3);
        //foreach (var post in posts)
        //{
        //    Console.WriteLine("ID       : {0}",post.Id);
        //    Console.WriteLine("Title    : {0}", post.Title);
        //    Console.WriteLine("View     : {0}", post.ViewCount);
        //    Console.WriteLine("Date     : {0}:MM/dd/yyyy", post.PostedDate);
        //    Console.WriteLine("Author   : {0}", post.Author.FullName);
        //    Console.WriteLine("Category : {0}", post.Category.Name);
        //    Console.WriteLine("".PadRight(80, '-'));

        //}
        Console.ReadKey();
    }

    public static async void Run(BlogDbContext context)
    {
        IBlogRepository blogRepo = new BlogRepository(context);

        var pagingParams = new PagingParams
        {
            PageNumber = 1,
            PageSize = 5,
            SortColumn = "Name",
            SortOrder = "DESC"
        };
        var tagsList = await blogRepo.GetPagedTagAsync(pagingParams);

        foreach (var item in tagsList)
        {
            Console.WriteLine("{0,-5}{1,-50}{2,10}",
                item.Id, item.Name, item.PostCount);
        }
    }
}