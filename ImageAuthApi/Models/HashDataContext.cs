using Microsoft.EntityFrameworkCore;


namespace ImageAuthApi.Models;

public class HashDataContext: DbContext
{
    public HashDataContext(DbContextOptions<HashDataContext> options): base(options)
    { 
    
    }
    public DbSet<HashData> HashDatas { get; set; } = null!;

}
