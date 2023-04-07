using Common;
using RobotCafe.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class OtomatAsansorUnite : RTUDevice
    {
        private Asansor asansor { get; set; }
        public OtomatAsansorUnite()
        {
            this.slaveAddress = 0x05;

            ushort lastReg = 5;
            for (ushort regaddress = 2; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

            this.asansor = new Asansor();

        }

        public int DoHoming()
        {
            int ret = 0;
            ret =   this.SetPosition(ret: ret, yatayPos:0, dikeyPos:null);
            if (ret != 0)
                return 1;

            Thread.Sleep(8000);

            ret =   this.SetPosition(ret: ret, yatayPos: null, dikeyPos: 0);
            if (ret != 0)
                return 1;
            Thread.Sleep(8000);

            return 0;

        }




        public int SetPositionTask(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            ret =   SetPosition(ret, yatayPos, dikeyPos);
            ret =   IsPositionOK(ret, yatayPos, dikeyPos);

            if (ret != 0)
            {
                Logger.LogError("Otomat Asansör ünitesi SetPositionTask Error.");
            }

            return ret;
        }

        public int SetPosition(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            bool isTogether = false;
            List<Motor> motorList = new List<Motor>();
            if (yatayPos != null)
            {
                this.asansor.Asansor_YatayStep.TargetPosRegisterWrite.Register_Target_Value = (short)yatayPos;
                motorList.Add(this.asansor.Asansor_YatayStep);
            }
            if (dikeyPos != null)
            {
                this.asansor.Asansor_DikeyStep.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.asansor.Asansor_DikeyStep);
            }

            if(yatayPos != null && dikeyPos != null)
            {
                isTogether = true;
            }


            ret =   SetMotorPosition(motorList, isTogether: isTogether);
            if (ret != 0)
            {
                Logger.LogError("Otomat Asansör ünitesi SetPosition Error.");
            }

            return ret;
        }

        public int IsPositionOK(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (yatayPos != null)
            {
                this.asansor.Asansor_YatayStep.TargetPosRegisterWrite.Register_Target_Value = (short)yatayPos;
                motorList.Add(this.asansor.Asansor_YatayStep);
            }
            if (dikeyPos != null)
            {
                this.asansor.Asansor_DikeyStep.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.asansor.Asansor_DikeyStep);
            }
           

            ret =    IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Otomat Asansör ünitesi IsPositionOK Error.");
            }

            return ret;
        }



    }
}
