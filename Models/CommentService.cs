using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPost.Helpers;

namespace WebPost.Models
{
    public class CommentService
    {
        private readonly MongoHelper<Post> _posts;

        public CommentService()
        {
            _posts = new MongoHelper<Post>();
        }

        public void AddComment(ObjectId postId, Comment comment)
        {
            var filter = Builders<Post>.Filter.Eq("_id", postId);
            var update = Builders<Post>.Update.AddToSet("Comments", comment).Inc("TotalComments", 1);

            _posts.Collection.UpdateOne(filter, update);
        }

        public void RemoveComment(ObjectId postId, ObjectId commentId)
        {
            var filter = Builders<Post>.Filter.Eq("_id", postId);
            var update = Builders<Post>.Update.PullFilter("Comments", Builders<Comment>.Filter.Eq("_id", commentId)).Inc("TotalComments", -1);

            _posts.Collection.FindOneAndUpdate(filter, update);
        }

        public IList<Comment> GetComments(ObjectId postId, int skip, int limit, int totalComments)
        {
            var newComments = GetTotalComments(postId) - totalComments;
            skip += newComments;

            var filter = Builders<Post>.Filter.Eq("_id", postId);
            var projection = Builders<Post>.Projection
                    .Exclude("Date").Exclude("Title").Exclude("Url").Exclude("Summary").Exclude("Details").Exclude("Author").Exclude("TotalComments")
                    .Slice("Comments", -skip, limit);

            var post = _posts.Collection.Find(filter).Project<Post>(projection).Single();

            return post.Comments.OrderByDescending(c => c.Date).ToList();
        }

        private int GetTotalComments(ObjectId postId)
        {
            var filter = Builders<Post>.Filter.Eq("_id", postId);
            var projection = Builders<Post>.Projection.Include("TotalComments");

            var post = _posts.Collection.Find(filter).Project<Post>(projection).Single();

            return post.TotalComments;
        }
    }
}