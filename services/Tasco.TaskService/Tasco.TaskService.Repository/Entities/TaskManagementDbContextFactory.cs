using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tasco.TaskService.Repository.Entities
{
    public class TaskManagementDbContextFactory : IDesignTimeDbContextFactory<TaskManagementDbContext>
    {
        public TaskManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaskManagementDbContext>();
            // TODO: Replace with your actual connection string or load from config
            optionsBuilder.UseSqlServer("Server=(local);Database=TascoTask;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True");

            return new TaskManagementDbContext(optionsBuilder.Options);
        }
    }
}