using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MLearning.Core.Entities.json;

namespace MLearning.Web.Controllers
{
    public class ResourcesController : MLController
    {

        // GET: Resources
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<ActionResult> UploadImage(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                string width = "";
                string height = "";
                string result_url = null;
                var files_ = Request.Files;
                if (files_ != null && files_.Count > 0)
                {
                    var fileCover = files_[0];
                    if (fileCover != null && fileCover.ContentLength > 0)
                    {
                        using (MemoryStream target = new MemoryStream())
                        {
                            fileCover.InputStream.CopyTo(target);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(target);
                            width = image.Width.ToString();
                            height = image.Height.ToString();
                            result_url = await _mLearningService.UploadResource(target, null);
                        }
                    }
                }
                /*
                if (files != null && files.Count() > 0)
                {
                    var file = files.ElementAt(0);
                    if (file != null && file.ContentLength > 0)
                    {
                        using (MemoryStream target = new MemoryStream())
                        {
                            file.InputStream.CopyTo(target);
                            result_url = await _mLearningService.UploadResource(target, null);
                        }
                    }
                }*/
                return Json(new JsonActionResult() { errors = new String[] { width, height }, url = result_url });
            }
            catch (Exception e)
            {
                return Json(new JsonActionResult() { errors = new String[] { e.Message } });
            }
        }
        [Authorize]
        public async Task<ActionResult> Read_tags()
        {
            //TODO filter values in sql server            
            //string value = text?? default(String);
            var tags = await _mLearningService.GetAllTags();
            return Json(tags);
        }

    }
}