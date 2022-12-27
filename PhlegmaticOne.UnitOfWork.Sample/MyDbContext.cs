using Microsoft.EntityFrameworkCore;

namespace PhlegmaticOne.UnitOfWork.Sample;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
        
    }
}