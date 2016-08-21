using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebPost.Models
{
    public class Post
    {
        [ScaffoldColumn(false)]
        [BsonId]
        public ObjectId PostId { get; set; }

        [ScaffoldColumn(false)]
        public DateTime Date { get; set; }

        [Required]
        public string Title { get; set; }

        [ScaffoldColumn(false)]
        public String Url { get; set; }

        [Required]
        public String Summary { get; set; }

        [UIHint("WYSIWIG")]
        [AllowHtml]
        public string Details { get; set; }

        [ScaffoldColumn(false)]
        public String Author { get; set; }

        [ScaffoldColumn(false)]
        public int TotalComments { get; set; }

        [ScaffoldColumn(false)]
        public IList<Comment> Comments { get; set; }
    }

    public class Comment
    {
        [BsonId]
        public ObjectId CommentId { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        [Required]
        public string Detail { get; set; }
    }
}