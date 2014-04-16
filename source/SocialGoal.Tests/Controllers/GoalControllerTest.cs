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
using Mvc.Mailer;
using System.Security;
using System.Security.Principal;
using System.Threading;
using SocialGoal.Web.Core.Models;
using System.Web;
using System.Linq.Expressions;
using SocialGoal.Web.Mailers;
using System.Net.Mail;
using System.Web.Security;
using SocialGoal.Web.Core.Authentication;
using SocialGoal.Tests.Helpers;


namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class GoalControllerTest
    {
        Mock<IGoalRepository> goalRepository;
        Mock<IFollowUserRepository> followuserRepository;
        Mock<ISupportRepository> supportRepository;
        Mock<IGoalStatusRepository> goalStatusRepository;
        Mock<IFocusRepository> focusRepository;
        Mock<IMetricRepository> metricRepository;
        Mock<IUpdateRepository> updateRepository;
        Mock<IUserRepository> userRepository;
        Mock<ISupportRepository> supportrepository;
        Mock<ISupportInvitationRepository> supportInvitationrepository;
        Mock<ICommentRepository> commentRepository;
        Mock<ICommentUserRepository> commentUserRepository;
        Mock<ISecurityTokenRepository> securityTokenrepository;
        Mock<IUserProfileRepository> userProfileRepository;
        Mock<IUpdateSupportRepository> updateSupportRepository;


        IGoalService goalService;
        IFollowUserService followUserService;
        IMetricService metricService;
        IFocusService focusService;
        IUpdateService updateService;
        ISupportService supportService;
        ICommentService commentService;
        ISecurityTokenService securityTokenService;
        IGoalStatusService goalStatusService;
        IUserService userService;
        ICommentUserService commentUserService;
        ISupportInvitationService supportInvitationService;
        IUserProfileService userProfileService;
        IUpdateSupportService updateSupportService;

        Mock<IUnitOfWork> unitOfWork;
        Mock<IUserMailer> userMailer;
        Mock<UserMailer> userMailerMock;
        Mock<MailerBase> mailerBase;

        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<IPrincipal> principal;
        // Mock<HttpContext> httpContext;
        Mock<HttpContextBase> contextBase;
        Mock<HttpRequestBase> httpRequest;
        Mock<HttpResponseBase> httpResponse;
        Mock<GenericPrincipal> genericPrincipal;

        //Mock<>
        [SetUp]
        public void SetUp()
        {
            goalRepository = new Mock<IGoalRepository>();
            followuserRepository = new Mock<IFollowUserRepository>();
            supportRepository = new Mock<ISupportRepository>();
            goalStatusRepository = new Mock<IGoalStatusRepository>();
            focusRepository = new Mock<IFocusRepository>();
            metricRepository = new Mock<IMetricRepository>();
            updateRepository = new Mock<IUpdateRepository>();
            userRepository = new Mock<IUserRepository>();
            supportrepository = new Mock<ISupportRepository>();
            supportInvitationrepository = new Mock<ISupportInvitationRepository>();
            commentRepository = new Mock<ICommentRepository>();
            commentUserRepository = new Mock<ICommentUserRepository>();
            securityTokenrepository = new Mock<ISecurityTokenRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            updateSupportRepository = new Mock<IUpdateSupportRepository>();

            userMailer = new Mock<IUserMailer>();
            userMailerMock = new Mock<UserMailer>();
            mailerBase = new Mock<MailerBase>();
            unitOfWork = new Mock<IUnitOfWork>();

            goalService = new GoalService(goalRepository.Object, followuserRepository.Object, unitOfWork.Object);
            supportService = new SupportService(supportRepository.Object, followuserRepository.Object, unitOfWork.Object);
            goalStatusService = new GoalStatusService(goalStatusRepository.Object, unitOfWork.Object);
            focusService = new FocusService(focusRepository.Object, unitOfWork.Object);
            metricService = new MetricService(metricRepository.Object, unitOfWork.Object);
            updateService = new UpdateService(updateRepository.Object, goalRepository.Object, unitOfWork.Object, followuserRepository.Object);
            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
            supportService = new SupportService(supportrepository.Object, followuserRepository.Object, unitOfWork.Object);
            supportInvitationService = new SupportInvitationService(supportInvitationrepository.Object, unitOfWork.Object);
            commentService = new CommentService(commentRepository.Object, commentUserRepository.Object, unitOfWork.Object, followuserRepository.Object);
            commentUserService = new CommentUserService(commentUserRepository.Object, userRepository.Object, unitOfWork.Object);
            securityTokenService = new SecurityTokenService(securityTokenrepository.Object, unitOfWork.Object);
            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);
            updateSupportService = new UpdateSupportService(updateSupportRepository.Object, unitOfWork.Object);

            MailerBase.IsTestModeEnabled = true;
            userMailerMock.CallBase = true;

            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContextBase>();
            httpRequest = new Mock<HttpRequestBase>();
            httpResponse = new Mock<HttpResponseBase>();
            genericPrincipal = new Mock<GenericPrincipal>();


            identity = new Mock<IIdentity>();
            principal = new Mock<IPrincipal>();
        }
        [TearDown]
        public void TearDown()
        {
            TestSmtpClient.SentMails.Clear();
        }



        [Test]
        public void Index()
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

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());


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
            Goal goal = new Goal()
            {
                GoalId = 1,
                GoalName = "t",
                GoalStatusId = 1,
                Desc = "x",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)

            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);
            IEnumerable<GoalStatus> fake = new List<GoalStatus> {
            new GoalStatus { GoalStatusId =1, GoalStatusType ="Inprogress"},
            new GoalStatus { GoalStatusId =2, GoalStatusType ="OnHold"},
         
          }.AsEnumerable();
            goalStatusRepository.Setup(x => x.GetAll()).Returns(fake);

            Mapper.CreateMap<Goal, GoalViewModel>();

            ViewResult result = controller.Index(1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(GoalViewModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as GoalViewModel;
            Assert.AreEqual("t", data.GoalName);
        }

        [Test]
        public void MyGoal_Test()
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

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
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
            IEnumerable<Goal> fake = new List<Goal> 
            {
            new Goal { GoalId=1,GoalName="Test1"},
            new Goal { GoalId=2,GoalName="Test2"},
            new Goal { GoalId=3,GoalName="Test3"},
            new Goal { GoalId=4,GoalName="Test4"},           
         
          }.AsEnumerable();
            goalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Goal, bool>>>())).Returns(fake);


            contextBase.Setup(x => x.User.Identity).Returns(identity.Object);
            Mapper.CreateMap<Goal, GoalViewModel>();
            ViewResult result = controller.MyGoal() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(IEnumerable<GoalViewModel>), result.ViewData.Model, "Wrong model");
            var data = result.ViewData.Model as IEnumerable<GoalViewModel>;
            Assert.AreEqual(4, data.Count(), "not matching");
        }


        [Test]
        public void Create_Goal_Get_ReturnsView()
        {

            IEnumerable<Focus> fakeFocus = new List<Focus> 
            {
            new Focus { FocusId = 1, FocusName="Test1",GroupId = 1},
             new Focus { FocusId = 2, FocusName="Test2",GroupId = 1},
            new Focus { FocusId = 3, FocusName="Test3",GroupId = 1}
          }.AsEnumerable();
            focusRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Focus, bool>>>())).Returns(fakeFocus);

            IEnumerable<Metric> fakeMatrices = new List<Metric> 
            {
                new Metric{MetricId=1, Type="Test1"},
                new Metric{MetricId=2,Type="Test2"},
                new Metric{MetricId=3,Type="Test3"}
            }.AsEnumerable();

            metricRepository.Setup(x => x.GetAll()).Returns(fakeMatrices);
            GoalFormModel goal = new GoalFormModel();
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            PartialViewResult result = controller.Create() as PartialViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GoalFormModel),
                result.ViewData.Model, "Wrong View Model");


        }
        [Test]
        public void Create_Goal()
        {
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

            // Act
            Mapper.CreateMap<GoalFormModel, Goal>();


            GoalFormModel goal = new GoalFormModel()
            {
                GoalName = "t",

                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Desc = "t",

                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",


            };
            var result = (RedirectToRouteResult)controller.Create(goal);
            Assert.AreEqual("Index", result.RouteValues["action"]);


        }
        [Test]
        public void Edit_Goal_Get_View()
        {

            Goal goal = new Goal()
            {
                GoalId = 1,
                GoalName = "t",
                Desc = "t",
                GoalStatusId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);
            IEnumerable<Focus> fakeFocus = new List<Focus> 
            {
            new Focus { FocusId = 1, FocusName="Test1",GroupId = 1},
             new Focus { FocusId = 2, FocusName="Test2",GroupId = 1},
            new Focus { FocusId = 3, FocusName="Test3",GroupId = 1}
          }.AsEnumerable();
            focusRepository.Setup(x => x.GetMany(p => p.GroupId.Equals(1))).Returns(fakeFocus);

            IEnumerable<Metric> fakeMatrices = new List<Metric> 
            {
                new Metric{MetricId=1, Type="Test1"},
                new Metric{MetricId=2,Type="Test2"},
                new Metric{MetricId=3,Type="Test3"}
            }.AsEnumerable();

            metricRepository.Setup(x => x.GetAll()).Returns(fakeMatrices);

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            Mapper.CreateMap<Goal, GoalFormModel>();
            ViewResult result = controller.Edit(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GoalFormModel),
                result.ViewData.Model, "Wrong View Model");
            var data = result.ViewData.Model as GoalFormModel;
            Assert.AreEqual("t", data.Desc);
        }

        [Test]
        public void Edit_Goal_Post()
        {

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

            Mapper.CreateMap<GoalFormModel, Goal>();


            GoalFormModel goal = new GoalFormModel()
            {
                GoalName = "t",
                GoalId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Desc = "t",
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"

            };
            var result = (RedirectToRouteResult)controller.Edit(goal);
            Assert.AreEqual("Index", result.RouteValues["action"]);



        }

        [Test]
        public void Goal_Status_Post_Test()
        {
            GoalStatus status = new GoalStatus()
            {
                GoalStatusId = 1,
                GoalStatusType = "InProgress"
            };
            Goal goal = new Goal()
            {
                GoalId = 1,
                GoalStatus = status,
                GoalStatusId = 1
            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            string result = controller.GoalStatus(1, 1) as string;
            Assert.AreEqual("InProgress", result);
        }

        [Test]
        public void Delete_Goal_Get_ReturnsView()
        {


            Goal fake = new Goal()
            {
                GoalId = 1,
                GoalName = "test",
                Desc = "test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                GoalStatusId = 1,


            };


            goalRepository.Setup(x => x.GetById(1)).Returns(fake);

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            ViewResult result = controller.Delete(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(Goal),
                 result.ViewData.Model, "Wrong View Model");
            var group = result.ViewData.Model as Goal;
            Assert.AreEqual("test", group.Desc, "Got wrong Focus Description");

        }

        [Test]
        public void Delete_Goal_Post()
        {

            Goal goal = new Goal()
            {
                GoalId = 1,
                GoalName = "t",
                Desc = "t"

            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            var result = controller.DeleteConfirmed(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void MyGoals_test()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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
            IEnumerable<Goal> fake = new List<Goal> 
            {
            new Goal { GoalId=1,GoalName="Test1",UserId ="402bd590-fdc7-49ad-9728-40efbfe512ec",},
            new Goal { GoalId=2,GoalName="Test2",UserId ="402bd590-fdc7-49ad-9728-40efbfe512ec",},
            new Goal { GoalId=3,GoalName="Test3",UserId ="402bd590-fdc7-49ad-9728-40efbfe512ec",},
            new Goal { GoalId=4,GoalName="Test4",UserId ="402bd590-fdc7-49ad-9728-40efbfe512ec",}          
         
          }.AsEnumerable();
            goalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Goal, bool>>>())).Returns(fake);

            PartialViewResult result = controller.MyGoals() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(IEnumerable<Goal>),
                result.ViewData.Model, "Wrong View Model");


        }
        [Test]
        public void Goals_Following_Test()
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




            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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


            PartialViewResult result = controller.GoalsFollowing() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(IEnumerable<Goal>), result.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Display_Updates_Test()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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
            Metric fakeMetric = new Metric()
            {
                MetricId = 1,
                Type = "%"
            };
            Goal goal = new Goal()
            {
                Metric = fakeMetric,
                Target = 100
            };
            goalRepository.Setup(x => x.GetById(2)).Returns(goal);


            IEnumerable<Update> updt = new List<Update> {            
            new Update { UpdateId =1, Updatemsg = "t1", GoalId = 1},
             new Update { UpdateId =2, Updatemsg = "t2", GoalId=2 },
              new Update { UpdateId =3, Updatemsg = "t3",GoalId=2},
            
          }.AsEnumerable();
            updateRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Update, bool>>>())).Returns(updt);
            Mapper.CreateMap<Update, UpdateViewModel>();
            //GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            PartialViewResult rslt = controller.DisplayUpdates(2) as PartialViewResult;
            Assert.IsNotNull(rslt);
            Assert.IsInstanceOf(typeof(UpdateListViewModel),
           rslt.ViewData.Model, "Wrong View Model");


        }
        [Test]
        public void Supporters_List_Test()
        {
            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetAll()).Returns(fakeUser);
            IEnumerable<Support> fake = new List<Support> {            
            new Support { SupportId =1, GoalId = 1, UserId ="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new Support { SupportId =2, GoalId = 1, UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new Support { SupportId =3, GoalId = 1, UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new Support { SupportId =4, GoalId = 1, UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
          
          }.AsEnumerable();
            supportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Support, bool>>>())).Returns(fake);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            ViewResult result = controller.Supporters(1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(GoalSupporterViewModel), result.ViewData.Model, "Wrong View Model");

        }
        [Test]
        public void Save_Update_Post()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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
            //GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            Mapper.CreateMap<UpdateFormModel, Update>();
            Mapper.CreateMap<Update, UpdateViewModel>();

            Metric fakeMetric = new Metric()
            {
                MetricId = 1,
                Type = "%"
            };
            Goal goal = new Goal()
            {
                Metric = fakeMetric,
                Target = 100,
                GoalId = 1
            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);


            IEnumerable<Update> updt = new List<Update> {            
            new Update { UpdateId =1, Updatemsg = "t1",GoalId =1,status=5},
             new Update { UpdateId =2, Updatemsg = "t2",GoalId =1,status=6},
              new Update { UpdateId =3, Updatemsg = "t3",GoalId =2,status=2},
            
          }.AsEnumerable();

            updateRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Update, bool>>>())).Returns(updt);
            UpdateFormModel mock = new UpdateFormModel();
            mock.Updatemsg = "mock";
            mock.GoalId = 1;
            mock.status=9;
            PartialViewResult result = controller.SaveUpdate(mock) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(UpdateListViewModel), result.ViewData.Model, "Wrong View Model");
        }
        [Test]
        public void Save_Update_Update_Mandatory_Test()
        {
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

            // The MVC pipeline doesn't run, so binding and validation don't run. 
            controller.ModelState.AddModelError("", "mock error message");
            UpdateFormModel update = new UpdateFormModel();
            update.Updatemsg = string.Empty;
            var result = controller.SaveUpdate(update) as RedirectToRouteResult;

            Assert.IsNull(result);
        }

        [Test]
        public void Invite_User()
        {

            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();

            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = user.UserName,
                RoleName = Enum.GetName(typeof(UserRoles), 0)
            };
            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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



            var userId = "402bd590-fdc7-49ad-9728-40efbfe512ec";
            var id = 1;
            PartialViewResult rslt = controller.InviteUser(id, userId) as PartialViewResult;
            Assert.IsNotNull(rslt);
            Assert.IsInstanceOf(typeof(ApplicationUser),
              rslt.ViewData.Model, "Wrong View Model");
            var userView = rslt.ViewData.Model as ApplicationUser;
            Assert.AreEqual("adarsh@foo.com", userView.Email);

        }

        [Test]
        public void DisplayComments()
        {
            IEnumerable<Comment> cmnt = new List<Comment> {            

            new Comment { CommentId =1, UpdateId = 1,CommentText="x"},
            new Comment { CommentId =2, UpdateId = 1,CommentText="y"},
            new Comment { CommentId =3, UpdateId = 1,CommentText="z"},
             
            
          }.AsEnumerable();

            commentRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Comment, bool>>>())).Returns(cmnt);
            CommentUser cmtuser = new CommentUser()
            {
                CommentId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                CommentUserId = 1
            };
            commentUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<CommentUser, bool>>>())).Returns(cmtuser);
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.GetById("402bd590-fdc7-49ad-9728-40efbfe512ec")).Returns(applicationUser);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

            Mapper.CreateMap<Comment, CommentsViewModel>();
            PartialViewResult rslt = controller.DisplayComments(1) as PartialViewResult;
            Assert.IsNotNull(rslt, "View Result is null");
            Assert.IsInstanceOf(typeof(IEnumerable<CommentsViewModel>),
             rslt.ViewData.Model, "Wrong View Model");
            var cmntsView = rslt.ViewData.Model as IEnumerable<CommentsViewModel>;
            Assert.AreEqual(3, cmntsView.Count(), "Got wrong number of Comments");
        }
        [Test]
        public void SaveComment()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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

            // Act          
            CommentFormModel Cmnt = new CommentFormModel();
            Mapper.CreateMap<CommentFormModel, Comment>();
            Cmnt.CommentText = "Mock";
            var result = controller.SaveComment(Cmnt) as RedirectToRouteResult;
            // Assert 
            Assert.AreEqual("DisplayComments", result.RouteValues["action"]);
        }

        [Test]
        public void Display_Comment_Count_Test()
        {
            IEnumerable<Comment> cmnt = new List<Comment> {            

            new Comment { CommentId =1, UpdateId = 1,CommentText="x"},
            new Comment { CommentId =2, UpdateId = 1,CommentText="y"},
            new Comment { CommentId =3, UpdateId = 1,CommentText="z"},
             
            
          }.AsEnumerable();

            commentRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Comment, bool>>>())).Returns(cmnt);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            JsonResult count = controller.DisplayCommentCount(1) as JsonResult;
            Assert.IsNotNull(count);
            Assert.AreEqual(3, count.Data);

        }

        [Test]
        public void SearchGoalSearch_User_Which_Support_The_Goal()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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
            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
          new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fakeUser);


            var searchString = "e";
            JsonResult result = controller.SearchUser(searchString, 1);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(JsonResult), result);

        }

        [Test]
        public void Create_Support_Test()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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

            //Act
            var rslt = controller.SupportGoalNow(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", rslt.RouteValues["action"]);
        }
        [Test]
        public void Support_Invite_View_Returns_Test()
        {
            Goal goal = new Goal()
            {
                GoalStatusId = 1,
                GoalId = 1
            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            PartialViewResult result = controller.SupportInvitation(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Goal),
           result.ViewData.Model, "Wrong View Model");

        }

        [Test]
        public void Invite_Email_Post()
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

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
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



            var mailMessage = new MvcMailMessage();
            Guid goalIdToken = Guid.NewGuid();


            string email = "a@gmail.com";

            // userMailerMock.Setup(mailer => mailer.PopulateBody(It.IsAny<MvcMailMessage>(), "SupportGoal", null));
            mailerBase.Setup(x => x.PopulateBody(It.IsAny<MailMessage>(), "SupportGoal", null));

            userMailer.Setup(x => x.Support(email, goalIdToken)).Returns(mailMessage);



            InviteEmailFormModel inviteEmail = new InviteEmailFormModel();
            inviteEmail.Email = "a@gmail.com";
            inviteEmail.GrouporGoalId = 1;
            // string result = controller.InviteEmail(inviteEmail) as string;
            //Assert.AreEqual("")
        }


        [Test]
        public void Get_GoalReport_Test()
        {
            Goal goal = new Goal()
            {
                GoalStatusId = 1,
                GoalId = 1
            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            JsonResult reslt = controller.GetGoalReport(1) as JsonResult;
            Assert.IsNotNull(reslt);

        }


        [Test]
        public void ListOfGoals()
        {
            // Arrange      
            IEnumerable<Goal> fake = new List<Goal> {
            new Goal { GoalName = "Test1", Desc="Test1Desc"},
            new Goal { GoalName = "Test2", Desc="Test2Desc"},
          
          }.AsEnumerable();
            goalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Goal, bool>>>())).Returns(fake);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

            // Act
            ViewResult result = controller.ListOfGoals() as ViewResult;
            // Assert
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(IEnumerable<Goal>), result.ViewData.Model, "Wrong View Model");
            var gol = result.ViewData.Model as IEnumerable<Goal>;
            Assert.AreEqual(2, gol.Count(), "Got wrong number of Goals");
        }

        [Test]
        public void Goals_List()
        {
            IEnumerable<Goal> fake = new List<Goal> {
            new Goal { GoalName = "Test1", Desc="Test1Desc"},
            new Goal { GoalName = "Test2", Desc="Test2Desc"},
          
          }.AsEnumerable();
            goalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Goal, bool>>>())).Returns(fake);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            PartialViewResult result = controller.Goalslist(0, 0) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(IEnumerable<Goal>), result.ViewData.Model, "Wrong View Model");
            var gol = result.ViewData.Model as IEnumerable<Goal>;
            Assert.AreEqual(2, gol.Count(), "Got wrong number of Goals");
        }

        [Test]
        public void Edit_Update_Get_View()
        {

            Update update = new Update()
            {
                UpdateId = 1,
                Updatemsg = "abc",
                GoalId = 1,

            };
            updateRepository.Setup(x => x.GetById(1)).Returns(update);

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            Mapper.CreateMap<Update, UpdateFormModel>();
            PartialViewResult result = controller.EditUpdate(1) as PartialViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(UpdateFormModel),
                result.ViewData.Model, "Wrong View Model");
            var data = result.ViewData.Model as UpdateFormModel;
            Assert.AreEqual("abc", data.Updatemsg);
        }


        [Test]
        public void Edit_Update_Post()
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



            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);

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
            //GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            Mapper.CreateMap<UpdateFormModel, Update>();
            Mapper.CreateMap<Update, UpdateViewModel>();

            Metric fakeMetric = new Metric()
            {
                MetricId = 1,
                Type = "%"
            };
            Goal goal = new Goal()
            {
                Metric = fakeMetric,
                Target = 100,
                GoalId = 1
            };
            goalRepository.Setup(x => x.GetById(1)).Returns(goal);


            IEnumerable<Update> updt = new List<Update> {            
            new Update { UpdateId =1, Updatemsg = "t1",GoalId =1},
             new Update { UpdateId =2, Updatemsg = "t2",GoalId =1},
              new Update { UpdateId =3, Updatemsg = "t3",GoalId =2},
            
          }.AsEnumerable();
            updateRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Update, bool>>>())).Returns(updt);
            UpdateFormModel mock = new UpdateFormModel();
            mock.Updatemsg = "mock";
            mock.GoalId = 1;
            mock.status = 34;
            PartialViewResult result = controller.EditUpdate(mock) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(UpdateListViewModel),
            result.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Delete_Update_Get_ReturnsView()
        {

            Update update = new Update()
            {
                UpdateId = 1,
                Updatemsg = "abc",
                GoalId = 1,

            };
            updateRepository.Setup(x => x.GetById(1)).Returns(update);

            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            PartialViewResult result = controller.DeleteUpdate(1) as PartialViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(Update),
                 result.ViewData.Model, "Wrong View Model");
            var group = result.ViewData.Model as Update;
            Assert.AreEqual("abc", group.Updatemsg, "Got wrong message");

        }

        [Test]
        public void Delete_Update_Post()
        {

            Update update = new Update()
            {
                UpdateId = 1,
                Updatemsg = "abc",
                GoalId = 1,

            };
            updateRepository.Setup(x => x.GetById(1)).Returns(update);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            var result = controller.DeleteConfirmedUpdate(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void Update_Supporters_Count()
        {
            IEnumerable<UpdateSupport> updtsprt = new List<UpdateSupport> {            

            new UpdateSupport { UpdateSupportId =1, UpdateId = 1,UserId =  "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new UpdateSupport { UpdateSupportId =1, UpdateId = 1,UserId =  "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new UpdateSupport { UpdateSupportId =1, UpdateId = 1,UserId =  "402bd590-fdc7-49ad-9728-40efbfe512ec"},
          
             
            
          }.AsEnumerable();

            updateSupportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<UpdateSupport, bool>>>())).Returns(updtsprt);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            int count = controller.NoOfSupports(1);
            Assert.IsNotNull(count);
            Assert.AreEqual("3", count.ToString());

        }

        [Test]
        public void Display_Update_Supporters_Count()
        {
            IEnumerable<UpdateSupport> updtsprt = new List<UpdateSupport> {            

            new UpdateSupport { UpdateSupportId =1, UpdateId = 1,UserId =  "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new UpdateSupport { UpdateSupportId =1, UpdateId = 1,UserId =  "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new UpdateSupport { UpdateSupportId =1, UpdateId = 1,UserId =  "402bd590-fdc7-49ad-9728-40efbfe512ef"},
          
             
            
          }.AsEnumerable();

            updateSupportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<UpdateSupport, bool>>>())).Returns(updtsprt);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            JsonResult count = controller.DisplayUpdateSupportCount(1) as JsonResult;
            Assert.IsNotNull(count);
            Assert.AreEqual(3, count.Data);

        }

        [Test]
        public void Supporters_Of_Updates_List()
        {
            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
            new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetAll()).Returns(fakeUser);
            IEnumerable<UpdateSupport> fake = new List<UpdateSupport> {            
            new UpdateSupport { UpdateSupportId =1, UpdateId = 1, UserId ="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new UpdateSupport { UpdateSupportId =2, UpdateId = 1, UserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new UpdateSupport { UpdateSupportId =3, UpdateId = 1, UserId ="402bd590-fdc7-49ad-9728-40efbfe512ef"},
          
          }.AsEnumerable();
            updateSupportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<UpdateSupport, bool>>>())).Returns(fake);
            GoalController controller = new GoalController(goalService, metricService, focusService, supportService, updateService, commentService, userService, securityTokenService, supportInvitationService, goalStatusService, commentUserService, updateSupportService);
            PartialViewResult result = controller.SupportersOfUpdate(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(UpdateSupportersViewModel), result.ViewData.Model, "Wrong View Model");

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
