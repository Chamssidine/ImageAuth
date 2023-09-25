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


    [HttpPost, Route("process-json")]
    public async Task<List<Models.Data>> GetOjectFromJsonFile( IFormFile jsonFile )
    {
             List<string> strings = new List<string>();
            if (jsonFile == null || jsonFile.Length == 0)
                throw new ArgumentException("Invalid or empty JSON file.");

            var userDataList = new List<Models.Data>();

            try
            {
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    var jsonString = await reader.ReadToEndAsync();
                    var rootObjectsList = JsonConvert.DeserializeObject<Root>(jsonString);

                    if (rootObjectsList != null && rootObjectsList.data.Count > 0)
                    {
                        var httpParser = new HttpParser();

                        foreach (var data in rootObjectsList.data)
                        {
                            data.Fenetre = await httpParser.GetImageUrlAsync(data.Fenetre);
                            data.Porte = await httpParser.GetImageUrlAsync(data.Porte);
                            data.Toiture = await httpParser.GetImageUrlAsync(data.Toiture);
                            data.Maison = await httpParser.GetImageUrlAsync(data.Maison);
                            data.Autrephoto = await httpParser.GetImageUrlAsync(data.Autrephoto);

                            userDataList.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing the JSON file.", ex);
            }

            return userDataList;
        }


    [HttpPost, Route("hashJson")]
    public async Task<IActionResult> HashJsonFIle(IFormFile jsonFile) {

        if (jsonFile == null || jsonFile.Length == 0)
            throw new ArgumentException("Invalid or empty JSON file.");

        var userDataList = new List<Models.Data>();
        try
        {
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                var jsonString = await reader.ReadToEndAsync();
                JsonHasher hasher = new JsonHasher();
                hasher.HashThisJson(jsonString);
                return Ok(hasher.JsonHash);
            }
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }


    }
}

