using Newtonsoft.Json;
using Payment.Commands;
using Payment.Device;
using Payment.Responses;
using Payment.Serial;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Payment
{
    //public class OdemeController
    //{
    //    static AutoResetEvent _AREvt;
    //    public PaxDevice paxDevice;
    //    public SerialManager serialDeviceManager;

    //    private RobomatConfig robomatConfig;


    //    private bool IsHomingOK = false;
    //    public bool IsOdemeControllerReady = false;
    //    private bool PreviousOdemeControllerReadyFlag = false;


    //    public OdemeController()
    //    {
    //        _AREvt = new AutoResetEvent(false);
    //        this.robomatConfig = JsonConvert.DeserializeObject<RobomatConfig>(File.ReadAllText(@"C:\Robomat\RobomatConfig\RobomatConfig.json"));
    //        serialDeviceManager = new SerialManager(portName: this.robomatConfig.IM20_port);

    //        this.paxDevice = new PaxDevice(serialDeviceManager);
    //        serialDeviceManager.Attach(this.paxDevice);

    //    }

    //    public PaymentIslemResult DoSatisIslem(int price)
    //    {

    //        var satisIslemTask = Task.Run(() => paxDevice.SendSatisIstekCommand(price));
    //        satisIslemTask.Wait();
    //        return satisIslemTask.Result;

    //    }

    //    //public int DoGunSonuIslem()
    //    //{

    //    //    var satisIslemTask = Task.Run(() => paxDevice.SendGunSonuCommand());
    //    //    satisIslemTask.Wait();
    //    //    return satisIslemTask.Result;


    //    //}


    //}
}