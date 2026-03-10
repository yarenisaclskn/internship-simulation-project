using System.ComponentModel.DataAnnotations;
namespace HaritaUygulamasi.Models
{
    public class HaritaNesnesiRota
    {
        [Key]
        public int RotaId { get; set; }
        public string RotaAdi { get; set; }
        public string RouteData { get; set; }
    }
}