using PhlegmaticOne.OperationResults;

namespace PhlegmaticOne.UnitOfWork.Abstractions.Extensions;

public static class UnitOfWorkExtensions
{
    public static async Task<OperationResult<T>> ResultFromExecutionInTransaction<T>(
        this IUnitOfWork unitOfWork, Func<Task<T>> operation)
    {
        try
        {
            var result = await operation();
            await unitOfWork.SaveChangesAsync();
            return OperationResult.Successful(result);
        }
        catch (Exception e)
        {
            return OperationResult.Failed<T>(e.Message);
        }
    }

    public static async Task<OperationResult> ResultFromExecutionInTransaction(
        this IUnitOfWork unitOfWork, Func<Task> operation)
    {
        try
        {
            await operation();
            await unitOfWork.SaveChangesAsync();
            return OperationResult.Success;
        }
        catch (Exception e)
        {
            return OperationResult.Failed(e.Message);
        }
    }
}