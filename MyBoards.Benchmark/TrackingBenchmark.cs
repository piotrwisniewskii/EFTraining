using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;

namespace MyBoards.Benchmark
{
    public class TrackingBenchmark
    {
        public int WithTracking()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyBoardsContext>()
                .UseSqlServer("Server=.;Database=MyBoardsDb;Trusted_Connection=True;");

            var _dbContext = new MyBoardsContext(optionsBuilder.Options);

            var comments = _dbContext.Comments.ToList();

            return comments.Count;
        }
        
        public int WithNoTracking()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyBoardsContext>()
                .UseSqlServer("Server=.;Database=MyBoardsDb;Trusted_Connection=True;");

            var _dbContext = new MyBoardsContext(optionsBuilder.Options);

            var comments = _dbContext.Comments
                .AsNoTracking()
                .ToList();

            return comments.Count;
        }
    }
}
