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
using System.IO;
using System.Web.SessionState;
using System.Reflection;
using SocialGoal.Tests.Helpers;


namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class EmailRequestControllerTest
    {
        Mock<ISecurityTokenRepository> securityTokenRepository;
        Mock<IGroupUserRepository> groupUserRepository;
        Mock<ISupportRepository> supportRepository;
        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<IUserRepository> userRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<UserProfileRepository> userProfileRepository;


        Mock<IUnitOfWork> unitOfWork;

        Mock<TempDataDictionary> tempData;
        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<IPrincipal> principal;
        Mock<HttpContextBase> contextBase;
        Mock<HttpRequestBase> httpRequest;
        Mock<HttpResponseBase> httpResponse;
        Mock<GenericPrincipal> genericPrincipal;

        public ISecurityTokenService securityTokenService;
        public IGroupUserService groupUserService;
        public ISupportService supportService;
        public IGroupInvitationService groupInvitationService;
        public IUserService userService;


        [SetUp]
        public void SetUp()
        {
            securityTokenRepository = new Mock<ISecurityTokenRepository>();
            supportRepository = new Mock<ISupportRepository>();
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            groupUserRepository = new Mock<IGroupUserRepository>();
            userRepository = new Mock<IUserRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            //userProfileRepository=new Mock<UserProfileRepository>();

            unitOfWork = new Mock<IUnitOfWork>();

            tempData = new Mock<TempDataDictionary>();
            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContextBase>();
            principal = new Mock<IPrincipal>();
            identity = new Mock<IIdentity>();
            httpRequest = new Mock<HttpRequestBase>();
            httpResponse = new Mock<HttpResponseBase>();
            genericPrincipal = new Mock<GenericPrincipal>();

            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);
            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            groupUserService = new GroupUserService(groupUserRepository.Object, userRepository.Object, unitOfWork.Object);
            supportService = new SupportService(supportRepository.Object, followUserRepository.Object, unitOfWork.Object);
            // userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }
        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Add_GroupUser()
        {
            Guid guidToken = Guid.NewGuid();
            SecurityToken token = new SecurityToken()
            {
                SecurityTokenId = 1,
                Token = guidToken,
                ActualID = 1
            };

            securityTokenRepository.Setup(x => x.Get(It.IsAny<Expression<Func<SecurityToken, bool>>>())).Returns(token);

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



            EmailRequestController controller = new EmailRequestController(securityTokenService, groupUserService, supportService, groupInvitationService);

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

            var httprequest = new HttpRequest("", "http://yoursite/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httprequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id",
                                    new SessionStateItemCollection(),
                                    new HttpStaticObjectsCollection(),
                                    10,
                                    true,
                                    HttpCookieMode.AutoDetect,
                                    SessionStateMode.InProc,
                                    false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, CallingConventions.Standard,
                        new[] { typeof(HttpSessionStateContainer) },
                        null)
                        .Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
            //HttpContext.Current.Request.Session["somevalue"];

            controller.TempData = tempData.Object;
            controller.TempData["grToken"] = guidToken;
            var result = controller.AddGroupUser() as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void Add_Support_ToGoal()
        {

            Guid guidToken = Guid.NewGuid();
            SecurityToken token = new SecurityToken()
            {
                SecurityTokenId = 1,
                Token = guidToken,
                ActualID = 1
            };

            securityTokenRepository.Setup(x => x.Get(It.IsAny<Expression<Func<SecurityToken, bool>>>())).Returns(token);

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



            EmailRequestController controller = new EmailRequestController(securityTokenService, groupUserService, supportService, groupInvitationService);

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

            var httprequest = new HttpRequest("", "http://yoursite/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httprequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id",
                                    new SessionStateItemCollection(),
                                    new HttpStaticObjectsCollection(),
                                    10,
                                    true,
                                    HttpCookieMode.AutoDetect,
                                    SessionStateMode.InProc,
                                    false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, CallingConventions.Standard,
                        new[] { typeof(HttpSessionStateContainer) },
                        null)
                        .Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;

            controller.TempData = tempData.Object;
            controller.TempData["goToken"] = guidToken;
            var result = controller.AddSupportToGoal() as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
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
