using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SocialGoal.Web.Models;
using SocialGoal.Models;
using SocialGoal.Data.Models;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Mailers;
using SocialGoal.Web.ViewModels;
using AutoMapper;
using System.Drawing;
using System.IO;
using SocialGoal.Properties;
using System.Net;
using System.Drawing.Drawing2D;
using SocialGoal.Web.Helpers;

namespace SocialGoal.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService userService;
        private IUserProfileService userProfileService;
        private IGoalService goalService;
        private IUpdateService updateService;
        private ICommentService commentService;
        private IFollowRequestService followRequestService;
        private IFollowUserService followUserService;
        private ISecurityTokenService securityTokenService;
        private IUserMailer userMailer = new UserMailer();
        private UserManager<ApplicationUser> UserManager;
        public AccountController(IUserService userService, IUserProfileService userProfileService, IGoalService goalService, IUpdateService updateService, ICommentService commentService, IFollowRequestService followRequestService, IFollowUserService followUserService, ISecurityTokenService securityTokenService, UserManager<ApplicationUser> userManager)
        {
            this.userService = userService;
            this.userProfileService = userProfileService;
            this.goalService = goalService;
            this.updateService = updateService;
            this.commentService = commentService;
            this.followRequestService = followRequestService;
            this.followUserService = followUserService;
            this.securityTokenService = securityTokenService;
            this.UserManager = userManager;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.QueryString["guid"] != null)
                SocialGoalSessionFacade.JoinGroupOrGoal = Request.QueryString["guid"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var userId = user.Id;
                    var userName = user.UserName;
                    userProfileService.CreateUserProfile(userId);
                    await SignInAsync(user, isPersistent: false);
                    if (SocialGoalSessionFacade.JoinGroupOrGoal != null)
                    {
                        string groupOrGoalJoinToken = SocialGoalSessionFacade.JoinGroupOrGoal;
                        if (groupOrGoalJoinToken.StartsWith("gr:"))
                        {
                            Guid groupIdToken = new Guid(groupOrGoalJoinToken.Substring(3));
                            TempData["grToken"] = groupIdToken;
                            return RedirectToAction("AddGroupUser", "EmailRequest");
                        }
                        else if (groupOrGoalJoinToken.StartsWith("go:"))
                        {
                            Guid goalIdToken = new Guid(groupOrGoalJoinToken.Substring(3));
                            TempData["goToken"] = goalIdToken;
                            return RedirectToAction("AddSupportToGoal", "EmailRequest");
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            SocialGoalSessionFacade.Clear();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public PartialViewResult LoginPartial()
        {
            return PartialView("_LoginPartial", new LoginViewModel());
        }

        [AllowAnonymous]
        public PartialViewResult RegisterPartial()
        {
            return PartialView("_Registerpartial", new RegisterViewModel());
        }

        public IEnumerable<ApplicationUser> SearchUser(string name)
        {
            var users = userService.SearchUser(name);
            return users;
        }

        public ActionResult ImageUpload()
        {
            UploadImageViewModel imageVM = new UploadImageViewModel();
            imageVM.LocalPath = userService.GetUser(User.Identity.GetUserId()).ProfilePicUrl;
            return PartialView(imageVM);
        }

        [HttpPost]
        public ActionResult UploadImage(UploadImageViewModel model)
        {
            //Check if all simple data annotations are valid
            if (ModelState.IsValid)
            {
                //Prepare the needed variables
                Bitmap original = null;
                var name = "newimagefile";
                var errorField = string.Empty;

                if (model.IsUrl)
                {
                    errorField = "Url";
                    name = GetUrlFileName(model.Url);
                    original = GetImageFromUrl(model.Url);
                }
                else if (model.File != null)
                {
                    errorField = "File";
                    name = Path.GetFileNameWithoutExtension(model.File.FileName);
                    original = Bitmap.FromStream(model.File.InputStream) as Bitmap;
                }

                //If we had success so far
                if (original != null)
                {
                    var img = CreateImage(original, model.X, model.Y, model.Width, model.Height);
                    var fileName = Guid.NewGuid().ToString();
                    var oldFilepath = userService.GetUser(User.Identity.GetUserId()).ProfilePicUrl;
                    var oldFile = Server.MapPath(oldFilepath);
                    //Demo purposes only - save image in the file system
                    var fn = Server.MapPath("~/Content/ProfilePics/" + fileName + ".png");
                    img.Save(fn, System.Drawing.Imaging.ImageFormat.Png);
                    userService.SaveImageURL(User.Identity.GetUserId(), "~/Content/ProfilePics/" + fileName + ".png");
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                    return RedirectToAction("UserProfile", new { id = User.Identity.GetUserId() });
                }
                else //Otherwise we add an error and return to the (previous) view with the model data
                    ModelState.AddModelError(errorField, Resources.UploadError);
            }

            return View("ImageUpload", model);
        }

        /// <summary>
        /// Gets an image from the specified URL.
        /// </summary>
        /// <param name="url">The URL containing an image.</param>
        /// <returns>The image as a bitmap.</returns>
        Bitmap GetImageFromUrl(string url)
        {
            var buffer = 1024;
            Bitmap image = null;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return image;

            using (var ms = new MemoryStream())
            {
                var req = WebRequest.Create(url);

                using (var resp = req.GetResponse())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        var bytes = new byte[buffer];
                        var n = 0;

                        while ((n = stream.Read(bytes, 0, buffer)) != 0)
                            ms.Write(bytes, 0, n);
                    }
                }

                image = Bitmap.FromStream(ms) as Bitmap;
            }

            return image;
        }

        /// <summary>
        /// Gets the filename that is placed under a certain URL.
        /// </summary>
        /// <param name="url">The URL which should be investigated for a file name.</param>
        /// <returns>The file name.</returns>
        string GetUrlFileName(string url)
        {
            var parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var last = parts[parts.Length - 1];
            return Path.GetFileNameWithoutExtension(last);
        }

        /// <summary>
        /// Creates a small image out of a larger image.
        /// </summary>
        /// <param name="original">The original image which should be cropped (will remain untouched).</param>
        /// <param name="x">The value where to start on the x axis.</param>
        /// <param name="y">The value where to start on the y axis.</param>
        /// <param name="width">The width of the final image.</param>
        /// <param name="height">The height of the final image.</param>
        /// <returns>The cropped image.</returns>
        Bitmap CreateImage(Bitmap original, int x, int y, int width, int height)
        {
            var img = new Bitmap(width, height);

            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            }

            return img;
        }


        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        [HttpGet]
        public ViewResult UserProfile(string id)
        {
            var currentuserid = User.Identity.GetUserId();
            var user = userService.GetUserProfile(id);
            var userdetail = userProfileService.GetUser(id);
            UserProfileViewModel userprofile = new UserProfileViewModel()
            {
                FirstName = userdetail.FirstName,
                LastName = userdetail.LastName,
                Email = userdetail.Email,
                UserName = user.UserName,
                DateCreated = user.DateCreated,
                LastLoginTime = user.LastLoginTime,
                UserId = user.Id,
                ProfilePicUrl = user.ProfilePicUrl,
                DateOfBirth = userdetail.DateOfBirth,
                Gender = userdetail.Gender,
                Address = userdetail.Address,
                City = userdetail.City,
                State = userdetail.State,
                Country = userdetail.Country,
                ZipCode = userdetail.ZipCode,
                ContactNo = userdetail.ContactNo,
            };
            if (followRequestService.RequestSent((User.Identity.GetUserId()), id))
            {
                userprofile.RequestSent = true;
            }
            if (followUserService.IsFollowing(currentuserid, id))
            {
                userprofile.Following = true;
            }
            return View(userprofile);
        }


        public ActionResult EditBasicInfo()
        {
            var user = userProfileService.GetUser(User.Identity.GetUserId());
            UserProfileFormModel editUser = Mapper.Map<UserProfile, UserProfileFormModel>(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("EditBasicInfo", editUser);
        }

        public ActionResult EditPersonalInfo()
        {
            var user = userProfileService.GetUser(User.Identity.GetUserId());
            UserProfileFormModel editUser = Mapper.Map<UserProfile, UserProfileFormModel>(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("EditPersonalInfo", editUser);
        }


        [HttpPost]
        public ActionResult EditProfile(UserProfileFormModel editedProfile)
        {
            UserProfile user = Mapper.Map<UserProfileFormModel, UserProfile>(editedProfile);
            ApplicationUser applicationUser = userService.GetUser(editedProfile.UserId);
            applicationUser.FirstName = editedProfile.FirstName;
            applicationUser.LastName = editedProfile.LastName;
            applicationUser.Email = editedProfile.Email;
            if (ModelState.IsValid)
            {
                userService.UpdateUser(applicationUser);
                userProfileService.UpdateUserProfile(user);
                return RedirectToAction("UserProfile", new { id = editedProfile.UserId });
            }
            return PartialView("EditProfile", editedProfile);
        }


        public ActionResult FollowRequest(string id)
        {
            var followRequestFormModel = new FollowRequestFormModel()
            {
                FromUserId = User.Identity.GetUserId(),
                ToUserId = id,
                FromUser = userService.GetUser(User.Identity.GetUserId()),
                ToUser = userService.GetUser(id)
            };
            var followRequest = Mapper.Map<FollowRequestFormModel, FollowRequest>(followRequestFormModel);
            followRequestService.CreateFollowRequest(followRequest);
            return RedirectToAction("UserProfile", new { id = followRequestFormModel.ToUserId });
        }


        public ActionResult AcceptRequest(string touserid, string fromuserid)
        {
            var newFollowUser = new FollowUser()
            {
                Accepted = true,
                FromUserId = fromuserid,
                ToUserId = touserid,
                FromUser = userService.GetUser(fromuserid),
                ToUser = userService.GetUser(touserid)
            };
            followUserService.CreateFollowUserFromRequest(newFollowUser, followRequestService);
            return RedirectToAction("Index", "Notification");
        }

        public ActionResult RejectRequest(string toUserId, string fromuserId)
        {
            followRequestService.DeleteFollowRequest(fromuserId, toUserId);
            return RedirectToAction("Index", "Notification");
        }

        public ActionResult Unfollow(string id)
        {
            followUserService.DeleteFollowUser(id, User.Identity.GetUserId());
            return RedirectToAction("UserProfile", new { id = id });

        }


        /// <summary>
        /// Followed users by page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Followers(int page = 0)
        {
            var users = followUserService.GetFollowers(User.Identity.GetUserId(), page, 10);
            var followers = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<FollowersViewModel>>(users);

            if (Request.IsAjaxRequest())
            {
                if (followers.Count() != 0)
                    return PartialView("_FollowersView", followers);
                else
                    return null;
            }

            return View("FollowerUsers", followers);
        }

        /// <summary>
        /// following users by page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Followings(int page = 0)
        {
            var users = followUserService.GetFollowings(User.Identity.GetUserId(), page, 10);
            var followings = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<FollowingViewModel>>(users);

            if (Request.IsAjaxRequest())
            {
                if (followings.Count() != 0)
                    return PartialView("_FollowingsView", followings);
                else
                    return null;
            }

            return View("FollowingUsers", followings);
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //public IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //    set { _authnManager = value; }
        //}

        // Add this private variable
      private IAuthenticationManager _authnManager;

        // Modified this from private to public and add the setter
      public IAuthenticationManager AuthenticationManager
      {
          get
          {
              if (_authnManager == null)
                  _authnManager = HttpContext.GetOwinContext().Authentication;
              return _authnManager;
          }
          set { _authnManager = value; }
      }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}