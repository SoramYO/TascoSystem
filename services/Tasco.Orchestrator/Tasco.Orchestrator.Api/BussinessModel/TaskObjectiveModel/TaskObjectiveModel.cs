using System.ComponentModel.DataAnnotations;

namespace Tasco.Orchestrator.Api.BussinessModel.TaskObjectiveModel
{
    /// <summary>
    /// DTO cho tạo task objective
    /// </summary>
    public class CreateTaskObjectiveRequestDto
    {
        /// <summary>
        /// Tiêu đề task objective
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả task objective
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// DTO cho cập nhật task objective
    /// </summary>
    public class UpdateTaskObjectiveRequestDto
    {
        /// <summary>
        /// Tiêu đề task objective
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả task objective
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// DTO cho hoàn thành task objective
    /// </summary>
    public class CompleteTaskObjectiveRequestDto
    {
        /// <summary>
        /// Trạng thái hoàn thành
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
