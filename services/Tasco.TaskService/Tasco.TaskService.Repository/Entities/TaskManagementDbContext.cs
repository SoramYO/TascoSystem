using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
	public class TaskManagementDbContext : DbContext
	{
		public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
			: base(options)
		{
		}

		// DbSets
		public DbSet<Task> Tasks { get; set; }
		public DbSet<SubTask> SubTasks { get; set; }
		public DbSet<Comment> Comments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			// WorkArea relationships
			modelBuilder.Entity<Task>()
				.HasMany(wa => wa.SubTasks)
				.WithOne(wt => wt.Task)
				.HasForeignKey(wt => wt.ParentTaskId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Task>()
				.HasMany(wa => wa.Comments)
				.WithOne(wt => wt.Task)
				.HasForeignKey(wt => wt.TaskId)
				.OnDelete(DeleteBehavior.Cascade);

			// Seed data (optional)
			SeedData(modelBuilder);
		}

		private void SeedData(ModelBuilder modelBuilder)
		{
		}

		// Override SaveChanges để tự động thêm TaskAction khi có thay đổi
		public override int SaveChanges()
		{
			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await base.SaveChangesAsync(cancellationToken);
		}

		// Method để track changes với user context
		public async Task<int> SaveChangesAsync(Guid currentUserId, string currentUserName, CancellationToken cancellationToken = default)
		{
			return await base.SaveChangesAsync(cancellationToken);
		}

		public int SaveChanges(Guid currentUserId, string currentUserName)
		{
			return base.SaveChanges();
		}

	}
}