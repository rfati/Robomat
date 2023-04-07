using Common;
using Newtonsoft.Json;
using Payment.Commands;
using Payment.Device;
using Payment.Responses;
using Payment.Serial;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Payment
{
    public class OdemeController
    {


        private PaxDevice paxDevice;
        private RobomatConfig robomatConfig;

        public bool IsRunning = false;


        public OdemeController()
        {
        }
        public void Start()
        {
            try
            {
                this.robomatConfig = JsonConvert.DeserializeObject<RobomatConfig>(File.ReadAllText(@"C:\Robomat\RobomatConfig\RobomatConfig.json"));
                this.paxDevice = new PaxDevice();
                bool isPaxConnected = this.paxDevice.Connect(this.robomatConfig.IM20_port);
                if(isPaxConnected)
                {
                    this.paxDevice.serialManager.OnDeviceDisconnected += PaxDevice_OnDeviceDisconnected;
                    this.paxDevice.serialManager.OnDeviceConnected += PaxDevice_OnDeviceConnected;
                    this.IsRunning = true;
                }
                else
                {
                    this.IsRunning = false;
                }

            }
            catch(Exception ex)
            {
                this.IsRunning = false;
                Logger.LogError("OdemeController--> Start() exception : " + ex.Message);
            }

        }

        public void Restart()
        {
            Logger.LogInfo("OdemeController--->Restart()");

            if (this.paxDevice.serialManager != null)
            {
                this.paxDevice.serialManager.OnDeviceDisconnected -= PaxDevice_OnDeviceDisconnected;
                this.paxDevice.serialManager.OnDeviceConnected -= PaxDevice_OnDeviceConnected;
            }
            this.paxDevice.Dispose();
            bool isPaxConnected = this.paxDevice.Connect(this.robomatConfig.IM20_port);
            if (isPaxConnected)
            {
                this.paxDevice.serialManager.OnDeviceDisconnected += PaxDevice_OnDeviceDisconnected;
                this.paxDevice.serialManager.OnDeviceConnected += PaxDevice_OnDeviceConnected;
            }
        }

        private void PaxDevice_OnDeviceConnected(string SerialPortName)
        {
            this.IsRunning = true;
        }

        private void PaxDevice_OnDeviceDisconnected(string SerialPortName)
        {
            this.IsRunning = false;
        }

        public async Task<PaymentIslemResult> DoSatisIslem(int price)
        {
            //if (paxDevice.GetState() == PaxState.Idle)
            //{
            //    PaymentIslemResult ret = await paxDevice.SendSatisIstekCommand(price);
            //    paxDevice.SetIdle();

            //    return ret;

            //    //await Task.Delay(3000);
            //    //PaymentIslemResult retIptal = await paxDevice.SendIptalIstekCommand(price);
            //    //return retIptal;
            //}
            //else
            //{
            //    PaymentIslemResult ret = new PaymentIslemResult();
            //    ret.returnCode = 100;
            //    ret.returnDesc = "PAX Device islem devam ediyor. Yeni islem başarısız";
            //    return ret;
            //}

            PaymentIslemResult ret = await paxDevice.SendSatisIstekCommand(price);
            //paxDevice.SetIdle();

            return ret;

        }


    }
}