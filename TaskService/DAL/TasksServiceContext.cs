using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.DAL
{
    public class TasksServiceContext : DbContext
    {
        public TasksServiceContext() : base("TasksServiceContext") {}

        public DbSet<TaskService.Models.Task> Tasks {get; set;}
    }
}
