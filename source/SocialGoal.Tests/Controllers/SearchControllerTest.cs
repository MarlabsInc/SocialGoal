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

namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class SearchControllerTest
    {
        Mock<IGoalRepository> goalRepository;
        Mock<IUserRepository> userRepository;
        Mock<IGroupRepository> groupRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<IGroupUserRepository> groupUserRepository;
        Mock<IUserProfileRepository> userProfileRepository;

        IGoalService goalService;
        IGroupService groupService;
        IUserService userService;

        Mock<IUnitOfWork> unitOfWork;

        [SetUp]
        public void SetUp()
        {
            goalRepository = new Mock<IGoalRepository>();
            userRepository = new Mock<IUserRepository>();
            groupRepository = new Mock<IGroupRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            groupUserRepository = new Mock<IGroupUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            unitOfWork = new Mock<IUnitOfWork>();

            goalService = new GoalService(goalRepository.Object, followUserRepository.Object, unitOfWork.Object);
            groupService = new GroupService(groupRepository.Object, followUserRepository.Object, groupUserRepository.Object, unitOfWork.Object);
            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Search_All()
        {
            IEnumerable<Goal> fakegoal = new List<Goal> {
            new Goal{ GoalStatusId =1, GoalName ="a",GoalType = false},
            new Goal{ GoalStatusId =1, GoalName ="abc",GoalType = false},
            new Goal{ GoalStatusId =1, GoalName ="aedg",GoalType = false},

           
          }.AsEnumerable();
            goalRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Goal, bool>>>())).Returns(fakegoal);

            IEnumerable<ApplicationUser> fakeUser = new List<ApplicationUser> {            
              new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fakeUser);

            IEnumerable<Group> fakeGroups = new List<Group> {
            new Group { GroupName = "Test1", Description="Test1Desc"},
            new Group {  GroupName= "Test2", Description="Test2Desc"},
            new Group { GroupName = "Test3", Description="Test3Desc"}
          }.AsEnumerable();
            groupRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Group, bool>>>())).Returns(fakeGroups);

            Mapper.CreateMap<Goal, GoalViewModel>();
            Mapper.CreateMap<Group, GroupViewModel>();
            SearchController controller = new SearchController(goalService, userService, groupService);
            ViewResult result = controller.SearchAll("a") as ViewResult;
            Assert.IsNotNull(result, "View Result is null");
            Assert.IsInstanceOf(typeof(SearchViewModel),
            result.ViewData.Model, "Wrong View Model");



        }



    }
}
