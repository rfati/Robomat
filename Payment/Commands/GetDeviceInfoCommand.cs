using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Commands
{
    
    public class GetDeviceInfoCommand : CardReaderCommand
    {
        public GetDeviceInfoCommand()
        {
            string cmdStr = Newtonsoft.Json.JsonConvert.SerializeObject(new GetDeviceInfo());
            byte[] cmdBytes = Encoding.ASCII.GetBytes(cmdStr);
            int lenValue = cmdBytes.Length + 2;
            byte len1 = (byte)(lenValue % 256);
            byte len0 = (byte)(lenValue / 256);

            command.Add(this.STX);
            command.Add(len0);
            command.Add(len1);
            command.AddRange(cmdBytes);
            foreach(byte byteVal in cmdBytes)
            {
                this.LRC ^= byteVal;
            }

            command.Add(this.LRC);
            command.Add(this.ETX);
        }
    }


    public class GetDeviceInfo
    {
        public string MessageType = "FF8A60";
    }
}
