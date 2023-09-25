using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using ImageAuthApi.Contracts.HashStorage.ContractDefinition;

namespace ImageAuthApi.Contracts.HashStorage
{
    public partial class HashStorageService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, HashStorageDeployment hashStorageDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<HashStorageDeployment>().SendRequestAndWaitForReceiptAsync(hashStorageDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, HashStorageDeployment hashStorageDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<HashStorageDeployment>().SendRequestAsync(hashStorageDeployment);
        }

        public static async Task<HashStorageService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, HashStorageDeployment hashStorageDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, hashStorageDeployment, cancellationTokenSource);
            return new HashStorageService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public HashStorageService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public HashStorageService(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> StoreRequestAsync(StoreFunction storeFunction)
        {
             return ContractHandler.SendRequestAsync(storeFunction);
        }

        public Task<TransactionReceipt> StoreRequestAndWaitForReceiptAsync(StoreFunction storeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(storeFunction, cancellationToken);
        }

        public Task<string> StoreRequestAsync(string hash)
        {
            var storeFunction = new StoreFunction();
                storeFunction.Hash = hash;
            
             return ContractHandler.SendRequestAsync(storeFunction);
        }

        public Task<TransactionReceipt> StoreRequestAndWaitForReceiptAsync(string hash, CancellationTokenSource cancellationToken = null)
        {
            var storeFunction = new StoreFunction();
                storeFunction.Hash = hash;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(storeFunction, cancellationToken);
        }

        public Task<CheckIfHashExistsOutputDTO> CheckIfHashExistsQueryAsync(CheckIfHashExistsFunction checkIfHashExistsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<CheckIfHashExistsFunction, CheckIfHashExistsOutputDTO>(checkIfHashExistsFunction, blockParameter);
        }

        public Task<CheckIfHashExistsOutputDTO> CheckIfHashExistsQueryAsync(string hash, BlockParameter blockParameter = null)
        {
            var checkIfHashExistsFunction = new CheckIfHashExistsFunction();
                checkIfHashExistsFunction.Hash = hash;
            
            return ContractHandler.QueryDeserializingToObjectAsync<CheckIfHashExistsFunction, CheckIfHashExistsOutputDTO>(checkIfHashExistsFunction, blockParameter);
        }

        public Task<bool> CompareQueryAsync(CompareFunction compareFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CompareFunction, bool>(compareFunction, blockParameter);
        }

        
        public Task<bool> CompareQueryAsync(string hash, BlockParameter blockParameter = null)
        {
            var compareFunction = new CompareFunction();
                compareFunction.Hash = hash;
            
            return ContractHandler.QueryAsync<CompareFunction, bool>(compareFunction, blockParameter);
        }

        public Task<GetHashAtIndexOutputDTO> GetHashAtIndexQueryAsync(GetHashAtIndexFunction getHashAtIndexFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHashAtIndexFunction, GetHashAtIndexOutputDTO>(getHashAtIndexFunction, blockParameter);
        }

        public Task<GetHashAtIndexOutputDTO> GetHashAtIndexQueryAsync(BigInteger index, BlockParameter blockParameter = null)
        {
            var getHashAtIndexFunction = new GetHashAtIndexFunction();
                getHashAtIndexFunction.Index = index;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetHashAtIndexFunction, GetHashAtIndexOutputDTO>(getHashAtIndexFunction, blockParameter);
        }

        public Task<GetHashDataOutputDTO> GetHashDataQueryAsync(GetHashDataFunction getHashDataFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHashDataFunction, GetHashDataOutputDTO>(getHashDataFunction, blockParameter);
        }

        public Task<GetHashDataOutputDTO> GetHashDataQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHashDataFunction, GetHashDataOutputDTO>(null, blockParameter);
        }

        public Task<string> GetHashDataByIndexQueryAsync(GetHashDataByIndexFunction getHashDataByIndexFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetHashDataByIndexFunction, string>(getHashDataByIndexFunction, blockParameter);
        }

        
        public Task<string> GetHashDataByIndexQueryAsync(BigInteger index, BlockParameter blockParameter = null)
        {
            var getHashDataByIndexFunction = new GetHashDataByIndexFunction();
                getHashDataByIndexFunction.Index = index;
            
            return ContractHandler.QueryAsync<GetHashDataByIndexFunction, string>(getHashDataByIndexFunction, blockParameter);
        }

        public Task<BigInteger> GetHashStorageLengthQueryAsync(GetHashStorageLengthFunction getHashStorageLengthFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetHashStorageLengthFunction, BigInteger>(getHashStorageLengthFunction, blockParameter);
        }

        
        public Task<BigInteger> GetHashStorageLengthQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetHashStorageLengthFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> GetImageDataLengthQueryAsync(GetImageDataLengthFunction getImageDataLengthFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetImageDataLengthFunction, BigInteger>(getImageDataLengthFunction, blockParameter);
        }

        
        public Task<BigInteger> GetImageDataLengthQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetImageDataLengthFunction, BigInteger>(null, blockParameter);
        }

        public Task<GetImageDataListOutputDTO> GetImageDataListQueryAsync(GetImageDataListFunction getImageDataListFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetImageDataListFunction, GetImageDataListOutputDTO>(getImageDataListFunction, blockParameter);
        }

        public Task<GetImageDataListOutputDTO> GetImageDataListQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetImageDataListFunction, GetImageDataListOutputDTO>(null, blockParameter);
        }

        public Task<ImageDataListOutputDTO> ImageDataListQueryAsync(ImageDataListFunction imageDataListFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<ImageDataListFunction, ImageDataListOutputDTO>(imageDataListFunction, blockParameter);
        }

        public Task<ImageDataListOutputDTO> ImageDataListQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var imageDataListFunction = new ImageDataListFunction();
                imageDataListFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<ImageDataListFunction, ImageDataListOutputDTO>(imageDataListFunction, blockParameter);
        }
    }
}
