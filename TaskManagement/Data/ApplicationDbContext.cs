using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // define table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<TaskItem>().ToTable("Tasks");
            modelBuilder.Entity<TaskUser>().ToTable("TaskUsers");

            //TaskUser has TaskUserId as primary key
            modelBuilder.Entity<TaskUser>().HasKey(tu => tu.TaskUserId);

            //TaskUser (associative table) for many to many relation between tasks and users

            //A Task can be shared with many users  
            modelBuilder.Entity<TaskUser>().HasOne(tu => tu.Task)
                                           .WithMany(t => t.Participants)
                                           .HasForeignKey(tu => tu.TaskId)
                                           .OnDelete(DeleteBehavior.Cascade); //if a task is deleted also delete related TaskUser

            //A User can have many shared tasks
            modelBuilder.Entity<TaskUser>().HasOne(tu => tu.User)
                                           .WithMany(u => u.SharedTasks)
                                           .HasForeignKey(tu => tu.UserId)
                                           .OnDelete(DeleteBehavior.Cascade); //if a user is deleted also delete related TaskUser

            // A TaskItem has one Reporter (user)
            // A User can report many tasks (one to many relation)
            modelBuilder.Entity<TaskItem>().HasOne(t => t.Reporter)
                                           .WithMany(u => u.ReportedTasks)
                                           .HasForeignKey(t => t.ReporterId)
                                           .OnDelete(DeleteBehavior.Restrict); //prevent deleting a reporter that has tasks

            //store enums as strings

            modelBuilder.Entity<TaskItem>().Property(t => t.Status)
                                           .HasConversion<string>();
            
            modelBuilder.Entity<TaskItem>().Property(t => t.Priority)
                                           .HasConversion<string>();
            
            modelBuilder.Entity<User>().Property(u => u.Role)
                                       .HasConversion<string>();   
        }
    }
}
