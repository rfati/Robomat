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

    public class ColdServiceMethod : IServiceMethod
    {
        private ColdServiceXArmPath path;
        public ColdServiceMethod(IXArmPath path)
        {
            this.path = (ColdServiceXArmPath)path;

        }

        public async Task<int> GetReadyToService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            int ret = 0;
            ret = await robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 0);
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
                    if (i == 0)
                        paralelTasks.Add(robotCafeUnite.DoKaseYerlestirmeTest());

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


                ret = await robotCafeUnite.DoAmbalajAcmaTest();
                if (ret != 0)
                {
                    return 1;
                }

                ret = await robotCafeUnite.DoBosaltmaKaseTest();
                if (ret != 0)
                {
                    return 1;
                }


                ret = await robotCafeUnite.DoCopAtmaTest();
                if (ret != 0)
                {
                    return 1;
                }


                ret = await robotCafeUnite.DoKaseKapakYerlestirmeTest();
                if (ret != 0)
                {
                    return 1;
                }


                ret = await robotCafeUnite.DoKapKapatmaTest(product.KapType);
                if (ret != 0)
                {
                    return 1;
                }

                ret = await robotCafeUnite.DoKaseSunumTest();
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
