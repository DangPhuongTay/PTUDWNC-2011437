using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO
{
    public class PagerQuery
    {
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
