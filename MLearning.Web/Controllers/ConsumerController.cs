using Core.Repositories;
using Core.Security;
using MLearning.Core.Configuration;
using MLearning.Core.Entities;
using MLearning.Core.Services;
using MLearningDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MLearning.Web.Controllers
{
    [Authorize(Roles = Constants.ConsumerRole)]
    public class ConsumerController : MLController
    {
        //
        IMLearningService ml;

        public ConsumerController()
            : base()
        {
            IRepositoryService repo = new WAMSRepositoryService();
            ml = new MLearningAzureService(repo);
        }
        // GET: /Consumer/

        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult Institution()
        {
            return PartialView();
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        async public Task<ActionResult> Consumer()
        {

            List<user_by_circle> list = await ml.GetUsersInCircle(1);

            return View(list);
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                User user = new User();
                user.name = collection.Get("name");
                user.lastname = collection.Get("lastname");
                user.username = collection.Get("username");
                user.password = EncryptionService.encrypt(collection.Get("password"));
                user.email = collection.Get("email");
                // TODO: Add insert logic here
                await ml.CreateAccount<User>(user, u => u.id, UserType.Consumer);

                await ml.AddUserToCircle(user.id, Convert.ToInt32(collection.Get("Circle_id")));

                return RedirectToAction("Consumer");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        async public Task<ActionResult> Edit(int id)
        {
            user_by_circle user = await ml.GetObjectWithId<user_by_circle>(id);
            return View(user);
        }

        // POST: /Customer/Edit/5
        [Authorize(Roles = Constants.ConsumerRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            try
            {
                User user = await ml.GetObjectWithId<User>(id);
                user.name = collection.Get("name");
                user.lastname = collection.Get("lastname");
                user.username = collection.Get("username");
                user.password = EncryptionService.encrypt(collection.Get("password"));
                user.email = collection.Get("email");

                await ml.UpdateObject<User>(user);
                // TODO: Add update logic here

                return RedirectToAction("Consumer");
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult Courses()
        {
            return PartialView();
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        public async Task<JsonResult> GetListCourse()
        {
            var courses = await _mLearningService.GetCircles();
            return Json(courses);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        public async Task<JsonResult> GetInstitution()
        {
            Institution_has_User consumer = await _mLearningService.GetInstitutionByUser(UserID);
            InstitutionID = consumer.institution_id;
            var institution = await _mLearningService.GetObjectWithId<Institution>(InstitutionID);
            var circlesByInstitution = await _mLearningService.GetCirclesByInstitution(institution.id);
            var consumerByInstitution = await _mLearningService.GetConsumersByInstitution(institution.id);
            var data = new
            {
                institution = institution,
                circlesByInstitution = circlesByInstitution,
                consumerByInstitution = consumerByInstitution
            };

            return Json(data);
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        public async Task<JsonResult> GetCirclesByUser()
        {
            var circlesByUser = await _mLearningService.GetCirclesByUser(UserID);
            var data = new
            {
                ciclesByUser = circlesByUser
            };

            return Json(data);
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        public async Task<JsonResult> GetCirclesByInstitution()
        {
            var circlesByInstitution = await _mLearningService.GetCirclesByInstitution(InstitutionID);
            var data = new
            {
                circlesByInstitution = circlesByInstitution
            };

            return Json(data);
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<ActionResult> GetCircleById(int idCircle)
        {
            var circleUser = await _mLearningService.GetCircleUser(UserID, idCircle);
            if (circleUser == null)
            {
                var data1 = new
                {
                    noCircle = true
                };
                return Json(data1, JsonRequestBehavior.AllowGet);
            }
            var circle = await _mLearningService.GetObjectWithId<Circle>(idCircle);
            var learningObjectsByCircle = await _mLearningService.GetLOByCircle(idCircle);
            var consumerByCircle = await _mLearningService.GetConsumersByCircle(idCircle);
            var data = new
            {
                circle = circle,
                learningObjectsByCircle = learningObjectsByCircle,
                consumerByCircle = consumerByCircle
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetPostsByCircle(int idCircle)
        {
            var postsByCircle = await _mLearningService.GetPostByCircle(idCircle);
            var data = new
            {
                postsByCircle = postsByCircle
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetCommnetsByLO(int idLO)
        {
            var commentsByLO = await _mLearningService.GetLOComments(idLO);
            var data = new
            {
                commentsByLO = commentsByLO
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetLOByCircle(int idCircle)
        {
            var learningObjects = await _mLearningService.GetLOByCircle(idCircle);
            var data = new
            {
                learningObjects = learningObjects
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetQuizzesByCircle(int idCircle)
        {
            var quizzesByCircle = await _mLearningService.GetQuizzesByCircle(idCircle);

            var data = new
            {
                quizzesByCircle = quizzesByCircle
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetSectionByLO(int idLO)
        {
            var sections = await _mLearningService.GetSectionsByLO(idLO);

            var data = new
            {
                sections = sections
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetPagesBySection(int idSection)
        {
            var pages = await _mLearningService.GetPagesByLOSection(idSection);

            var data = new
            {
                pages = pages
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetTags()
        {
            var tags = await _mLearningService.GetAllTags();
            var data = new
            {
                tags = tags
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> GetCirclesByName(string startWith)
        {
            var circles = await _mLearningService.GetCircles(startWith);
            var data = new
            {
                circles = circles
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> AddUserToCircle(int circleId)
        {
            await _mLearningService.AddUserToCircle(UserID, circleId);
            var data = new
            {
                success = true,
                message = "Se agrego correctamente"
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> AddPostToCircle(int circleId, string text)
        {
            Post post = new Post
            {
                user_id = UserID,
                circle_id = circleId,
                text = text,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };
            await _mLearningService.CreateObject<Post>(post, p => p.id);
            post.created_at = DateTime.UtcNow;
            var data = new
            {
                success = true,
                message = "Se agrego correctamente",
                post = post
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpGet]
        public async Task<JsonResult> AddCommentToLO(int loId, string text)
        {
            LOComment loComment = new LOComment
            {
                user_id = UserID,
                lo_id = loId,
                text = text,
                created_at = DateTime.UtcNow
            };
            await _mLearningService.CreateObject<LOComment>(loComment, c => c.id);
            var data = new
            {
                success = true,
                message = "Se agrego correctamente",
                comment = loComment
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        [HttpPost]
        public async Task<JsonResult> AddCourse(Circle circle)
        {
            CircleUser circleUser = await _mLearningService.GetCircleUser(UserID, circle.id);
            if (circleUser == null)
            {
                try
                {
                    await _mLearningService.AddUserToCircle(UserID, circle.id);
                    var data = new
                    {
                        success = true,
                        message = "Se agrego correctamente"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    var data = new {
                        success = false
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var data = new
                {
                    success = false,
                    message = "El usuario ya esta en el curso."
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        // partial views
        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult Circle()
        {
            return PartialView();
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult LearningObjects()
        {
            return PartialView();
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult Quizzes()
        {
            return PartialView();
        }
        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult SectionDetail()
        {
            return PartialView();
        }

        [Authorize(Roles = Constants.ConsumerRole)]
        public ActionResult AddCourses()
        {
            return PartialView();
        }
    }
}