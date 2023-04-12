using RobotCafe.Serial;
using RobotCafe.xarm;
using System;
using System.Configuration;
using System.Threading;
using System.Timers;
using RobotCafe.Devices;
using System.Collections.Generic;
using RobotCafe.Service;
using Model;
using System.Linq;
using System.Threading.Tasks;
using Common;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RobotCafe
{

    public class RobotCafeController
    {
        public bool IsRunning = false;

        //public bool isCafeUniteConnected = false;
        //public bool isKiskacUniteConnected = false;
        public bool isRobotArmConnected = false;

        public CafeAsansorUnite asansorUnite;
        public CafeKesiciUnite kesiciUnite;
        public CafeKapakKapatmaUnite kapakKapatmaUnite;
        public CafeUrunAlmaUnite urunAlmaUnite;
        public CafeIsiticiUnite isiticiUnite;
        public CafeKapUnite cafeKapUnite;
        public CafeVakumUnite vakumUnite;

        public CafeRobotTutucuKiskacUnite cafeRobotTutucuKiskacUnite;

        public List<OtomatMotorUnite> otomatMotorUniteList;
        public OtomatUrunAlmaUnite otomatUrunAlmaUnite;

        

        public OtomatAsansorUnite otomatAsansorUnite;

        public RobotArm robotArm;

        public SerialManager cafeUniteSerialManager;
        public bool isCafeUniteCOMConnected = false;

        public SerialManager OtomatUniteSerialManager;
        public bool isOtomatUniteCOMConnected = false;

        //public SerialManager kiskacUniteSerialManager;
        //public bool isKiskacUniteCOMConnected = false;

        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;

        public SaleService saleService;
        public HomingService homingService;

        private IXArmPath hotServiceXArmPath;
        private IXArmPath coldServiceXArmPath;
        private IXArmPath packageServiceXArmPath;
        private GetReadyToServiceXArmPath getReadyToServiceXArmPath;


        private RobomatConfig robomatConfig;


        public bool IsHomingOK = false;

        public bool IsReadyToSaleService = false;




        public RobotCafeController()
        {
        }

        public void Start()
        {
            try
            {
                this.robomatConfig = JsonConvert.DeserializeObject<RobomatConfig>(File.ReadAllText(@"C:\Robomat\RobomatConfig\RobomatConfig.json"));

                this.hotServiceXArmPath = JsonConvert.DeserializeObject<HotServiceXArmPath>(File.ReadAllText(@"C:\Robomat\XArmPathConfig\HotServiceXArmPath.json"));
                this.coldServiceXArmPath = JsonConvert.DeserializeObject<ColdServiceXArmPath>(File.ReadAllText(@"C:\Robomat\XArmPathConfig\ColdServiceXArmPath.json"));
                this.packageServiceXArmPath = JsonConvert.DeserializeObject<PackageServiceXArmPath>(File.ReadAllText(@"C:\Robomat\XArmPathConfig\PackageServiceXArmPath.json"));
                this.getReadyToServiceXArmPath = JsonConvert.DeserializeObject<GetReadyToServiceXArmPath>(File.ReadAllText(@"C:\Robomat\XArmPathConfig\GetReadyToServiceXArmPath.json"));


                this.asansorUnite = new CafeAsansorUnite();
                this.kesiciUnite = new CafeKesiciUnite(this.robomatConfig);
                this.kapakKapatmaUnite = new CafeKapakKapatmaUnite(this.robomatConfig);
                this.urunAlmaUnite = new CafeUrunAlmaUnite(this.robomatConfig);
                this.isiticiUnite = new CafeIsiticiUnite(this.robomatConfig);
                this.cafeKapUnite = new CafeKapUnite();
                this.vakumUnite = new CafeVakumUnite();
                this.cafeRobotTutucuKiskacUnite = new CafeRobotTutucuKiskacUnite();

                this.cafeUniteSerialManager = new SerialManager(portName: this.robomatConfig.cafeUniteSerialManager_port);
                this.isCafeUniteCOMConnected = this.cafeUniteSerialManager.Open();
                if (this.isCafeUniteCOMConnected)
                {
                    this.cafeUniteSerialManager.OnDeviceDisconnected += CafeUniteSM_OnCOMDisconnected;
                    this.cafeUniteSerialManager.OnDeviceConnected += CafeUniteSM_OnCOMConnected;

                    this.asansorUnite.Attach(this.cafeUniteSerialManager);
                    this.kesiciUnite.Attach(this.cafeUniteSerialManager);
                    this.kapakKapatmaUnite.Attach(this.cafeUniteSerialManager);
                    this.urunAlmaUnite.Attach(this.cafeUniteSerialManager);
                    this.isiticiUnite.Attach(this.cafeUniteSerialManager);
                    this.cafeKapUnite.Attach(this.cafeUniteSerialManager);
                    this.vakumUnite.Attach(this.cafeUniteSerialManager);
                    this.cafeRobotTutucuKiskacUnite.Attach(this.cafeUniteSerialManager);
                }



                this.otomatMotorUniteList = new List<OtomatMotorUnite>();
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x11));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x12));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x13));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x14));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x15));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x16));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x17));
                this.otomatMotorUniteList.Add(new OtomatMotorUnite(slaveAddress: 0x18));

                this.otomatUrunAlmaUnite = new OtomatUrunAlmaUnite();
                this.otomatAsansorUnite = new OtomatAsansorUnite();
                this.OtomatUniteSerialManager = new SerialManager(portName: this.robomatConfig.cafeOtomatUniteSerialManager_port);
                this.isOtomatUniteCOMConnected = this.OtomatUniteSerialManager.Open();
                if (this.isOtomatUniteCOMConnected)
                {
                    this.OtomatUniteSerialManager.OnDeviceDisconnected += OtomatUniteSM_OnCOMDisconnected;
                    this.OtomatUniteSerialManager.OnDeviceConnected += OtomatUniteSM_OnCOMConnected;
                    foreach (var item in this.otomatMotorUniteList)
                    {
                        item.Attach(this.OtomatUniteSerialManager);
                    }

                    this.otomatUrunAlmaUnite.Attach(this.OtomatUniteSerialManager);
                    this.otomatAsansorUnite.Attach(this.OtomatUniteSerialManager);
                }



                //this.cafeRobotTutucuKiskacUnite = new CafeRobotTutucuKiskacUnite();
                //this.kiskacUniteSerialManager = new SerialManager(portName: this.robomatConfig.cafeRobotTutucuKiskacUniteSerialManager_port);
                //this.isKiskacUniteCOMConnected = this.kiskacUniteSerialManager.Open();
                //if (this.isKiskacUniteCOMConnected)
                //{
                //    this.kiskacUniteSerialManager.OnDeviceDisconnected += KiskacUnite_OnDeviceDisconnected;
                //    this.kiskacUniteSerialManager.OnDeviceConnected += KiskacUnite_OnDeviceConnected;

                //    this.cafeRobotTutucuKiskacUnite.Attach(this.kiskacUniteSerialManager);
                //}



                this.robotArm = new RobotArm();
                this.isRobotArmConnected = this.robotArm.Connect(this.robomatConfig.XArm_Ip);
                Logger.LogInfo("xarm_ip: " + this.robomatConfig.XArm_Ip);
                if (this.isRobotArmConnected)
                {
                    this.robotArm.xArmController.OnDeviceDisconnected += RobotArm_OnDeviceDisconnected;
                    this.robotArm.xArmController.OnDeviceConnected += RobotArm_OnDeviceConnected;
                }

                this.otomatUnite = new OtomatUnite(this.otomatMotorUniteList, this.otomatAsansorUnite, this.otomatUrunAlmaUnite);
                this.robotCafeUnite = new RobotCafeUnite(this.asansorUnite, this.kesiciUnite, this.kapakKapatmaUnite, this.isiticiUnite, this.urunAlmaUnite, this.vakumUnite, this.cafeKapUnite, this.cafeRobotTutucuKiskacUnite, this.robotArm, this.getReadyToServiceXArmPath);



                this.saleService = new SaleService(this.otomatUnite, this.robotCafeUnite);
                this.homingService = new HomingService(this.otomatUnite, this.robotCafeUnite);




                //if (this.isOtomatUniteCOMConnected && this.isCafeUniteCOMConnected && isKiskacUniteCOMConnected && isRobotArmConnected)
                //{
                //    this.IsRunning = true;
                //    Logger.LogInfo("RobotCafeController---> Start().. IsRunning = TRUE");
                //}
                //else
                //{
                //    this.IsRunning = false;
                //    Logger.LogError("RobotCafeController---> Start() Is NOT Running properly..  isCafeUniteConnected: " + isCafeUniteCOMConnected.ToString() + " isKiskacUniteConnected: " + isKiskacUniteCOMConnected.ToString() + " isRobotArmConnected: " + isRobotArmConnected.ToString() + " isCafeOtomatUniteConnected: " + isOtomatUniteCOMConnected.ToString());
                //}



                if (this.isOtomatUniteCOMConnected && this.isCafeUniteCOMConnected && isRobotArmConnected)
                {
                    this.IsRunning = true;
                    Logger.LogInfo("RobotCafeController---> Start().. IsRunning = TRUE");
                }
                else
                {
                    this.IsRunning = false;
                    Logger.LogError("RobotCafeController---> Start() Is NOT Running properly..  isCafeUniteConnected: " + isCafeUniteCOMConnected.ToString() + " isRobotArmConnected: " + isRobotArmConnected.ToString() + " isCafeOtomatUniteConnected: " + isOtomatUniteCOMConnected.ToString());
                }

            }
            catch (Exception ex)
            {
                this.IsRunning = false;
                Logger.LogError("RobotCafeController---> Start() exception: " + ex.Message);
            }
        }


        private void RobotArm_OnDeviceConnected()
        {
            this.isRobotArmConnected = true;
        }

        private void RobotArm_OnDeviceDisconnected()
        {
            this.isRobotArmConnected = false;
            this.IsRunning = false;
        }

        //private void KiskacUnite_OnDeviceConnected(string SerialPortName)
        //{
        //    this.isKiskacUniteCOMConnected = true;
        //}

        //private void KiskacUnite_OnDeviceDisconnected(string SerialPortName)
        //{
        //    this.isKiskacUniteCOMConnected = false;
        //    this.IsRunning = false;
        //}

        private void CafeUniteSM_OnCOMConnected(string SerialPortName)
        {
            this.isCafeUniteCOMConnected = true;
        }

        private void CafeUniteSM_OnCOMDisconnected(string SerialPortName)
        {
            this.isCafeUniteCOMConnected = false;
            this.IsRunning = false;
        }

        private void OtomatUniteSM_OnCOMConnected(string SerialPortName)
        {
            this.isOtomatUniteCOMConnected = true;
        }

        private void OtomatUniteSM_OnCOMDisconnected(string SerialPortName)
        {
            this.isOtomatUniteCOMConnected = false;
            this.IsRunning = false;
        }

        public void Restart()
        {
            Logger.LogInfo("RobotCafeController--->Restart()");

            if (this.isCafeUniteCOMConnected == false)
            {
                Logger.LogInfo("RobotCafeController.CafeUnite--->Restart() --Trying...");

                this.asansorUnite.Detach();
                this.kesiciUnite.Detach();
                this.kapakKapatmaUnite.Detach();
                this.urunAlmaUnite.Detach();
                this.isiticiUnite.Detach();
                this.cafeKapUnite.Detach();
                this.vakumUnite.Detach();
                this.cafeRobotTutucuKiskacUnite.Detach();

                this.cafeUniteSerialManager.OnDeviceDisconnected -= CafeUniteSM_OnCOMDisconnected;
                this.cafeUniteSerialManager.OnDeviceConnected -= CafeUniteSM_OnCOMConnected;

                this.cafeUniteSerialManager.Dispose();
                this.isCafeUniteCOMConnected = this.cafeUniteSerialManager.Open();

                if (this.isCafeUniteCOMConnected)
                {
                    this.cafeUniteSerialManager.OnDeviceDisconnected += CafeUniteSM_OnCOMDisconnected;
                    this.cafeUniteSerialManager.OnDeviceConnected += CafeUniteSM_OnCOMConnected;

                    this.asansorUnite.Attach(this.cafeUniteSerialManager);
                    this.kesiciUnite.Attach(this.cafeUniteSerialManager);
                    this.kapakKapatmaUnite.Attach(this.cafeUniteSerialManager);
                    this.urunAlmaUnite.Attach(this.cafeUniteSerialManager);
                    this.isiticiUnite.Attach(this.cafeUniteSerialManager);
                    this.cafeKapUnite.Attach(this.cafeUniteSerialManager);
                    this.vakumUnite.Attach(this.cafeUniteSerialManager);
                    this.cafeRobotTutucuKiskacUnite.Attach(this.cafeUniteSerialManager);
                }
            }


            if (this.isOtomatUniteCOMConnected == false)
            {
                Logger.LogInfo("RobotCafeController.CafeUnite--->Restart() --Trying...");

                foreach (var item in this.otomatMotorUniteList)
                {
                    item.Detach();
                }
                this.otomatUrunAlmaUnite.Detach();
                this.otomatAsansorUnite.Detach();

                this.OtomatUniteSerialManager.OnDeviceDisconnected -= OtomatUniteSM_OnCOMDisconnected;
                this.OtomatUniteSerialManager.OnDeviceConnected -= OtomatUniteSM_OnCOMConnected;

                this.OtomatUniteSerialManager.Dispose();
                this.isOtomatUniteCOMConnected = this.OtomatUniteSerialManager.Open();

                if (this.isOtomatUniteCOMConnected)
                {
                    this.OtomatUniteSerialManager.OnDeviceDisconnected += OtomatUniteSM_OnCOMDisconnected;
                    this.OtomatUniteSerialManager.OnDeviceConnected += OtomatUniteSM_OnCOMConnected;

                    foreach (var item in this.otomatMotorUniteList)
                    {
                        item.Attach(this.OtomatUniteSerialManager);
                    }
                    this.otomatUrunAlmaUnite.Attach(this.OtomatUniteSerialManager);
                    this.otomatAsansorUnite.Attach(this.OtomatUniteSerialManager);
                }
            }

            //if (this.isKiskacUniteCOMConnected == false)
            //{
            //    Logger.LogInfo("RobotCafeController.KiskacUnite--->Restart() --Trying...");

            //    this.cafeRobotTutucuKiskacUnite.Detach();

            //    this.kiskacUniteSerialManager.OnDeviceDisconnected -= KiskacUnite_OnDeviceDisconnected;
            //    this.kiskacUniteSerialManager.OnDeviceConnected -= KiskacUnite_OnDeviceConnected;

            //    this.kiskacUniteSerialManager.Dispose();
            //    this.isKiskacUniteCOMConnected = this.kiskacUniteSerialManager.Open();

            //    if (this.isKiskacUniteCOMConnected)
            //    {
            //        this.kiskacUniteSerialManager.OnDeviceDisconnected += KiskacUnite_OnDeviceDisconnected;
            //        this.kiskacUniteSerialManager.OnDeviceConnected += KiskacUnite_OnDeviceConnected;

            //        this.cafeRobotTutucuKiskacUnite.Attach(this.kiskacUniteSerialManager);

            //    }
            //}

            if (this.isRobotArmConnected == false)
            {
                Logger.LogInfo("RobotCafeController.RobotArm--->Restart() --Trying...");

                if (this.robotArm.xArmController != null)
                {
                    this.robotArm.xArmController.OnDeviceDisconnected -= RobotArm_OnDeviceDisconnected;
                    this.robotArm.xArmController.OnDeviceConnected -= RobotArm_OnDeviceConnected;
                }
                this.robotArm.Dispose();
                this.isRobotArmConnected = this.robotArm.Connect(this.robomatConfig.XArm_Ip);
                if (this.isRobotArmConnected)
                {
                    this.robotArm.xArmController.OnDeviceDisconnected += RobotArm_OnDeviceDisconnected;
                    this.robotArm.xArmController.OnDeviceConnected += RobotArm_OnDeviceConnected;
                }
            }

            //if (this.isCafeUniteCOMConnected && this.isOtomatUniteCOMConnected && isKiskacUniteCOMConnected && this.isRobotArmConnected)
            //{
            //    this.IsRunning = true;
            //}

            if (this.isCafeUniteCOMConnected && this.isOtomatUniteCOMConnected && this.isRobotArmConnected)
            {
                this.IsRunning = true;
            }
        }


        public int DoServiceCommand(Product product)
        {
            
                int ret = 1;
                Logger.LogInfo("DoServiceCommand HotServiceMethod starting ....");
                saleService.SetServiceMethod(new HotServiceMethod(HotServiceType.Sicak, this.hotServiceXArmPath));
                ret = saleService.DoService(product);

                return ret;

        }


        public int DoHoming()
        {
            int ret = -1;
            try
            {
                this.homingService.DoHoming();
                ret = this.homingService.HomingResult;

            }
            catch (Exception e)
            {
                Logger.LogError("DoHoming process exception: " + e.Message);
            }

            if (ret == 0)
            {
                Logger.LogInfo("DoHoming process OK.");
                this.IsHomingOK = true;
            }
            else
            {
                Logger.LogError("DoHoming process NOK!!!");
                this.IsHomingOK = false;
            }
            return ret;

        }

        public int GetReadyToSaleService()
        {
            int ret = -1;
            try
            {
                this.homingService.GetReadyToService();
                ret = this.homingService.GetReadyToServiceResult;
            }
            catch (Exception e)
            {
                Logger.LogError("DoHoming process exception: " + e.Message);
            }

            if (ret == 0)
            {
                Logger.LogInfo("DoHoming process OK.");
                this.IsReadyToSaleService = true;
            }
            else
            {
                Logger.LogError("DoHoming process NOK!!!");
                this.IsReadyToSaleService = false;
            }
            return ret;

        }


    }
}