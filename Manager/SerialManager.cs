using Common;
using Payment.Device;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Manager
{
    public class SerialManager : IDisposable
    {

        private SerialPort serialPort;
        private BackgroundWorker serialPortBackgroundWorker = null;

        public delegate void DeviceDataReceived(byte command);
        public event DeviceDataReceived OnDeviceDataReceived;

        public delegate void DeviceDisconnected(string SerialPortName);
        public event DeviceDisconnected OnDeviceDisconnected;

        public delegate void DeviceConnected(string SerialPortName);
        public event DeviceConnected OnDeviceConnected;

        public delegate void DeviceDataErrorReceived(string deviceDataError);
        public event DeviceDataErrorReceived OnDeviceDataErrorReceived;


        public SerialManager(string portName, int baudRate = 115200)
        {
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



        public void Notify(byte command)
        {
            if (OnDeviceDataReceived != null)
                OnDeviceDataReceived(command);
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int receivedByteLen = serialPort.BytesToRead;
            byte readByte;

            if(receivedByteLen == 1)
            {
                readByte = (byte)serialPort.ReadByte();
                this.Notify(readByte);
            }

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