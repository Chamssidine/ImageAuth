using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ImageAuthApi.Models
{
    public class FixedAuditData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("num_dmd")]
        public string NumDmd { get; set; }

        [JsonProperty("entreprise_id")]
        public string EntrepriseId { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("audit_id")]
        public string AuditId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("demandeur")]
        public string Demandeur { get; set; }

        [JsonProperty("adresse_lieu_audit")]
        public string AdresseLieuAudit { get; set; }

        [JsonProperty("telephone_lieu_audit")]
        public string TelephoneLieuAudit { get; set; }

        [JsonProperty("email_lieu_audit")]
        public string EmailLieuAudit { get; set; }

        [JsonProperty("adresse_ics")]
        public string AdresseIcs { get; set; }

        [JsonProperty("code_porte_ics")]
        public string CodePorteIcs { get; set; }

        [JsonProperty("code_portail_ics")]
        public string CodePortailIcs { get; set; }

        [JsonProperty("type_logement")]
        public string TypeLogement { get; set; }

        [JsonProperty("surface_totale")]
        public string SurfaceTotale { get; set; }

        [JsonProperty("nbre_etage")]
        public string NbreEtage { get; set; }

        [JsonProperty("type_chauffage")]
        public string TypeChauffage { get; set; }

        [JsonProperty("annee_fabrication")]
        public DateTime AnneeFabrication { get; set; }

        [JsonProperty("confirm_travaux_effectuee")]
        public string ConfirmTravauxEffectuee { get; set; }

        [JsonProperty("vitrage_type_huisserie")]
        public string VitrageTypeHuisserie { get; set; }

        [JsonProperty("porte_fenetres_type_huisserie")]
        public string PorteFenetresTypeHuisserie { get; set; }

        [JsonProperty("type_installation")]
        public string TypeInstallation { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("transact_id")]
        public string TransactId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("rendez_vous_demande_audit_energetique_id")]
        public string RendezVousDemandeAuditEnergetiqueId { get; set; }

        [JsonProperty("negociation_id")]
        public string NegociationId { get; set; }
 
        
    }
    
    public  class FilteredData
    {
        public string? TransactId  { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyValue { get; set; }
    }
    public class FilteredConfirmedData
    {
  
        [JsonProperty("transact_id")]
        public string? TransactId { get; set; }
        [JsonProperty("property_name")]
        public string? PropertyName { get; set; }
        [JsonProperty("property_value")]
        public string? PropertyValue { get; set; }
    }
    public class FilteredConfirmedDataList
    {
        public List<FilteredConfirmedData>? Value { get; set; }
    }
    public class DataID
    {

        public int Id { get; set; }


    }
}
