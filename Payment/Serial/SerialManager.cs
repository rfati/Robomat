using Common;
using Payment.Commands;
using Payment.Device;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Payment.Serial
{
    public class SerialManager : IDisposable
    {

        private SerialPort serialPort;
        public CardReaderPacket serialPacket;
        private BackgroundWorker serialPortBackgroundWorker = null;

        public delegate void DeviceDataReceived(CardReaderPacket packetReceived);
        public event DeviceDataReceived OnDeviceDataReceived;

        public delegate void DeviceDisconnected(string SerialPortName);
        public event DeviceDisconnected OnDeviceDisconnected;

        public delegate void DeviceConnected(string SerialPortName);
        public event DeviceConnected OnDeviceConnected;

        public delegate void DeviceDataErrorReceived(string deviceDataError);
        public event DeviceDataErrorReceived OnDeviceDataErrorReceived;


        private CardReaderPacketStatus PackStatus;


        public SerialManager(string portName, int baudRate = 115200)
        {
            serialPacket = new CardReaderPacket();
            try
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
                    serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPortErrorReceived);

                    serialPortBackgroundWorker = new BackgroundWorker();
                    serialPortBackgroundWorker.DoWork += new DoWorkEventHandler(SerialPortBackgroundWorker_DoWork);
                    serialPortBackgroundWorker.RunWorkerAsync();

                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Pax com connection error: " + ex.Message);
                throw ex;
            }

        }

        void SerialPortBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            while (true)
            {
                try
                {
                    if (serialPortBackgroundWorker == null)
                    {
                        serialPortBackgroundWorker.Dispose();
                        return;
                    }
                    if (!serialPort.IsOpen)
                    {
                        if (OnDeviceDisconnected != null)
                            OnDeviceDisconnected(serialPort.PortName);
                    }
                    else
                    {
                        if (OnDeviceConnected != null)
                            OnDeviceConnected(serialPort.PortName);
                    }
                }
                catch (Exception ex)
                {
                }

                Thread.Sleep(1000);
            }
        }



        void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                lock (this)
                {
                    SerialPort comPort = (SerialPort)sender;
                    if (comPort.IsOpen)
                    {
                        Thread.Sleep(1000);
                        string frameReceived = comPort.ReadExisting();
                        if (OnDeviceDataErrorReceived != null)
                            OnDeviceDataErrorReceived(frameReceived);
                    }
                }

            }
            catch (Exception ex)
            {
                if (OnDeviceDataErrorReceived != null)
                    OnDeviceDataErrorReceived("Exception " + ex.Message);
            }
        }



        public void Notify(CardReaderPacket packet)
        {
            CardReaderPacket notifyPacket = new CardReaderPacket();
            notifyPacket.ETX = packet.ETX;
            notifyPacket.FirstByte = packet.FirstByte;
            notifyPacket.Len0 = packet.Len0;
            notifyPacket.Len1 = packet.Len1;
            notifyPacket.LRC = packet.LRC;
            notifyPacket.PackStatus = packet.PackStatus;
            notifyPacket.RawData.AddRange(packet.RawData);
            notifyPacket.STX = packet.STX;
            if (OnDeviceDataReceived != null)
                OnDeviceDataReceived(notifyPacket);
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int receivedByteLen = serialPort.BytesToRead;
            byte readByte;

            while (receivedByteLen >= 0)
            {
                readByte = (byte)serialPort.ReadByte();

                switch (PackStatus)
                {
                    case CardReaderPacketStatus.Empty:
                        serialPacket.FirstByte = readByte;
                        if (readByte == 0x02)
                        {
                            serialPacket.STX = readByte;
                            this.PackStatus = CardReaderPacketStatus.LEN0_Wait;
                        }
                        else if ((readByte == 0x06) || (readByte == 0x15) || (readByte == 0x04))
                        {
                            this.Notify(serialPacket);
                        }

                        break;

                    case CardReaderPacketStatus.LEN0_Wait:
                        serialPacket.Len0 = readByte;
                        this.PackStatus = CardReaderPacketStatus.LEN1_wait;
                        break;
                    case CardReaderPacketStatus.LEN1_wait:
                        serialPacket.Len1 = readByte;
                        this.PackStatus = CardReaderPacketStatus.Data_Wait;
                        break;
                    case CardReaderPacketStatus.Data_Wait:

                        serialPacket.LRC ^= readByte;
                        int len = 256 * serialPacket.Len0 + serialPacket.Len1;
                        serialPacket.RawData.Add(readByte);
                        if (serialPacket.RawData.Count == (len - 2))
                        {
                            this.PackStatus = CardReaderPacketStatus.LRC_Wait;
                        }
                        else
                        {
                            this.PackStatus = CardReaderPacketStatus.Data_Wait;
                        }

                        break;
                    case CardReaderPacketStatus.LRC_Wait:
                        if (serialPacket.LRC == readByte)
                        {
                            this.PackStatus = CardReaderPacketStatus.ETX_Wait;
                        }
                        else
                        {
                            this.PackStatus = CardReaderPacketStatus.Packet_Fail;
                        }
                        break;

                    case CardReaderPacketStatus.ETX_Wait:
                        this.PackStatus = CardReaderPacketStatus.Packet_Ready;
                        break;

                    default:
                        this.PackStatus = CardReaderPacketStatus.Empty;
                        break;
                }

                if(PackStatus == CardReaderPacketStatus.Packet_Ready)
                {
                    this.Notify(serialPacket);
                    serialPacket.Reset();
                }
                else if (PackStatus == CardReaderPacketStatus.Packet_Fail)
                {
                    serialPacket.Reset();
                }

                receivedByteLen--;
            }

        }


        public bool SendACK()
        {
            byte[] data = { 0x06 };
            return this.SendData(data);

        }

        public bool SendEOT()
        {
            byte[] data = { 0x04 };
            return this.SendData(data);

        }

        public bool SendCommand(CardReaderCommand command)
        {
            byte[] data = command.getData().ToArray();
            return this.SendData(data);

        }

        private bool SendData(byte[] data)
        {
            bool result = false;
            try
            {
                if (serialPort == null)
                    return false;
                if (serialPort.IsOpen)
                {
                    this.serialPort.Write(data, 0, data.Length);
                    result = true;
                }
                else
                {
                    if (OnDeviceDisconnected != null)
                        OnDeviceDisconnected(serialPort.PortName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Pax SerialManager.SendData() exception: " + ex.Message);
                return false;
            }
            return result;
        }


        public void ClosePort()
        {
            try
            {
                if (serialPort == null)
                    return;
                if (serialPortBackgroundWorker != null)
                {
                    serialPortBackgroundWorker.Dispose();
                }
                serialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialPortDataReceived);
                serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPortErrorReceived);
                Thread.Sleep(1000);
                serialPort.Close();
                serialPort.Dispose();
                serialPort = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Dispose()
        {
            ClosePort();
        }


    }
}