using Microsoft.EntityFrameworkCore;

namespace AadFriendsApi.Model;

public class FriendsContext : DbContext
{
    public FriendsContext(DbContextOptions<FriendsContext> opt) : base(opt) { }

    public DbSet<Friend> Friends => Set<Friend>();


}
