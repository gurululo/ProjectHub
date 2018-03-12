using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectHub.Web.Models
{
    public class Project : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "请输入项目名称")]
        [StringLength(100, ErrorMessage = "项目名称不能超过 100 个字符")]
        [Display(Name = "项目名称")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "项目描述不能超过 1000 个字符")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "项目描述")]
        public string Description { get; set; }

        [Required(ErrorMessage = "请选择开始日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "开始日期")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "请选择结束日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "结束日期")]
        public DateTime EndDate { get; set; }

        [Display(Name = "项目状态")]
        public ProjectStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate == default(DateTime))
            {
                yield return new ValidationResult("请选择开始日期。", new[] { nameof(StartDate) });
            }

            if (EndDate == default(DateTime))
            {
                yield return new ValidationResult("请选择结束日期。", new[] { nameof(EndDate) });
            }

            if (StartDate != default(DateTime) &&
                EndDate != default(DateTime) &&
                EndDate.Date < StartDate.Date)
            {
                yield return new ValidationResult("结束日期不能早于开始日期。", new[] { nameof(EndDate) });
            }
        }
    }
}
