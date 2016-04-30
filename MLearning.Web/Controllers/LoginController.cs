using Cirrious.CrossCore;
using Core.Entities.json;
using Core.Repositories;
using Core.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.WindowsAzure.MobileServices;
using MLearning.Core.Configuration;
using MLearning.Core.Entities;
using MLearning.Core.Services;
using MLearning.Web.Singleton;
using MLearningDB;
using MLearningDBResult;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MLearning.Web.Controllers
{
    public class LoginController : MLController
    {


        IMLearningService _mLearningService;

        public LoginController()
        {

            _mLearningService = ServiceManager.GetService();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SignIn(bool isPersistent, MLearningDBResult.User user, UserType type)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = new ClaimsIdentity(new[] { 
                new Claim(ClaimTypes.NameIdentifier, user.username), 
                new Claim(ClaimTypes.Role,type.ToString()), 
                new Claim(ClaimTypes.Name, user.username)}, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

        }


        // GET: Login
        public async Task<ActionResult> Index()
        {
            if (Request.IsAuthenticated && userType != null)
            {
                return redirectToRoleHome();
            }
            var institutions = await _mLearningService.GetInstitutions();
            ViewBag.institutions = institutions;
            return View();
        }

        [HttpPost]
        async public Task<ActionResult> Login(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                string username = collection.Get("username");

                string password = EncryptionService.encrypt(collection.Get("password"));

                MLearningDBResult.User user = new MLearningDBResult.User { username = username, password = password };


                LoginOperationResult<MLearningDBResult.User> result = await _mLearningService.ValidateLogin<MLearningDBResult.User>(user, u => u.password == user.password && u.username == user.username, u => u.id, u => u.type);
                // LoginOperationResult result = await _mLearningService.ValidateConsumerLogin(user.username,user.password);
                UserID = result.id;
                if (result.successful)
                {
                    Institution_has_User consumer = await _mLearningService.GetInstitutionByUser(UserID);
                    if (consumer != null)
                        InstitutionID = consumer.institution_id;
                    Password = password;
                    userType = (UserType)result.userType;
                    //Session Code HERE
                    SignIn(false, user, userType ?? default(UserType));

                    return redirectToRoleHome();

                }
                else
                {
                    ViewBag.success = false;
                    ViewBag.message = "El nombre de usuario o la contraseña no son válidas.";
                    //return RedirectToAction("Index");
                    var institutions = await _mLearningService.GetInstitutions();
                    ViewBag.institutions = institutions;
                    return View("Index");
                }

            }
            catch
            {
                return View();
            }

        }
        [HttpPost]
        public async Task<ActionResult> Register(MLearningDB.User model, int institution)
        {
            try
            {
                // TODO: Add insert logic here
                int idInstitution = institution;
                InstitutionID = idInstitution;
                model.password = EncryptionService.encrypt(model.password);
                bool exists = await _mLearningService.CheckIfExistsNoLocale<MLearningDB.User>
                    (usr => usr.username == model.username, (it) => it.updated_at, it => it.id);
                if (exists)
                {
                    ViewBag.success = false;
                    ViewBag.message = "El nombre de usuario ya existe.";
                    var institutions = await _mLearningService.GetInstitutions();
                    ViewBag.institutions = institutions;
                    return View("Index", model);
                }
                OperationResult op = await _mLearningService.CreateAndRegisterConsumer(model, InstitutionID);

                MLearningDB.User user = new MLearningDB.User { username = model.username, password = model.password };

                // LoginOperationResult<User> result = await _mLearningService.ValidateLogin<User>(user, u => u.password == user.password && u.username == user.username, u => u.id, u => u.type);
                // LoginOperationResult<user_consumer> result = await _mLearningService.ValidateConsumerLogin(user.username, user.password);
                UserID = op.id;
                userType = UserType.Consumer;
                //Session Code HERE
                MLearningDBResult.User user2 = new MLearningDBResult.User();
                user2.username = user.username;
                user2.password = user.password;

                //  Registramos por defecto a un usuario en el curso o circulo 38
                await _mLearningService.AddUserToCircle(UserID, 38);
                SignIn(false, user2, userType ?? default(UserType));

                return redirectToRoleHome();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                return RedirectToAction("Index");
            }
        }
        //MobileServiceUser user1;
        [HttpPost]
        public async Task<ActionResult> LoginFacebook(string socialId, string token, int idInstitution)
        {
            socialId = "";
            /*
            MobileServiceUser user = new MobileServiceUser();
            user.UserId = socialId;
            user.MobileServiceAuthenticationToken = token;
            */
            // string json = @"{'access_token':" + token + "}";
            JObject access_token = new JObject();
            access_token["access_token"] = token;
            try
            {
                WAMSRepositoryService service3 = new WAMSRepositoryService();
                int provider = 3; // facebook
                MobileServiceUser user1 = await _mLearningService.LoginProvider(provider, access_token);
                socialId = user1.UserId;
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
                //return View("Login");
            }

            int userId = await _mLearningService.TryCreateUser(socialId, idInstitution);
            //_mLearningService.RegisterUserToInstitution(userId, InstitutionID);
            MLearningDBResult.User user = await _mLearningService.GetObjectWithId<MLearningDBResult.User>(userId);
            if (user != null)
            {
                UserID = userId;
                Institution_has_User consumer = await _mLearningService.GetInstitutionByUser(userId);
                if (consumer != null)
                    InstitutionID = consumer.institution_id;
                Password = user.password;
                userType = (UserType) user.type;

                //  Registramos por defecto a un usuario en el curso o circulo 38
                try{
                    await _mLearningService.AddUserToCircle(UserID, 38);
                }
                catch (Exception e){
                    SignIn(false, user, userType ?? default(UserType));
                }
                //Session Code HERE
                SignIn(false, user, userType ?? default(UserType));
                return redirectToRoleHome();
            }
            else
            {
                ViewBag.success = false;
                ViewBag.message = "No de ha podido registrar el usuario, actualice la pagina y vuelvalo a intentar.";
                //return RedirectToAction("Index");
                var institutions = await _mLearningService.GetInstitutions();
                ViewBag.institutions = institutions;
                return View("Index");
            }
        }

        ActionResult redirectToRoleHome()
        {
            UserType uType = (userType ?? default(UserType));
            switch ((UserType)uType)
            {
                case UserType.SuperAdmin:

                    return RedirectToAction("Index", "Admin");

                case UserType.Head:

                    return RedirectToAction("Index", "Head", new { id = UserID });

                case UserType.Publisher:

                    return RedirectToAction("Index", "Publisher", new { id = UserID });

                case UserType.Consumer:
                    return RedirectToAction("Index", "Consumer");

                default:
                    return RedirectToAction("Index");

            }
        }

        [HttpPost]
        public async Task<ActionResult> LogOff()
        {
            var user = await _mLearningService.GetObjectWithId<MLearningDB.User>(UserID);
            user.is_online = false;
            await _mLearningService.UpdateObject<MLearningDB.User>(user);
            _mLearningService.Logout();
            AuthenticationManager.SignOut();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<JsonResult> GetUserLogged()
        {
            if (Request.IsAuthenticated && userType != null)
            {
                var user = await _mLearningService.GetObjectWithId<MLearningDBResult.User>(UserID);
                var UserSession = new
                {
                    id = user.id,
                    name = user.name,
                    photo = user.image_url,
                    role = userType.ToString()
                };
                return Json(UserSession);
            }
            else
            {
                var UserSession = new
                {
                    id = 0,
                    name = "",
                    photo = "",
                    role = ""
                };
                return Json(UserSession);
            }
        }
    }
}
