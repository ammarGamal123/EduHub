using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("LocalConnection"); // For fallback
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Student> Students { get; set; }    
        public DbSet<Department> Departments { get;set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<DepartmentSubject> DepartmentSubjects { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<InstructorSubject> InstructorSubjects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstructorSubject>()
                .HasKey(i => new { i.SubjectID, i.InstructorID });

            modelBuilder.Entity<StudentSubject>()
                .HasKey(s => new { s.SubID, s.StudID });

            modelBuilder.Entity<DepartmentSubject>()
                .HasKey(d => new { d.SubID, d.DeptID });

            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.Supervisor)
                .WithMany(i => i.Instructors)
                .HasForeignKey(i => i.SupervisorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Instructor)
                .WithOne(d => d.DepartmentManager)
                .HasForeignKey<Department>(d => d.InstructorManager)
                .OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(modelBuilder);
        }


    }
}
