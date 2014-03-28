using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SocialGoal.Model.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using SocialGoal.Data.Configuration;
using System.Reflection;

namespace SocialGoal.Data.Models
{
    public class SocialGoalEntities : IdentityDbContext<ApplicationUser>
    {

        public SocialGoalEntities()
            : base("SocialGoalEntities")
        {
        }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Focus> Focuses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Update> Updates { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Metric> Metrics { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<SecurityToken> SecurityTokens { get; set; }
        public DbSet<Support> Support { get; set; }
        public DbSet<GroupInvitation> GroupInvitation { get; set; }
        public DbSet<SupportInvitation> SupportInvitation { get; set; }
        public DbSet<GroupGoal> GroupGoal { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<GoalStatus> GoalStatus { get; set; }
        public DbSet<GroupUpdate> GroupUpdate { get; set; }
        public DbSet<GroupComment> GroupComment { get; set; }
        public DbSet<GroupRequest> GroupRequests { get; set; }
        public DbSet<FollowRequest> FollowRequest { get; set; }
        public DbSet<FollowUser> FollowUser { get; set; }
        public DbSet<GroupUpdateUser> GroupUpdateUsers { get; set; }
        public DbSet<GroupCommentUser> GroupCommentUsers { get; set; }
        public DbSet<CommentUser> CommentUsers { get; set; }
        public DbSet<UpdateSupport> UpdateSupports { get; set; }
        public DbSet<GroupUpdateSupport> GroupUpdateSupports { get; set; }
   

        public virtual void Commit()
        {
            base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            modelBuilder.Configurations.Add(new CommentConfiguration());
            modelBuilder.Configurations.Add(new CommentUserConfiguration());
            modelBuilder.Configurations.Add(new FocusConfiguration());
            modelBuilder.Configurations.Add(new FollowRequestConfiguration());
            modelBuilder.Configurations.Add(new FollowUserConfiguration());
            modelBuilder.Configurations.Add(new GoalConfiguration());
            modelBuilder.Configurations.Add(new GoalStatusConfiguration());
            modelBuilder.Configurations.Add(new GroupCommentConfiguration());
            modelBuilder.Configurations.Add(new GroupCommentUserConfguration());
            modelBuilder.Configurations.Add(new GroupConfiguration());
            modelBuilder.Configurations.Add(new GroupGoalConfiguration());
            modelBuilder.Configurations.Add(new GroupInvitationConfiguration());
            modelBuilder.Configurations.Add(new GroupRequestConfiguration());
            modelBuilder.Configurations.Add(new GroupUpdateSupportConfiguration());
            modelBuilder.Configurations.Add(new GroupUpdateUserConfiguration());
            modelBuilder.Configurations.Add(new GroupUserConfiguration());
            modelBuilder.Configurations.Add(new MetricConfiguration());
            modelBuilder.Configurations.Add(new RegistrationTokenConfiguration());
            modelBuilder.Configurations.Add(new SupportConfiguration());
            modelBuilder.Configurations.Add(new SupportInvitationConfiguration());
            modelBuilder.Configurations.Add(new UpdateConfiguration());
            modelBuilder.Configurations.Add(new UpdateSupportConfiguration());
            modelBuilder.Configurations.Add(new UserProfileConfiguration());

        }
    }
}