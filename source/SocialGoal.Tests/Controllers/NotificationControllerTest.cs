using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Core;
using SocialGoal.Web.ViewModels;
using SocialGoal.Web.Core.Extensions;
using SocialGoal.Web.Controllers;
using NUnit.Framework;
using AutoMapper;
using Moq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using SocialGoal.Web.Core.Authentication;
using System.Web.Security;
using SocialGoal.Web.Core.Models;
using SocialGoal.Tests.Helpers;


namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class NotificationControllerTest
    {

        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<ISupportInvitationRepository> supportInvitationRepository;
        Mock<IFollowRequestRepository> followRequestRepository;
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;

        Mock<IUnitOfWork> unitOfWork;

        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IGroupInvitationService groupInvitationService;
        ISupportInvitationService supportInvitationService;
        IFollowRequestService followRequestService;
        IUserService userService;

        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<IPrincipal> principal;
        Mock<HttpContextBase> contextBase;
        Mock<HttpRequestBase> httpRequest;
        Mock<HttpResponseBase> httpResponse;
        Mock<GenericPrincipal> genericPrincipal;

        [SetUp]
        public void SetUp()
        {
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            supportInvitationRepository = new Mock<ISupportInvitationRepository>();
            followRequestRepository = new Mock<IFollowRequestRepository>();
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();


            unitOfWork = new Mock<IUnitOfWork>();

            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContextBase>();
            principal = new Mock<IPrincipal>();
            identity = new Mock<IIdentity>();
            httpRequest = new Mock<HttpRequestBase>();
            httpResponse = new Mock<HttpResponseBase>();
            genericPrincipal = new Mock<GenericPrincipal>();

            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            supportInvitationService = new SupportInvitationService(supportInvitationRepository.Object, unitOfWork.Object);
            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }
        [Test]
        public void Index_If_AjaxRequest_IsNull()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
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

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService);


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
            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);
            Mapper.CreateMap<GroupInvitation, NotificationViewModel>();
            Mapper.CreateMap<SupportInvitation, NotificationViewModel>();
            Mapper.CreateMap<FollowRequest, NotificationViewModel>();
            ViewResult result = controller.Index(1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void Index_If_AjaxRequest_Is_NotNull()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
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

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService);


            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(s => s.HttpContext.Request["X-Requested-With"]).Returns("XMLHttpRequest");
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
            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);
            Mapper.CreateMap<GroupInvitation, NotificationViewModel>();
            Mapper.CreateMap<SupportInvitation, NotificationViewModel>();
            Mapper.CreateMap<FollowRequest, NotificationViewModel>();
            PartialViewResult result = controller.Index(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_NotificationList", result.ViewName);
        }

        [Test]
        public void Get_All_Notifications()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
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

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService);


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
            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);
            Mapper.CreateMap<GroupInvitation, NotificationViewModel>();
            Mapper.CreateMap<SupportInvitation, NotificationViewModel>();
            Mapper.CreateMap<FollowRequest, NotificationViewModel>();
            IEnumerable<NotificationViewModel> result = controller.GetNotifications(5, 5) as IEnumerable<NotificationViewModel>;
            Assert.IsNotNull(result);
        }

        public ApplicationUser getApplicationUser()
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                DateCreated = DateTime.Now,
                LastLoginTime = DateTime.Now,
                ProfilePicUrl = null,
            };
            return applicationUser;
        }


    }
}
