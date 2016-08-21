using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebPost.Helpers
{
    public class MongoHelper<T> where T : class
    {
        public IMongoCollection<T> Collection { get; private set; }

        public MongoHelper()
        {
            var con = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;

            var client = new MongoClient(con);
            var db = client.GetDatabase("blog");
            Collection = db.GetCollection<T>(typeof(T).Name.ToLower());
        }
    }
}