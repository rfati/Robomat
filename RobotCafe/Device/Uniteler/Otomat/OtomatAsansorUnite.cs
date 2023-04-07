using Common;
using RobotCafe.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class OtomatAsansorUnite : RTUDevice
    {
        private Asansor asansor { get; set; }
        public OtomatAsansorUnite()
        {
            this.slaveAddress = 0x05;
            this.NextReadDelay = 10;

            ushort lastReg = 5;
            for (ushort regaddress = 2; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

            this.asansor = new Asansor();

        }

        public async Task<int> DoHoming()
        {
            int ret = -1;
            ret = await this.SetPositionTask(ret: ret, yatayPos:0, dikeyPos:null);
            if (ret != 0)
                return 1;

            ret = await this.SetPositionTask(ret: ret, yatayPos: null, dikeyPos: 0);
            if (ret != 0)
                return 1;

            return 0;

        }


        //public async Task<int> SetPosition(short dikeyPos, short yatayPos)
        //{
        //    int ret = -1;

        //    List<Motor> motorList = new List<Motor>();
        //    MotorCommandResult retMotor = null;

        //    motorList.Clear();
        //    this.asansor.Asansor_YatayStep.TargetPosRegisterWrite.Register_Target_Value = yatayPos;
        //    this.asansor.Asansor_DikeyStep.TargetPosRegisterWrite.Register_Target_Value = dikeyPos;

        //    motorList.Add(this.asansor.Asansor_YatayStep);
        //    motorList.Add(this.asansor.Asansor_DikeyStep);

        //    retMotor = await WriteReadMultipleMotor(motorList, isTogether:true);

        //    if (!retMotor.IsSuccess())
        //    {
        //        ret = -1;
        //        return ret;
        //    }

        //    ret = 0;
        //    return ret;

        //}


        public async Task<int> SetPositionTask(int ret, short? yatayPos, short? dikeyPos, int delay=1000)
        {
            if (ret != 0)
                return 1;
            ret = await SetPosition(ret, yatayPos, dikeyPos);
            return await IsPositionOK(ret, yatayPos, dikeyPos);
        }

        public async Task<int> SetPosition(int ret, short? yatayPos, short? dikeyPos)
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


            return await SetMotorPosition(motorList, isTogether:true);
        }

        public async Task<int> IsPositionOK(int ret, short? yatayPos, short? dikeyPos)
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
           

            return await IsMotorPositionOK(motorList);
        }



    }
}
