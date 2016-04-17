﻿using Core.DownloadCache;
using MLearning.Core.Configuration;
using MLearning.Core.Entities;
using MLearning.Core.Entities.json;
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

namespace MLearning.Web.Controllers
{
    public class PageController : MLController
    {


        private IMLearningService _mLearningService;
        public PageController()
            : base()
        {
            _mLearningService = ServiceManager.GetService();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            // Let other exceptions just go unhandled
            if (filterContext.Exception is ArgumentException)
            {
                // Default view is "error"
                RedirectToAction("Index", "Home");
            }
        }

        //public ActionResult Details() 
        // GET: /Page/Create
        //
        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> Index(int? id, int? sectionId)
        {
            PageID = ViewBag.PageID = id ?? default(int);
            Page page = null;
            if (id != null)
            {
                page = await _mLearningService.GetObjectWithId<Page>(id ?? default(int));
                ViewBag.currentLO = await _mLearningService.GetObjectWithId<LearningObject>(page.lo_id);
                //var tagList = await _mLearningService.GetTagsByPage(page.id);
                //ViewBag.pageTag = tagList.First();
                sectionId = page.LOsection_id;
            }
            if (sectionId != null && sectionId != default(int))
            {
                //ViewBag.currentLOsection = await _mLearningService.GetObjectWithId<LOsection>(sectionId ?? default(int));
                LOsection currentLOsection = await _mLearningService.GetObjectWithId<LOsection>(sectionId ?? default(int));
                ViewBag.currentLOsection = currentLOsection;
                ViewBag.currentLO = await _mLearningService.GetObjectWithId<LearningObject>(currentLOsection.LO_id);
            }
            return View(page);
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        public async Task<ActionResult> Create(Page page/*, Tag tag*/)
        {
            page.created_at = DateTime.UtcNow;
            page.updated_at = DateTime.UtcNow;
            int id = await _mLearningService.CreateObject<Page>(page, p => p.id);
            //await _mLearningService.AddTagToPage(tag.id, id);
            return Json(new JsonActionResult { resultId = id, url = Url.Action("", new { id = id }) });
        }
        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> CreateTag(Tag tag)
        {
            tag.created_at = DateTime.UtcNow;
            tag.updated_at = DateTime.UtcNow;
            int id = await _mLearningService.CreateObject<Tag>(tag, t => t.id);
            return Json(new JsonActionResult { resultId = id});
        }
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        public async Task<ActionResult> CreatePages(ICollection<Page> pages)
        {
            foreach (var page in pages)
            {
                page.created_at = DateTime.UtcNow;
                page.updated_at = DateTime.UtcNow;
                int id = await _mLearningService.CreateObject<Page>(page, p => p.id);
            }
            //await _mLearningService.AddTagToPage(tag.id, id);
            return Json(new JsonActionResult { resultId = 1, url = Url.Action("", new { id = 1 }) });
        }

        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> Update(int id)
        {
            PageID = ViewBag.PageID = id;
            Page page = null;
            int? sectionId = null;
            page = await _mLearningService.GetObjectWithId<Page>(id);
            ViewBag.currentLO = await _mLearningService.GetObjectWithId<LearningObject>(page.lo_id);
            var tagList = await _mLearningService.GetTagsByPage(page.id);
            ViewBag.pageTag = tagList.First();
            sectionId = page.LOsection_id;
            if (sectionId != null && sectionId != default(int))
            {
                ViewBag.currentLOsection = await _mLearningService.GetObjectWithId<LOsection>(sectionId ?? default(int));
            }
            return View(page);
        }

        [Authorize(Roles = Constants.PublisherRole)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Update(Page page/*, Tag tag*/)
        {
            page.updated_at = DateTime.UtcNow;
            await _mLearningService.UpdateObject<Page>(page);
            //var tagList = await _mLearningService.GetTagsByPage(page.id);
            //var _pageTag = tagList.First();
            /*if (tag.id != _pageTag.tag_id)
            {
                await _mLearningService.DeleteTagFromPage(_pageTag.tag_id, page.id);
                await _mLearningService.AddTagToPage(tag.id, page.id);
            }*/
            return Json(new JsonActionResult());
        }

        [Authorize(Roles = Constants.PublisherRole)]
        public ActionResult Create(int lo_id)
        {

            LOID = lo_id;
            //  return View("PageCreate");
            return View("PageTest");
        }

        //
        // Post: /Page/Create
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        [ValidateInput(false)]
        async public Task<ActionResult> Create_(Page page/*JsonPage page*/)
        {
            try
            {
                //Upload XML, and image if needed 
                // string xmlData = page.xml_content;

                // Page newPage = new Page { title = page.title, description = page.description, url_img = page.url_img, content = xmlData,lo_id=LOID,created_at=DateTime.UtcNow,updated_at=DateTime.UtcNow };

                //await _mLearningService.CreateObject<Page>(newPage,p=>p.id);

                page.lo_id = LOID;
                page.created_at = DateTime.UtcNow;
                page.updated_at = DateTime.UtcNow;

                await _mLearningService.CreateObject<Page>(page, p => p.id);



                //Create Page

                // return Json(Url.Action("EditLO", "Publisher", new { lo_id = LOID }));

                return RedirectToAction("EditLO", "Publisher", new { lo_id = LOID });
            }
            catch
            {
                return View();
            }
        }


        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> Tags(int? page_id)
        {

            if (page_id != null)
            {
                PageID = page_id ?? default(int);
            }
            else
            {
                if (PageID == default(int))
                    return RedirectToAction("Index", "Home");
            }


            var page_tags = await _mLearningService.GetTagsByPage(PageID);

            var all_tags = await _mLearningService.GetAllTags();



            return View(new PageTagViewModel { PageTags = page_tags, AllTags = all_tags });


        }


        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> DeleteTag(int tag_id)
        {

            await _mLearningService.DeleteTagFromPage(tag_id, PageID);

            return RedirectToAction("Tags", new { page_id = PageID });
        }

        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> AddTag(int tag_id)
        {

            try
            {
                await _mLearningService.AddTagToPage(tag_id, PageID);
            }
            catch
            {
                Console.WriteLine("Duplicated not allowed");
            }



            return RedirectToAction("Tags", new { page_id = PageID });
        }


        [Authorize(Roles = Constants.PublisherRole)]
        public async Task<ActionResult> TagCreate()
        {

            return View("TagCreate");
        }
        //
        // POST: /Page/Create
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        public ActionResult TagCreate(Tag tag)
        {
            try
            {

                tag.updated_at = DateTime.UtcNow;
                tag.created_at = DateTime.UtcNow;


                _mLearningService.CreateObject<Tag>(tag, t => t.id);

                // TODO: Add insert logic here

                return RedirectToAction("Tags", new { page_id = PageID });
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Page/Edit/5
        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> Edit(int? page_id)
        {
            if (page_id != null)
            {
                PageID = page_id ?? default(int);
            }
            else
            {
                if (PageID == default(int))
                {
                    RedirectToAction("Index", "Home");
                }
            }



            var page = await _mLearningService.GetObjectWithId<Page>(PageID);

            LOID = page.lo_id;
            return View("PageEdit", page);
        }

        //
        // POST: /Page/Edit/5
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        public ActionResult Edit(int page_id, Page page)
        {
            try
            {

                page.lo_id = LOID;

                _mLearningService.UpdateObject<Page>(page);
                // TODO: Add update logic here

                return RedirectToAction("EditLO", "Publisher", new { lo_id = LOID });
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Page/Delete/5
        [Authorize(Roles = Constants.PublisherRole)]
        async public Task<ActionResult> Delete(int page_id, int lo_id)
        {
            LOID = lo_id;
            Page todelete = await _mLearningService.GetObjectWithId<Page>(page_id);
            return View(todelete);
        }

        //
        // POST: /Page/Delete/5
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        public ActionResult Delete(int page_id, FormCollection form)
        {
            try
            {
                // TODO: Add delete logic here

                _mLearningService.DeleteObject<Page>(new Page { id = page_id });

                return RedirectToAction("EditLO", "Publisher", new { lo_id = LOID });
            }
            catch
            {
                return View();
            }
        }
        // GET: /Page/Delete/5
        [Authorize(Roles = Constants.PublisherRole)]
        [HttpPost]
        async public Task<ActionResult> DeletePage(int id)
        {
            //LOID = lo_id;
            Page todelete = await _mLearningService.GetObjectWithId<Page>(id);
            await _mLearningService.DeleteObject<Page>(todelete);
            var data = new
            {
                success = true
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
