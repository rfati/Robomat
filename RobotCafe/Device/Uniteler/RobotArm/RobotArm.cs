using Common;
using RobotCafe.Serial;
using RobotCafe.xarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class RobotArm : IDisposable
    {

        public XArmController xArmController;


        public RobotArm()
        {
        }

        public bool Connect(string Ip)
        {
            try
            {
                this.xArmController = new XArmController(ip:Ip);
                if(this.xArmController.IsXArmReady == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (this.xArmController != null)
            {
                this.xArmController.Dispose();
                this.xArmController = null;
            }



        }



        public async Task<int> SetPosition(List<Pos> position)
        {
            int ret = xArmController.SetPosition(position);
            return ret;
        }

        public async Task<int> SetPosition(int ret, List<float[]> position, float speed = 5, bool wait = true)
        {
            if (ret != 0)
                return 1;
            ret = xArmController.SetPosition(position, speed, wait);
            return ret;
        }

        public async Task<int> SetTCPPosition(float[] pos, float speed = 5, bool wait = true)
        {
            int ret = xArmController.SetTCPPosition(pos: pos, speed: speed, wait: wait);
            return ret;

        }

        public async Task<int> DoHoming()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position

            positionList.Clear();
            positionList.Add(pos1);

            return await this.SetPosition(ret, positionList, speed: 10);

        }
    }
}
