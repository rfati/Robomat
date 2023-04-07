using BLL;
using Common;
using Model;
using Newtonsoft.Json;
using Payment;
using Payment.Device;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RobotCafe;
using RobotCafe.Devices;
using RobotCafe.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Manager
{

    public class OtomatManager
    {
        private static OtomatManager _instance;
        private static object syncLock = new object();

        static object Kontrol = new object();
        KapInfo kapinfo = new KapInfo();


        private ModeController _modeController;
        private RobotCafeController _robotCafeController;
        private OdemeController _odemeController;



        private BackgroundWorker deviceConnectionBackgroundWorker = null;

        public bool allConnectionsOK = false;
        private bool isHomingOK = false;
        private Mode _mode;
        private OtomatState _state;

        protected OtomatManager()
        {
        }
        public static OtomatManager GetInstance()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new OtomatManager();
                    }
                }
            }

            return _instance;

        }



        public void Init()
        {
            Logger.LogInfo("OtomatManager--> Init()");

            this._mode = Mode.Idle;
            this._modeController = new ModeController();
            this._odemeController = new OdemeController();
            this._robotCafeController = new RobotCafeController();

        }


        public void Start()
        {
            Logger.LogInfo("OtomatManager--> Starting...");


            this._modeController.Start();
            this._odemeController.Start();
            this._robotCafeController.Start();


            //Thread.Sleep(30000);

            //deviceConnectionBackgroundWorker = new BackgroundWorker();
            //deviceConnectionBackgroundWorker.DoWork += new DoWorkEventHandler(DeviceConnectionBW_DoWork);
            //deviceConnectionBackgroundWorker.RunWorkerAsync();

            //this._modeController.SetMode(Mode.SaleService);
            Logger.LogInfo("OtomatManager--> Started");

            //this.SetMode(Mode.Idle);

        }



        void DeviceConnectionBW_DoWork(object sender, DoWorkEventArgs e)
        {

            while (true)
            {
                try
                {
                    if (deviceConnectionBackgroundWorker == null)
                    {
                        deviceConnectionBackgroundWorker.Dispose();
                        return;
                    }
                    if (_modeController.IsRunning == false)
                    {
                        _modeController.Restart();
                    }
                    if (_odemeController.IsRunning == false)
                    {
                        _odemeController.Restart();
                    }
                    if (_robotCafeController.IsRunning == false)
                    {
                        _robotCafeController.Restart();
                    }

                    this.CheckConnections();

                }
                catch (Exception ex)
                {
                    Logger.LogError("OtomatManager--> DeviceConnectionBW_DoWork() exception : " + ex.Message);
                }

                Thread.Sleep(3000);
            }
        }

        private void CheckConnections()
        {
            lock (Kontrol)
            {
                if (_modeController.IsRunning && _odemeController.IsRunning && _robotCafeController.isCafeUniteCOMConnected && _robotCafeController.isRobotArmConnected)
                {
                    this.allConnectionsOK = true;
                }
                else
                    this.allConnectionsOK = false;
            }

        }

        public void SetMode(Mode mode)
        {
            this._mode = mode;
            if (this._mode == Mode.SaleService)
            {
                int doHoming = _robotCafeController.DoHoming();
                if (doHoming == 0)
                    _robotCafeController.GetReadyToSaleService();
            }
            else if (this._mode == Mode.Bakim)
            {
                int doHoming = _robotCafeController.DoHoming();
            }
            else if (this._mode == Mode.Yukleme)
            {
                int doHoming = _robotCafeController.DoHoming();
            }
            else if (this._mode == Mode.Idle)
            {
                int doHoming = _robotCafeController.DoHoming();
            }
        }

        public Mode GetMode()
        {
            return this._mode;
        }

        public OtomatState GetState()
        {
            if (_robotCafeController.IsReadyToSaleService == true)
                return OtomatState.Normal;
            else
                return OtomatState.OutService;
        }

        public int DoSatisIslem(SaleOrderCommand command)
        {
            Payment.RobomatConfig robomatConfig = JsonConvert.DeserializeObject<Payment.RobomatConfig>(File.ReadAllText(@"C:\Robomat\RobomatConfig\RobomatConfig.json"));
            if (robomatConfig.PaymentSelector.Equals("0"))
            {
                Thread.Sleep(3000);
                return 0;
            }


            decimal price = Decimal.Round(Convert.ToDecimal(command.TotalPrice), 2);
            decimal send_price = price * 100;

            var satisIslemTask = Task.Run(() => _odemeController.DoSatisIslem((int)send_price));
            satisIslemTask.Wait();
            if (satisIslemTask.Result.returnCode != 0)
            {
                return 1;
            }

            return 0;
        }


        public int DoServisIslem(SaleOrderCommand command)
        {

            foreach (var cartItem in command.SaleOrder)
            {
                Product product = ProductServices.GetById(cartItem.ProductId);
                if(product == null)
                {
                    Logger.LogError("product is null");
                    return 1;
                }
                int serviceResult = _robotCafeController.DoServiceCommand(cartItem, product);
                if (serviceResult != 0)
                {
                    return 1;
                }
            }

            return 0;
        }




    }


}
