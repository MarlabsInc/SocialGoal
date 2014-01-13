using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SocialGoal.Model.Models;
using System.ComponentModel;


namespace SocialGoal.Web.Core.Extensions
{
    public static class SelectListExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItems(
              this IEnumerable<Metric> metrics, int selectedId)
        {
            return

                metrics.OrderBy(metric => metric.Type)
                      .Select(metric =>
                          new SelectListItem
                          {
                              Selected = (metric.MetricId == selectedId),
                              Text = metric.Type,
                              Value = metric.MetricId.ToString()
                          });
        }

        public static IEnumerable<SelectListItem> ToSelectListItems(
              this IEnumerable<Focus> focus, int selectedId)
        {
            return

                focus.OrderBy(f => f.FocusName)
                      .Select(f =>
                          new SelectListItem
                          {
                              Selected = (f.FocusId == selectedId),
                              Text = f.FocusName,
                              Value = f.FocusId.ToString()
                          });
        }

        public static IEnumerable<SelectListItem> ToSelectListItems(
              this IEnumerable<GoalStatus> goalStatus, int selectedId)
        {
            return

                goalStatus.OrderBy(gs => gs.GoalStatusType)
                      .Select(gs =>
                          new SelectListItem
                          {
                              Selected = (gs.GoalStatusId == selectedId),
                              Text = gs.GoalStatusType,
                              Value = gs.GoalStatusId.ToString()
                          });
        }
    }
}
