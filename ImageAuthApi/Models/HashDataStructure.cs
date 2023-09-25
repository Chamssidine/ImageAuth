using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ImageAuthApi.Models;

[FunctionOutput]
public class HashDataStructure
{

    [Parameter("HashData", "hashData_")]
    public HashData? Hash { get; set; }
}
