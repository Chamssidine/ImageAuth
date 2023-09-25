using ImageAuthApi.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageAuthApi.Utils
{
    public class ImgHasher
    {
        private string imageHash = string.Empty;

        public string ImageHash => imageHash;

        public OperationResult HashThis( byte[] images )
        {
            OperationResult result = new OperationResult();
            try
            {
                imageHash = CalculateHash(images);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error: {ex.Message}";
                // Gérer les exceptions ici, en renvoyant false si une exception est levée.
                return result;
            }
        }
        public OperationResult HashThis(IFormFile file)
        {
            OperationResult result = new OperationResult();
           
            ImageChecker checker = new ImageChecker();
            if (!checker.IsImage(file))
            {
                Console.WriteLine("this is not an image file");
                result.Message = $"error: {file.FileName} is not an image.";
                result.IsSuccess = false;
                return result;
            }
            if (file == null || file.Length == 0)
            {
                result.IsSuccess = false;
                result.Message = $"error: null filw or empty!";
                return result;
            }

            try
            {
                imageHash = CalculateHash(file);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error: {ex.Message}";
                // Gérer les exceptions ici, en renvoyant false si une exception est levée.
                return result;
            }
        }

        //private string CalculateHash( string imagePath )
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    using (FileStream stream = File.OpenRead(imagePath))
        //    {
        //        byte[] hashBytes = sha256.ComputeHash(stream);
        //        StringBuilder builder = new StringBuilder();
        //        foreach (byte b in hashBytes)
        //        {
        //            builder.Append(b.ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}
        private string CalculateHash(IFormFile file)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byte[] hashBytes = sha256.ComputeHash(memoryStream.ToArray());
                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
        }
        private string CalculateHash( byte[] file )
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(file);
                StringBuilder builder = new StringBuilder(hashBytes.Length * 2);

                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }


    }
}
