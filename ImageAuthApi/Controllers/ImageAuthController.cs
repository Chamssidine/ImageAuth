using ImageAuthApi.Data;
using ImageAuthApi.Models;
using ImageAuthApi.Utils;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
 

namespace ImageAuthApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageAuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DataBaseManager _dataBaseManager;
    private readonly ContractManager _contractManager;
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly object imageUrlList;

    public ImageAuthController(IConfiguration configuration, DataBaseManager dataBaseManager, ContractManager contractManager )
    {
        _configuration = configuration;
        _dataBaseManager = dataBaseManager;
        _contractManager = contractManager;
    }

    [HttpGet]
    public IActionResult Get()
    {
        string query = "SELECT * FROM imagedata;";
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");

        using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
        {
            myconn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
            {
                using (MySqlDataReader myReader = myCommand.ExecuteReader())
                {
                    table.Load(myReader);
                }
            }
        }

        // Convertir la DataTable en une liste de dictionnaires pour une représentation JSON
        var result = table.AsEnumerable()
            .Select(row => row.Table.Columns.Cast<DataColumn>()
                .ToDictionary(column => column.ColumnName, column => row[column]))
            .ToList();

        return new JsonResult(result);
    }


    //[HttpPost, Route("process-json")]
    //public async Task<List<Models.Data>> GetOjectFromJsonFile( IFormFile jsonFile )
    //{
    //         List<string> strings = new List<string>();
    //        if (jsonFile == null || jsonFile.Length == 0)
    //            throw new ArgumentException("Invalid or empty JSON file.");

    //        var userDataList = new List<Models.Data>();

    //        try
    //        {
    //            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
    //            {
    //                var jsonString = await reader.ReadToEndAsync();
    //                var rootObjectsList = JsonConvert.DeserializeObject<Root>(jsonString);

    //                if (rootObjectsList != null && rootObjectsList.data.Count > 0)
    //                {
    //                    var httpParser = new HttpParser();

    //                    foreach (var data in rootObjectsList.data)
    //                    {
    //                        data.Fenetre = await httpParser.GetImageUrlAsync(data.Fenetre);
    //                        data.Porte = await httpParser.GetImageUrlAsync(data.Porte);
    //                        data.Toiture = await httpParser.GetImageUrlAsync(data.Toiture);
    //                        data.Maison = await httpParser.GetImageUrlAsync(data.Maison);
    //                        data.Autrephoto = await httpParser.GetImageUrlAsync(data.Autrephoto);

    //                        userDataList.Add(data);
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ApplicationException("An error occurred while processing the JSON file.", ex);
    //        }

    //        return userDataList;
    //    }


    //[HttpPost, Route("hashJson")]
    //public async Task<IActionResult> HashJsonFIle(IFormFile jsonFile) {

    //    if (jsonFile == null || jsonFile.Length == 0)
    //        throw new ArgumentException("Invalid or empty JSON file.");
    //    try
    //    {
    //        using (var reader = new StreamReader(jsonFile.OpenReadStream()))
    //        {
    //            var jsonString = await reader.ReadToEndAsync();
    //            JsonHasher hasher = new JsonHasher();
    //            hasher.HashThisJson(jsonString);
    //            return Ok(hasher.JsonHash);
    //        }
    //    }
    //    catch(Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }


    //}

    //[HttpPost, Route("CheckImage")]
    //public async Task<IActionResult> CheckImgData(IFormFile image)
    //{
    //    var OpResult = _hasher.HashThis(image);
    //    if (!OpResult.IsSuccess)
    //    {
    //        return BadRequest($"{OpResult}");
    //    }

    //    //bool hashExists = await _contractManager.CheckIfExists(_hasher.ImageHash); this is for the string[] hashdata verification
    //    //Console.WriteLine("Hash exists: " + hashExists);
    //    var VerificationResponse = await _contractManager.CheckIfExistsStruct(_hasher.ImageHash);
    //    if (VerificationResponse != null)
    //    {
    //        _operationResult.Message = $"Images exists hash = {VerificationResponse.ReturnValue1.ImageHash} id = {VerificationResponse.ReturnValue1.Id}   timeOfSave = {VerificationResponse.ReturnValue1.TimeOfSave}";
    //        _operationResult.IsSuccess = true;
    //    }
    //    else
    //    {
    //        _operationResult.Message = "Images does not exists";
    //        _operationResult.IsSuccess = false;
    //    }
    //    //_operationResult.Hash = _hasher.ImageHash;

    //    return Ok(_operationResult);
    //}

    //[HttpPost, Route("UploadImage")]
    //public async Task<IActionResult> UploadImage(IFormFile jsonFile)
    //{

    //    JsonHasher jsonHasher = new JsonHasher();
    //    List<string> hashList = new List<string>();

    //    List<OperationResult> result = new List<OperationResult>();
    //    List<OperationResult> ErrorResult = new List<OperationResult>();
    //    _ = new List<string>();
    //    OperationResult OpResult = new();
    //    _ = new UserDataAndImageList();
    //    List<UserDataAndImageList> imageDataList = new();
    //    var ObjectDataList = await new DataFromJsonExtractor().GetObjectFromJsonFile(jsonFile);
    //    if (ObjectDataList != null)
    //    {
    //        ImageDowlnoader dowloader = new();
    //        for (int i = 0; i < ObjectDataList.Count; i++)
    //        {
    //            UserDataAndImageList imageData = await dowloader.DownLoadImageFrom(ObjectDataList[i]);
    //            if (imageData != null)
    //            {
    //                imageDataList.Add(imageData);
    //            }

    //        }
    //    }
    //    Console.WriteLine($"imageDataListSize:{imageDataList[0].Image.Count}");
    //    hashList = new ImageHashExtractor().ExtractHashFrom(imageDataList);
    //    Payloader payloader = new Payloader();
    //    result = await payloader.SendePayload(hashList, _contractManager);
    //    return Ok(result);
    //}
}

