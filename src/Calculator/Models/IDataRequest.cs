namespace Calculator3.Models
{
    public interface IDataRequest
    {
        public double Amount { get; set; }
        public int Term { get; set; }
        public double Rate { get; set; }
        public Constants.Constants.TimeFrequency TimeUnit { get; set; }
    }
}
