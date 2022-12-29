using Payment.Commands;
using System;
using System.IO.Ports;

namespace Payment.Serial
{
    public class SerialCardReader
    {

        private SerialPort serialPort;
        public CardReaderPacket serialPacket;
        public byte[] ret = new byte[1000];

        public event EventHandler serialPacketReadyEvent;

        public SerialCardReader(string portName, int baudRate = 115200)
        {
            //serialPacket = new SerialPacket();
            serialPort = new SerialPort(portName, baudRate);
            serialPort.DataReceived += SerialPortDataReceived;

            serialPort.Open();
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int byteLen = serialPort.BytesToRead;
            if(byteLen > 0)
            {
                for(int i=0; i< byteLen; i++)
                {
                    int data = serialPort.ReadByte();
                    this.serialPacket.Parse((byte)data);
                    if (this.serialPacket.PackStatus == CardReaderPacketStatus.Packet_Ready)
                    {
                        break;
                    }
                }
                serialPacketReadyEvent.Invoke(null, EventArgs.Empty);
                //serialPacketReadyEvent.Invoke(this, e);
            }

        }

        public void SendACK()
        {
            byte[] data = { 0x06 };
            this.serialPort.Write(data, 0, data.Length);

        }

        public void SendEOT()
        {
            byte[] data = { 0x04 };
            this.serialPort.Write(data, 0, data.Length);

        }

        public void SendCommand(CardReaderCommand command)
        {
            byte[] data = command.getData().ToArray();
            this.serialPort.Write(data, 0, data.Length);

        }

        public void SendStringCommand(string strCommand)
        {
            byte[] command = System.Text.Encoding.UTF8.GetBytes(strCommand);
            this.serialPort.Write(command, 0, command.Length);
        }



    }
}