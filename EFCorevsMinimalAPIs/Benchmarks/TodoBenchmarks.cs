using BechmarkADOvsEntity.Models;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BechmarkADOvsEntity.Benchmarks
{
    [MemoryDiagnoser]
    public class TodoBenchmarks
    {
        private TodoContext _dbContext;

        [GlobalSetup]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseSqlServer("Server=localhost;Database=BenchmarkTest;Trusted_Connection=True;TrustServerCertificate=True")
                .Options;

            _dbContext = new TodoContext(options);
        }

        [Benchmark]
        public async Task AddTodoItemWithEFCore()
        {
            var todo = new TodoItem { Title = "Benchmark EF Core", IsCompleted = false };
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync();
        }

        [Benchmark]
        public async Task AddTodoItemWithMinimalAPI()
        {
            // Simulate adding a Todo item with Minimal API logic
            var todo = new TodoItem { Title = "Benchmark Minimal API", IsCompleted = false };
            using var dbContext = new TodoContext(new DbContextOptionsBuilder<TodoContext>()
                .UseSqlServer("Server=localhost;Database=BenchmarkTest;Trusted_Connection=True;TrustServerCertificate=True")
                .Options);

            dbContext.TodoItems.Add(todo);
            await dbContext.SaveChangesAsync();
        }
    }
}
