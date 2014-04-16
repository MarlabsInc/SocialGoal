
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
using System.Security;
using System.Security.Principal;
using System.Threading;
using SocialGoal.Web.Core.Models;
using System.Web;
using System.Linq.Expressions;
using SocialGoal.Web.Core.Authentication;
using System.Web.Security;
using SocialGoal.Tests.Helpers;

namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class GroupControllerTest
    {
        //Mock<IIdentity> iIdentity;
        Mock<IGroupRepository> groupRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<IUserRepository> userRepository;
        Mock<IGroupUserRepository> groupUserRepository;
        Mock<IFocusRepository> focusRepository;
        Mock<IMetricRepository> metricRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<IGroupGoalRepository> groupGoalRepository;
        Mock<ICommentRepository> commentRepository;
        Mock<IGroupUpdateRepository> groupUdateRepository;
        Mock<IGroupUpdateUserRepository> updateUserRepository;
        Mock<IGroupCommentRepository> groupCommentRepository;
        Mock<IGroupCommentUserRepository> groupCommentUserRepository;
        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<IGroupRequestRepository> groupRequestRepository;
        Mock<IGoalStatusRepository> goalStatusRepository;
        Mock<IUserProfileRepository> userProfileRepository;
        Mock<IGroupUpdateSupportRepository> groupUpdateSupportRepository;
        Mock<IGroupUpdateUserRepository> groupUpdateUserRepository;
        //Mock<CreateGroupFormModel> group;
        //Mock<FocusFormModel> focus;
        //Mock<GroupViewModel> groupView;


        IGroupService groupService;
        IGroupInvitationService groupInvitationService;
        IUserService userService;
        IGroupUserService groupUserService;
        IMetricService metricService;
        IFocusService focusService;
        IGroupGoalService groupgoalService;
        ISecurityTokenService securityTokenService;
        IGroupUpdateService groupUpdateService;
        IGroupCommentService groupCommentService;
        IGoalStatusService goalStatusService;
        IGroupRequestService groupRequestService;
        IFollowUserService followUserService;
        IGroupCommentUserService groupCommentUserService;
        IGroupUpdateUserService updateUserService;
        IUserProfileService userProfileService;
        IGroupUpdateSupportService groupUpdateSupportService;
        IGroupUpdateUserService groupUpdateUserService;


        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<IPrincipal> principal;
        Mock<HttpContext> httpContext;
        Mock<HttpContextBase> contextBase;
        Mock<HttpRequestBase> httpRequest;
        Mock<HttpResponseBase> httpResponse;
        Mock<GenericPrincipal> genericPrincipal;


        [SetUp]
        public void SetUp()
        {
            groupRepository = new Mock<IGroupRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            groupUserRepository = new Mock<IGroupUserRepository>();
            focusRepository = new Mock<IFocusRepository>();
            commentRepository = new Mock<ICommentRepository>();
            groupGoalRepository = new Mock<IGroupGoalRepository>();
            metricRepository = new Mock<IMetricRepository>();
            userRepository = new Mock<IUserRepository>();
            groupUdateRepository = new Mock<IGroupUpdateRepository>();
            updateUserRepository = new Mock<IGroupUpdateUserRepository>();
            groupCommentRepository = new Mock<IGroupCommentRepository>();
            groupCommentUserRepository = new Mock<IGroupCommentUserRepository>();
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            groupRequestRepository = new Mock<IGroupRequestRepository>();
            goalStatusRepository = new Mock<IGoalStatusRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            groupUpdateSupportRepository = new Mock<IGroupUpdateSupportRepository>();
            groupUpdateUserRepository = new Mock<IGroupUpdateUserRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContextBase>();
            // httpContext = new Mock<HttpContext>();
            httpRequest = new Mock<HttpRequestBase>();
            httpResponse = new Mock<HttpResponseBase>();
            genericPrincipal = new Mock<GenericPrincipal>();


            identity = new Mock<IIdentity>();
            principal = new Mock<IPrincipal>();



            groupService = new GroupService(groupRepository.Object, followUserRepository.Object, groupUserRepository.Object, unitOfWork.Object);
            focusService = new FocusService(focusRepository.Object, unitOfWork.Object);
            metricService = new MetricService(metricRepository.Object, unitOfWork.Object);
            groupgoalService = new GroupGoalService(groupGoalRepository.Object, unitOfWork.Object);
            groupUserService = new GroupUserService(groupUserRepository.Object, userRepository.Object, unitOfWork.Object);
            groupUpdateService = new GroupUpdateService(groupUdateRepository.Object, updateUserRepository.Object, groupGoalRepository.Object, unitOfWork.Object);
            groupCommentService = new GroupCommentService(groupCommentRepository.Object, groupCommentUserRepository.Object, groupUdateRepository.Object, unitOfWork.Object);
            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            groupRequestService = new GroupRequestService(groupRequestRepository.Object, unitOfWork.Object);
            groupCommentUserService = new GroupCommentUserService(groupCommentUserRepository.Object, unitOfWork.Object, userRepository.Object);
            goalStatusService = new GoalStatusService(goalStatusRepository.Object, unitOfWork.Object);
            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);
            groupUpdateSupportService = new GroupUpdateSupportService(groupUpdateSupportRepository.Object, unitOfWork.Object);
            groupUpdateUserService = new GroupUpdateUserService(groupUpdateUserRepository.Object, unitOfWork.Object, userRepository.Object);
        }
        [TearDown]
        public void TearDown()
        {
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

            var testTicket = new FormsAuthenticationTicket(
                1,
                user.Id,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userContext.ToString());

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);


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


            GroupUser grpUser = new GroupUser()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                Admin = true
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpUser);

            Group group = new Group() { GroupId = 1, GroupName = "Test", Description = "test" };
            groupRepository.Setup(x => x.GetById(1)).Returns(group);

            IEnumerable<GroupGoal> fakegoal = new List<GroupGoal> {
            new GroupGoal{ GroupId = 1, Description="Test1Desc",GroupGoalId =1,GroupUser = grpUser},
             new GroupGoal{ GroupId = 1, Description="Test1Desc",GroupGoalId =1,GroupUser = grpUser},
          
          
          }.AsEnumerable();
            groupGoalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupGoal, bool>>>())).Returns(fakegoal);

            IEnumerable<Focus> fakeFocus = new List<Focus> 
            {
            new Focus { FocusId = 1, FocusName="Test1",GroupId = 1},
             new Focus { FocusId = 2, FocusName="Test2",GroupId = 1},
            new Focus { FocusId = 3, FocusName="Test3",GroupId = 2}
          }.AsEnumerable();
            focusRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Focus, bool>>>())).Returns(fakeFocus);

            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetAll()).Returns(fakeUser);



            Mapper.CreateMap<Group, GroupViewModel>();
            Mapper.CreateMap<GroupGoal, GroupGoalViewModel>();

            ViewResult result = controller.Index(1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
            Assert.IsInstanceOfType(typeof(GroupViewModel),
                result.ViewData.Model, "WrongType");
        }


        [Test]
        public void Group_Goal_Page_View()
        {
            //Arrange 
            GroupUser grpUser = new GroupUser()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            GroupGoal goal = new GroupGoal()
            {
                GroupGoalId = 1,
                GoalName = "t",
                GoalStatusId = 1,
                Description = "x",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                GroupUser = grpUser

            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);

            ApplicationUser user = new ApplicationUser()
            {
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(user);
            IEnumerable<GoalStatus> fake = new List<GoalStatus> {
            new GoalStatus { GoalStatusId =1, GoalStatusType ="Inprogress"},
            new GoalStatus { GoalStatusId =2, GoalStatusType ="OnHold"},
         
          }.AsEnumerable();
            goalStatusRepository.Setup(x => x.GetAll()).Returns(fake);

            Mapper.CreateMap<GroupGoal, GroupGoalViewModel>();
            //Act
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            ViewResult result = controller.GroupGoal(1) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(GroupGoalViewModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as GroupGoalViewModel;
            Assert.AreEqual("t", data.GoalName);
        }

        [Test]
        public void Create_Group_Get_ReturnsView()
        {
            //Arrange
            GroupFormModel group = new GroupFormModel();
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            //Act
            PartialViewResult result = controller.CreateGroup() as PartialViewResult;
            //Assert
            Assert.IsNotNull(result);

        }

        [Test]
        public void Group_name_Required()
        {
            // Arrange 
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


            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);


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


            principal.Setup(x => x.Identity).Returns(goalsetterUser);



            // The MVC pipeline doesn't run, so binding and validation don't run. 
            controller.ModelState.AddModelError("", "mock error message");

            // Act          
            Mapper.CreateMap<GroupFormModel, Group>();
            GroupFormModel group = new GroupFormModel()
            {
                GroupName = string.Empty,
                Description = "Test"

            };


            var result = controller.CreateGroup(group) as ViewResult;

            // Assert - check that we are passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.AreEqual("CreateGroup", result.ViewName);

        }

        [Test]
        public void Group_Description_Required()
        {
            //Arrange

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



            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

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


            principal.Setup(x => x.Identity).Returns(goalsetterUser);


            // The MVC pipeline doesn't run, so binding and validation don't run. 
            controller.ModelState.AddModelError("", "mock error message");


            // Act          

            Mapper.CreateMap<GroupFormModel, Group>();
            GroupFormModel group = new GroupFormModel();
            group.Description = string.Empty;
            var result = (ViewResult)controller.CreateGroup(group);
            // Assert - check that we are passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.AreEqual("CreateGroup", result.ViewName);

        }

        [Test]
        public void Create_Group()
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



            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
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
            Mapper.CreateMap<GroupFormModel, Group>();
            GroupFormModel group = new GroupFormModel()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                CreatedDate = DateTime.Now,
                Description = "Mock",
                GroupName = "Mock",
                GroupId = 1,

            };
            var result = (RedirectToRouteResult)controller.CreateGroup(group);

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);



        }

        [Test]
        public void Focus_Name_Mandatory()
        {
            // Arrange                    
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            // The MVC pipeline doesn't run, so binding and validation don't run. 
            controller.ModelState.AddModelError("", "mock error message");
            // Act          

            Mapper.CreateMap<FocusFormModel, Focus>();
            FocusFormModel focus = new FocusFormModel();
            focus.FocusName = string.Empty;
            var result = (ViewResult)controller.CreateFocus(focus);

            // Assert - check that we are passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.AreEqual("CreateFocus", result.ViewName);
        }

        [Test]
        public void Focus_Description_Mandatory()
        {
            // Arrange                    
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            // The MVC pipeline doesn't run, so binding and validation don't run. 
            controller.ModelState.AddModelError("", "mock error message");

            // Act          
            Mapper.CreateMap<FocusFormModel, Focus>();
            FocusFormModel focus = new FocusFormModel();
            focus.Description = string.Empty;
            var result = (ViewResult)controller.CreateFocus(focus);

            // Assert - check that we are passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.AreEqual("CreateFocus", result.ViewName);
        }

        [Test]
        public void Create_Focus()
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
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
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
            principal.Setup(x => x.Identity).Returns(goalsetterUser);




            FocusFormModel focus = new FocusFormModel();
            focus.GroupId = 1;
            focus.FocusName = "t";
            focus.Description = "t";
            Focus mock = new Focus()
            {
                FocusName = "x",
                FocusId = 2,
                Description = "t",
                GroupId = 1
            };

            //Create Mapping 
            Mapper.CreateMap<FocusFormModel, Focus>();


            // Act
            var result = (RedirectToRouteResult)controller.CreateFocus(focus);
            Assert.AreEqual("Focus", result.RouteValues["action"]);


        }


        [Test]
        public void Focus()
        {
            Focus mock = new Focus()
            {
                FocusName = "x",
                FocusId = 1,
                Description = "t",
                GroupId = 1
            };
            focusRepository.Setup(x => x.GetById(1)).Returns(mock);

            GroupUser grpUser = new GroupUser()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                Admin = true
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpUser);

            IEnumerable<GroupGoal> fakegoal = new List<GroupGoal> {
            new GroupGoal{ GroupId = 1, Description="Test1Desc",GroupGoalId =1,FocusId =1,GroupUser = grpUser},
             new GroupGoal{ GroupId = 1, Description="Test1Desc",GroupGoalId =2,FocusId = 2,GroupUser = grpUser},
          
          
          }.AsEnumerable();
            groupGoalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupGoal, bool>>>())).Returns(fakegoal);

            Mapper.CreateMap<Focus, FocusViewModel>();
            Mapper.CreateMap<GroupGoal, GroupGoalViewModel>();


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


            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetAll()).Returns(fakeUser);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            principal.SetupGet(x => x.Identity.Name).Returns("abc");
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

            ViewResult result = controller.Focus(1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Focus", result.ViewName);


        }

        [Test]
        public void Create_Focus_Get_ReturnsView()
        {
            //Arrange

            Group group = new Group() { GroupId = 1, GroupName = "Test", Description = "test" };
            groupRepository.Setup(x => x.GetById(1)).Returns(group);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            //Act
            ActionResult result = controller.CreateFocus(1) as ActionResult;
            //Assert
            Assert.IsNotNull(result);

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
            GroupGoalFormModel goal = new GroupGoalFormModel();
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            ViewResult result = controller.CreateGoal(1) as ViewResult;

            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupGoalFormModel),
                result.ViewData.Model, "Wrong View Model");


        }

        [Test]
        public void Create_Goal()
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
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
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
            GroupUser grpuser = new GroupUser()
            {
                GroupId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpuser);


            // Act
            Mapper.CreateMap<GroupGoalFormModel, GroupGoal>();

            GroupGoalFormModel goal = new GroupGoalFormModel()
            {
                GoalName = "t",
                GroupGoalId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Description = "t",
                GroupId = 1,
                GroupUserId = 1,


            };
            var result = (RedirectToRouteResult)controller.CreateGoal(goal);


            Assert.AreEqual("Index", result.RouteValues["action"]);


        }
        [Test]
        public void Edit_Group_Get_ReturnsView()
        {


            Group group = new Group() { GroupId = 1, GroupName = "Test", Description = "test" };
            groupRepository.Setup(x => x.GetById(1)).Returns(group);
            Mapper.CreateMap<Group, GroupFormModel>();
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);


            ViewResult actual = controller.EditGroup(1) as ViewResult;


            Assert.IsNotNull(actual, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupFormModel),
                actual.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Edit_Focus_Get_ReturnsView()
        {


            Focus focus = new Focus() { FocusId = 1, FocusName = "Test", Description = "test" };
            focusRepository.Setup(x => x.GetById(1)).Returns(focus);
            Mapper.CreateMap<Focus, FocusFormModel>();
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            ViewResult actual = controller.EditFocus(1) as ViewResult;
            Assert.IsNotNull(actual, "View Result is null");
            Assert.IsInstanceOf(typeof(FocusFormModel),
                actual.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Delete_Focus_Get_ReturnsView()
        {
            Focus fake = new Focus()
            {
                FocusId = 1,
                FocusName = "test",
                Description = "test"
            };
            focusRepository.Setup(x => x.GetById(1)).Returns(fake);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            ViewResult result = controller.DeleteFocus(1) as ViewResult;

            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(Focus),
                 result.ViewData.Model, "Wrong View Model");
            var focus = result.ViewData.Model as Focus;
            Assert.AreEqual("test", focus.Description, "Got wrong Focus Description");

        }

        [Test]
        public void Delete_Group_Get_ReturnsView()
        {
            Group fake = new Group()
            {
                GroupId = 1,
                GroupName = "test",
                Description = "test"
            };
            groupRepository.Setup(x => x.GetById(1)).Returns(fake);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            ViewResult result = controller.DeleteGroup(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(Group),
                 result.ViewData.Model, "Wrong View Model");
            var group = result.ViewData.Model as Group;
            Assert.AreEqual("test", group.Description, "Got wrong Focus Description");

        }
        [Test]
        public void Delete_Goal_Get_ReturnsView()
        {


            GroupGoal fake = new GroupGoal()
            {
                GroupGoalId = 1,
                GoalName = "test",
                Description = "test",
                GroupId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                GoalStatusId = 1,
                GroupUserId = 2

            };


            groupGoalRepository.Setup(x => x.GetById(1)).Returns(fake);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            ViewResult result = controller.DeleteGoal(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupGoal),
                 result.ViewData.Model, "Wrong View Model");
            var group = result.ViewData.Model as GroupGoal;
            Assert.AreEqual("test", group.Description, "Got wrong Focus Description");

        }

        [Test]
        public void Delete_Focus_Post()
        {
            Focus fake = new Focus()
            {
                FocusId = 1,
                FocusName = "test",
                Description = "test"
            };

            focusRepository.Setup(x => x.GetById(1)).Returns(fake);
            Assert.IsNotNull(fake);
            Assert.AreEqual("test", fake.FocusName);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.DeleteConfirmedFocus(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);


        }
        [Test]
        public void Delete_Group_Post()
        {
            Group fake = new Group()
            {
                GroupId = 1,
                GroupName = "test",
                Description = "test"
            };
            groupRepository.Setup(x => x.GetById(fake.GroupId)).Returns(fake);


            Assert.IsNotNull(fake);
            Assert.AreEqual(1, fake.GroupId);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.DeleteConfirmedGroup(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }
        [Test]
        public void ListOfGroups_Test()
        {
            // Arrange      
            IEnumerable<Group> fakeCategories = new List<Group> {
            new Group { GroupName = "Test1", Description="Test1Desc"},
            new Group {  GroupName= "Test2", Description="Test2Desc"},
            new Group { GroupName = "Test3", Description="Test3Desc"}
          }.AsEnumerable();
            groupRepository.Setup(x => x.GetAll()).Returns(fakeCategories);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            // Act
            ViewResult result = controller.ListOfGroups() as ViewResult;
            // Assert
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(IEnumerable<Group>),
            result.ViewData.Model, "Wrong View Model");
            var grp = result.ViewData.Model as IEnumerable<Group>;
            Assert.AreEqual(3, grp.Count(), "Got wrong number of Groups");
        }

        [Test]
        public void Joined_User_Test()
        {
            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetAll()).Returns(fakeUser);


            IEnumerable<GroupUser> fakeGroupUser = new List<GroupUser> {            
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ee"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ef"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512eg"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=2,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512eh"}
           
          }.AsEnumerable();
            groupUserRepository.Setup(x => x.GetAll()).Returns(fakeGroupUser);


            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            // Act
            ViewResult result = controller.JoinedUsers(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupMemberViewModel),
            result.ViewData.Model, "Wrong View Model");




        }

        [Test]
        public void Edit_Group_Post()
        {
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            // Act
            Mapper.CreateMap<GroupFormModel, Group>();
            GroupFormModel group = new GroupFormModel()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                CreatedDate = DateTime.Now,
                Description = "Mock",
                GroupName = "Mock",
                GroupId = 1

            };
            var result = (RedirectToRouteResult)controller.EditGroup(group);
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void Edit_Focus_Post()
        {
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            // Act
            Mapper.CreateMap<FocusFormModel, Focus>();
            FocusFormModel group = new FocusFormModel()
            {
                FocusId = 1,
                FocusName = "test",
                Description = "test",
                GroupId = 1


            };

            Group grp = new Group()
            {
                GroupId = 1,
                GroupName = "t",
                Description = "t"
            };
            groupRepository.Setup(x => x.GetById(1)).Returns(grp);
            var result = (RedirectToRouteResult)controller.EditFocus(group);
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }
        [Test]
        public void Edit_Goal_Get_View()
        {
            GroupUser user = new GroupUser()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                GroupId = 1,
                GroupUserId = 1,
                Admin = false

            };
            GroupGoal goal = new GroupGoal()
            {
                GroupGoalId = 1,
                GroupId = 1,
                GoalName = "t",
                Description = "t",
                GoalStatusId = 1,
                GroupUserId = 1,
                GroupUser = user,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);
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

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            Mapper.CreateMap<GroupGoal, GroupGoalFormModel>();
            ViewResult result = controller.EditGoal(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupGoalFormModel),
                result.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Edit_Goal_Post()
        {

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            Mapper.CreateMap<GroupGoalFormModel, GroupGoal>();

            GroupGoalFormModel goal = new GroupGoalFormModel()
            {
                GoalName = "t",
                GroupGoalId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Description = "t",
                GroupId = 1,
                GroupUserId = 1

            };
            var result = (RedirectToRouteResult)controller.EditGoal(goal);
            Assert.AreEqual("GroupGoal", result.RouteValues["action"]);



        }

        [Test]
        public void Members_View()
        {

            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetAll()).Returns(fakeUser);


            IEnumerable<GroupUser> fakeGroupUser = new List<GroupUser> {            
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ee"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ef"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512eg"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=2,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512eh"}
           
          }.AsEnumerable();
            groupUserRepository.Setup(x => x.GetAll()).Returns(fakeGroupUser);


            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            // Act
            ViewResult result = controller.Members(1) as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupMemberViewModel),
            result.ViewData.Model, "Wrong View Model");

        }

        [Test]
        public void Delete_Member()
        {
            GroupUser user = new GroupUser()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                GroupUserId = 1,
                GroupId = 1
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(user);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.DeleteMember("402bd590-fdc7-49ad-9728-40efbfe512ec", 1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void Delete_Goal_Post()
        {
            GroupUser user = new GroupUser()
            {
                GroupId = 1
            };
            GroupGoal goal = new GroupGoal()
            {
                GroupGoalId = 1,
                GroupId = 1,
                GoalName = "t",
                Description = "t",
                GroupUser = user
            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.DeleteConfirmed(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void Save_Update_Update_Mandatory_Test()
        {
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            // The MVC pipeline doesn't run, so binding and validation don't run. 
            controller.ModelState.AddModelError("", "mock error message");
            GroupUpdateFormModel update = new GroupUpdateFormModel();
            update.Updatemsg = string.Empty;
            var result = controller.SaveUpdate(update) as RedirectToRouteResult;

            Assert.IsNull(result);
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

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");            //identity.Setup(x=>x.)
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

            Mapper.CreateMap<GroupUpdateFormModel, GroupUpdate>();

            Metric fakeMetric = new Metric()
            {
                MetricId = 1,
                Type = "%"
            };
            GroupGoal goal = new GroupGoal()
            {
                Metric = fakeMetric,
                Target = 100,
                GroupGoalId = 1
            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);


            IEnumerable<GroupUpdate> updt = new List<GroupUpdate> {            
            new GroupUpdate { GroupUpdateId =1, Updatemsg = "t1",GroupGoalId =1},
             new GroupUpdate { GroupUpdateId =2, Updatemsg = "t2",GroupGoalId =1},
              new GroupUpdate { GroupUpdateId =3, Updatemsg = "t3",GroupGoalId =2},
            
          }.AsEnumerable();
            groupUdateRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUpdate, bool>>>())).Returns(updt);

            GroupUser grpuser = new GroupUser()
            {
                GroupUserId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpuser);

            GroupUpdateUser updtuser = new GroupUpdateUser()
            {
                GroupUpdateId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUpdateUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUpdateUser, bool>>>())).Returns(updtuser);

            Mapper.CreateMap<GroupUpdate, GroupUpdateViewModel>();
            GroupUpdateFormModel mock = new GroupUpdateFormModel();
            mock.GroupId = 1;
            mock.Updatemsg = "mock";
            mock.GroupGoalId = 1;
            PartialViewResult result = controller.SaveUpdate(mock) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(GroupUpdateListViewModel),
            result.ViewData.Model, "Wrong View Model");


        }

        [Test]
        public void Display_Updates()
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

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");            //identity.Setup(x=>x.)
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
            GroupGoal goal = new GroupGoal()
            {
                Metric = fakeMetric,
                Target = 100
            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);

            IEnumerable<GroupUpdate> updt = new List<GroupUpdate> {            
            new GroupUpdate { GroupUpdateId =1, Updatemsg = "t1", GroupGoalId = 1},
             new GroupUpdate { GroupUpdateId =2, Updatemsg = "t2", GroupGoalId=2 },
              new GroupUpdate { GroupUpdateId =3, Updatemsg = "t3", GroupGoalId=2},
            
          }.AsEnumerable();
            groupUdateRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUpdate, bool>>>())).Returns(updt);

            GroupUser grpuser = new GroupUser()
            {
                GroupUserId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpuser);

            GroupUpdateUser updtuser = new GroupUpdateUser()
            {
                GroupUpdateId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUpdateUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUpdateUser, bool>>>())).Returns(updtuser);

            Mapper.CreateMap<GroupUpdate, GroupUpdateViewModel>();
            // GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            PartialViewResult rslt = controller.DisplayUpdates(1) as PartialViewResult;
            Assert.IsNotNull(rslt);
            Assert.IsInstanceOf(typeof(GroupUpdateListViewModel),
           rslt.ViewData.Model, "Wrong View Model");


        }

        [Test]
        public void DisplayComments()
        {
            IEnumerable<GroupComment> cmnt = new List<GroupComment> {            

            new GroupComment { GroupCommentId =1, GroupUpdateId = 1,CommentText="x"},
            new GroupComment { GroupCommentId =2, GroupUpdateId = 1,CommentText="y"},
            new GroupComment { GroupCommentId =3, GroupUpdateId = 1,CommentText="z"},
             
            
          }.AsEnumerable();

            groupCommentRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupComment, bool>>>())).Returns(cmnt);

            GroupCommentUser gcuser = new GroupCommentUser()
            {
                GroupCommentId = 1,
                GroupCommentUserId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupCommentUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupCommentUser, bool>>>())).Returns(gcuser);
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            Mapper.CreateMap<GroupComment, GroupCommentsViewModel>();
            PartialViewResult rslt = controller.DisplayComments(1) as PartialViewResult;
            Assert.IsNotNull(rslt, "View Result is null");
            Assert.IsInstanceOf(typeof(IEnumerable<GroupCommentsViewModel>),
             rslt.ViewData.Model, "Wrong View Model");
            var cmntsView = rslt.ViewData.Model as IEnumerable<GroupCommentsViewModel>;
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



            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

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
            // Arrange                    


            // Act          
            GroupCommentFormModel Cmnt = new GroupCommentFormModel();
            Mapper.CreateMap<GroupCommentFormModel, GroupComment>();
            Cmnt.CommentText = "Mock";
            var result = controller.SaveComment(Cmnt) as RedirectToRouteResult;
            // Assert 
            Assert.AreEqual("DisplayComments", result.RouteValues["action"]);
        }
        [Test]
        public void DisplayCommentCount()
        {
            IEnumerable<GroupComment> cmnt = new List<GroupComment> {            

            new GroupComment { GroupCommentId =1, GroupUpdateId = 1,CommentText="x"},
            new GroupComment { GroupCommentId =2, GroupUpdateId = 1,CommentText="y"},
            new GroupComment { GroupCommentId =3, GroupUpdateId = 1,CommentText="z"},
             
            
          }.AsEnumerable();

            groupCommentRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupComment, bool>>>())).Returns(cmnt);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            JsonResult count = controller.DisplayCommentCount(1) as JsonResult;
            Assert.IsNotNull(count);
            Assert.AreEqual(3, count.Data);

        }
        [Test]
        public void SearchUserForGroup()
        {

            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fakeUser);

            Group grp = new Group()
            {
                GroupId = 1
            };
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var searchString = "e";
            JsonResult result = controller.SearchUserForGroup(searchString, 1);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(JsonResult), result);



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
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
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
        public void Invite_Users()
        {
            Mapper.CreateMap<Group, GroupViewModel>();
            Group grp = new Group()
            {
                GroupId = 1,
                GroupName = "t",
                Description = "t"
            };
            groupRepository.Setup(x => x.GetById(1)).Returns(grp);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            ViewResult reslt = controller.InviteUsers(1) as ViewResult;
            Assert.IsNotNull(reslt);
            Assert.IsInstanceOf(typeof(GroupViewModel),
               reslt.ViewData.Model, "Wrong View Model");
            //var grpview = reslt.ViewData.Model as GroupViewModel;
            //Assert.AreEqual("t", grpview.GroupName);

        }
        [Test]
        public void Users_List()
        {

            IEnumerable<GroupUser> fakeGroupUser = new List<GroupUser> {            
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=2,UserId="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=3,UserId="402bd590-fdc7-49ad-9728-40efbfe512ee"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=4,UserId="402bd590-fdc7-49ad-9728-40efbfe512ef"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=5,UserId="402bd590-fdc7-49ad-9728-40efbfe512eg"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=2,GroupUserId=6,UserId="402bd590-fdc7-49ad-9728-40efbfe512eh"}
           
          }.AsEnumerable();
            groupUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(fakeGroupUser);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            ViewResult result = controller.UsersList(2) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(List<GroupUser>), result.ViewData.Model, "Wrong Model");
            var grpusr = result.ViewData.Model as List<GroupUser>;
            Assert.AreEqual(6, grpusr.Count(), "Got wrong number of GroupUser");


        }
        [Test]
        public void Join_Group_Get_Action()
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





            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            //principal.SetupGet(x=>x.Get(((SocialGoalUser)(User.Identity)).UserId).Callback(1));

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            //identity.Setup(x=>x.)
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

            var result = controller.JoinGroup(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void Group_Join_Request_Post_Action_Test()
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





            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            //principal.SetupGet(x=>x.Get(((SocialGoalUser)(User.Identity)).UserId).Callback(1));

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            //identity.Setup(x=>x.)
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
            Mapper.CreateMap<GroupRequestFormModel, GroupRequest>();

            var rslt = controller.GroupJoinRequest(1) as RedirectToRouteResult;
            Assert.AreEqual("Index", rslt.RouteValues["action"]);

        }

        [Test]
        public void Goal_Status_Post_Test()
        {
            GoalStatus status = new GoalStatus()
            {
                GoalStatusId = 1,
                GoalStatusType = "InProgress"
            };
            GroupGoal goal = new GroupGoal()
            {
                GroupId = 1,
                GroupGoalId = 1,
                GoalStatusId = 1,
                GoalStatus = status

            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            string result = controller.GoalStatus(1, 1) as string;
            Assert.AreEqual("InProgress", result);
        }

        [Test]
        public void Get_GoalReport_Test()
        {
            GroupGoal goal = new GroupGoal()
            {
                GoalStatusId = 1,
                GroupGoalId = 1
            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            JsonResult reslt = controller.GetGoalReport(1) as JsonResult;
            Assert.IsNotNull(reslt);

        }

        [Test]
        public void Groups_List_Test()
        {
            IEnumerable<Group> fake = new List<Group> {
            new Group{ GroupName = "Test1", Description="Test1Desc"},
            new Group { GroupName = "Test2", Description="Test2Desc"},
          
          }.AsEnumerable();
            groupRepository.Setup(x => x.GetAll()).Returns(fake);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            PartialViewResult result = controller.Groupslist(0) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(IEnumerable<Group>), result.ViewData.Model, "Wrong View Model");
            var gol = result.ViewData.Model as IEnumerable<Group>;
            Assert.AreEqual(2, gol.Count(), "Got wrong number of Groups");
        }
        [Test]
        public void Accept_Group_Join_Request_Post_Action()
        {
            GroupInvitation invitation = new GroupInvitation()
            {
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                GroupId = 1
            };
            groupInvitationRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupInvitation, bool>>>())).Returns(invitation);

            GroupRequest request = new GroupRequest()
            {
                GroupId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupRequest, bool>>>())).Returns(request);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.AcceptRequest(1, "402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            Assert.AreEqual("ShowAllRequests", result.RouteValues["action"]);
        }

        [Test]
        public void Group_Join_Request_Reject_Post_Action_Test()
        {
            GroupRequest request = new GroupRequest()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                GroupId = 1
            };
            groupRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupRequest, bool>>>())).Returns(request);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.RejectRequest(1, "402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            Assert.AreEqual("ShowAllRequests", result.RouteValues["action"]);
        }

        [Test]
        public void Show_All_Request_View()
        {
            IEnumerable<GroupRequest> request = new List<GroupRequest> {            
            new GroupRequest {GroupId =1,Accepted = false},
            new GroupRequest {GroupId =1,Accepted = false},
            new GroupRequest {GroupId =1,Accepted = false},
            
          }.AsEnumerable();
            groupRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupRequest, bool>>>())).Returns(request);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

            Mapper.CreateMap<GroupRequest, GroupRequestViewModel>();
            ViewResult result = controller.ShowAllRequests(1) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_RequestsView", result.ViewName);
            Assert.IsInstanceOf(typeof(IEnumerable<GroupRequestViewModel>), result.ViewData.Model, "Wrong View Model");
            var gol = result.ViewData.Model as IEnumerable<GroupRequestViewModel>;
            Assert.AreEqual(3, gol.Count(), "Got wrong number of Groups");


        }

        [Test]
        public void Search_Goal_For_Assigning()
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

            //userRepository.Setup(x => x.GetById(1)).Returns(user);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");            //identity.Setup(x=>x.)
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

            IEnumerable<GroupUser> fakeGroupUser = new List<GroupUser> {            
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=1,UserId="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=2,UserId="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new GroupUser {AddedDate=DateTime.Now,Admin=false,GroupId=1,GroupUserId=3,UserId="402bd590-fdc7-49ad-9728-40efbfe512ee"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=4,UserId="402bd590-fdc7-49ad-9728-40efbfe512ef"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=1,GroupUserId=5,UserId="402bd590-fdc7-49ad-9728-40efbfe512eg"},
            new GroupUser {AddedDate=DateTime.Now,Admin=true,GroupId=2,GroupUserId=6,UserId="402bd590-fdc7-49ad-9728-40efbfe512eh"}
           
          }.AsEnumerable();
            groupUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(fakeGroupUser);


            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fakeUser);

            PartialViewResult result = controller.SearchMemberForGoalAssigning(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_MembersToSearch", result.ViewName);
            Assert.IsInstanceOf(typeof(IEnumerable<MemberViewModel>), result.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Edit_Update_Get_View()
        {

            GroupUpdate update = new GroupUpdate()
            {
                GroupUpdateId = 1,
                Updatemsg = "abc",
                GroupGoalId = 1,

            };
            groupUdateRepository.Setup(x => x.GetById(1)).Returns(update);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            Mapper.CreateMap<GroupUpdate, GroupUpdateFormModel>();
            PartialViewResult result = controller.EditUpdate(1) as PartialViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupUpdateFormModel),
                result.ViewData.Model, "Wrong View Model");
            var data = result.ViewData.Model as GroupUpdateFormModel;
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


            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);

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
            Mapper.CreateMap<GroupUpdateFormModel, GroupUpdate>();
            Mapper.CreateMap<GroupUpdate, GroupUpdateViewModel>();

            Metric fakeMetric = new Metric()
            {
                MetricId = 1,
                Type = "%"
            };
            GroupGoal goal = new GroupGoal()
            {
                Metric = fakeMetric,
                Target = 100,
                GroupGoalId = 1
            };
            groupGoalRepository.Setup(x => x.GetById(1)).Returns(goal);


            IEnumerable<GroupUpdate> updt = new List<GroupUpdate> {            
            new GroupUpdate { GroupUpdateId =1, Updatemsg = "t1",GroupGoalId =1},
             new GroupUpdate { GroupUpdateId =2, Updatemsg = "t2",GroupGoalId =1},
              new GroupUpdate { GroupUpdateId =3, Updatemsg = "t3",GroupGoalId =2},
            
          }.AsEnumerable();
            groupUdateRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUpdate, bool>>>())).Returns(updt);

            GroupUser grpuser = new GroupUser()
            {
                GroupUserId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpuser);

            GroupUpdateUser updtuser = new GroupUpdateUser()
            {
                GroupUpdateId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUpdateUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUpdateUser, bool>>>())).Returns(updtuser);
            GroupUpdateFormModel mock = new GroupUpdateFormModel();
            mock.Updatemsg = "mock";
            mock.GroupGoalId = 1;
            mock.status = 34;
            PartialViewResult result = controller.EditUpdate(mock) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(GroupUpdateListViewModel),
            result.ViewData.Model, "Wrong View Model");
        }

        [Test]
        public void Delete_Update_Get_ReturnsView()
        {

            GroupUpdate update = new GroupUpdate()
            {
                GroupUpdateId = 1,
                Updatemsg = "abc",
                GroupGoalId = 1,

            };
            groupUdateRepository.Setup(x => x.GetById(1)).Returns(update);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            PartialViewResult result = controller.DeleteUpdate(1) as PartialViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(GroupUpdate),
                 result.ViewData.Model, "Wrong View Model");
            var group = result.ViewData.Model as GroupUpdate;
            Assert.AreEqual("abc", group.Updatemsg, "Got wrong message");

        }

        [Test]
        public void Delete_Update_Post()
        {

            GroupUpdate update = new GroupUpdate()
            {
                GroupUpdateId = 1,
                Updatemsg = "abc",
                GroupGoalId = 1,

            };
            groupUdateRepository.Setup(x => x.GetById(1)).Returns(update);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            var result = controller.DeleteConfirmedUpdate(1) as RedirectToRouteResult;
            Assert.AreEqual("GroupGoal", result.RouteValues["action"]);

        }


        [Test]
        public void Update_Supporters_Count()
        {
            IEnumerable<GroupUpdateSupport> updtsprt = new List<GroupUpdateSupport> {            

            new GroupUpdateSupport { GroupUpdateSupportId =1, GroupUpdateId = 1,GroupUserId = 1},
            new GroupUpdateSupport { GroupUpdateSupportId =2, GroupUpdateId = 1,GroupUserId = 2},
            new GroupUpdateSupport { GroupUpdateSupportId =3, GroupUpdateId = 1,GroupUserId = 3},
          
             
            
          }.AsEnumerable();

            groupUpdateSupportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUpdateSupport, bool>>>())).Returns(updtsprt);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            int count = controller.NoOfSupports(1);
            Assert.IsNotNull(count);
            Assert.AreEqual("3", count.ToString());

        }

        [Test]
        public void Display_Update_Supporters_Count()
        {
            IEnumerable<GroupUpdateSupport> updtsprt = new List<GroupUpdateSupport> {            

            new GroupUpdateSupport { GroupUpdateSupportId =1, GroupUpdateId = 1,GroupUserId = 1},
            new GroupUpdateSupport { GroupUpdateSupportId =2, GroupUpdateId = 1,GroupUserId = 2},
            new GroupUpdateSupport { GroupUpdateSupportId =3, GroupUpdateId = 1,GroupUserId = 3},
          
             
            
          }.AsEnumerable();

            groupUpdateSupportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUpdateSupport, bool>>>())).Returns(updtsprt);
            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
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
            IEnumerable<GroupUpdateSupport> fake = new List<GroupUpdateSupport> {            
            new GroupUpdateSupport { GroupUpdateSupportId =1, GroupUpdateId = 1, GroupUserId = 1},
            new GroupUpdateSupport { GroupUpdateSupportId =2, GroupUpdateId = 1, GroupUserId = 2},
            new GroupUpdateSupport { GroupUpdateSupportId =3, GroupUpdateId = 1, GroupUserId = 3},
          
          }.AsEnumerable();
            groupUpdateSupportRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<GroupUpdateSupport, bool>>>())).Returns(fake);

            GroupUser grpuser = new GroupUser()
            {
                GroupUserId = 1,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            groupUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<GroupUser, bool>>>())).Returns(grpuser);

            GroupController controller = new GroupController(groupService, groupUserService, userService, metricService, focusService, groupgoalService, groupInvitationService, securityTokenService, groupUpdateService, groupCommentService, goalStatusService, groupRequestService, followUserService, groupCommentUserService, groupUpdateSupportService, groupUpdateUserService);
            PartialViewResult result = controller.SupportersOfUpdate(1) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(GroupUpdateSupportersViewModel), result.ViewData.Model, "Wrong View Model");
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
