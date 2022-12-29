using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class RafBolme
    {
        public int RafBolmeId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int RafNo { get; set; }
        public int BolmeNo { get; set; }
        public int StockAdet { get; set; }
        public byte MotorSlaveAddress { get; set; }
        public int MotorRegNo { get; set; }

    }
}
