using ImageAuthApi.Data;
using ImageAuthApi.Models;
using ImageAuthApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            var Message = await _contractManager.Deploy(key);
            return new JsonResult(Message);
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
                _operationResult.Message = "Images does not exists";
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

    //GET
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

}