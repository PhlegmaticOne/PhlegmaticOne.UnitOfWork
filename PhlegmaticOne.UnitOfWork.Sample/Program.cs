using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhlegmaticOne.UnitOfWork.Extensions;
using PhlegmaticOne.UnitOfWork.Interfaces;
using PhlegmaticOne.UnitOfWork.Sample;

var serviceCollection = new ServiceCollection();

serviceCollection.AddDbContext<MyDbContext>(x => x.UseInMemoryDatabase("Memory"));
serviceCollection.AddUnitOfWork<MyDbContext>();

serviceCollection.AddScoped<IDependency, Dependency>();

var services = serviceCollection.BuildServiceProvider();

var repository = services.GetRequiredService<IUnitOfWork>();

Console.WriteLine(repository);