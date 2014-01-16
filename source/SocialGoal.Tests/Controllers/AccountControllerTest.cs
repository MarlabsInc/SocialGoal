using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialGoal.Web.Controllers;
using NUnit.Framework;
using SocialGoal.Model.Models;
using Microsoft.AspNet.Identity;
using SocialGoal.Tests.Helpers;
using SocialGoal.Data.Repository;
using Moq;
using SocialGoal.Data.Infrastructure;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web;
using System.IO;
using SocialGoal.Web.Core.Authentication;
using SocialGoal.Service;
using SocialGoal.Web.Mailers;
using Mvc.Mailer;
using SocialGoal.Models;
using System.Linq.Expressions;
using Microsoft.Owin.Security;
using System.Web.Security;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.ViewModels;
using AutoMapper;
namespace SocialGoal.Web.Controllers.Test
{
    [TestFixture()]
    public class AccountControllerTest
    {
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;
        Mock<IFollowRequestRepository> followRequestRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<ISecurityTokenRepository> securityTokenRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<IPrincipal> principal;
        Mock<HttpContext> httpContext;
        Mock<HttpContextBase> contextBase;
        Mock<HttpRequestBase> httpRequest;
        Mock<HttpResponseBase> httpResponse;
        Mock<HttpSessionStateBase> httpSession;
        Mock<GenericPrincipal> genericPrincipal;

        Mock<TempDataDictionary> tempData;
        Mock<HttpPostedFileBase> file;
        Mock<Stream> stream;
        Mock<IFormsAuthentication> authentication;

        IUserService userService;
        IUserProfileService userProfileService;
        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IFollowRequestService followRequestService;
        IFollowUserService followUserService;
        ISecurityTokenService securityTokenService;
        IUserMailer userMailer = new UserMailer();
        //UserManager<ApplicationUser> userManager;
        Mock<AccountController> accountController;


        [SetUp]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            followRequestRepository = new Mock<IFollowRequestRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            securityTokenRepository = new Mock<ISecurityTokenRepository>();


            unitOfWork = new Mock<IUnitOfWork>();

            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);
            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
            followUserService = new FollowUserService(followUserRepository.Object, unitOfWork.Object);
            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);

            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContextBase>();
            httpRequest = new Mock<HttpRequestBase>();
            httpResponse = new Mock<HttpResponseBase>();
            genericPrincipal = new Mock<GenericPrincipal>();
            httpSession = new Mock<HttpSessionStateBase>();
            authentication = new Mock<IFormsAuthentication>();


            identity = new Mock<IIdentity>();
            principal = new Mock<IPrincipal>();
            tempData = new Mock<TempDataDictionary>();
            file = new Mock<HttpPostedFileBase>();
            stream = new Mock<Stream>();
            //testUserStore = new Mock<IUserStore<ApplicationUser>>();
            //testPasswordStore = new Mock<IUserPasswordStore<ApplicationUser>>();
            //userManager = new Mock<UserManager<ApplicationUser>>();
            accountController = new Mock<AccountController>();

        }

        [TearDown]
        public void TearDown()
        {
            TestSmtpClient.SentMails.Clear();
        }

        [Test]
        public void SearchUser()
        {
            IEnumerable<ApplicationUser> fake = new List<ApplicationUser> 
            {
             new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
              
         
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fake);
            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            IEnumerable<ApplicationUser> result = contr.SearchUser("u") as IEnumerable<ApplicationUser>;
            Assert.IsNotNull(result);


            Assert.AreEqual(4, result.Count(), "not matching");

        }

        [Test]
        public void Image_Upload_GetView()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                RoleId = 0
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;


            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            PartialViewResult result = controller.ImageUpload() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(UploadImageViewModel), result.ViewData.Model, "Wrong model");

            var data = result.ViewData.Model as UploadImageViewModel;

            Assert.AreEqual(null, data.LocalPath, "not matching");

        }

        [Test]
        public void Upload_Image_Post()
        {
            UploadImageViewModel image = new UploadImageViewModel()
            {
                IsFile = true,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                LocalPath = "dddd"
            };
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            ViewResult result = controller.UploadImage(image) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(UploadImageViewModel), result.ViewData.Model, "WrongType");
            Assert.AreEqual("ImageUpload", result.ViewName);

        }



        [Test]
        public void UserProfile()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName="adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);


            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);


            UserProfile prfil = new UserProfile()
            {
                FirstName="Adarsh",
                LastName="Vikraman",
                UserName="adarsh",
                DateOfBirth = DateTime.Now,
                Gender = true,
                Address = "a",
                City = "a",
                State = "a",
                Country = "a",
                ZipCode = 2344545,
                ContactNo = 1223344556,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(prfil);

            IEnumerable<FollowRequest> fake = new List<FollowRequest> {
            new FollowRequest { FollowRequestId =1, FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest { FollowRequestId =2, FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
            };
            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(fake);

            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee" },
           
            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);

            ViewResult result = controller.UserProfile("402bd590-fdc7-49ad-9728-40efbfe512ec") as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(UserProfileViewModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileViewModel;
            Assert.AreEqual("adarsh", data.UserName);
        }


        [Test]
        public void Edit_Basic_Info()
        {
            MemoryUser user = new MemoryUser("adarsh");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            UserProfile userProfile = new UserProfile()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                Email = "adarsh@foo.com",


            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(userProfile);

            Mapper.CreateMap<UserProfile, UserProfileFormModel>();

            PartialViewResult result = controller.EditBasicInfo() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(UserProfileFormModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileFormModel;
            Assert.AreEqual("adarsh@foo.com", data.Email);
        }

        [Test]
        public void Edit_Personal_Info()
        {
            MemoryUser user = new MemoryUser("adarsh");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            UserProfile grpuser = new UserProfile()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                Address = "t",
                City = "t",
                State = "adarsh@foo.com",


            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(grpuser);

            Mapper.CreateMap<UserProfile, UserProfileFormModel>();

            PartialViewResult result = controller.EditPersonalInfo() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(UserProfileFormModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileFormModel;
            Assert.AreEqual("t", data.Address);
        }

        [Test]
        public void Editprofile_Post()
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            UserProfileFormModel profile = new UserProfileFormModel();

            Mapper.CreateMap<UserProfileFormModel, UserProfile>();
            profile.FirstName ="adarsh";

            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            var result = contr.EditProfile(profile) as RedirectToRouteResult;

            Assert.AreEqual("UserProfile", result.RouteValues["action"]);
        }

        [Test]
        public void Follow_Request()
        {
            MemoryUser user = new MemoryUser("adarsh");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            Mapper.CreateMap<FollowRequestFormModel, FollowRequest>();


            var result = controller.FollowRequest("402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            Assert.AreEqual("UserProfile", result.RouteValues["action"]);
        }

        [Test]
        public void Accept_Request()
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                Email = "adarsh@foo.com",
                UserName = "t t",
                DateCreated = DateTime.Now,
                LastLoginTime = DateTime.Now,
                ProfilePicUrl = "",

            };
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(user);
            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            var result = contr.AcceptRequest("402bd590-fdc7-49ad-9728-40efbfe512ed","402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void Delete_Follow_Request()
        {
            FollowRequest request = new FollowRequest()
            {
                FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",

            };
            followRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(request);
            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
            var result = contr.RejectRequest("402bd590-fdc7-49ad-9728-40efbfe512ed","402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
        [Test]
        public void UnFollow()
        {
            MemoryUser user = new MemoryUser("adarsh");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            FollowUser flwuser = new FollowUser()
            {
                FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",

            };
            followUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(flwuser);

            var result = controller.Unfollow("402bd590-fdc7-49ad-9728-40efbfe512ed") as RedirectToRouteResult;
            Assert.AreEqual("UserProfile", result.RouteValues["action"]);
        }

        [Test]
        public void Followers_List()
        {
            MemoryUser user = new MemoryUser("adarsh");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec" },

            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);

            Mapper.CreateMap<ApplicationUser, FollowersViewModel>();


            ViewResult result = controller.Followers() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(IEnumerable<FollowersViewModel>), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as IEnumerable<FollowersViewModel>;
            Assert.AreEqual(2, data.Count());
        }


        [Test]
        public void Followings_list()
        {

            MemoryUser user = new MemoryUser("adarsh");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(new HttpCookieCollection());

            var formsAuthentication = new DefaultFormsAuthentication();



            formsAuthentication.SetAuthCookie(contextBase.Object, testTicket);

            HttpCookie authCookie = contextBase.Object.Response.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = formsAuthentication.Decrypt(authCookie.Value);
            var goalsetterUser = new SocialGoalUser(ticket);
            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee" },

            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);

            Mapper.CreateMap<ApplicationUser, FollowersViewModel>();


            ViewResult result = controller.Followings() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(IEnumerable<FollowingViewModel>), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as IEnumerable<FollowingViewModel>;
            Assert.AreEqual(2, data.Count());
        }

        //[Test()]
        //public void LoginTest()
        //{
        //    var userManager = new UserManager<ApplicationUser>(new TestUserStore());
        //    var controller = new AccountController(userManager);
        //    var mockAuthenticationManager = new Mock<IAuthenticationManager>();
        //    mockAuthenticationManager.Setup(am => am.SignOut());
        //    mockAuthenticationManager.Setup(am => am.SignIn());
        //    controller.AuthenticationManager = mockAuthenticationManager.Object;
        //    var result = controller.Login(new LoginViewModel { Email = "adarsh", Password = "123456", RememberMe = false }, "abcd").Result;
        //    Assert.IsNotNull(result);
        //    //var loggedInUser=userManager.
        //    //Assert.AreEqual("adarsh", addedUser.UserName);
        //}



        //[Test()]
        //public void RegisterTest()
        //{
        //    // Arrange
        //    var userManager = new UserManager<ApplicationUser>(new TestUserStore());
        //    //var mockAccountController = new Mock<AccountController>(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);
        //    var userprofileService = new Mock<UserProfileService>();


        //    //var controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService);


        //    var controller = new AccountController(userManager);

        //    //controller.
        //    var userProfile = new UserProfile()
        //    {
        //        UserName = "adarsh"
        //    };

        //    userProfileRepository.Setup(x => x.Add(userProfile));

        //    // Modify the test to setup a mock IAuthenticationManager
        //    var mockAuthenticationManager = new Mock<IAuthenticationManager>();
        //    mockAuthenticationManager.Setup(am => am.SignOut());
        //    mockAuthenticationManager.Setup(am => am.SignIn());

        //    // Add it to the controller - this is why you have to make a public setter
        //    controller.AuthenticationManager = mockAuthenticationManager.Object;

        //    // Act
        //    var result =
        //        controller.Register(new RegisterViewModel
        //        {
        //            UserName = "adarsh",
        //            Password = "123456",
        //            ConfirmPassword = "123456"
        //        }).Result;

        //    // Assert
        //    Assert.IsNotNull(result);

        //    var addedUser = userManager.FindByName("adarsh");
        //    Assert.IsNotNull(addedUser);
        //    Assert.AreEqual("adarsh", addedUser.UserName);


        //}
    }
}
