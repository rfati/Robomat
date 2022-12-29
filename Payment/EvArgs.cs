using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment
{
    public class PaymentEventArgs : EventArgs
    {
        public OdemeControllerNotifyEvent eventData { get; set; }
        public CommandType commandType { get; set; }
    }
}

