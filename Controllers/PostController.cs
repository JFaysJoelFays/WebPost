using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPost.Helpers;
using WebPost.Models;

namespace WebPost.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly CommentService _commentService;

        public PostController()
        {
            _postService = new PostService();
            _commentService = new CommentService();
        }

        // GET: Post
        public ActionResult Index()
        {
            var posts = _postService.GetPosts();


            return View(posts);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Post());
        }

        [HttpPost]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.Url = post.Title.GenerateSlug();
                post.Author = User.Identity.Name;
                post.Date = DateTime.Now;

                _postService.Create(post);

                return RedirectToAction("Index");
            }

            return View();
        } 

        [HttpGet]
        public ActionResult Update(string id)
        {
            var post = _postService.GetPost(id);

            return View(post);
        }

        [HttpPost]
        public ActionResult Update(Post post)
        {
            if (ModelState.IsValid)
            {
                post.Url = post.Title.GenerateSlug();

                _postService.Edit(post);

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Delete(ObjectId id)
        {
            return View(_postService.GetPost(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(ObjectId id)
        {
            _postService.Delete(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            var post = _postService.GetPost(id);
            ViewBag.PostId = post.PostId;

            ViewBag.TotalComments = post.TotalComments;
            ViewBag.LoadedComments = 5;

            return View(post);
        }

        [HttpPost]
        public ActionResult AddComment(ObjectId postId, Comment comment)
        {
            if (ModelState.IsValid)
            {
                var newComment = new Comment()
                {
                    CommentId = ObjectId.GenerateNewId(),
                    Author = User.Identity.Name,
                    Date = DateTime.Now,
                    Detail = comment.Detail
                };

                _commentService.AddComment(postId, newComment);

                ViewBag.PostId = postId;
                return Json(
                    new
                    {
                        Result = "ok",
                        CommentHtml = RenderPartialViewToString("Comment", newComment),
                        FormHtml = RenderPartialViewToString("AddComment", new Comment())
                    });
            }

            ViewBag.PostId = postId;
            return Json(
                new
                {
                    Result = "fail",
                    FormHtml = RenderPartialViewToString("AddComment", comment)
                });
        }

        public ActionResult RemoveComment(ObjectId postId, ObjectId commentId)
        {
            _commentService.RemoveComment(postId, commentId);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult CommentList(ObjectId postId, int skip, int limit, int totalComments)
        {
            ViewBag.TotalComments = totalComments;
            ViewBag.LoadedComments = skip + limit;

            return PartialView(_commentService.GetComments(postId, ViewBag.LoadedComments, limit, totalComments));
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            }

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}