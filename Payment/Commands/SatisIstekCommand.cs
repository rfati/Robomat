using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Commands
{
    
    public class SatisIstekCommand : CardReaderCommand
    {

        public SatisIstekCommand(int price)
        {
            SatisIstek cmd = new SatisIstek();
            cmd.Amount = price;
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

    public class SatisIstek
    {
        public string MessageType = "FF8A72";
        public string TranType = "Satis";
        public int Amount { get; set; }
        private string AcquirerID { get; set; }  //Gönderilmemesi durumunda banka seçimi otomatik yapılır.

    }
    public class SatisKaydetIstekCommand : CardReaderCommand
    {

        public SatisKaydetIstekCommand(int BatchNo, int Stan, int SaveStatus)
        {
            SatisKaydetIstek cmd = new SatisKaydetIstek();
            cmd.BatchNo = BatchNo;
            cmd.Stan = Stan;
            cmd.AcquirerID = "";
            cmd.TranType = 1;
            cmd.SaveStatus = SaveStatus;
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

    public class SatisKaydetIstek
    {
        public string MessageType = "FF8A7B";
        public int BatchNo { get; set; }
        public int Stan { get; set; }
        public string AcquirerID { get; set; } //Gönderilmemesi durumunda banka seçimi otomatik yapılır.
        //public string TranType = "Satis";
        public int TranType { get; set; }
        public int SaveStatus { get; set; }  //Kaydetme durumu. 1 ise işlem batch’e kaydedilir, 0 ise kaydedilmez

    }
}
