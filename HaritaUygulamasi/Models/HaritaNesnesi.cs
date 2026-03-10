using System.ComponentModel.DataAnnotations;
namespace HaritaUygulamasi.Models
{
    public class HaritaNesnesi
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Pitch { get; set; }
        public string glb_file { get; set; }
        public bool IsVisible { get; set; } //kalıcı tutmak için
        public int? RouteData { get; set; } // Yeni eklenecek foreign key

        public int? RotaId { get; set; } // Yeni eklenecek foreign key



    }
}