// Models/MobileDevice.cs
namespace MobileStoreApi.Models
{
    public class MobileDevice
    {
        public int IdDevice { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public int BuiltInMemory { get; set; }
        public int YearRelease { get; set; }
        public string CountryOfManufacturer { get; set; }
        public string DeviceType { get; set; }
        public decimal DeviceCost { get; set; }
    }
}