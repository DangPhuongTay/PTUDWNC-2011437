using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Globalization;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Controllers;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class PostFilterModel
    {
       
        [DisplayName("Từ khóa")]
        public string Keyword { get; set; }
        [DisplayName("Tác giả")]
        public int? AuthorId { get; set; }
        [DisplayName("Chủ đề")]
        public int? CategoryId { get; set; }
        [DisplayName("Năm")]
        public int? Year { get; set; }
        [DisplayName("Tháng")]
        public int? Mouth { get; set; }
        public IEnumerable<SelectListItem> AuthorList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> MouthList { get; set; }

        public PostFilterModel() {
        MouthList = Enumerable.Range(1,12)
                .Select(m => new SelectListItem()
                {
                    Value = m.ToString(),
                    Text=CultureInfo.CurrentCulture
                    .DateTimeFormat.GetMonthName(m)
                })
                .ToList();
        }
    }

}
