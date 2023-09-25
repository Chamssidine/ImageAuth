using ImageAuthApi.Data;
using ImageAuthApi.Models;
using ImageAuthApi.Utils;
using Microsoft.AspNetCore.Mvc;
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


    [HttpGet, Route("DeployContract")]
    public async Task<JsonResult> DeployContract()
    {

        var Message = await _contractManager.Deploy();
        return new JsonResult(Message);
    }


    [HttpPost, Route("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile jsonFile)
    {

        JsonHasher jsonHasher = new JsonHasher();
        List<string> hashList = new List<string>();

        List<OperationResult> result = new List<OperationResult>();
        List<OperationResult> ErrorResult = new List<OperationResult>();
        _ = new List<string>();
        OperationResult OpResult = new();
        _ = new UserDataAndImageList();
        List<UserDataAndImageList> imageDataList = new();
        var ObjectDataList = await new DataFromJsonExtractor().GetObjectFromJsonFile(jsonFile);
        if (ObjectDataList != null)
        {
            ImageDowlnoader dowloader = new();
            for (int i = 0; i < ObjectDataList.Count; i++)
            {
                UserDataAndImageList imageData = await dowloader.DownLoadImageFrom(ObjectDataList[i]);
                if (imageData != null)
                {
                    imageDataList.Add(imageData);
                }

            }
        }
        Console.WriteLine($"imageDataListSize:{imageDataList[0].Image.Count}");
        hashList = new ImageHashExtractor().ExtractHashFrom(imageDataList);
        Payloader payloader = new Payloader();
        result = await payloader.SendePayload(hashList, _contractManager);
        return Ok(result);
    }


    [HttpPost, Route("UploadJsonData")]
    public async Task<IActionResult> UploadJsonFile(IFormFile jsonFile)
    {
        List<OperationResult> result = new List<OperationResult>();
        JsonFileUploader uploader = new JsonFileUploader();
        var response = await uploader.UploadJsonFile(jsonFile);
        var hashList = uploader.GetHashList();
        if (response.IsSuccess)
        {
            Payloader payloader = new Payloader();
            result = await payloader.SendePayload(hashList, _contractManager);
        }
        return Ok(result);
    }


    [HttpPost, Route("CheckImage")]
    public async Task<IActionResult> CheckImgData(IFormFile image)
    {
        var OpResult = _hasher.HashThis(image);
        if (!OpResult.IsSuccess)
        {
            return BadRequest($"{OpResult}");
        }

        //bool hashExists = await _contractManager.CheckIfExists(_hasher.ImageHash); this is for the string[] hashdata verification
        //Console.WriteLine("Hash exists: " + hashExists);
        var dataFromVerification = await _contractManager.CheckIfExistsStruct(_hasher.ImageHash);
        if (dataFromVerification != null)
        {
            _operationResult.Message = $"Images exists hash = {dataFromVerification.ReturnValue1.ImageHash} id = {dataFromVerification.ReturnValue1.Id}   timeOfSave = {dataFromVerification.ReturnValue1.TimeOfSave}";
            _operationResult.IsSuccess = true;
        }
        else
        {
            _operationResult.Message = "Images does not exists";
            _operationResult.IsSuccess = false;
        }
        //_operationResult.Hash = _hasher.ImageHash;

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
        Console.WriteLine(hashDataList.ReturnValue1[0].Id);
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