using System.ComponentModel;

namespace TatBlog.WebApp.Areas.Admin.Models;

public interface SearchModel
{
    [DisplayName("Từ khóa")]
    public string Keyword { get; set; }
}