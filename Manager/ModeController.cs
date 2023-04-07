using Common;
using Newtonsoft.Json;
using Payment.Commands;
using Payment.Device;
using Payment.Responses;
using Payment.Serial;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Manager
{
    public class ModeController
    {
        private Publisher publisher;

        private RobomatConfig robomatConfig;
        private SerialManager serialManager;

        public bool IsModeSelectorConnected = false;
        public bool IsModeConsumerConnected = false;
        public MessageQueueConsumer modeConsumer;

        public bool IsRunning = false;
        public ModeController()
        {
        }
        public void Start()
        {
            try
            {
                this.robomatConfig = JsonConvert.DeserializeObject<RobomatConfig>(File.ReadAllText(@"C:\Robomat\RobomatConfig\RobomatConfig.json"));
                bool isPaxConnected = this.Connect(this.robomatConfig.ModeSelector_port);
                if(isPaxConnected)
                {
                    Logger.LogInfo("ModeController connected....");
                    this.serialManager.OnDeviceDisconnected += ModeSelectorDevice_OnDeviceDisconnected;
                    this.serialManager.OnDeviceConnected += ModeSelectorDevice_OnDeviceConnected;
                    this.serialManager.OnDeviceDataReceived += ModeSelectorDevice_OnDeviceDataReceived;
                    this.IsModeSelectorConnected = true;
                }
                else
                {
                    Logger.LogError("ModeController NOT  connected....");
                    this.IsModeSelectorConnected = false;
                }

                //this.modeConsumer = new MessageQueueConsumer("ModeSelectorResponseQueue");
                //this.modeConsumer.Start();
                //if (this.modeConsumer.connection.IsOpen == true)
                //{
                //    this.modeConsumer.connection.ConnectionShutdown += ModeConsumer_OnDeviceDisconnected;
                //    this.modeConsumer.consumer.Received += this.HandleModeConsumerResponseMessage;
                //    this.IsModeConsumerConnected = true;
                //}
                    


                if(this.IsModeSelectorConnected)
                {
                    this.IsRunning = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ModeController NOT  connected....exception:  " + ex.Message);
                this.IsModeSelectorConnected = false;
            }

        }


        public void Restart()
        {
            Logger.LogInfo("ModeController--->Restart()");

            if(this.IsModeSelectorConnected == false)
            {
                if (this.serialManager != null)
                {
                    this.serialManager.OnDeviceDisconnected -= ModeSelectorDevice_OnDeviceDisconnected;
                    this.serialManager.OnDeviceConnected -= ModeSelectorDevice_OnDeviceConnected;
                    this.serialManager.OnDeviceDataReceived -= ModeSelectorDevice_OnDeviceDataReceived;
                }
                this.Dispose();
                bool isModeSelectorConnected = this.Connect(this.robomatConfig.ModeSelector_port);
                if (isModeSelectorConnected)
                {
                    this.serialManager.OnDeviceDisconnected += ModeSelectorDevice_OnDeviceDisconnected;
                    this.serialManager.OnDeviceConnected += ModeSelectorDevice_OnDeviceConnected;
                    this.serialManager.OnDeviceDataReceived += ModeSelectorDevice_OnDeviceDataReceived;
                }

            }

            if (this.IsModeConsumerConnected == false)
            {
                if (this.modeConsumer.consumer != null)
                {
                    this.modeConsumer.connection.ConnectionShutdown -= ModeConsumer_OnDeviceDisconnected;
                    this.modeConsumer.consumer.Received -= this.HandleModeConsumerResponseMessage;
                }
                modeConsumer.Restart();
                if (this.modeConsumer.connection.IsOpen == true)
                {
                    this.modeConsumer.connection.ConnectionShutdown += ModeConsumer_OnDeviceDisconnected;
                    this.modeConsumer.consumer.Received += this.HandleModeConsumerResponseMessage;
                    this.IsModeConsumerConnected = true;
                }
            }
        }

        private bool Connect(string portName)
        {
            try
            {
                this.serialManager = new SerialManager(portName: portName, baudRate: 9600);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Dispose()
        {
            if (this.serialManager != null)
            {
                this.serialManager.Dispose();
                this.serialManager = null;
            }

        }



        private void ModeSelectorDevice_OnDeviceConnected(string SerialPortName)
        {
            this.IsModeSelectorConnected = true;
        }

        private void ModeSelectorDevice_OnDeviceDisconnected(string SerialPortName)
        {
            this.IsModeSelectorConnected = false;
            this.IsRunning = false;
        }


        private void ModeConsumer_OnDeviceDisconnected(object sender, ShutdownEventArgs e)
        {
            this.IsModeConsumerConnected = false;
            this.IsRunning = false;
        }


        private void ModeSelectorDevice_OnDeviceDataReceived(string receivedStr)
        {
            Logger.LogInfo("Mode Selector data input received: " + receivedStr);
            string[] values = receivedStr.Split(':');
            Logger.LogInfo("value 0 -" + values[0]);
            Logger.LogInfo("value 1 -" + values[1]);


            if (values[0].Equals("Button value"))
            {
                if (values[1].StartsWith("4"))
                {
                    Logger.LogInfo("Yükleme modunda");
                    this.SetMode(Mode.Yukleme);
                }
                else if (values[1].StartsWith("2"))
                {
                    Logger.LogInfo("Bakım modunda");
                    this.SetMode(Mode.Bakim);
                }
                else if (values[1].StartsWith("1"))
                {
                    Logger.LogInfo("Satış modunda");
                    this.SetMode(Mode.SaleService);
                }
                else
                {

                    Logger.LogInfo("yanlış komut değeri: ");

                }
            }
            else
            {
                Logger.LogInfo("ali bircan diyorki yok artık");
            }

        }


        public async void SetMode(Mode mode)
        {
            OtomatManager.GetInstance().SetMode(mode);

            RobomatStatus robomatStatus = new RobomatStatus();
            robomatStatus.mode = mode;
            string jsonMessage = JsonConvert.SerializeObject(robomatStatus);
            publisher = new Publisher(queueName: "RobomatServiceQueue", message: jsonMessage);

            Thread.Sleep(2000);
        }


        private void HandleModeConsumerResponseMessage(object sender, BasicDeliverEventArgs e)
        {
            string str = Encoding.Default.GetString(e.Body.ToArray());
            //var task = Task.Run(() => HandleServiceMessageData(str));
            //task.Wait();
        }


    }
}