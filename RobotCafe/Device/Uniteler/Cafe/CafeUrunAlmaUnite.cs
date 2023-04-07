using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeUrunAlmaUnite : RTUDevice
    {
        private UrunAlma urunAlma { get; set; }

        public short Servo_Init_Pos { get; set; }
        public short Servo_Verme_Pos { get; set; }
        public short Lineer_Init_Pos { get; set; }
        public short Lineer_Verme_Pos { get; set; }

        public CafeUrunAlmaUnite(RobomatConfig robomatConfig)
        {

            this.Servo_Init_Pos = robomatConfig.UrunAlma_Servo_Init_Pos;
            this.Servo_Verme_Pos = robomatConfig.UrunAlma_Servo_Verme_Pos;
            this.Lineer_Init_Pos = robomatConfig.UrunAlma_Lineer_Init_Pos;
            this.Lineer_Verme_Pos = robomatConfig.UrunAlma_Lineer_Verme_Pos;


            this.urunAlma = new UrunAlma();
            this.slaveAddress = 0x29;

            for (ushort regaddress = this.urunAlma.FirstReadAddress; regaddress <= this.urunAlma.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }


        }
        public int SetPositionTask(int ret, short? donmePos, short? lineerPos, int delay = 10)
        {
            if (ret != 0)
                return 1;
            ret =   SetPosition(ret, donmePos, lineerPos);
            ret =   IsPositionOK(ret, donmePos, lineerPos);

            if (ret != 0)
            {
                Logger.LogError("Cafe Urun Alma ünitesi SetPositionTask Error.");
            }

            return ret;

        }
        public int SetPosition(int ret, short? donmePos, short? lineerPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (donmePos != null)
            {
                this.urunAlma.UrunAlma_Servo.TargetPosRegisterWrite.Register_Target_Value = (short)donmePos;
                motorList.Add(this.urunAlma.UrunAlma_Servo);
            }
            if (lineerPos != null)
            {
                this.urunAlma.UrunAlma_Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)lineerPos;
                motorList.Add(this.urunAlma.UrunAlma_Lineer);
            }

            ret =   SetMotorPosition(motorList);
            if (ret != 0)
            {
                Logger.LogError("Cafe Urun Alma ünitesi SetPosition Error.");
            }

            return ret;
        }

        public int IsPositionOK(int ret, short? donmePos, short? lineerPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (donmePos != null)
            {
                this.urunAlma.UrunAlma_Servo.TargetPosRegisterWrite.Register_Target_Value = (short)donmePos;
                motorList.Add(this.urunAlma.UrunAlma_Servo);
            }
            if (lineerPos != null)
            {
                this.urunAlma.UrunAlma_Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)lineerPos;
                motorList.Add(this.urunAlma.UrunAlma_Lineer);
            }

            ret =   IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Cafe Urun Alma ünitesi IsPositionOK Error.");
            }

            return ret;
        }



        public bool UrunAlindimi()
        {
            int isSet = -1;
            var sensorOkuResult = SensorOku();
            if (sensorOkuResult != null)
            {
                int sensorValue = sensorOkuResult.CurrentValRegisterRead.Register_Read_Value;
                isSet = sensorValue & (0x0001);
                if (isSet == 1)
                    return false;
                else
                    return true;
            }
            else
            {
                Logger.LogError("Cafe Urun Alma ünitesi UrunAlindimi Error.");
                return false;
            }

        }

        private Sensor SensorOku()
        {
            List<Sensor> sensorList = new List<Sensor>();
            sensorList.Clear();
            sensorList.Add(this.urunAlma.urunAlmaSensor);

            SensorCommandResult ret =   ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList.First();

        }




    }
}
