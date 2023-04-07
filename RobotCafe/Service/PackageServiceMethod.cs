using Common;
using Model;
using RobotCafe.Devices;
using RobotCafe.xarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotCafe.Service
{

    public class PackageServiceMethod : IServiceMethod
    {
        private PackageServiceXArmPath path;
        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;
        private Product product;

        private int OtomatServiceResult = -1;
        private int KapYerlestirResult = -1;
        private int OtomatReadyResult = -1;
        private int CafeReadyResult = -1;
        private int UrunTeslimResult = -1;

        public PackageServiceMethod(IXArmPath path)
        {
            this.path = (PackageServiceXArmPath)path;
        }


        public int UrunTeslimVeGetReadyToService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            this.UrunTeslimVeGetReadyToService();
            if (OtomatReadyResult == 0 && CafeReadyResult == 0 && UrunTeslimResult == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private void UrunTeslimVeGetReadyToService()
        {
            Thread t1 = new Thread(new ThreadStart(DoOtomatReadyToService));
            Thread t2 = new Thread(new ThreadStart(DoCafeReadyToService));
            Thread t3 = new Thread(new ThreadStart(DoUrunTeslim));

            t3.Start();
            t2.Start();
            t1.Start();

            t3.Join();
            t2.Join();
            t1.Join();

        }

        private void DoOtomatReadyToService()
        {
            OtomatReadyResult = otomatUnite.GetReadyToService();
        }

        private void DoCafeReadyToService()
        {
            CafeReadyResult = robotCafeUnite.GetReadyToNewService();
        }

        private void DoUrunTeslim()
        {
            UrunTeslimResult = robotCafeUnite.DoUrunTeslim(ServiceType.Package);
        }

        private void OtomatDoService()
        {
            bool isUrunCafede = false;
            int ret = 0;
            for (int i = 0; i < 3; i++)
            {

                OtomatServiceResult = otomatUnite.DoService(this.product);
                if (OtomatServiceResult == 0)
                {
                    int tryCounter = 0;
                    while (true)
                    {
                        tryCounter++;
                        isUrunCafede = robotCafeUnite.urunAlmaUnite.UrunAlindimi();
                        if (isUrunCafede == false)
                        {
                            Thread.Sleep(20);
                            if (tryCounter > 200)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (isUrunCafede == true)
                    {
                        ret = this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 40);
                        ret = this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: 33, lineerPos: null);

                        OtomatServiceResult = ret;
                        break;
                    }
                    else
                    {
                        OtomatServiceResult = 1;
                        break;
                    }



                }
                else
                {
                    ret = otomatUnite.GetReadyToService();
                    if (ret != 0)
                    {
                        OtomatServiceResult = 1;
                        break;
                    }
                }
            }


        }




        public int DoService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite, Product product)
        {
            int ret = 0;

            this.otomatUnite = otomatUnite;
            this.robotCafeUnite = robotCafeUnite;
            this.product = product;

            OtomatDoService();

            if (OtomatServiceResult == 0)
            {
                ret = 0;
            }
            else
            {
                return 1;
            }


            ret = robotCafeUnite.DoUrunAlma(ServiceType.Package);
            if (ret != 0)
            {
                return 1;
            }
            ret = robotCafeUnite.DoPackageService();
            if (ret != 0)
            {
                return 1;
            }

            ret = this.UrunTeslimVeGetReadyToService(otomatUnite, robotCafeUnite);

            if (ret != 0)
            {
                return 1;
            }




            return 0;


        }



    }
}
