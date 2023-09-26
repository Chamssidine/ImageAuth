using ImageAuthApi.Contracts.HashStorage;
using ImageAuthApi.Contracts.HashStorage.ContractDefinition;
using ImageAuthApi.Models;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ImageAuthApi.Utils;

public class ContractManager
{
    public string? Message;
    private bool _isContractInitialized;
    private HashStorageDeployment _hashStorageDeployment = new();
    private HashStorageService _hashStorageService;
    private string? _privateKey = null;
    private Nethereum.Web3.Accounts.Account? _account = null;
    private Web3? _web3 = null;
    private bool _isContractDeployed = false;
    private int _IdCounter = 0;
    public HashData HashData;
    public OperationResult Init(string privateKey)
    {
        if(privateKey == null)
        {
            return new OperationResult("Failed to deploy contyract: privateKey could not be null",false);
        }
        //_privateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
        //_account = new Nethereum.Web3.Accounts.Account(_privateKey);
        //_web3 = new Web3(_account, "https://localhost:8845");
        //_privateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
        _privateKey = privateKey;
        _account = new Nethereum.Web3.Accounts.Account(_privateKey);
       // _web3 = new Web3(_account, "http://192.168.88.61:8545");
        _web3 = new Web3(_account, "https://ethereum-sepolia.blockpi.network/v1/rpc/public");
        Console.WriteLine(_web3.Client);
        if (_hashStorageService != null)
        {
            _isContractInitialized =  true;
        }
        else
        {
            _isContractInitialized = false;
        }
        return new OperationResult("succes", true);
    }
    public async Task<OperationResult?> Deploy(string privateKey)
    {
        OperationResult response = new();

        var initResult = Init(privateKey);
        if(!initResult.IsSuccess)
        {
            return initResult;
        }
        if (_isContractDeployed)
        {
            response.Message =  $"Contract already deployed at address:{_hashStorageService.ContractHandler.ContractAddress}";
            return response;
        }
        if (_privateKey != null && _account != null && _web3 != null)
        {
            try
            {
                _hashStorageService = await HashStorageService.DeployContractAndGetServiceAsync(_web3, _hashStorageDeployment);
                response.Message = $"Contract Deployed at address: {_hashStorageService.ContractHandler.ContractAddress}";
                _isContractInitialized = true;
                _isContractDeployed = true;
                response.IsSuccess = true;
            }
            catch (Exception ex) 
            {
                response.Message = $"there was an error: {ex.Message}";
                response.IsSuccess = false;
            }
            
        }
        return response;
    }
    public async Task<OperationResult?> SendData(string data)
    {
        OperationResult response = new();

        if (!_isContractInitialized)
        {
            response.Message = "Failed to send data, try to deploy a contract first.";
            response.IsSuccess = false;
            return response;
        }

        try
        {
            if (await CheckIfExists(data))
            {
                response.Message = "Hash already exists, no need to re-save it!";
                response.IsSuccess = false;
                return response;
            }

            var transactionHash = await _hashStorageService.StoreRequestAndWaitForReceiptAsync(data);
            response.Message = $"data sent successfuly. TransactionHash:{transactionHash.TransactionHash}";
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = $"An error occurred: {ex.Message}";
            response.IsSuccess = false;
        }

        return response;
    }


    public async Task<bool> CheckIfExists(string data)
    {
        if (_isContractInitialized)
        {
            try
            {
                return await _hashStorageService.CompareQueryAsync(data);
            }
            catch
            {

            }
        }
        return false;
    }
    public async Task<GetHashDataOutputDTO?> GetData()
    {

        if (_isContractInitialized)
        {
            Console.WriteLine($"fetching data...");
            try
            {
             
                var result =  await _hashStorageService.GetHashDataQueryAsync();
                Console.WriteLine($"data fetched.");
                return result;
            }
            catch
            {
                return null;
            }
             

        }
        return null;
    }
    public async Task<string?> GetHashAtIndex(int index)
    {
        if (_isContractInitialized)
        {
            try
            {
                return await _hashStorageService.GetHashDataByIndexQueryAsync(index);
            }
            catch
            {

            }
           
        }
        return null;
    }
    public async Task<int> GetLength()
    {
        var length = 0;
        if (_isContractInitialized)
        {
            try
            {
                length = (int)await _hashStorageService.GetHashStorageLengthQueryAsync();
            }
            catch
            {

            }
        }
        return length;
    }

    //for the struct typed data in the contract 
    public async Task<GetHashAtIndexOutputDTO?> GetHashDataByIndex(int index)
    {
        if (_isContractInitialized && _isContractDeployed)
        {
            try
            {
                var result = await _hashStorageService.GetHashAtIndexQueryAsync(index);
                return result;
            } catch(Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }
        }
        return null;
    }
    public async Task<Transaction?> GetTransactionDetails( string transactionHash )
    {
       
        if (_hashStorageService != null && _isContractDeployed)
        { 
            try
            {
                 EthereumTransactionReader transactionReader = new EthereumTransactionReader(_web3);
                 var transactionDetails = await transactionReader.GetTransactionAsync(transactionHash);
                 return transactionDetails;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"{ex.Message}");
            }
            
        }

        return null;
    }
    public async Task<GetImageDataListOutputDTO?> GetHashDataList()
    {

        if (_isContractInitialized)
        {
            Console.WriteLine($"fetching data...");
            try
            {
                var result = await _hashStorageService.GetImageDataListQueryAsync();
                Console.WriteLine($"data fetched:{result.ReturnValue1[0].TimeOfSave}");
                return result;
            }
            catch
            {
                return null;
            }


        }
        return null;
    }
    public async Task<int> GetHashDataLength()
    {
        var length = 0;
        if (_isContractInitialized)
        {
            try
            {
                length = (int)await _hashStorageService.GetImageDataLengthQueryAsync();
                Console.WriteLine("length:" + length.ToString());
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"error:{ex.Message}");
            }
        }
        return length;
    }
    public async Task<CheckIfHashExistsOutputDTO?> CheckIfExistsStruct(string hash)
    {
        if (_isContractInitialized)
        {
            try
            {
                return await _hashStorageService.CheckIfHashExistsQueryAsync(hash);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }
}

