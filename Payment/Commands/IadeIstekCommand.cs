using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Commands
{
    
    public class IadeIstekCommand : CardReaderCommand
    {
        private string MessageType = "FF8A72";
        private string TranType = "İade";
        private int Amount { get; set; }
        private string AcquirerID { get; set; } //Gönderilmemesi durumunda banka seçimi otomatik yapılır.
        private int InsCount { get; set; } // Taksit sayısı
        private int BatchNo { get; set; }
        private int Stan { get; set; }



        public IadeIstekCommand()
        {
            string cmdStr = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            byte[] cmdBytes = Encoding.ASCII.GetBytes(cmdStr);
            int lenValue = cmdBytes.Length + 2;
            byte[] intBytes = BitConverter.GetBytes(lenValue);
            Array.Reverse(intBytes);
            this.LEN = intBytes;

            command.Add(this.STX);
            this.LRC = this.STX;

            command.AddRange(this.LEN);
            this.LRC ^= this.LEN[0];
            this.LRC ^= this.LEN[1];

            command.AddRange(cmdBytes);
            foreach (byte byteVal in cmdBytes)
            {
                this.LRC ^= byteVal;
            }

            command.Add(this.LRC);
            command.Add(this.ETX);
        }
    }



    public class IadeKaydetIstekCommand : CardReaderCommand
    {

        public IadeKaydetIstekCommand(int BatchNo, int Stan)
        {
            IadeKaydetIstek cmd = new IadeKaydetIstek();
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

    public class IadeKaydetIstek
    {
        public string MessageType = "FF8A7B";
        public string TranType = "İade";
        public string AcquirerID { get; set; } //Gönderilmemesi durumunda banka seçimi otomatik yapılır.
        public int BatchNo { get; set; }
        public int Stan { get; set; }

    }

}
