using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Commands
{
    
    public class IptalIstekCommand : CardReaderCommand
    {

        public IptalIstekCommand(int price, int stan, int batchno, string refNo)
        {
            IptalIstek cmd = new IptalIstek();
            cmd.Amount = price;
            cmd.BatchNo = batchno;
            cmd.Stan = stan;
            cmd.RefNo = refNo;
            string cmdStr = Newtonsoft.Json.JsonConvert.SerializeObject(cmd);
            byte[] cmdBytes = Encoding.ASCII.GetBytes(cmdStr);
            int lenValue = cmdBytes.Length + 2;
            byte len1 = (byte)(lenValue % 256);
            byte len0 = (byte)(lenValue / 256);

            command.Add(this.STX);
            command.Add(len0);
            command.Add(len1);
            command.AddRange(cmdBytes);
            foreach (byte byteVal in cmdBytes)
            {
                this.LRC ^= byteVal;
            }

            command.Add(this.LRC);
            command.Add(this.ETX);
        }
    }


    public class IptalIstek
    {
        public string MessageType = "FF8A24";
        public string TranType = "Iptal";
        public int Amount { get; set; }
        private string AcquirerID { get; set; } //Gönderilmemesi durumunda banka seçimi otomatik yapılır.
        public int BatchNo { get; set; }
        public int Stan { get; set; }
        public string RefNo { get; set; }
    }



    public class IptalKaydetIstekCommand : CardReaderCommand
    {

        public IptalKaydetIstekCommand(int BatchNo, int Stan)
        {
            IptalKaydetIstek cmd = new IptalKaydetIstek();
            cmd.BatchNo = BatchNo;
            cmd.Stan = Stan;
            string cmdStr = Newtonsoft.Json.JsonConvert.SerializeObject(cmd);
            byte[] cmdBytes = Encoding.ASCII.GetBytes(cmdStr);
            int lenValue = cmdBytes.Length + 2;
            byte len1 = (byte)(lenValue % 256);
            byte len0 = (byte)(lenValue / 256);

            command.Add(this.STX);
            command.Add(len0);
            command.Add(len1);
            command.AddRange(cmdBytes);
            foreach (byte byteVal in cmdBytes)
            {
                this.LRC ^= byteVal;
            }

            command.Add(this.LRC);
            command.Add(this.ETX);
        }
    }

    public class IptalKaydetIstek
    {
        public string MessageType = "FF8A7B";
        public string TranType = "İptal";
        public string AcquirerID { get; set; } //Gönderilmemesi durumunda banka seçimi otomatik yapılır.
        public int BatchNo { get; set; }
        public int Stan { get; set; }

    }

}
