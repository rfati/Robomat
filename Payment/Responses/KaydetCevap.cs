using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Responses
{
    
    public class KaydetCevap : CardReaderCevap
    {
        public string InternalErrCode { get; set; }
        public string InternalErrDesc { get; set; }
    }

}
