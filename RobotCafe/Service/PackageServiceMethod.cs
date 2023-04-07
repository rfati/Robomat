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

    public class PackageServiceMethod : IServiceMethod
    {
        private PackageServiceXArmPath path;
        public PackageServiceMethod(IXArmPath path)
        {
            this.path = (PackageServiceXArmPath)path;
        }

        public async Task<int> GetReadyToService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            return 0;
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

                paralelTasks.Add(robotCafeUnite.DoUrunAlmaTest(product.PackageType, packagedService:true));
                paralelTasks.Add(otomatUnite.GetReadyToService());

                ret = await this.RunAsyncParalelTasks(paralelTasks);
                if (ret != 0)
                {
                    return 1;
                }
                paralelTasks.Clear();



                ret = await robotCafeUnite.DoAmbalajSunumTest(); 
                if (ret != 0)
                {
                    return 1;
                }


                ret = await robotCafeUnite.DoServisSetiHazirlamaTest(setNo: 0);
                if (ret != 0)
                {
                    return 1;
                }

                paralelTasks.Add(robotCafeUnite.DoAmbalajTeslimTest());
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
