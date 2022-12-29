using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Device
{
    public class PaymentIslemResult
    {
        /// <summary>
        //0: İşlem  başarılı Tamamlandı
        //1:TimeOut
        //2: işlem başarısız 
        /// </summary>
        public int returnCode { get; set; }  
        public string returnDesc { get; set; }
    }
}
