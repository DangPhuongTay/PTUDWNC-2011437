using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;


namespace TatBlog.Data.Seeders
{
    public class DataSeeder : IDataSeeder
    {
        private readonly BlogDbContext _dbContext;

        public object categories { get; private set; }

        public DataSeeder(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Initialize()
        {
            _dbContext.Database.EnsureCreated();
            if (_dbContext.Posts.Any()) return;

            var authors = AddAthors();
            var categories = AddCategories();
            var tags = AddTags();
            var posts = AddPosts(authors, categories, tags);



        }
        private IList<Author> AddAthors()
        {

            var authors = new List<Author>()
            {
                new()
                {
                    FullName = "Jason Mouth",
                    UrlSlug = "jason-mouth",
                    Email = "jason@gmail.com",
                    JoinedDate = new DateTime(2022, 10, 21)
                },
                new()
                {
                    FullName = "Jessica Wonder",
                    UrlSlug = "jessica-wonder",
                    Email = "jessica665@motip.com",
                    JoinedDate = new DateTime(2029, 4, 19)
                }
            };
            _dbContext.Authors.AddRange(authors);
            _dbContext.SaveChanges();

            return authors;
        }
        private IList<Category> AddCategories() {
            var categories = new List<Category>()
            { 
                new() { Name = "Net Core", Description = "Net Core", UrlSlug = "Net Core",ShowMenu = false },
                new() { Name = "Architecture", Description = "Architecture", UrlSlug = "Architecture",ShowMenu = false },
                new() { Name = "Messaging", Description = "Messaging", UrlSlug = "Messaging",ShowMenu = false },
                new() { Name = "OOP", Description = "OOP", UrlSlug = "OOP",ShowMenu = false },
                new() { Name = "Design Patterns", Description = "Design Patterns", UrlSlug = "Design Patterns",ShowMenu = false}

            };
            _dbContext.AddRange(categories);
            _dbContext.SaveChanges();

            return categories;
        
        }
        private IList<Tag> AddTags() {
            var tags = new List<Tag>()
            {
                new() { Name = "Google", Description = "Google", UrlSlug = "Google" },
                new() { Name = "ASP.NET MVC", Description = "ASP.NET MVC", UrlSlug = "ASP.NET MVC" },
                new() { Name = "Razor Page", Description = "Razor Page", UrlSlug = "Razor Page" },
                new() { Name = "Blazor", Description = "Blazor", UrlSlug = "Blazor" },
                new() { Name = "Deep Learning", Description = "Deep Learning", UrlSlug = "Deep Learning" },
                new() { Name = "Neural Network", Description = "Neural Network", UrlSlug = "Neural Network" }

            };
            _dbContext.AddRange(tags);
            _dbContext.SaveChanges();

            return tags;

        }
        private IList<Post> AddPosts(
            IList<Author> authors,
            IList<Category> categories,
            IList<Tag> tags)
        {
            var posts = new List<Post>()
            {
                new()
                {
                    Title = "ASP .NET Core Diagnostic Scenarios",
                    ShortDescription = "David and friends has a great repos",
                    Description = "Here's a few great DON'T and DO examples",
                    Meta = "David and friends has a great repository filled",
                    UrlSlug = "aspnet-core-diagnostic-scenarios",
                    Published = true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 10,
                    Author = authors[0],
                    Category = categories[0],
                    Tags = new List<Tag>()
                    {
                        tags[0]
                    }
                }
            };
            _dbContext.AddRange(posts);
                _dbContext.SaveChanges();

            return posts;


        }


    }
}
