//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SocialGoal.Web.Controllers;
//using NUnit.Framework;
//using SocialGoal.Model.Models;
//using Microsoft.AspNet.Identity;
//using SocialGoal.Tests.Helpers;
//using SocialGoal.Data.Repository;
//using Moq;
//using SocialGoal.Data.Infrastructure;
//using System.Web.Mvc;
//using System.Security.Principal;
//using System.Web;
//using System.IO;
//using SocialGoal.Web.Core.Authentication;
//using SocialGoal.Service;
//using SocialGoal.Web.Mailers;
//using Mvc.Mailer;
//using SocialGoal.Models;
//using System.Linq.Expressions;
//namespace SocialGoal.Web.Controllers.Test
//{
//    [TestFixture()]
//    public class AccountControllerTest
//    {
//        Mock<IUserRepository> userRepository;
//        Mock<IUserProfileRepository> userProfileRepository;
//        Mock<IFollowRequestRepository> followRequestRepository;
//        Mock<IFollowUserRepository> followUserRepository;
//        Mock<ISecurityTokenRepository> securityTokenRepository;
//        Mock<IRegistrationTokenRepository> registrationTokenRepository;

//        Mock<IUnitOfWork> unitOfWork;

//        Mock<ControllerContext> controllerContext;
//        Mock<IIdentity> identity;
//        Mock<IPrincipal> principal;
//        Mock<HttpContext> httpContext;
//        Mock<HttpContextBase> contextBase;
//        Mock<HttpRequestBase> httpRequest;
//        Mock<HttpResponseBase> httpResponse;
//        Mock<HttpSessionStateBase> httpSession;
//        Mock<GenericPrincipal> genericPrincipal;

//        Mock<TempDataDictionary> tempData;
//        Mock<HttpPostedFileBase> file;
//        Mock<Stream> stream;
//        Mock<IFormsAuthentication> authentication;
//        Mock<IUserStore<ApplicationUser>> testUserStore;
//        //Mock<UserManager<ApplicationUser>> userManager;


//        IUserService userService;
//        IUserProfileService userProfileService;
//        IGoalService goalService;
//        IUpdateService updateService;
//        ICommentService commentService;
//        IFollowRequestService followRequestService;
//        IFollowUserService followUserService;
//        ISecurityTokenService securityTokenService;
//        IRegistrationTokenService registrationTokenService;
//        IUserMailer userMailer = new UserMailer();
//        //UserManager<ApplicationUser> userManager;
        


//        [SetUp]
//        public void SetUp()
//        {
//            userRepository = new Mock<IUserRepository>();
//            userProfileRepository = new Mock<IUserProfileRepository>();
//            followRequestRepository = new Mock<IFollowRequestRepository>();
//            followUserRepository = new Mock<IFollowUserRepository>();
//            securityTokenRepository = new Mock<ISecurityTokenRepository>();
//            registrationTokenRepository = new Mock<IRegistrationTokenRepository>();


//            unitOfWork = new Mock<IUnitOfWork>();

//            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
//            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);
//            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
//            followUserService = new FollowUserService(followUserRepository.Object, unitOfWork.Object);
//            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);
//            registrationTokenService = new RegistrationTokenService(registrationTokenRepository.Object, unitOfWork.Object);

//            controllerContext = new Mock<ControllerContext>();
//            contextBase = new Mock<HttpContextBase>();
//            httpRequest = new Mock<HttpRequestBase>();
//            httpResponse = new Mock<HttpResponseBase>();
//            genericPrincipal = new Mock<GenericPrincipal>();
//            httpSession = new Mock<HttpSessionStateBase>();
//            authentication = new Mock<IFormsAuthentication>();


//            identity = new Mock<IIdentity>();
//            principal = new Mock<IPrincipal>();
//            tempData = new Mock<TempDataDictionary>();
//            file = new Mock<HttpPostedFileBase>();
//            stream = new Mock<Stream>();
//            testUserStore = new Mock<IUserStore<ApplicationUser>>();
//            //userManager = new Mock<UserManager<ApplicationUser>>();

//        }

//        [TearDown]
//        public void TearDown()
//        {
//            TestSmtpClient.SentMails.Clear();
//        }

//        //[Test()]
//        //public void AccountControllerTest()
//        //{
//        //    Assert.Fail();
//        //}

//        [Test()]
//        public void AccountControllerTest1()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void LoginTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void LoginTest1()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void RegisterTest()
//        {

//            var userManager = new UserManager<ApplicationUser>(testUserStore.Object);
//            var user=new ApplicationUser()
//            {
//                UserName="adarsh"
//            };

            
//            var controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, registrationTokenService);
//            //var regResult = new Task<IdentityResult>();

//            //testUserStore.Setup<Task<ActionResult>>(userManager.CreateAsync(user, "123456")).Returns(regResult);
//            //userManager.Setup(x => x.CreateAsync(user, "123456")).Returns(regResult);
//            userManager.CreateAsync(user, "123456");
//            var result =
//            controller.Register(new RegisterViewModel
//            {
//                 UserName="adarsh",
//                  Password="123456",
//                  ConfirmPassword="123456"
//            }).Result;

//            // Assert
//            Assert.IsNotNull(result);

//            var addedUser = userManager.FindByName("adarsh");
//            Assert.IsNotNull(addedUser);
//            Assert.AreEqual("adarsh", addedUser.UserName);
//        }

//        [Test()]
//        public void RegisterTest1()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void DisassociateTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ManageTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ManageTest1()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ExternalLoginTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ExternalLoginCallbackTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void LinkLoginTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void LinkLoginCallbackTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ExternalLoginConfirmationTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void LogOffTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ExternalLoginFailureTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void RemoveAccountListTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void LoginPartialTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void RegisterPartialTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void SearchUserTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void ImageUploadTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void UploadImageTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void UserProfileTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void EditBasicInfoTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void EditPersonalInfoTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void EditProfileTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void FollowRequestTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void AcceptRequestTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void RejectRequestTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void UnfollowTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void FollowersTest()
//        {
//            Assert.Fail();
//        }

//        [Test()]
//        public void FollowingsTest()
//        {
//            Assert.Fail();
//        }
//    }
//}
