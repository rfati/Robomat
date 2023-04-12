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
    public enum State
    {
        Idle,
        Data_Sending,
        Data_Sent,
        Data_Receiving,
        Data_Received,
        TimeOut
    }
    public class TXRXData
    {
        public TXRXData()
        {

        }
        public byte SlaveAddress;
        public byte[] data;
        public int receiveSize;
    }


    public class SerialManager
    {
       
        public string portName;
        private int baudRate;
        object lockd = new object();

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

        List<byte> readBuffer = new List<byte>();

        ConcurrentQueue<TXRXData> transmitQueue = new ConcurrentQueue<TXRXData>();

        private State state;
        public SerialManager(string portName, int baudRate = 9600)
        {

            serialPacket = new RTUResponsePacket();
            this.portName = portName;
            this.baudRate = baudRate;
            this.state = State.Idle;

        }

        private void ResponseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.state = State.TimeOut;
            Logger.LogInfo("Serial Manager ResponseTimer_Elapsed.....TimeOut");

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


        public bool Open()
        {
            try
            {
                serialPort = new SerialPort(this.portName, this.baudRate);
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPortErrorReceived);

                    serialPortBackgroundWorker = new BackgroundWorker();
                    serialPortBackgroundWorker.DoWork += new DoWorkEventHandler(SerialPortBackgroundWorker_DoWork);
                    serialPortBackgroundWorker.RunWorkerAsync();


                    sendReceiveBW = new BackgroundWorker();
                    sendReceiveBW.DoWork += new DoWorkEventHandler(SendReceiveBW_DoWork);
                    sendReceiveBW.RunWorkerAsync();

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

        private void SendReceiveBW_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Timers.Timer ResponseTimer = new System.Timers.Timer();
                int ResponseTimeOutInterval = 2000;

                ResponseTimer.Interval = ResponseTimeOutInterval;
                ResponseTimer.Elapsed += ResponseTimer_Elapsed;
                ResponseTimer.AutoReset = false;
                ResponseTimer.Enabled = false;



                Logger.LogInfo("sendReceiveBW  running.......");
                while (true)
                {
                    this.state = State.Idle;
                    if (sendReceiveBW == null)
                    {
                        Logger.LogInfo("sendReceiveBW  disposing");
                        sendReceiveBW.Dispose();
                        return;
                    }

                    if (this.transmitQueue.Count > 0)
                    {

                        bool isDequed = this.transmitQueue.TryDequeue(out TXRXData txRXdata);


                        int size = txRXdata.receiveSize;
                        if (isDequed)
                        {
                            bool sent = this.SendData(txRXdata.data);
                            if (sent)
                            {
                                Thread.Sleep(50);
                                this.state = State.Data_Sent;

                                Logger.LogInfo("txrxdata Sent slaveAddress: " + txRXdata.SlaveAddress);

                                ResponseTimer.Enabled = true;

                                while (size > 0)
                                {
                                    if (this.state == State.TimeOut)
                                    {
                                        Logger.LogInfo("Timer elapsed ama data gelmedi..");
                                        break;
                                    }
                                    try
                                    {
                                        if(serialPort.BytesToRead > 0)
                                        {
                                            int readInt = serialPort.ReadByte();
                                            if (readInt != -1)
                                            {
                                                this.state = State.Data_Receiving;
                                                ResponseTimer.Enabled = false;
                                                readBuffer.Add((byte)(readInt));
                                                size--;
                                            }
                                            else
                                            {
                                                Logger.LogInfo("-1 int değeri okundu");
                                            }
                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogError("Readbyte exception ex: " + ex.Message);
                                    }

                                    if(size == 0)
                                    {
                                        this.state = State.Data_Received;
                                    }

                                }

                                if (this.state == State.Data_Received)
                                {
                                    Logger.LogInfo("process data startinggg......");
                                    ProcessData();
                                    Logger.LogInfo("process data finish......");
                                    readBuffer.Clear();

                                }
                                else
                                {
                                    Logger.LogInfo("ProcessNullData startinggg......");
                                    ProcessNullData(txRXdata.SlaveAddress);
                                    Logger.LogInfo("ProcessNullData finish......");
                                }

                            }
                            else
                            {
                                Logger.LogInfo("send data error......");
                            }

                        }
                        else
                        {
                            Logger.LogError("Transmit que dequee error...");
                        }

                    }
                    else
                    {
                        Thread.Sleep(1);
                    }

                }
            }
            catch(Exception ex)
            {
                Logger.LogError("SendReceiveBW_DoWork exception ex: " + ex.Message);
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



        public bool SendReq(TXRXData data)
        {
            try
            {
                this.transmitQueue.Enqueue(data);
                Logger.LogInfo("Enquee data slaveAddres: " + data.SlaveAddress);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError("Serial Manager--SendReq(TXRXData data)  exception: " + e.Message);
                return false;
            }
        }


        private bool SendData(byte[] data)
        {
            bool result = false;
            try
            {
                this.serialPort.Write(data, 0, data.Length);
                result = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("RobotCafe SerialManager.SendData() exception: " + ex.Message);
                if (OnDeviceDisconnected != null)
                    OnDeviceDisconnected(serialPort.PortName);
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
                if (sendReceiveBW != null)
                {
                    sendReceiveBW.Dispose();
                }

                serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPortErrorReceived);
                //serialPort.DataReceived -= SerialPortDataReceived;
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




        private void ProcessData()
        {
            try
            {
                for (int i = 0; i < readBuffer.Count; i++)
                {
                    this.serialPacket.Parse(readBuffer[i]);

                }
            }
            catch(Exception e)
            {
                Logger.LogError("serialPacke parsing exception: " + e.Message);
            }


            if (serialPacket.PackStatus == RTUPackStatus.Packet_Ready)
            {
                readBuffer.Clear();
                this.Notify(serialPacket);
            }
            else
            {
                Logger.LogError("serialPacket.PackStatus != RTUPackStatus.Packet_Ready");
                this.serialPacket.Reset();
            }


        }

        private void ProcessNullData(byte slaveAddress)
        {

            foreach (RTUDevice device in observers)
            {
                if (device.slaveAddress == slaveAddress)
                {
                    device.Update(null);
                    break;
                }

            }
        }

    }
}