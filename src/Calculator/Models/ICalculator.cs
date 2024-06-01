namespace Calculator3.Models
{
    internal interface ICalculator
    {
        public IDataResponse Calculate(IDataRequest request);
    }
}
