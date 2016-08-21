using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPost.Helpers;

namespace WebPost.Models
{
    public class PostService
    {
        private readonly MongoHelper<Post> _posts;

        public PostService()
        {
            _posts = new MongoHelper<Post>();
        }
            
        public void Create (Post post)
        {
            post.Comments = new List<Comment>();
            _posts.Collection.InsertOne(post);
        }

        public void Edit (Post post)
        {
            var filter = Builders<Post>.Filter.Eq("_id", post.PostId);
            var update = Builders<Post>.Update
                .Set("Title", post.Title)
                .Set("Url", post.Url)
                .Set("Summary", post.Summary)
                .Set("Details", post.Details);
            var result = _posts.Collection.UpdateOne(filter, update);
        }

        public void Delete(ObjectId postId)
        {
            var filter = Builders<Post>.Filter.Eq("_id", postId);

            _posts.Collection.DeleteOne(filter);
        }

        public IList<Post> GetPosts()
        {
            var filter = FilterDefinition<Post>.Empty;
            var projection = Builders<Post>.Projection.Exclude("Comments");
            var posts = _posts.Collection.Find(filter).Project<Post>(projection).ToList();
            posts = posts.OrderByDescending(c => c.Date).ToList();

            return posts;
        }

        public Post GetPost(ObjectId id)
        {
            var filter = Builders<Post>.Filter.Eq("_id", id);
            var projection = Builders<Post>.Projection.Slice("Comments", -5);
            var post = _posts.Collection.Find(filter).Project<Post>(projection).Single();
            post.Comments = post.Comments.OrderByDescending(c => c.Date).ToList();

            return post;
        }

        public Post GetPost(string url)
        {
            var filter = Builders<Post>.Filter.Eq("Url", url);
            var projection = Builders<Post>.Projection.Slice("Comments", -5);
            var post = _posts.Collection.Find(filter).Project<Post>(projection).Single();
            post.Comments = post.Comments.OrderByDescending(c => c.Date).ToList();

            return post;
        }
    }
}