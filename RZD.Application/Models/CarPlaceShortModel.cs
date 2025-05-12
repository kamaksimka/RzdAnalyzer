namespace RZD.Application.Models
{
    public class CarPlaceShortModel
    {
        public long Id { get; set; }
        public string? CarType { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        public bool IsFree { get; set; }

    }
}
