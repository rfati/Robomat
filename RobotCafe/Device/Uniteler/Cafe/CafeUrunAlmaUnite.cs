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
            //this.maxResponseWaitTime = 500;
            //this.stateChangeTime = 3;
            //this.MAX_TRY_COUNTER = 20;
            this.NextReadDelay = 0;



            for (ushort regaddress = this.urunAlma.FirstReadAddress; regaddress <= this.urunAlma.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }


        }
        public async Task<int> SetPositionTask(int ret, short? donmePos, short? lineerPos, int delay = 10)
        {
            if (ret != 0)
                return 1;
            ret = await SetPosition(ret, donmePos, lineerPos);
            return await IsPositionOK(ret, donmePos, lineerPos);

        }
        public async Task<int> SetPosition(int ret, short? donmePos, short? lineerPos)
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

            return await SetMotorPosition(motorList);
        }

        public async Task<int> IsPositionOK(int ret, short? donmePos, short? lineerPos)
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

            return await IsMotorPositionOK(motorList);
        }

        public async Task<int> ServoPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();

            this.urunAlma.UrunAlma_Servo.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.urunAlma.UrunAlma_Servo);
            return await SetMotorPosition(motorList);
        }

        public async Task<int> LineerPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();

            this.urunAlma.UrunAlma_Lineer.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.urunAlma.UrunAlma_Lineer);
            return await SetMotorPosition(motorList);
        }


        public bool UrunAlindimi()
        {
            int isSet = -1;
            var sensorOkuTask = Task.Run(() => SensorOku());
            sensorOkuTask.Wait();
            if (sensorOkuTask.Result != null)
            {
                int sensorValue = sensorOkuTask.Result.CurrentValRegisterRead.Register_Read_Value;
                isSet = sensorValue & (0x0001);
                if (isSet == 1)
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }

        }

        private async Task<Sensor> SensorOku()
        {
            List<Sensor> sensorList = new List<Sensor>();
            sensorList.Clear();
            sensorList.Add(this.urunAlma.urunAlmaSensor);

            SensorCommandResult ret = await ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList.First();

        }




    }
}
