using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorateDrive.Models
{
    [Table("Documents")]
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; }
    }
}
