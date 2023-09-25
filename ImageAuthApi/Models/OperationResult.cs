using System.Numerics;

namespace ImageAuthApi.Models;

public class OperationResult
{
    public OperationResult()
    {
    }

    public OperationResult(string? message, bool isSuccess )
    {
        Message = message;
        IsSuccess = isSuccess;
    }

    public string? Message { get; set; }
    public bool IsSuccess { get; set; } = false;

}
