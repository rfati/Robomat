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
    public class CafeAsansorUnite : RTUDevice
    {
        private CafeAsansor asansor { get; set; }
        public CafeAsansorUnite()
        {
            this.slaveAddress = 0x41;
            //this.maxResponseWaitTime = 500;
            //this.stateChangeTime = 3;
            //this.MAX_TRY_COUNTER = 20;
            this.NextReadDelay = 0;


            ushort lastReg = 3;
            for (ushort regaddress = 1; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

            this.asansor = new CafeAsansor();
        }


        public async Task<int> SetPosition(short Pos)
        {

            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            motorList.Clear();
            this.asansor.Lineer.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.asansor.Lineer);

            retMotor = await WriteReadMultipleMotor(motorList);

            if (!retMotor.IsSuccess())
            {
                return 1;
            }
            return 0;
        }



        public bool UrunVarmi()
        {
            int isSet = -1;
            var sensorOkuTask = Task.Run(() => SensorOku());
            sensorOkuTask.Wait();
            if (sensorOkuTask.Result != null)
            {
                int sensorValue = sensorOkuTask.Result.CurrentValRegisterRead.Register_Read_Value;
                isSet = sensorValue & (0x0001);
                if (isSet == 0)
                    return true;
                else
                    return false;
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
            sensorList.Add(this.asansor.urunTeslimSensor);

            SensorCommandResult ret = await ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList.First();

        }

        public async Task<int> SetPositionTask(int ret, short? dikeyPos)
        {
            if (ret != 0)
                return 1;

            ret = await SetPosition(ret, dikeyPos);
            return await IsPositionOK(ret, dikeyPos);
        }
        public async Task<int> SetPosition(int ret, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (dikeyPos != null)
            {
                this.asansor.Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.asansor.Lineer);
            }

            return await SetMotorPosition(motorList);
        }

        public async Task<int> IsPositionOK(int ret, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (dikeyPos != null)
            {
                this.asansor.Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.asansor.Lineer);
            }

            return await IsMotorPositionOK(motorList);
        }
    }
}
