namespace MasterLabel.Models
{
    public class ReportData
    {
        public string SerialNumber { get; set; }
        public string Job { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public string OrderNumber { get; set; }
        public string OrderLine { get; set; }
        public string LPN { get; set; }
        public string TagNumber { get; set; }
        public string ShipCode { get; set; }
        public string IRNO { get; set; }
        public string Subinv { get; set; }
        public DateTime? Date { get; set; }
        public string Address { get; set; }
    }
}
