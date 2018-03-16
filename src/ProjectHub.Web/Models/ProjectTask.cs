using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectHub.Web.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "请选择所属项目")]
        [Display(Name = "所属项目")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "请输入任务名称")]
        [StringLength(100, ErrorMessage = "任务名称不能超过 100 个字符")]
        [Display(Name = "任务名称")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "任务描述不能超过 1000 个字符")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "任务描述")]
        public string Description { get; set; }

        [Required(ErrorMessage = "请选择截止日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "截止日期")]
        public DateTime DueDate { get; set; }

        [Display(Name = "任务状态")]
        public ProjectTaskStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
