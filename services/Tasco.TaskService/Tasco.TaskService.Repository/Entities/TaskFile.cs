using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
    public class TaskFile
    {
        [Key]
        public Guid Id { get; set; }

        public Guid WorkTaskId { get; set; }

        [Required]
        [StringLength(300)]
        public string FileName { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.Now;

        [Required]
        public Guid UploadedByUserId { get; set; }

        [StringLength(200)]
        public string UploadedByUserName { get; set; }

        // Foreign key
        [ForeignKey("WorkTaskId")]
        public virtual WorkTask WorkTask { get; set; }
    }
}