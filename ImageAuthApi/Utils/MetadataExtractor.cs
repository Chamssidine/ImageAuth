using System.Drawing.Imaging;
using System.Text;

namespace ImageAuthApi.Utils;

public sealed class MetadataExtractor
{
    public void ExtractMetadataFrom( IFormFile file )
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var image = System.Drawing.Image.FromStream(stream);
            // Accéder aux propriétés de l'image (métadonnées)
            // Exemple : Largeur et hauteur de l'image
            int width = image.Width;
            int height = image.Height;

            Console.WriteLine("Largeur de l'image : " + width);
            Console.WriteLine("Hauteur de l'image : " + height);
            var properties = image.PropertyItems;

          
            foreach (PropertyItem property in properties)
            {
                int propertyId = property.Id;
                Console.WriteLine($"PropertyId:{propertyId}");
                string propertyName = GetPropertyName(propertyId);
                string propertyValue = ConvertBytesToString(property); // pass the entire property
                Console.WriteLine("Propriété : " + propertyName + ", Valeur : " + propertyValue);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de l'extraction des métadonnées : " + ex.Message);
        }

    }
    private string GetPropertyName(int propertyId)
    {
        switch (propertyId)
        {
            case 0x010E:
                return "ImageDescription";
            case 0x010F:
                return "Make";
            case 0x0110:
                return "Model";
            case 0x0112:
                return "Orientation";
            case 0x011A:
                return "XResolution";
            // Add more cases here...
            default:
                return $"UnknownProperty {propertyId}";
        }
    }


    private string ConvertBytesToString(PropertyItem property)
    {
        switch (property.Type)
        {
            case 1: // byte
                return string.Join(", ", property.Value);
            case 2: // ASCII string
                return Encoding.ASCII.GetString(property.Value);
            case 3: // unsigned short
                return BitConverter.ToUInt16(property.Value, 0).ToString();
            case 4: // unsigned long
                return BitConverter.ToUInt32(property.Value, 0).ToString();
            case 5: // unsigned rational
                uint numerator = BitConverter.ToUInt32(property.Value, 0);
                uint denominator = BitConverter.ToUInt32(property.Value, 4);
                return $"{numerator}/{denominator}";
            // Add more cases here...
            default:
                return "Unknown format";
        }
    }


}

