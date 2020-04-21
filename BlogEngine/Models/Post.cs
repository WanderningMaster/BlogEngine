using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogEngine.Models
{
    public class Post
    {
        public int? Id { get; set; }
        public DateTime PostTime { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
    }
}
