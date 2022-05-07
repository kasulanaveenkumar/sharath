using Microsoft.EntityFrameworkCore;

namespace Config.API.Entities.AdditionalData
{
    public class AdditionalDataContext : DbContext
    {
        private static AdditionalDataContext _context;

        public static AdditionalDataContext Context
        {
            get
            {
                if (_context == null)
                {
                    // Create db context options specifying in memory database
                    var options = new DbContextOptionsBuilder<AdditionalDataContext>()
                        .UseInMemoryDatabase(databaseName: "AdditionalData")
                        .Options;

                    //Use this to instantiate the db context
                    _context = new AdditionalDataContext(options);
                }

                return _context;
            }
        }
        private AdditionalDataContext(DbContextOptions<AdditionalDataContext> options)
            : base(options)
        {
        }

        //public DbSet<Users> Users { get; set; }

        //public DbSet<BrokerUsers> BrokerUsers { get; set; }
    }
}
