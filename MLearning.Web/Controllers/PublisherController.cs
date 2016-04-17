using MLearning.Core.Configuration;
using MLearning.Core.Services;
using MLearning.Web.Models;
using MLearning.Web.Singleton;
using MLearningDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MLearning.Core.Entities;
using MLearning.Core.Entities.json;
using System.Web.Script.Serialization;

namespace MLearning.Web.Controllers
{
    [Authorize]
    public class PublisherController : MLController
    {


        //
        // GET: /Publisher/
        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> Index(int? id)
        {


            if (id != null)
            {
                int nonull_id = id ?? default(int);
                UserID = nonull_id;

            }
            else
            {

                if (UserID == default(int))
                {
                    // NO user authenticated
                    return RedirectToAction("Index", "Home");
                }

            }


            InstitutionID = await _mLearningService.GetPublisherInstitutionID(UserID);     
            ViewBag.institution = await _mLearningService.GetObjectWithId<Institution>(InstitutionID);
       

            var list = await _mLearningService.GetPublishersByInstitution(InstitutionID);

            PublisherID = list.Where(p => p.id == UserID).ToList().FirstOrDefault().publisher_id;

            //var circlesList = await _mLearningService.GetCirclesByOwner(UserID);

            var loList = await _mLearningService.GetLOByUserOwner(UserID);
            //viewData["totalCircles"] = 
            //return View("ConsumerLOList", new AdminPublisherViewModel { Circles=circlesList, LearningObjects =loList});
            return View("AssignedCircles");
        }


        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> Circle_Read([DataSourceRequest] DataSourceRequest request)
        {
            var circlesList = await _mLearningService.GetCirclesByOwner(UserID);
            var data = circlesList.ToDataSourceResult(request);
            return Json(data);
        }
        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> CircleConsumers(int id, int? idInst)
        {
            CircleID = id;
            var circle = await _mLearningService.GetObjectWithId<Circle>(id); 
            ViewBag.CircleName = circle.name;

            if (idInst != null)
            {
                ViewBag.InstitutionId = InstitutionID = idInst ?? default(int);
            }
            ViewBag.institution = await _mLearningService.GetObjectWithId<Institution>(InstitutionID);

            return View();
        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> CircleConsumers_read([DataSourceRequest] DataSourceRequest request)
        {
            List<consumer_by_circle> cc = await _mLearningService.GetConsumersByCircle(CircleID);
            return Json(cc.ToDataSourceResult(request));
        }


        [Authorize(Roles = Constants.PublisherRole)]

        public async Task<ActionResult> LearningObjects(int id, int? idInst)
        {
            CircleID = ViewBag.CircleID = id;
            var circle = await _mLearningService.GetObjectWithId<Circle>(id);
            ViewBag.CircleName = circle.name;

            if (idInst != null)
            {
                ViewBag.InstitutionId = InstitutionID = idInst ?? default(int);
            }

            ViewBag.institution = await _mLearningService.GetObjectWithId<Institution>(InstitutionID);

            return View();
        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> LO(int? id, int? circleId)
        {
            if (id != null)
            {
                LOID = ViewBag.LOID = id ?? default(int);
                LearningObject model = await _mLearningService.GetObjectWithId<LearningObject>(LOID);
                ViewBag.CircleID = CircleID;
                //ViewBag.tagList = await _mLearningService.GetTagsByLO(LOID);
                ViewBag.tagList = await _mLearningService.GetLOTags(LOID);
                //ViewBag.tagList = tagList.Select(t => t.tag_id);
                return View(model);
            }
            if (circleId != null)
                CircleID = ViewBag.CircleID = circleId ?? default(int);



            return View();
        }
        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> LOs_read([DataSourceRequest] DataSourceRequest request)
        {
            var los = await _mLearningService.GetLOByCircle(CircleID);
            foreach (var item in los)
            {
                item.description = "";
            }
            return Json(los.ToDataSourceResult(request));
        }
        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> LODetail(int id)
        {
            LOID = ViewBag.LOID = id;
            LearningObject model = await _mLearningService.GetObjectWithId<LearningObject>(LOID);
            Circle_has_LO circle_has_lo = await _mLearningService.GetCircleLOByIdLO(model.id);
            ViewBag.idCircle = circle_has_lo.circle_id;

            ViewBag.LOsections = await _mLearningService.GetSectionsByLO(LOID);

            return View(model);
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> Read_SectionPages(int id)
        {
            var pages = await _mLearningService.GetPagesByLOSection(id);
            return Json(pages);
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> Read_LOPages(int id)
        {
            var pages = await _mLearningService.GetPagesByLO(id);
            return Json(pages);
        }

        //Not used
        //Upload controller used instead
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> CreateLO(LearningObject obj, string __description/*, List<Tag> tags*/)
        {
            var files = Request.Files;
           // var tags = Request.Form["tags"];
            //var js = new JavaScriptSerializer();
            //ICollection<object> tagsDeserialize = js.Deserialize<ICollection<object>>(tags);

            //var file1 = files_.Get("fileCover");

            if (PublisherID == default(int))
                return Json(new JsonActionResult { errors = new String[] { "No publisher defined" } });
            try
            {
                // upload images
                string url_cover = null;
                string url_background = null;
                if (files != null && files.Count > 0)
                {
                    var fileCover = files.Get("fileCover");
                    if (fileCover != null && fileCover.ContentLength > 0)
                    {
                        using (MemoryStream target = new MemoryStream())
                        {
                            fileCover.InputStream.CopyTo(target);
                            url_cover = await _mLearningService.UploadResource(target, null);
                        }
                    }
                    if (files.Count > 1)
                    {
                        var fileBackground = files.Get("fileBackground");
                        if (fileBackground != null && fileBackground.ContentLength > 0)
                        {
                            using (MemoryStream target = new MemoryStream())
                            {
                                fileBackground.InputStream.CopyTo(target);
                                url_background = await _mLearningService.UploadResource(target, null);
                            }
                        }
                    }
                }
                //Create LO
                
                obj.created_at = DateTime.UtcNow;
                obj.updated_at = DateTime.UtcNow;
                obj.url_cover = url_cover;
                obj.url_background = url_background;
                obj.description = __description;
                obj.Publisher_id = PublisherID;

                int LO_id = await _mLearningService.CreateObject<LearningObject>(obj, lo => lo.id);

                await _mLearningService.PublishLearningObjectToCircle(CircleID, LO_id);
                /*
                foreach (Tag t in tags)
                {
                    await _mLearningService.AddTagToLO(t.id, LO_id);
                }*/

                /*foreach (var item in tagsDeserialize)
                {
                    Dictionary<string, object> tag = (Dictionary<string, object>)item;
                    int id = Convert.ToInt32(tag["id"]);
                    await _mLearningService.AddTagToLO(id, LO_id);
                }*/

                this.AddToastMessage("Guardado", "La información se guardo con éxito.", ToastType.Success);

                return Json(new JsonActionResult { url = Url.Action("LODetail", new { id = LO_id }) });
            }
            catch (Exception e)
            {
                //return Json(new { errors = new Object[] { e } });
                return Json(new JsonActionResult() { errors = new String[] { e.Message } });
            }
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> UpdateLO(LearningObject obj, string __description/*, List<Tag> tags*/)
        {
            var files = Request.Files;
           /* var tags = Request.Form["tags"];
            var js = new JavaScriptSerializer();
            ICollection<object> tagsDeserialize = js.Deserialize<ICollection<object>>(tags);*/

            try
            {
                //actualizando la imagen -- Verificar aqui.. 
                string url_cover = null;
                string url_background = null;

                if (files != null && files.Count > 0)
                {
                    var fileCover = files.Get("fileCover");
                    if (fileCover != null && fileCover.ContentLength > 0)
                    {
                        using (MemoryStream target = new MemoryStream())
                        {
                            fileCover.InputStream.CopyTo(target);
                            url_cover = await _mLearningService.UploadResource(target, null);
                        }
                    }
                    /*if (files.Count > 1)
                    {*/
                        var fileBackground = files.Get("fileBackground");
                        if (fileBackground != null && fileBackground.ContentLength > 0)
                        {
                            using (MemoryStream target = new MemoryStream())
                            {
                                fileBackground.InputStream.CopyTo(target);
                                url_background = await _mLearningService.UploadResource(target, null);
                            }
                        }
                    //}
                }

                obj.updated_at = DateTime.UtcNow;
                obj.url_cover = url_cover != null ? url_cover : obj.url_cover;
                obj.url_background = url_background != null ? url_background : obj.url_background;
                obj.description = __description;
                obj.Publisher_id = PublisherID;
                await _mLearningService.UpdateObject<LearningObject>(obj);

                ///Update tag list
                /*if (tags != null)
                {
                    var curtags = await _mLearningService.GetLOTags(obj.id);
                    var curtagsids = curtags.Select(t => t.id);
                    var tagids = tags.Select(t => t.id);

                    var toDelete = curtagsids.Except(tagids).ToList();
                    var toAdd = tagids.Except(curtagsids).ToList();

                    foreach (int id in toDelete)
                    {
                        await _mLearningService.DeleteTagFromLO(id, obj.id);
                    }

                    foreach (int id in toAdd)
                    {
                        await _mLearningService.AddTagToLO(id, obj.id);
                    }
                }

                */
                
                this.AddToastMessage("Guardado", "La información se guardo con éxito.", ToastType.Success);

                return Json(new JsonActionResult());
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", "Hubo un problema al actualizar la información.", ToastType.Error);

                return Json(new { errors = new String[] { e.Message } });
            }
        }

        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> UpdateSection(LOsection obj /*, List<Tag> tags*/)
        {
            await _mLearningService.UpdateObject<LOsection>(obj);
            var data = new
            {
                success = true
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> DeleteSection(LOsection section)
        {

            try
            {
                await _mLearningService.DeleteObject<LOsection>(section);
                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var data = new
                {
                    success = false,
                    message = e.Message
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> createLOSection(LOsection data)
        {
            String[] _errors;
            int id = default(int);
            try
            {
                id = await _mLearningService.CreateObject<LOsection>(data, o => o.id);
                _errors = new String[] { };
            }
            catch (Exception e)
            {
                _errors = new String[] { e.Message };
            }

            return Json(new { errors = _errors, result_id = id, url = Url.Action("", "Page", new { sectionId = id }) });
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        async public Task<ActionResult> Upload(HttpPostedFileBase file, HttpPostedFileBase bg_file, LearningObject obj)
        {

            string cover_url = null;
            string bg_url = null;
            if (file != null && file.ContentLength > 0)
            {


                using (MemoryStream target = new MemoryStream())
                {

                    file.InputStream.CopyTo(target);

                    cover_url = await _mLearningService.UploadResource(target, null);
                }


            }

            if (bg_file != null && bg_file.ContentLength > 0)
            {


                using (MemoryStream target = new MemoryStream())
                {
                    bg_file.InputStream.CopyTo(target);

                    bg_url = await _mLearningService.UploadResource(target, null);
                }


            }


            obj.created_at = DateTime.UtcNow;
            obj.updated_at = DateTime.UtcNow;
            //obj.url_cover = cover_url;
            //obj.url_background = bg_url;

            obj.Publisher_id = PublisherID;


            int LO_id = await _mLearningService.CreateObject<LearningObject>(obj, lo => lo.id);

            return RedirectToAction("Index", new { id = UserID });
        }


        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> EditLO(int? lo_id)
        {

            if (lo_id != null)
            {
                int nonull = lo_id ?? default(int);

                LOID = nonull;
            }
            else
            {
                if (LOID == default(int))
                {
                    return RedirectToAction("Index", "Home");
                }
            }


            var lo = await _mLearningService.GetObjectWithId<LearningObject>(LOID);

            var pages = await _mLearningService.GetPagesByLO(LOID);

            List<Quiz> quizzes = await _mLearningService.GetQuizzesByLO(LOID);


            return View("LOEdit", new LearningObjectPageViewModel { LO = lo, Pages = pages, Quizzes = quizzes });


        }
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> EditLO(int lo_id, LearningObjectPageViewModel vm)
        {
            try
            {
                LearningObject obj = vm.LO;


                //Update DB
                await _mLearningService.UpdateObject<LearningObject>(obj);


                return RedirectToAction("Index", new { id = UserID });

            }
            catch
            {
                return RedirectToAction("Index", new { id = UserID });
            }

        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> DeleteLO(int lo_id)
        {
            var circle = await _mLearningService.GetObjectWithId<LearningObject>(lo_id);



            return View("LODelete", circle);
        }

        //
        // POST: /Default1/Delete/5
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteLO(int lo_id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here


                _mLearningService.DeleteObject<LearningObject>(new LearningObject { id = lo_id });



                return RedirectToAction("Index", new { id = UserID });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { id = UserID });
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> LO_Destroy([DataSourceRequest] DataSourceRequest request, lo_in_circle lo_in_c)
        {
            try
            {
                LearningObject learningObject = new LearningObject
                {
                    id = lo_in_c.id
                };
                Circle_has_LO circleHasLO = new Circle_has_LO
                {
                    id = lo_in_c.circle_has_lo_id
                };
                await _mLearningService.DeleteObject(circleHasLO);
                await _mLearningService.DeleteObject(learningObject);

                this.AddToastMessage("Eliminado", "La unidad se eliminó con éxito.", ToastType.Success);

                return Json(new[] { lo_in_c }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", "Hubo un problema al eliminar la unidad", ToastType.Error);
                return Json(new { success = false, message = e.Message });
            }
        }

        /*********************************************************************************************************/
        #region OLD cruds
        // GET: /Publisher/CreateCircle
        [Authorize(Roles = Constants.PublisherRole)]
        public ActionResult CreateCircle()
        {
            return View("CircleCreate");
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        async public Task<ActionResult> CreateCircle(Circle circleObj)
        {
            try
            {
                int circle_id = await _mLearningService.CreateCircle(UserID, circleObj.name, circleObj.type);

                //Register the Publisher as a user in a Circle
                await _mLearningService.CreateObject<CircleUser>(new CircleUser { Circle_id = circle_id, User_id = UserID, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow }, c => c.id);

                return RedirectToAction("Index", new { id = UserID });
            }
            catch
            {
                return RedirectToAction("Index", new { id = UserID });
            }
        }

        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> ManageCircle(int circle_id)
        {


            CircleID = circle_id;

            var losInCircle = await _mLearningService.GetLOByCircle(circle_id);

            var consumersInCircle = await _mLearningService.GetConsumersByCircle(circle_id);

            var consumersInInst = await _mLearningService.GetConsumersByInstitution(InstitutionID);

            var lospublic = await _mLearningService.GetPublicLOs();

            return View("CircleManage", new ManageCircleViewModel { LOInCircle = losInCircle, LOPublic = lospublic, ConsumerInCircle = consumersInCircle, ConsumerInInst = consumersInInst });


        }



        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> EditCircle(int circle_id)
        {

            var circle = await _mLearningService.GetObjectWithId<Circle>(circle_id);



            return View("CircleEdit", circle);


        }


        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> EditCircle(int circle_id, Circle circleObj)
        {
            try
            {



                //Update DB
                await _mLearningService.UpdateObject<Circle>(circleObj);


                return RedirectToAction("Index", new { id = UserID });

            }
            catch
            {
                return RedirectToAction("Index", new { id = UserID });
            }

        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> DeleteCircle(int circle_id)
        {
            var circle = await _mLearningService.GetObjectWithId<Circle>(circle_id);



            return View("CircleDelete", circle);
        }

        //
        // POST: /Default1/Delete/5
        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteCircle(int circle_id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here


                _mLearningService.DeleteObject<Circle>(new Circle { id = circle_id });



                return RedirectToAction("Index", new { id = UserID });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { id = UserID });
            }
        }



        // GET: /Publisher/CreateLO
        [Authorize(Roles = Constants.PublisherRole)]
        public ActionResult CreateLO()
        {


            return View("LOCreate");
        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> Remove(int user_id)
        {


            if (CircleID != null)
            {
                await _mLearningService.UnSubscribeConsumerFromCircle(user_id, CircleID);
            }


            return RedirectToAction("ManageCircle", new { circle_id = CircleID });
        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> Add(int user_id)
        {


            if (CircleID != null)
            {
                await _mLearningService.AddUserToCircle(user_id, CircleID);
            }


            return RedirectToAction("ManageCircle", new { circle_id = CircleID });
        }



        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> AddLOToCircle(int lo_id)
        {


            if (CircleID != null)
            {
                await _mLearningService.PublishLearningObjectToCircle(CircleID, lo_id);
            }


            return RedirectToAction("ManageCircle", new { circle_id = CircleID });
        }



    }
        #endregion
}