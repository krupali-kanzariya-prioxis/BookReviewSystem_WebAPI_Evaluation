using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Review.Models.SpDbContext
{
    public partial class ReviewManagementSpContext : DbContext
    {
        public ReviewManagementSpContext() { }
        public ReviewManagementSpContext(DbContextOptions<ReviewManagementSpContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ExecutreStoreProcedureResult> ExecutreStoreProcedureResult { get; set; }
        public virtual DbSet<ExecutreStoreProcedureResultWithSID> ExecutreStoreProcedureResultWithSID { get; set; }
        public virtual DbSet<ExecuteStoreProcedureResultWithId> ExecuteStoreProcedureResultWithId { get; set; }
        public virtual DbSet<ExecutreStoreProcedureResultList> ExecutreStoreProcedureResultList { get; set; }
        public virtual DbSet<ExecutreStoreProcedureResultWithEntitySID> ExecutreStoreProcedureResultWithEntitySID { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region SP           
            modelBuilder.Entity<ExecutreStoreProcedureResult>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();

            modelBuilder.Entity<ExecutreStoreProcedureResultWithSID>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();
            modelBuilder.Entity<ExecutreStoreProcedureResultWithSID>().HasNoKey().Property(e => e.SID).HasConversion<System.String>();
            modelBuilder.Entity<ExecuteStoreProcedureResultWithId>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<string>();
            modelBuilder.Entity<ExecuteStoreProcedureResultWithId>().HasNoKey().Property(e => e.Id).HasConversion<int>();
            modelBuilder.Entity<ExecutreStoreProcedureResultList>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();
            modelBuilder.Entity<ExecutreStoreProcedureResultWithEntitySID>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();


            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
