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
    public class HotServiceMethod : IServiceMethod
    {
        private int BardakIsitmaSuresi;
        private int KaseIsitmaSuresi;
        private HotServiceXArmPath path;
        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;
        private Product product;
        private int OtomatServiceResult = -1;
        private int KapYerlestirResult = -1;
        private int OtomatReadyResult = -1;
        private int CafeReadyResult = -1;
        private int UrunTeslimResult = -1;
        private int ServisSetHazirResult = -1;

        public HotServiceMethod(HotServiceType hotServiceType, IXArmPath path)
        {
            this.path = (HotServiceXArmPath)path;
            if(hotServiceType == HotServiceType.Ilik)
            {
                this.BardakIsitmaSuresi = 10000; 
                this.KaseIsitmaSuresi = 8500; 
            }
            else if (hotServiceType == HotServiceType.Orta)
            {
                this.BardakIsitmaSuresi = 13500; 
                this.KaseIsitmaSuresi = 9500; 
            }
            if (hotServiceType == HotServiceType.Sicak)
            {
                this.BardakIsitmaSuresi = 15000; 
                this.KaseIsitmaSuresi = 10000; 
            }

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

            t1.Join();
            t3.Join();
            t2.Join();


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
            UrunTeslimResult = robotCafeUnite.DoUrunTeslim(ServiceType.Hot); 
        }

        private void OtomatDoService()
        {
            bool isUrunCafede = false;
            int ret = 0;
            for (int i=0;i<3;i++)
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
                    if(ret != 0)
                    {
                        OtomatServiceResult = 1;
                        break; 
                    }
                }
            }


        }


        private void CafeDoHazirlikKase()
        {
            KapYerlestirResult = robotCafeUnite.DoKaseYerlestirme();
            if (KapYerlestirResult == 0)
            {
                KapInfo senSorInfo = this.robotCafeUnite.cafeKapUnite.GetKapSensorInfo();
                if (senSorInfo != null)
                {
                    if (senSorInfo.dolu_set_no_list.Count > 0)
                    {
                        ServisSetHazirResult = robotCafeUnite.DoServisSetiHazirlama(senSorInfo.dolu_set_no_list[0]);
                    }

                }
            }
        }

        private void CafeDoHazirlikBardak()
        {
            KapYerlestirResult = robotCafeUnite.DoBardakYerlestirme();
            if(KapYerlestirResult == 0)
            {
                KapInfo senSorInfo = this.robotCafeUnite.cafeKapUnite.GetKapSensorInfo();
                if (senSorInfo != null)
                {
                    if (senSorInfo.dolu_set_no_list.Count > 0)
                    {
                        ServisSetHazirResult = robotCafeUnite.DoServisSetiHazirlama(senSorInfo.dolu_set_no_list[0]);
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

            Thread t1 = new Thread(new ThreadStart(OtomatDoService));
            Thread t2;
            if (product.KapType == KapType.Kase)
            {
                t2 = new Thread(new ThreadStart(CafeDoHazirlikKase));
            }
            else
            {
                t2 = new Thread(new ThreadStart(CafeDoHazirlikBardak));
            }

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            if (OtomatServiceResult == 0 && KapYerlestirResult == 0)
            {
                ret = 0;
            }
            else
            {
                return 1;
            }


            ret = robotCafeUnite.DoUrunAlma(ServiceType.Hot);
            if (ret != 0)
            {
                return 1;
            }


            ret = robotCafeUnite.DoKesme();
            if (ret != 0)
            {
                return 1;
            }

            if (product.KapType == KapType.Bardak)
            {
                ret = robotCafeUnite.DoBosaltmaBardak();
            }
            else if (product.KapType == KapType.Kase)
            {
                ret = robotCafeUnite.DoBosaltmaKase();
            }

            if (ret != 0)
            {
                return 1;
            }



            ret = robotCafeUnite.DoCopAtma();
            if (ret != 0)
            {
                return 1;
            }


            if (product.KapType == KapType.Bardak)
            {
                ret = robotCafeUnite.DoIsitmaBardak(this.BardakIsitmaSuresi);
            }
            else if (product.KapType == KapType.Kase)
            {
                ret = robotCafeUnite.DoIsitmaKase(this.KaseIsitmaSuresi);
            }

            if (ret != 0)
            {
                return 1;
            }

            if (product.KapType == KapType.Bardak)
            {
                ret = robotCafeUnite.DoBardakSunum();
            }
            else if (product.KapType == KapType.Kase)
            {
                ret = robotCafeUnite.DoKaseSunum();
            }

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
