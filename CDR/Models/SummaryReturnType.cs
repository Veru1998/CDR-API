namespace CDR.Models
{
    public class SummaryReturnType
    {
        public int TotalCalls { get; set; }
        public decimal TotalCost { get; set; }
        public double AverageDuration { get; set; }
        public string MostFrequentRecipient { get; set; }
        public string MostFrequentCaller { get; set; }
    }
}
