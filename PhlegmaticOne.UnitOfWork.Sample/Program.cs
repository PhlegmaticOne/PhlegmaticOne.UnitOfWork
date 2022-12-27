using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhlegmaticOne.UnitOfWork.Sample;

var serviceCollection = new ServiceCollection();

serviceCollection.AddDbContext<MyDbContext>(x => x.UseInMemoryDatabase("Memory"));
//serviceCollection.AddUnitOfWork<MyDbContext>();

var services = serviceCollection.BuildServiceProvider();

var repository = services.GetRequiredService<MyDbContext>();