using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ImageAuthApi.Models;

[FunctionOutput]
public class HashData
{
    public HashData(int id, int time, string? hash, string? txHash, string? imageUrl, string? dateOfSave)
    {
        Id = id;
        Time = time;
        Hash = hash;
        TxHash = txHash;
        ImageUrl = imageUrl;
        DateOfSave = dateOfSave;
    }

    public int Id;
    public int Time { get; set; }
 
    public string? Hash { get; set; }
    public string? TxHash { get; set; }
    public string? ImageUrl { get; set;}
    public string? DateOfSave { get; set; }



}
