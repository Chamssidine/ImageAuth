namespace ImageAuthApi.Models;

public class User
{
    public object adresse { get; set; }
    public double Long { get; set; }
    public double lat { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
}

public class Data2
{
    public string id { get; set; }
    public int users_id { get; set; }
    public User users { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
}

public class Original
{
    public Data2 data { get; set; }
}

public class ClientInfo
{
  //  public Headers headers { get; set; }
    public Original original { get; set; }
}

public class InspecteurComments
{
    public List<object> data { get; set; }
}

public class Data
{

    public ClientInfo client_info { get; set; }
    public string Porte { get; set; }
    public string Fenetre { get; set; }
    public string Toiture { get; set; }
    public string Maison { get; set; }
    public string Autrephoto { get; set; }
    public List<string> imageUrl { get; set; }
}

public class Root
{
    public List<Data> data { get; set; }
}

public class UserData
{
    public Data Data { get; set; }
    public List<string> imageUrl { get; set; }

}
