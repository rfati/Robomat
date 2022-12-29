using Common;
using Payment.Commands;
using Payment.Device;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

//namespace Payment.Serial
//{

//    public class SerialManager
//    {
//        private static SerialManager _instance;
//        private static object syncLock = new object();

//        private SerialPort serialPort;
//        public CardReaderPacket serialPacket;
//        byte[] ret = new byte[1000];

//        private PaxDevice observer;

//        protected SerialManager(string portName, int baudRate)
//        {
//            Initialize(portName, baudRate);
//        }

//        public static SerialManager GetInstance(string portName, int baudRate = 115200)
//        {
//            if (_instance == null)
//            {
//                lock (syncLock)
//                {
//                    if (_instance == null)
//                    {
//                        _instance = new SerialManager(portName, baudRate);
//                    }
//                }
//            }

//            return _instance;

//        }

//        public static SerialManager GetInstance()
//        {
//            if (_instance == null)
//            {
//                throw new InvalidOperationException("Singleton not created - use GetInstance(arg1, arg2)");
//            }

//            return _instance;

//        }



//        public void Initialize(string portName, int baudRate)
//        {

//            serialPacket = new CardReaderPacket();
//            serialPort = new SerialPort(portName, baudRate);
//            serialPort.DataReceived += SerialPortDataReceived;
//            try
//            {
//                serialPort.Open();
//            }
//            catch (Exception e)
//            {
//                Logger.LogError("Pax com connection error: " + e.Message);
//            }



//        }


//        public void Attach(PaxDevice observer)
//        {
//            this.observer = observer;
//        }

//        public void Notify(CardReaderPacket packet)
//        {
//            CardReaderPacket notifyPacket = new CardReaderPacket();
//            notifyPacket.ETX = packet.ETX;
//            notifyPacket.FirstByte = packet.FirstByte;
//            notifyPacket.Len0 = packet.Len0;
//            notifyPacket.Len1 = packet.Len1;
//            notifyPacket.LRC = packet.LRC;
//            notifyPacket.PackStatus = packet.PackStatus;
//            notifyPacket.RawData.AddRange(packet.RawData);
//            notifyPacket.STX = packet.STX;
//            this.observer.Update(notifyPacket);
//        }

//        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
//        {
//            Thread.Sleep(10);
//            serialPort.Read(ret, 0, ret.Length);
//            for (int i = 0; i < ret.Length; i++)
//            {
//                this.serialPacket.Parse(ret[i]);
//                if (serialPacket.PackStatus == CardReaderPacketStatus.Packet_Ready)
//                {
//                    break;
//                }
//            }
//            if (serialPacket.PackStatus == CardReaderPacketStatus.Packet_Ready)
//            {
//                this.Notify(serialPacket);
//            }
//            this.serialPacket.Reset();
//        }


//        public void SendACK()
//        {
//            byte[] data = { 0x06 };
//            this.serialPort.Write(data, 0, data.Length);

//        }

//        public void SendEOT()
//        {
//            byte[] data = { 0x04 };
//            this.serialPort.Write(data, 0, data.Length);

//        }

//        public void SendCommand(CardReaderCommand command)
//        {
//            byte[] data = command.getData().ToArray();
//            this.serialPort.Write(data, 0, data.Length);

//        }




//    }
//}