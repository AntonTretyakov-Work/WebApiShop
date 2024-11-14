// Models/Accessory.cs
namespace MobileStoreApi.Models
{
    public class Accessory
    {
        public int? IdAccessories { get; set; }
        public string? Name { get; set; }
        public decimal? Cost { get; set; }
        public int? IdMobileDevice { get; set; } // Nullable if no device is associated
    }
}