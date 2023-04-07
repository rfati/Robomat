using RobotCafe.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using RobotCafe.xarm;

namespace RobotCafe.Service
{
    public class HomingService
    {
        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;
        public HomingService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            this.otomatUnite = otomatUnite;
            this.robotCafeUnite = robotCafeUnite;
        }


        public async Task<int> DoHoming()
        {
            List<Task<int>> paralelTasks = new List<Task<int>>();

            paralelTasks.Add(otomatUnite.DoHoming());
            paralelTasks.Add(robotCafeUnite.DoHoming());

            int ret = await this.RunAsyncParalelTasks(paralelTasks);
            if (ret != 0)
            {
                return 1;
            }
            paralelTasks.Clear();
            paralelTasks = null;


            return 0;

        }

        public async Task<int> GetReadyToService()
        {
            List<Task<int>> paralelTasks = new List<Task<int>>();

            paralelTasks.Add(otomatUnite.GetHomingReadyToService());
            paralelTasks.Add(robotCafeUnite.GetReadyToService(ret: 0));

            int ret = await this.RunAsyncParalelTasks(paralelTasks);
            if (ret != 0)
            {
                return 1;
            }
            paralelTasks.Clear();
            paralelTasks = null;

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
                paralelTasks.Remove(finishedTask);
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
