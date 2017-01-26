using System.Data.Entity;

namespace TwitterStream.Tables
{
    public partial class TwitterStreamContext : DbContext
    {
        public TwitterStreamContext()
            : base("name=TwitterStreamDTO")
        {
        }

        public virtual DbSet<T_TS_CATEGORY> T_TS_CATEGORY { get; set; }
        public virtual DbSet<T_TS_EXCEPTION> T_TS_EXCEPTION { get; set; }
        public virtual DbSet<T_TS_PARAMETERS> T_TS_PARAMETERS { get; set; }
        public virtual DbSet<T_TS_TWEETS> T_TS_TWEETS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
