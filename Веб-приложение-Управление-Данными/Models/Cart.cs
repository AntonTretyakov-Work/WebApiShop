// Models/Cart.cs
namespace MobileStoreApi.Models
{
    public class Cart
    {
        public int IdCart { get; set; }
        public int IdMobileDevice { get; set; }
        public int IdService { get; set; }
        public int? IdAccessory { get; set; } // Nullable if no accessory is associated
    }
}