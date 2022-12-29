using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Responses
{
    
    public class DurumBilgilendirmeCevap : CardReaderCevap
    {
        public int AppstateInfo { get; set; }
        public string AppStateDesc { get; set; }
    }

}
