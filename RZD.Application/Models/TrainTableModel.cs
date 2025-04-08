namespace RZD.Application.Models
{
    public class TrainTableModel
    {
        public long Id { get; set; }

        public DateTimeOffset ArrivalDateTime { get; set; }
        public DateTimeOffset DepartureDateTime { get; set; }
        public List<string> CarServices { get; set; }
        public string TrainNumber { get; set; }
        public decimal TripDuration { get; set; }

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}
