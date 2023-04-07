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

            for (ushort regaddress = this.kesici.FirstReadAddress; regaddress <= this.kesici.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }



        }

        public int SetPositionTask(int ret, short? lineerPos, short? servoPos)
        {
            if (ret != 0)
                return 1;

            ret = SetPosition(ret, lineerPos, servoPos);
            ret = IsPositionOK(ret, lineerPos, servoPos);

            if (ret != 0)
            {
                Logger.LogError("Kesici ünitesi SetPositionTask Error.");
            }

            return ret;


        }



        public int SetPosition(int ret,short? lineerPos, short? servoPos)
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

            ret = SetMotorPosition(motorList);
            if (ret != 0)
            {
                Logger.LogError("Kesici ünitesi SetPosition Error.");
            }

            return ret;
        }

        public int IsPositionOK(int ret, short? lineerPos, short? servoPos)
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

            ret = IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Kesici ünitesi IsPositionOK Error.");
            }

            return ret;
        }




    }
}
