// Models/Order.cs
namespace ModelsApiOrder.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        public int IdClient { get; set; }
        public int IdEmployee { get; set; }
        public int IdCart { get; set; }
    }

    public class OrderDetails
    {
        public int IdOrder { get; set; }
        public string ClientLastName { get; set; }
        public string ClientFirstName { get; set; }
        public string MobileModel { get; set; }
        public decimal ServiceCost { get; set; }
        public string AccessoryName { get; set; }
    }

}