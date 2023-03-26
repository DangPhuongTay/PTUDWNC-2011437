using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TatBlog.WebApp.Areas.Admin.Models;

public class CategoryFilterModel : SearchModel
{
    [DisplayName("Từ khóa")]
    public string Keyword { get; set; }

    [DisplayName("Hiển thị trên Menu")]
    public bool ShowOnMenu { get; set; }
}
