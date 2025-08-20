using System;
using System.ComponentModel.DataAnnotations;

namespace Review.Models.RequestModel
{
    public class ReviewRequestModel
    {
        [Required(ErrorMessage = "BookId is required")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Reviewer name is required")]
        [StringLength(100, ErrorMessage = "Reviewer Name cannot exceed 100 characters")]
        public string ReviewerName { get; set; } = null!;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }

        [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Review date is required")]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }
    }
}
