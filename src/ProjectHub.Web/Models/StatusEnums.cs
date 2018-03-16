using System.ComponentModel.DataAnnotations;

namespace ProjectHub.Web.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "未开始")]
        NotStarted = 0,

        [Display(Name = "进行中")]
        InProgress = 1,

        [Display(Name = "已完成")]
        Completed = 2
    }

    public enum ProjectTaskStatus
    {
        [Display(Name = "待处理")]
        Pending = 0,

        [Display(Name = "进行中")]
        InProgress = 1,

        [Display(Name = "已完成")]
        Completed = 2
    }
}
