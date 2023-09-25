using ImageAuthApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ImageAuthApi.Data;

public class ImageObject
{
    private static ImageObject instance;
    public Dictionary<int,byte[]> Image { get; private set; }

    // Constructeur privé pour empêcher l'instanciation directe
    private ImageObject()
    {
        Image = new Dictionary<int,byte[]>();
    }

    // Méthode pour obtenir l'instance unique
    public static ImageObject GetInstance()
    {
        if (instance == null)
        {
            instance = new ImageObject();
            Console.WriteLine("NULL instance");

        }
        return instance;
    }
}

public class UserDataAndImageList
{
    public User Users { get; set; }
    public Dictionary<int, byte[]> Image { get; set; }
}
