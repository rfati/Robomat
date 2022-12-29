using Common;
using Model;
using RobotCafe.Devices;
using RobotCafe.xarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Service
{
    public class HotServiceMethod : IServiceMethod
    {
        private int IsitmaSuresi;
        private HotServiceXArmPath path;
        public HotServiceMethod(HotServiceType hotServiceType, IXArmPath path)
        {
            this.path = (HotServiceXArmPath)path;
            if(hotServiceType == HotServiceType.Ilik)
            {
                this.IsitmaSuresi = 10000; 
            }
            else if (hotServiceType == HotServiceType.Orta)
            {
                this.IsitmaSuresi = 20000; 
            }
            if (hotServiceType == HotServiceType.Sicak)
            {
                this.IsitmaSuresi = 25000; 
            }

        }

        public async Task<int> GetReadyToService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            int ret = 0;
            ret = await robotCafeUnite.GetReadyToService(ret);

            return ret;
        }



        public async Task<int> DoService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite, Product product)
        {
            int ret = 0;
            List<Task<int>> paralelTasks = new List<Task<int>>();


            bool isUrunCafede = false;
            for (int i = 0; i < 2; i++)
            {
                isUrunCafede = robotCafeUnite.urunAlmaUnite.UrunAlindimi();
                if (isUrunCafede == false)
                {
                    paralelTasks.Add(otomatUnite.ServisYap(product));
                    if (product.KapType == KapType.Kase)
                    {
                        if(i==0)
                            paralelTasks.Add(robotCafeUnite.DoKaseYerlestirmeTest());
                    }
                    else if (product.KapType == KapType.Bardak)
                    {
                        if (i == 0)
                            paralelTasks.Add(robotCafeUnite.DoBardakYerlestirmeTest());
                    }

                    ret = await this.RunAsyncParalelTasks(paralelTasks);
                    if (ret != 0)
                    {
                        return 1;
                    }
                    paralelTasks.Clear();

                }
                else
                {
                    break;
                }
            }

            isUrunCafede = robotCafeUnite.urunAlmaUnite.UrunAlindimi();
            if (isUrunCafede == false)
            {
                return 1;
            }
            else
            {

                paralelTasks.Add(robotCafeUnite.DoUrunAlmaTest(product.PackageType));
                paralelTasks.Add(otomatUnite.GetReadyToService());

                ret = await this.RunAsyncParalelTasks(paralelTasks);
                if (ret != 0)
                {
                    return 1;
                }
                paralelTasks.Clear();



                ret = await robotCafeUnite.DoKesmeTest(product.PackageType);
                if (ret != 0)
                {
                    return 1;
                }


                ret = await robotCafeUnite.DoIsitmaTest(isitmaSuresi:this.IsitmaSuresi, product.PackageType);
                if (ret != 0)
                {
                    return 1;
                }

                ret = await robotCafeUnite.DoUrunBosaltmaVeProbeYikamaTest(product.KapType, temizlemeSuresi:12000);
                if (ret != 0)
                {
                    return 1;
                }

                ret = await robotCafeUnite.DoCopAtmaTest();
                if (ret != 0)
                {
                    return 1;
                }



                if (product.KapType == KapType.Kase)
                {
                    ret = await robotCafeUnite.DoKaseKapakYerlestirmeTest();
                }
                else if (product.KapType == KapType.Bardak)
                {
                    ret = await robotCafeUnite.DoBardakKapakYerlestirmeTest();
                }
                if (ret != 0)
                {
                    return 1;
                }



                ret = await robotCafeUnite.DoKapKapatmaTest(product.KapType);
                if (ret != 0)
                {
                    return 1;
                }


                if (product.KapType == KapType.Kase)
                {
                    ret = await robotCafeUnite.DoKaseSunumTest();
                }
                else if (product.KapType == KapType.Bardak)
                {
                    ret = await robotCafeUnite.DoBardakSunumTest();
                }

                if (ret != 0)
                {
                    return 1;
                }



                ret = await robotCafeUnite.DoServisSetiHazirlamaTest(setNo: 0);
                if (ret != 0)
                {
                    return 1;
                }

                paralelTasks.Add(robotCafeUnite.DoUrunTeslimTest());
                paralelTasks.Add(robotCafeUnite.DoSelamlamaTest());
                ret = await this.RunAsyncParalelTasks(paralelTasks);
                if (ret != 0)
                {
                    return 1;
                }
                paralelTasks.Clear();


            }



            return 0;
        }



        private async Task<int> RunAsyncParalelTasks(List<Task<int>> paralelTasks)
        {
            int ret = -1;
            int taskCounter = paralelTasks.Count;
            while (taskCounter > 0)
            {
                Task<int> finishedTask = await Task.WhenAny(paralelTasks);
                taskCounter--;
                //paralelTasks.Remove(finishedTask);
            }
            foreach (var task in paralelTasks)
            {
                if (task.Result == 0)
                {
                    continue;
                }
                else
                {
                    return -1;
                }
            }

            ret = 0;
            return ret;

        }
    }
}
