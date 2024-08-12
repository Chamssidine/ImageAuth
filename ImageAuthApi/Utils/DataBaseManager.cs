using ImageAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Data;

namespace ImageAuthApi.Utils;

public class DataBaseManager
{

    public static DataBaseManager? Instance { get; private set; }
    public OperationResult WriteToDB( IConfiguration _configuration, FilteredConfirmedData filteredData , string hashFile)
    {
        if (filteredData == null)
        {
            return new("Error: can't write null data", false);
        }

        string query = @"
                     insert into confirmed_audit_data values(
                       0, @transact_id,@property_name,@property_value, @hash_value
                       );";
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");
        MySqlDataReader myReader;

        using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
        {
            myconn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
            {
                myCommand.Parameters.AddWithValue("@transact_id", filteredData.TransactId);
                myCommand.Parameters.AddWithValue("@property_name", filteredData.PropertyName);
                myCommand.Parameters.AddWithValue("@property_value", filteredData.PropertyValue);
                myCommand.Parameters.AddWithValue("@hash_value", hashFile);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
                myconn.Close();
            }
        }
        return new("Data writing  successfull!", true);
    }
    public JsonResult Get( IConfiguration _configuration , int id)
    {
        string query = "SELECT * FROM audit_data where id <= @id ;";
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");

        using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
        {
            myconn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
            {
                myCommand.Parameters.AddWithValue("@id", id);
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


    public OperationResult LoadIntoTempDB( IConfiguration _configuration, List<FilteredData> filteredData )
    {
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");

        using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
        {
            string query = @"
     INSERT INTO audit_data VALUES (0, @transact_id, @property_name, @property_value);";

            myconn.Open();
            try
            {
                using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
                {

                    myCommand.Parameters.Add("@transact_id", MySqlDbType.VarChar);
                    myCommand.Parameters.Add("@property_name", MySqlDbType.VarChar);
                    myCommand.Parameters.Add("@property_value", MySqlDbType.VarChar);

                    for (int i = 0 ; i < filteredData.Count ; i++)
                    {

                        myCommand.Parameters["@transact_id"].Value = filteredData[i].TransactId;
                        myCommand.Parameters["@property_name"].Value = filteredData[i].PropertyName;
                        myCommand.Parameters["@property_value"].Value = filteredData[i].PropertyValue;
                        ;

                        using (MySqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            table.Load(myReader);
                            myReader.Close();
                        }
                    }
                }
                return new($"Load successfulll", true);
            }

            catch (Exception ex)
            {
                return new($"ERROR: {ex.Message}", false);
            }
            myconn.Close();

        }



    }


    public JsonResult GetDataFromBase( IConfiguration _configuration, int id )
    {
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");
        string query = @"SELECT * FROM audit_data where id = @id";
        using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
        {
            myconn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
            {
                myCommand.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader myReader = myCommand.ExecuteReader())
                {
                    table.Load(myReader);
                }
            }
        }
        var result = table.AsEnumerable()
          .Select(row => row.Table.Columns.Cast<DataColumn>()
              .ToDictionary(column => column.ColumnName, column => row[column]))
          .ToList();

        return new JsonResult(result);
    }
    public JsonResult RemoveFromBase( IConfiguration _configuration, string propertyName )
    {
        DataTable table = new DataTable();
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");
        string query = @"DELETE  FROM audit_data where property_name = @property_name";
        try
        {
            using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
            {
                myconn.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
                {
                    myCommand.Parameters.AddWithValue("property_name", propertyName);
                    using (MySqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }
                myconn.Close();
            }
            return new(true);

        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public JsonResult ResetDB(IConfiguration _configuration )
    {
        List<string> queryList = new();
        queryList.Add(@"drop table confirmed_audit_data");
        queryList.Add(@"drop table audit_data");
        queryList.Add(@"create table confirmed_audit_data (id int primary key auto_increment, transact_id varchar(1000), property_name varchar(255), property_value varchar(1000), hash_value varchar(255))");
        queryList.Add(@"create table audit_data (id int primary key auto_increment, transact_id varchar(1000), property_name varchar(255), property_value varchar(1000))");  
        string sqlDataRessource = _configuration.GetConnectionString("ImageAuth");
        try
        {
            using (MySqlConnection myconn = new MySqlConnection(sqlDataRessource))
            {
           
                foreach(var query in queryList)
                {
                    myconn.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, myconn))
                    {
                        myCommand.ExecuteReader();

                    }
                    myconn.Close();
                }
               
            }
            return new("reset successful");

        }
        catch (Exception ex)
        {
            return new(ex.Message);
        }
    }
}
