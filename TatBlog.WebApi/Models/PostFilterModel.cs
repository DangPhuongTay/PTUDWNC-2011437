using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Globalization;

namespace TatBlog.WebApi.Models
{
    public class PostFilterModel:PagingModel
    {

        public string KeyWord { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }

    }



}