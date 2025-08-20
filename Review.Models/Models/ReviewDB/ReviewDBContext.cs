using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Review.Models.Models.ReviewDB;

public partial class ReviewDBContext : DbContext
{
    public ReviewDBContext(DbContextOptions<ReviewDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3DE0C20730D6E6EC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
