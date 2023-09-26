namespace ImageAuthApi.Models;

public class User
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string VilleResidence { get; set; }
    public string CodePostal { get; set; }
    public object Adresse { get; set; }
    public double Long { get; set; }
    public double Lat { get; set; }
    public bool Available { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
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
    public string Id { get; set; }
    public string num_dmd { get; set; }
    public string entreprise_id { get; set; }
    public string name { get; set; }
    public ClientInfo client_info { get; set; }
    public string Porte { get; set; }
    public string Fenetre { get; set; }
    public string Toiture { get; set; }
    public string Maison { get; set; }
    public string Autrephoto { get; set; }
    
}

public class Root
{
    public List<Object?> data { get; set; }
}

public class UserData
{
    public Data Data { get; set; }
   

}

public enum JsonType
{
    JsonArray,
    JsonObject,

}