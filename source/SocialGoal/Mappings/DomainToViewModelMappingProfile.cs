using AutoMapper;
using PagedList;
using SocialGoal.Model.Models;
using SocialGoal.Web.Core.AutoMapperConverters;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialGoal.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Goal, GoalViewModel>();
            Mapper.CreateMap<Goal, GoalFormModel>();
            Mapper.CreateMap<Comment, CommentsViewModel>();
            Mapper.CreateMap<UserProfile, UserProfileFormModel>();
            Mapper.CreateMap<Group, GroupGoalFormModel>();
            Mapper.CreateMap<Group, GroupFormModel>();
            Mapper.CreateMap<GroupGoal, GroupGoalFormModel>();
            Mapper.CreateMap<GroupInvitation, NotificationViewModel>();
            Mapper.CreateMap<SupportInvitation, NotificationViewModel>();
            Mapper.CreateMap<Group, GroupViewModel>();
            Mapper.CreateMap<GroupGoal, GroupGoalViewModel>();
            Mapper.CreateMap<GroupComment, GroupCommentsViewModel>();
            Mapper.CreateMap<Focus, FocusViewModel>();
            Mapper.CreateMap<Focus, FocusFormModel>();
            Mapper.CreateMap<GroupRequest, GroupRequestViewModel>();
            Mapper.CreateMap<FollowRequest, NotificationViewModel>();
            Mapper.CreateMap<ApplicationUser, FollowersViewModel>();
            Mapper.CreateMap<ApplicationUser, FollowingViewModel>();
            Mapper.CreateMap<Update, UpdateFormModel>();
            Mapper.CreateMap<GroupUpdate, GroupUpdateFormModel>();
            Mapper.CreateMap<Update, UpdateViewModel>();
            Mapper.CreateMap<GroupUpdate, GroupUpdateViewModel>();
            //Mapper.CreateMap<X, XViewModel>()
            //    .ForMember(x => x.Property1, opt => opt.MapFrom(source => source.PropertyXYZ));
            Mapper.CreateMap<Goal, GoalListViewModel>().ForMember(x => x.SupportsCount, opt => opt.MapFrom(source => source.Supports.Count))
                                                      .ForMember(x => x.UserName, opt => opt.MapFrom(source => source.User.UserName))
                                                      .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate.ToString("dd MMM yyyy")))
                                                      .ForMember(x => x.EndDate, opt => opt.MapFrom(source => source.EndDate.ToString("dd MMM yyyy")));
            Mapper.CreateMap<Group, GroupsItemViewModel>().ForMember(x => x.CreatedDate, opt => opt.MapFrom(source => source.CreatedDate.ToString("dd MMM yyyy")));

            Mapper.CreateMap<IPagedList<Group>, IPagedList<GroupsItemViewModel>>().ConvertUsing<PagedListConverter<Group, GroupsItemViewModel>>();
           

        }
    }
}