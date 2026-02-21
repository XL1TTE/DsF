
namespace Contracts;

public enum ResultStatus : byte
{
    None,
    Failed,
    Successed,
}

public class ActionResult<T>
{
    public ActionResult(ResultStatus status)
    {
        this.Status = status;
    }
    public T? Result;
    public ResultStatus Status;
    public string? Error;
}
