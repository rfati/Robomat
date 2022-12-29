using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Responses
{
    
    public class OdemeAlmaCevap : CardReaderCevap
    {
        public string TranType { get; set; }
        public string AuthRespCode { get; set; }
        public int Amount { get; set; }
        public string AcquirerID { get; set; }
        public int InsCount { get; set; }
        public int BatchNo { get; set; }
        public int Stan { get; set; }
        public string AuthCode { get; set; }
        public string RefNo { get; set; }
        public string TranDate { get; set; }
        public string TranTime { get; set; }
        public string MerchID { get; set; }
        public string TermID { get; set; }
        public string AuthRespMess { get; set; }
        public string InternalErrCode { get; set; }
        public string InternalErrDesc { get; set; }
    }

}
