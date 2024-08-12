using ImageAuthApi.Data;
using ImageAuthApi.Models;
using ImageAuthApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace ImageAuthApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HashDataController : ControllerBase
{
    private readonly HashDataContext _context;
    private readonly IConfiguration _configuration;
    private HashData hashData;
    private MetadataExtractor _extractor = new MetadataExtractor();
    private ImageChecker _imageChecker = new();
    OperationResult _operationResult = new();
    private readonly ContractManager _contractManager;
    private readonly DataBaseManager _dbManager;
    private ImgHasher _hasher = new();
    public HashDataController(
        HashDataContext context,
        IConfiguration configuration,
        ContractManager contractManager,
        DataBaseManager dataBaseManager
    )
    {
        _context = context;
        _configuration = configuration;
        _contractManager = contractManager;
        _dbManager = dataBaseManager;
    }


    [HttpPost, Route("DeployContract")]
    public async Task<JsonResult> DeployContract(IFormFile privateKey)
    {
        using(var reader =  new StreamReader(privateKey.OpenReadStream()))
        {
            var key =  await reader.ReadToEndAsync();
            if (key != null)
            {
                Console.WriteLine(key);
                var Message = await _contractManager.Deploy(key);
                return new JsonResult(Message);
            }
            else
            {

            }
            return new JsonResult("Error: private key is null ");
        }
    }

    [HttpPost, Route("UploadJsonFile")]
    public async Task<IActionResult> UploadJsonFile(IFormFile jsonFile)
    {
        if (jsonFile == null || jsonFile.Length == 0)
        {
            return BadRequest("Invalid or empty JSON file.");
        }
        List<OperationResult> result = new List<OperationResult>();
        JsonFileProcessor jsonProcessor = new JsonFileProcessor();
        var response = await jsonProcessor.DeserializeAndHash(jsonFile, JsonType.JsonArray);
        var hashList = jsonProcessor.GetHashList();
        if (response.IsSuccess)
        {
            Payloader payloader = new Payloader();
            result = await payloader.SendPayload(hashList, _contractManager);
        }
        return Ok(result);
    }

    [HttpPost, Route("AuthentificateJsonFile")]
    public async Task<IActionResult> CheckJsonFile(IFormFile jsonFile)
    {
        if (jsonFile == null || jsonFile.Length == 0)
        {
            return BadRequest("Invalid or empty JSON file.");
        }
        JsonFileProcessor jsonFileProcessor = new JsonFileProcessor();
      
        var response = await jsonFileProcessor.DeserializeAndHash(jsonFile, JsonType.JsonObject);
        if(response.IsSuccess)
        {
            
            var VerificationResponse = await _contractManager.CheckIfExistsStruct(jsonFileProcessor.GetJsonHash());
            if (VerificationResponse != null)
            {
                _operationResult.Message = $"Hash exists. hash:{VerificationResponse.ReturnValue1.ImageHash} id:{VerificationResponse.ReturnValue1.Id}   timeOfSave:{VerificationResponse.ReturnValue1.TimeOfSave}";
                _operationResult.IsSuccess = true;
            }
            else
            {
                _operationResult.Message = "Hash does not exists";
                _operationResult.IsSuccess = false;
            }
        }
        return Ok(_operationResult);

    }
    [HttpPost, Route("UploadJsonArrayObjectFile")]
    public async Task<IActionResult> UploadJsonArrayObjectFile(IFormFile jsonFile)
    {

        if (jsonFile == null || jsonFile.Length == 0)
        {
            return BadRequest("Invalid or empty JSON file.");
        }
        JsonFileProcessor jsonFileProcessor = new JsonFileProcessor();
        var result = await jsonFileProcessor.HashJsonArray(jsonFile);
        Console.WriteLine(jsonFileProcessor.GetJsonHash());
        if (result.IsSuccess)
        {
            _operationResult = await _contractManager.SendData(jsonFileProcessor.GetJsonHash().ToString());
        }
        return Ok(_operationResult);
    }

    [HttpPost, Route("AuthenticateJsonArrayFile")]
    public async Task<IActionResult> CheckJsonArrayFile(IFormFile jsonFile)
    {
        if (jsonFile == null || jsonFile.Length == 0)
        {
            return BadRequest("Invalid or empty JSON file.");
        }
        JsonFileProcessor jsonFileProcessor = new JsonFileProcessor();
        var result = await jsonFileProcessor.HashJsonArray(jsonFile);
        if (result.IsSuccess)
        {
            var VerificationResponse = await _contractManager.CheckIfExistsStruct(jsonFileProcessor.GetJsonHash());
            if (VerificationResponse != null)
            {
                _operationResult.Message = $"Hash exists. hash:{VerificationResponse.ReturnValue1.ImageHash} id:{VerificationResponse.ReturnValue1.Id}   timeOfSave:{VerificationResponse.ReturnValue1.TimeOfSave}";
                _operationResult.IsSuccess = true;
            }
            else
            {
                _operationResult.Message = "Images does not exists";
                _operationResult.IsSuccess = false;
            }
        }
        else
        {
            _operationResult = result;
        }
        return Ok(_operationResult);
    }

  
    [HttpGet, Route("GetHashData")]
    public async Task<IActionResult> GetAllHashData()
    {
        List<string> hashDataList = new List<string>();
        int hashStorageLength = await _contractManager.GetLength();

        for (int i = 0; i < hashStorageLength; i++)
        {
            string hashData = await _contractManager.GetHashAtIndex(i);
            hashDataList.Add(hashData);
        }

        return Ok(hashDataList);
    }

    [HttpPost, Route("GetTransactionData")]
    public async Task<ActionResult<EthereumTransaction>> GetTransactionDetails(string tx)
    {
        var transactionDetails = await _contractManager.GetTransactionDetails(tx);
        if (transactionDetails == null)
        {
            return NotFound();
        }

        return Ok(transactionDetails);
    }


    [HttpGet, Route("GetHashDataList")]
    public async Task<IActionResult> GetHashDataList()
    {
        var hashDataList = await _contractManager.GetHashDataList();
        if (hashDataList == null)
        {
            return BadRequest("error: no data found!");
        }

        //await _contractManager.GetHashDataLength();
        return Ok(hashDataList.ReturnValue1);
    }

    [HttpGet, Route("GetHashDataAtIndex")]
    public async Task<IActionResult> GetHashDataAtIndex(int id)
    {

        var hashDataList = await _contractManager.GetHashDataByIndex(id);
        if (hashDataList == null)
        {
            return BadRequest("error: no data found!");
        }
        return Ok(hashDataList);
    }

    /*update*/

    [HttpGet, Route("SendDataToBlockChain")]
    public async Task<IActionResult> Process( int lastRowID )
    {
     
        JsonFileProcessor JsonDeserializer = new JsonFileProcessor();
        List<JsonResult> list = new List<JsonResult>();
        Task<List<DataID>> deserializedDataID = null;
        JsonResult dataID = new("");
        dataID = _dbManager.Get(_configuration, lastRowID);
        deserializedDataID = JsonDeserializer.DeserializeDataIdAsync(dataID);
        if (deserializedDataID.IsCompleted)
        {
            for (var i = 0 ; i < deserializedDataID.Result.Count ; i++)
            {

                var dataToSave = _dbManager.GetDataFromBase(_configuration, deserializedDataID.Result[i].Id);
                var deserialsedDataTosave = (FilteredConfirmedDataList)JsonDeserializer.Deserialize<FilteredConfirmedDataList>(dataToSave);
                JsonHasher jsonHasher = new JsonHasher();
                var dataToSendHash = jsonHasher.HashThisJson(deserialsedDataTosave.ToJson());
                if (dataToSendHash.IsSuccess)
                {
                    Payloader payloader = new Payloader();
                    var hashFile = jsonHasher.JsonHash;
                    if (!_contractManager.CheckIfExists(hashFile).Result)
                    {
                        var result = await payloader.SendPayload(hashFile, _contractManager);

                        if (result.IsSuccess)
                        {

                            var writeResult = _dbManager.WriteToDB(_configuration, deserialsedDataTosave.Value[0], hashFile);

                            if (writeResult.IsSuccess)
                            {
                                Console.WriteLine("success");
                                _dbManager.RemoveFromBase(_configuration, deserialsedDataTosave.Value[0].PropertyName);

                            }
                            else
                            {
                                return new JsonResult($"writeResult:{writeResult.Message}");
                            }
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                    else
                    {
                        _dbManager.RemoveFromBase(_configuration, deserialsedDataTosave.Value[0].PropertyName);
                    }
                  
                }
                list.Add(dataToSave);
            }
          
            return Ok(list);
        }
        return new JsonResult(dataID);


    }


    [HttpPost, Route("LoadData")]
    public async Task<IActionResult> Load( IFormFile jsonFile )
    {
        JsonFileProcessor JsonDeserializer = new JsonFileProcessor();   
        var data = await JsonDeserializer.Deserialize(jsonFile);
        if (_dbManager.LoadIntoTempDB(_configuration, data).IsSuccess)
        {
            return Ok("data loaded successfully!");
        }
        else
        {
            return BadRequest("Error while loading data");
        }

    }

    [HttpGet, Route("Reset DB")]
    public async Task<IActionResult> Reset()
    {
        return Ok(_dbManager.ResetDB(_configuration));
    }

}