using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Commands
{
    public class CardReaderCommand
    {
        protected byte STX = 0x02;
        protected byte[] LEN;
        protected byte LRC;
        protected byte ETX = 0x03;

        protected List<byte> command = new List<byte>();

        public List<byte> getData()
        {
            return command;
        }


    }
}
