using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> save() {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
