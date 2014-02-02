using PagedList;
using SocialGoal.Service;

namespace SocialGoal.Web.ViewModels
{
    public class GroupsPageViewModel
    {
        public IPagedList<GroupsItemViewModel> GroupList { get; set; }

        public GroupFilter Filter { get; set; }
    }
}