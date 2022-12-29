using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeKesiciUnite : RTUDevice
    {
        private Kesici kesici { get; set; }

        public short Kesici_Lineer_Init_Pos { get; set; }
        public short Kesici_Lineer_Kesme_Pos { get; set; }
        public short Kesici_Servo_Init_Pos { get; set; }
        public short Kesici_Servo_Kesme_Pos { get; set; }

        public CafeKesiciUnite(RobomatConfig robomatConfig)
        {
            this.Kesici_Lineer_Init_Pos = robomatConfig.Kesici_Lineer_Init_Pos;
            this.Kesici_Lineer_Kesme_Pos = robomatConfig.Kesici_Lineer_Kesme_Pos;
            this.Kesici_Servo_Init_Pos = robomatConfig.Kesici_Servo_Init_Pos;
            this.Kesici_Servo_Kesme_Pos = robomatConfig.Kesici_Servo_Kesme_Pos;

            this.kesici = new Kesici();
            this.slaveAddress = 0x21;
            //this.maxResponseWaitTime = 500;
            //this.stateChangeTime = 3;
            //this.MAX_TRY_COUNTER = 20;
            this.NextReadDelay = 0;

            for (ushort regaddress = this.kesici.FirstReadAddress; regaddress <= this.kesici.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }



        }

        public async Task<int> SetPositionTask(int ret, short? lineerPos, short? servoPos)
        {
            if (ret != 0)
                return 1;

            ret = await SetPosition(ret, lineerPos, servoPos);
            return await IsPositionOK(ret, lineerPos, servoPos);


        }



        public async Task<int> SetPosition(int ret,short? lineerPos, short? servoPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if(lineerPos != null)
            {
                this.kesici.Kesici_Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)lineerPos;
                motorList.Add(this.kesici.Kesici_Lineer);
            }
            if (servoPos != null)
            {
                this.kesici.Kesici_Servo.TargetPosRegisterWrite.Register_Target_Value = (short)servoPos;
                motorList.Add(this.kesici.Kesici_Servo);
            }

            return await SetMotorPosition(motorList);
        }

        public async Task<int> IsPositionOK(int ret, short? lineerPos, short? servoPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (lineerPos != null)
            {
                this.kesici.Kesici_Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)lineerPos;
                motorList.Add(this.kesici.Kesici_Lineer);
            }
            if (servoPos != null)
            {
                this.kesici.Kesici_Servo.TargetPosRegisterWrite.Register_Target_Value = (short)servoPos;
                motorList.Add(this.kesici.Kesici_Servo);
            }

            return await IsMotorPositionOK(motorList);
        }


        

        public async Task<int> SikistirmaLineerPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.kesici.Kesici_Lineer.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.kesici.Kesici_Lineer);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> BicakServoPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.kesici.Kesici_Servo.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.kesici.Kesici_Servo);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }




    }
}
