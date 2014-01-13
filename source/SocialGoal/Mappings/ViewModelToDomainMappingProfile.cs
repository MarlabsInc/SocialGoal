using AutoMapper;
using SocialGoal.Model.Models;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialGoal.Mappings
{

    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<CommentFormModel, Comment>();
            Mapper.CreateMap<GroupFormModel, Group>();
            Mapper.CreateMap<FocusFormModel, Focus>();
            Mapper.CreateMap<UpdateFormModel, Update>();
            Mapper.CreateMap<UserFormModel, ApplicationUser>();
            Mapper.CreateMap<UserProfileFormModel, UserProfile>();
            Mapper.CreateMap<GroupGoalFormModel, GroupGoal>();
            Mapper.CreateMap<GroupUpdateFormModel, GroupUpdate>();
            Mapper.CreateMap<GroupCommentFormModel, GroupComment>();
            Mapper.CreateMap<GroupRequestFormModel, GroupRequest>();
            Mapper.CreateMap<FollowRequestFormModel, FollowRequest>();
            Mapper.CreateMap<GoalFormModel, Goal>();
            //Mapper.CreateMap<XViewModel, X()
            //    .ForMember(x => x.PropertyXYZ, opt => opt.MapFrom(source => source.Property1));     
        }
    }
}