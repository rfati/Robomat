using Common;
using RobotCafe.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RobotCafe.Serial
{

    public class PacketReceivedEventArgs : EventArgs
    {
        private RTUResponsePacket serialPacket;

        public RTUResponsePacket SerialPacket
        {
            get { return serialPacket; }
            set { serialPacket = value; }
        }
        public PacketReceivedEventArgs(RTUResponsePacket serialPacket)
        {
            SerialPacket = serialPacket;
        }
    }


    //public class SendReceiveData
    //{
    //    public byte[] data;
    //    public int receiveBufferSize;
    //    public int maxResponseWaitTime;

    //    public SendReceiveData(byte[] data, int receiveBufferSize, int maxResponseWaitTime)
    //    {
    //        this.data = data;
    //        this.receiveBufferSize = receiveBufferSize;
    //        this.maxResponseWaitTime = maxResponseWaitTime;
    //    }
    //}
    public class SerialManager
    {
        private string portName;
        private int baudRate;

        //private ConcurrentQueue<SendReceiveData> sendReceiveDataQueue = new ConcurrentQueue<SendReceiveData>();

        private SerialPort serialPort;
        public RTUResponsePacket serialPacket;
        private BackgroundWorker serialPortBackgroundWorker = null;

        private BackgroundWorker sendReceiveBW = null;

        public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs args);
        public event PacketReceivedEventHandler PacketReceived;

        public delegate void DeviceDisconnected(string SerialPortName);
        public event DeviceDisconnected OnDeviceDisconnected;

        public delegate void DeviceConnected(string SerialPortName);
        public event DeviceConnected OnDeviceConnected;

        public delegate void DeviceDataErrorReceived(string deviceDataError);
        public event DeviceDataErrorReceived OnDeviceDataErrorReceived;


        private List<RTUDevice> observers = new List<RTUDevice>();

        public bool portIsOpened = false;

        private object lockobj = new object();

        List<byte> readBuffer = new List<byte>();
        bool still_waiting_resp = false;

        int requiredRecSize;

        public SerialManager(string portName, int baudRate = 9600)
        {

            serialPacket = new RTUResponsePacket();
            this.portName = portName;
            this.baudRate = baudRate;

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
                catch (Exception)
                {
                }
                Thread.Sleep(1000);
            }
        }

        //async void SendReceiveBW_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    bool IsSendData = false;
        //    bool IsReceiveData = false;

        //    while (true)
        //    {

        //        if (sendReceiveDataQueue.TryDequeue(out SendReceiveData result))
        //        {
        //            try
        //            {
        //                IsSendData = this.SendData(result.data);
        //                if (IsSendData == true)
        //                {
        //                    await Task.Delay(10);
        //                    IsReceiveData = await this.GetResponse(result.receiveBufferSize, result.maxResponseWaitTime);
        //                    if (IsReceiveData == false)
        //                    {
        //                        Logger.LogError("receive data hatası...");
        //                    }
        //                }
        //                else
        //                {
        //                    Logger.LogError("Send data hatası...");
        //                }



        //            }
        //            catch
        //            {
        //                Logger.LogError("QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ");
        //            }

        //        }
        //        await Task.Delay(1);
        //    }
        //}


        public bool Open()
        {
            try
            {
                serialPort = new SerialPort(this.portName, this.baudRate);
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPortErrorReceived);
                    serialPort.DataReceived += SerialPortDataReceived;

                    serialPortBackgroundWorker = new BackgroundWorker();
                    serialPortBackgroundWorker.DoWork += new DoWorkEventHandler(SerialPortBackgroundWorker_DoWork);
                    serialPortBackgroundWorker.RunWorkerAsync();


                    //sendReceiveBW = new BackgroundWorker();
                    //sendReceiveBW.DoWork += new DoWorkEventHandler(SendReceiveBW_DoWork);
                    //sendReceiveBW.RunWorkerAsync();

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("connection error: " + ex.Message);
                return false;
            }
        }

        public void Attach(RTUDevice observer)
        {
            observers.Add(observer);
        }
        public void Detach(RTUDevice observer)
        {
            observers.Remove(observer);
        }

        public void Notify(RTUResponsePacket packet)
        {
            RTUResponsePacket clonePacket = new RTUResponsePacket();
            clonePacket.Count = packet.Count;
            clonePacket.counter = packet.counter;
            clonePacket.CRC = new byte[2];
            clonePacket.CRC[0] = packet.CRC[0];
            clonePacket.CRC[1] = packet.CRC[1];
            clonePacket.Data = packet.Data.GetRange(0, packet.Data.Count);
            clonePacket.Frame = packet.Frame.GetRange(0, packet.Frame.Count);
            clonePacket.FunctionCode = packet.FunctionCode;
            clonePacket.PackStatus = packet.PackStatus;
            clonePacket.Slaveaddress = packet.Slaveaddress;

            //if (PacketReceived != null)
            //    PacketReceived(this, new PacketReceivedEventArgs(clonePacket));

            this.serialPacket.Reset();

            foreach (RTUDevice device in observers)
            {
                if (device.slaveAddress == packet.Slaveaddress)
                {
                    device.Update(clonePacket);
                    break;
                }

            }

        }

        //public void SendReceive(byte[] data, int receiveBufferSize, int maxResponseWaitTime)
        //{
        //    lock(lockobj)
        //    {
        //        Logger.LogInfo("Qitem count at enquee: " + sendReceiveDataQueue.Count);
        //        SendReceiveData sendReceiveData = new SendReceiveData(data, receiveBufferSize, maxResponseWaitTime);
        //        sendReceiveDataQueue.Enqueue(sendReceiveData);
        //    }

        //}


        public bool SendReq(byte[] data, int receiveSize)
        {
            this.requiredRecSize = receiveSize;
            return this.SendData(data);
        }

        private async Task<bool> GetResponse(int size, int maxResponseWaitTime)
        {
            await Task.Delay(1);
            List<byte> readBuffer = new List<byte>();
            bool result = false;
            maxResponseWaitTime = 700;
            try
            {
                int readByte = 255;
                int notResponseCounter = 0;
                //do
                //{
                //    if (serialPort.BytesToRead == 0)
                //    {
                //        await Task.Delay(1);
                //        if (notResponseCounter >= maxResponseWaitTime)
                //        {
                //            Logger.LogInfo("maxResponseWaitTime  elapsed");
                //            return false;
                //        }


                //        notResponseCounter++;
                //        continue;
                //    }
                //    else
                //    {
                //        readByte = serialPort.ReadByte();
                //    }



                //}
                //while (readByte == 255);

                while (serialPort.BytesToRead == 0)
                {
                    await Task.Delay(1);
                    if (notResponseCounter >= maxResponseWaitTime)
                    {
                        Logger.LogInfo("maxResponseWaitTime  elapsed");
                        return false;
                    }

                    notResponseCounter++;
                }


                readByte = serialPort.ReadByte();
                readBuffer.Add((byte)(readByte));

                for (int i = 0; i < size - 1; i++)
                {
                    readByte = serialPort.ReadByte();
                    readBuffer.Add((byte)(readByte));

                }
                for (int i = 0; i < readBuffer.Count; i++)
                {
                    this.serialPacket.Parse(readBuffer[i]);
                    if (serialPacket.PackStatus == RTUPackStatus.Packet_Ready)
                    {
                        break;
                    }
                }
                if (serialPacket.PackStatus == RTUPackStatus.Packet_Ready)
                {
                    this.Notify(serialPacket);
                    result = true;
                }
                else
                {
                    Logger.LogError("packet NOT ready");
                    result = false;
                }
                this.serialPacket.Reset();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("Get Response exception: " + ex.Message);
                return false;
            }
        }


        private bool SendData(byte[] data)
        {
            foreach(byte d in data)
            {
                Logger.LogInfo("sendbyte: " + d);
            }
            Logger.LogInfo("--------------------------");
            if (serialPort == null)
                return false;

            bool result = false;
            try
            {

                if (serialPort.IsOpen)
                {
                    this.serialPort.Write(data, 0, data.Length);
                    result = true;
                }
                else
                {
                    Logger.LogError("RobotCafe SerialManager.SendData() serialPort.IsOpen: False");
                    if (OnDeviceDisconnected != null)
                        OnDeviceDisconnected(serialPort.PortName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("RobotCafe SerialManager.SendData() exception: " + ex.Message);
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

                serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPortErrorReceived);
                serialPort.DataReceived -= SerialPortDataReceived;
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


        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (lockobj)
            {
                int receivedByteLen = serialPort.BytesToRead;

                if (receivedByteLen > 0)
                {
                    CollectData(receivedByteLen);
                    if (readBuffer.Count >= requiredRecSize)
                    {
                        ProcessData();
                        readBuffer.Clear();
                    }
                    else
                    {
                        Logger.LogInfo("buffere eklendi: " + receivedByteLen);
                    }

                }


            }




            //lock (lockobj)
            //{
            //    int receivedByteLen = serialPort.BytesToRead;





            //    if (receivedByteLen > 0)
            //    {
            //        if (still_waiting_resp == true)
            //        {
            //            CollectData(receivedByteLen);
            //            if (ProcessData() == true)
            //            {

            //                this.readBuffer.Clear();
            //                still_waiting_resp = false;

            //                this.Notify(serialPacket);

            //            }
            //            else
            //            {
            //                this.serialPacket.Reset();
            //                still_waiting_resp = true;
            //            }
            //        }
            //        else
            //        {
            //            CollectData(receivedByteLen);
            //            if (ProcessData() == true)
            //            {

            //                this.readBuffer.Clear();
            //                still_waiting_resp = false;
            //                this.Notify(serialPacket);
            //            }
            //            else
            //            {
            //                this.serialPacket.Reset();
            //                still_waiting_resp = true;
            //            }
            //        }
            //    }

            //}

        }

        private void CollectData(int receivedByteLen)
        {
            byte readByte;
            while (receivedByteLen > 0)
            {
                readByte = (byte)serialPort.ReadByte();
                Logger.LogInfo("readByte: " + readByte);
                readBuffer.Add((byte)(readByte));
                receivedByteLen--;
            }
            Logger.LogInfo("--------------------------");
        }

        //private bool ProcessData()
        //{
        //    for (int i = 0; i < readBuffer.Count; i++)
        //    {
        //        this.serialPacket.Parse(readBuffer[i]);

        //    }
        //    if (serialPacket.PackStatus == RTUPackStatus.Packet_Ready)
        //    {
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        private void ProcessData()
        {
            for (int i = 0; i < readBuffer.Count; i++)
            {
                this.serialPacket.Parse(readBuffer[i]);

            }
            if (serialPacket.PackStatus == RTUPackStatus.Packet_Ready)
            {
                readBuffer.Clear();
                this.Notify(serialPacket);
            }
            else
            {
                this.serialPacket.Reset();
            }


        }
    }
}