using PhlegmaticOne.OperationResults;

namespace PhlegmaticOne.UnitOfWork.Abstractions.Extensions;

public static class UnitOfWorkExtensions
{
    public static async Task<DomainResult<T>> ResultFromExecutionInTransaction<T>(
        this IUnitOfWork unitOfWork, Func<Task<T>> operation)
    {
        try
        {
            var result = await operation();
            await unitOfWork.SaveChangesAsync();
            return DomainResult.Success(result);
        }
        catch (Exception e)
        {
            return DomainResult.Failure<T>(new("Exception.Error", e.Message));
        }
    }

    public static async Task<DomainResult> ResultFromExecutionInTransaction(
        this IUnitOfWork unitOfWork, Func<Task> operation)
    {
        try
        {
            await operation();
            await unitOfWork.SaveChangesAsync();
            return DomainResult.Success();
        }
        catch (Exception e)
        {
            return DomainResult.Failure(new("Exception.Error", e.Message));
        }
    }
}