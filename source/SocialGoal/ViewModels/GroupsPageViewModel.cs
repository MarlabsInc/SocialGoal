using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using PagedList;
using SocialGoal.Model.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SocialGoal.Web.ViewModels
{
    public class GroupsPageViewModel
    {
        public IPagedList<GroupsItemViewModel> GroupList { get; set; }

        public IEnumerable<SelectListItem> FilterBy { get; set; }

        public string SelectedFilter { get; set; }

        public GroupsPageViewModel(string selectedFilter)
        {
            SelectedFilter = selectedFilter;
            FilterBy = new SelectList(new[]{
                       new SelectListItem{ Text="All", Value="All"},
                       new SelectListItem{ Text="My Groups", Value="My Groups"},
                       new SelectListItem{ Text="My Followed Groups", Value="My Followed Groups"},
                       new SelectListItem{ Text="My Followings Groups", Value="My Followings Groups"}
                       }, "Text", "Value", SelectedFilter);
        }
    }

}