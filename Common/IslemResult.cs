
namespace Common
{
    public class ServiceIslemResult
    {
        public int paymentReturnCode { get; set; }
        public string paymentReturnDesc { get; set; }
        public int serviceReturnCode { get; set; }
        public string serviceReturnDesc { get; set; }
        public int parsingReturnCode { get; set; }
        public string parsingReturnDesc { get; set; }
        public int stockReturnCode { get; set; }
        public string stockReturnDesc { get; set; }
    }
}
