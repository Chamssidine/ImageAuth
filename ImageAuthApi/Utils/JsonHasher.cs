using ImageAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace ImageAuthApi.Utils
{
    public class JsonHasher
    {
        private string jsonHash = string.Empty;

        public string JsonHash => jsonHash;

        public OperationResult HashThisJson( string jsonData )
        {
            OperationResult result = new OperationResult();
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                result.IsSuccess = false;
                result.Message = $"Error Json data is empty!";
                return result;
            }

            try
            {
                jsonHash = CalculateJsonHash(jsonData);
                if (string.IsNullOrWhiteSpace(jsonHash)) { 
                    result.IsSuccess = false;
                    result.Message = $"could not hash the jsonFile";
                    return result;
                }
                else
                {
                    result.IsSuccess= true;
                    result.Message = $"json data hashed successfuly!";
                    return result;
                }
              
               
            }
            catch (Exception ex)
            {
            
                return new OperationResult($"error:{ex.Message}",false);
            }
        }
        public OperationResult HashThisJson(JsonResult jsonData)
        {
            OperationResult result = new OperationResult();
            if (jsonData == null)
            {
                result.IsSuccess = false;
                result.Message = $"Error Json data is empty!";
                return result;
            }

            try
            {
                jsonHash = CalculateJsonHash(jsonData);
                if (string.IsNullOrWhiteSpace(jsonHash))
                {
                    result.IsSuccess = false;
                    result.Message = $"could not hash the jsonFile";
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Message = $"json data hashed successfuly!";
                    return result;
                }

            }
            catch (Exception ex)
            {

                return new OperationResult($"error:{ex.Message}", false);
            }
        }
        private string CalculateJsonHash( string jsonData )
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);
                byte[] hashBytes = sha256.ComputeHash(dataBytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private  string CalculateJsonHash(JsonResult jsonData)
        {
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonData));

                byte[] hashBytes = sha256.ComputeHash(dataBytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

    }
}
