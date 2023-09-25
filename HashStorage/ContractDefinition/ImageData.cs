using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ImageAuthApi.Contracts.HashStorage.ContractDefinition
{
    public partial class ImageData : ImageDataBase { }

    public class ImageDataBase 
    {
        [Parameter("uint256", "id", 1)]
        public virtual BigInteger Id { get; set; }
        [Parameter("string", "imageHash", 2)]
        public virtual string ImageHash { get; set; }
        [Parameter("uint256", "timeOfSave", 3)]
        public virtual BigInteger TimeOfSave { get; set; }
    }
}
