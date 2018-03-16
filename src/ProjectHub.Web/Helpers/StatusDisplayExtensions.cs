using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using ProjectHub.Web.Models;

namespace ProjectHub.Web.Helpers
{
    public static class StatusDisplayExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var display = member == null ? null : member.GetCustomAttribute<DisplayAttribute>();

            return display != null && !string.IsNullOrEmpty(display.Name)
                ? display.Name
                : value.ToString();
        }

        public static string ToBadgeClass(this ProjectStatus status)
        {
            switch (status)
            {
                case ProjectStatus.NotStarted:
                    return "badge-soft-secondary";
                case ProjectStatus.InProgress:
                    return "badge-soft-primary";
                case ProjectStatus.Completed:
                    return "badge-soft-success";
                default:
                    return "badge-soft-secondary";
            }
        }

        public static string ToBadgeClass(this ProjectTaskStatus status)
        {
            switch (status)
            {
                case ProjectTaskStatus.Pending:
                    return "badge-soft-warning";
                case ProjectTaskStatus.InProgress:
                    return "badge-soft-primary";
                case ProjectTaskStatus.Completed:
                    return "badge-soft-success";
                default:
                    return "badge-soft-secondary";
            }
        }
    }
}
