using PhlegmaticOne.UnitOfWork.Interfaces;

namespace PhlegmaticOne.UnitOfWork.Sample;

public class RepositoryA : IRepository
{
    private readonly IDependency _dependency;

    public RepositoryA(IDependency dependency)
    {
        _dependency = dependency;
    }
}


public class RepositoryB : IRepository
{
}

public interface IDependency
{
}
public class Dependency : IDependency
{
}