using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Review.Models.Models.ReviewDB;

[Table("Reviews")]
public partial class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int BookId { get; set; }

    [Column("ReviewSID")]
    [StringLength(100)]
    [Unicode(false)]
    public string ReviewSid { get; set; } = null!;

    [StringLength(100)]
    public string ReviewerName { get; set; } = null!;

    public int? Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReviewDate { get; set; }
    public int Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedAt { get; set; }
}
