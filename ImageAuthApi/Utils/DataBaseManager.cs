using ImageAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace ImageAuthApi.Utils;

public class DataBaseManager
{

    public static DataBaseManager? Instance { get; private set; }
    public JsonResult Save( IConfiguration _configuration, HashData result)
    {
        string query = @"
                     insert into imagedata values(
                       0, @imageHash,@dateSave,@imageurl,@hashtx
                       );";
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");
        MySqlDataReader myReader;
        using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
        {
            myconn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
            {

                myCommand.Parameters.AddWithValue("@imageHash", result.Hash);
                myCommand.Parameters.AddWithValue("@dateSave", result.DateOfSave.ToString());
                myCommand.Parameters.AddWithValue("@imageUrl", result.ImageUrl);
                myCommand.Parameters.AddWithValue("@hashtx", result.TxHash);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
                myconn.Close();
            }
        }
        return new JsonResult("ObjectData saved successfuly");
    }
    public JsonResult Get( IConfiguration _configuration)
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
}
