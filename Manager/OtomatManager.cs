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

            this._modeController = new ModeController();
            this._odemeController = new OdemeController();
            this._robotCafeController = new RobotCafeController();

            this.SetMode(Mode.Idle);

        }

        public void Start()
        {
            //this._modeController.Start();
            this._odemeController.Start();
            this._robotCafeController.Start();

            //Thread.Sleep(30000);

            //deviceConnectionBackgroundWorker = new BackgroundWorker();
            //deviceConnectionBackgroundWorker.DoWork += new DoWorkEventHandler(DeviceConnectionBW_DoWork);
            //deviceConnectionBackgroundWorker.RunWorkerAsync();

            //this._modeController.SetMode(Mode.SaleService);

        }

        public int DoHoming()
        {
            return _robotCafeController.DoHoming();
        }

        public int GetReadyToSaleService()
        {
            return _robotCafeController.GetReadyToSaleService();
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
                if (_modeController.IsRunning && _odemeController.IsRunning && _robotCafeController.isCafeUniteCOMConnected && _robotCafeController.isKiskacUniteCOMConnected && _robotCafeController.isRobotArmConnected)
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
                int serviceResult = _robotCafeController.DoServiceCommand(cartItem, product);
                if (serviceResult != 0)
                {
                    return 1;
                }
            }

            return 0;
        }

        public int DoServisIslemTest()
        {
            Console.WriteLine("4.   Domates Çorbası");
            Console.WriteLine("6.   Yayla Çorbası");
            Console.WriteLine("10.  Siyezli Ezogelin Çorbası");
            Console.WriteLine("12.  Mercimek Çorbası");
            Console.WriteLine("13.  Kremalı Mantar Çorbası");
            Console.WriteLine("38.  Kayısı Hoşafı");
            Console.WriteLine("39.  Etli Nohut");
            Console.WriteLine("40.  Kuru Fasulye");

            string s = Console.ReadLine();

            Product product = ProductServices.GetById((Convert.ToInt32(s)));
            //Product product = new Product();

            int serviceResult = _robotCafeController.DoServiceCommandTest(null, product);
            if (serviceResult != 0)
            {
                return 1;
            }

            return 0;
        }


        public int DoTest(int selectedMethod)
        {
            Task<int> testTask = null;

            if (selectedMethod == 1)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBardakHazirlamaTest(bardakNo: 0));
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }

            if (selectedMethod == 888)
            {
                testTask = Task.Run(() => _robotCafeController.testService.boyacitesti());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }


            if (selectedMethod == 2)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBardakKapakHazirlamaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 3)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKaseHazirlamaTest(kaseNo: 2));
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 4)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKaseKapakHazirlamaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 5)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBardakYerlestirmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 6)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKaseYerlestirmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }

            if (selectedMethod == 1001)
            {

                testTask = Task.Run(() => _robotCafeController.testService.DoUrunAlmaTest());

                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoKesmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoBosaltmaBardakTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoCopAtmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }


            }

            if (selectedMethod == 1002)
            {

                testTask = Task.Run(() => _robotCafeController.testService.DoUrunAlmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoKesmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoBosaltmaKaseTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoCopAtmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }


            }


            if (selectedMethod == 33)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoIsitmaBardakTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }

            if (selectedMethod == 34)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoIsitmaKaseTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }




            if (selectedMethod == 7)
            {
                for (int i = 0; i < 1; i++)
                {
                    testTask = Task.Run(() => _robotCafeController.testService.DoUrunAlmaTest());
                    testTask.Wait();
                    if (testTask.Result != 0)
                    {
                        return 1;
                    }
                }
            }

            if (selectedMethod == 333)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoUrunAlmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 5005)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKaseYerlestirmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoUrunAlmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoKesmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoBosaltmaKaseTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoCopAtmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

                testTask = Task.Run(() => _robotCafeController.testService.DoIsitmaKaseTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }



            }

            if (selectedMethod == 8)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKesmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            //if (selectedMethod == 9)
            //{
            //    testTask = Task.Run(() => _robotCafeController.testService.DoIsitmaTest(ambalajType: AmbalajType.Size_12));
            //    testTask.Wait();
            //    if (testTask.Result != 0)
            //    {
            //        return 1;
            //    }

            //}
            if (selectedMethod == 10)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBosaltmaBardakTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 11)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoCopAtmaTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }

            }
            if (selectedMethod == 12)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBardakKapakYerlestirmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 13)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKapKapatmaTest(kapType: KapType.Bardak));
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 14)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBardakSunumTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 15)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKaseKapakYerlestirmeTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 16)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoKaseSunumTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 17)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoServisSetiHazirlamaTest(setNo: 0));
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 18)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoBosaltmaKaseTest());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 19)
            {
                for (int i = 0; i < 1; i++)
                {
                    Product product = ProductServices.GetById(4);
                    testTask = Task.Run(() => _robotCafeController.testService.OtomatServisYap(product));
                    testTask.Wait();
                    if (testTask.Result != 0)
                    {
                        return 1;
                    }
                }
            }
            if (selectedMethod == 2222)
            {
                Product product = ProductServices.GetById(4);

                for (int i = 0; i < 50; i++)
                {
                    testTask = Task.Run(() => _robotCafeController.testService.OtomatServisYap(product));
                    testTask.Wait();
                    if (testTask.Result != 0)
                    {
                        return 1;
                    }

                    testTask = Task.Run(() => _robotCafeController.testService.OtomatGetReadyToService());
                    testTask.Wait();
                    if (testTask.Result != 0)
                    {
                        return 1;
                    }
                }


            }
            if (selectedMethod == 100)
            {
                testTask = Task.Run(() => _robotCafeController.testService.DoHoming());
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 101)
            {
                testTask = Task.Run(() => _robotCafeController.testService.GetReadyToService());

                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 200)
            {
                Product product = ProductServices.GetById(4);
                testTask = Task.Run(() => _robotCafeController.testService.DoService(product));
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            if (selectedMethod == 300)
            {
                Product product = ProductServices.GetById(40);
                testTask = Task.Run(() => _robotCafeController.testService.DoService(product));
                testTask.Wait();
                if (testTask.Result != 0)
                {
                    return 1;
                }
            }
            return 0;
        }

    }


}
