using ImageAuthApi.Models;

namespace ImageAuthApi.Data
{
    public class ImageData
    {
        public int Id { get; set; }
        public string? Hash { get; set; }
        public string? DateOfSave { get; set; }
        public string? Url { get; set; }
        //public Models.ObjectData MetaData { get; set; }

    }
}
