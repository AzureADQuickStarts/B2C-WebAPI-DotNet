using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.DAL
{
    class TasksServiceInitializer : DropCreateDatabaseIfModelChanges<TasksServiceContext>
    {

    }
}
