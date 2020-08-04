using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using NovatorTestTask.Models;

namespace NovatorTestTask.Data
{
    public sealed class WorkerContext : DbContext
    {
        public WorkerContext()
        {
            if (Workers.Any())
                return;

            Workers.AddOrUpdate(
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"),
                Worker.GetWorker("Vasya", "Vasya", "Vasya"));
            SaveChanges();
        }

        public DbSet<Worker> Workers { get; set; }
    }
}