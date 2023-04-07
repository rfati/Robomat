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

            ushort lastReg = 3;
            for (ushort regaddress = 1; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

            this.asansor = new CafeAsansor();
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

        private Sensor SensorOku()
        {
            List<Sensor> sensorList = new List<Sensor>();
            sensorList.Clear();
            sensorList.Add(this.asansor.urunTeslimSensor);

            SensorCommandResult ret = ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList.First();

        }

        public int SetPositionTask(int ret, short? dikeyPos)
        {
            if (ret != 0)
                return 1;

            ret = SetPosition(ret, dikeyPos);
            ret = IsPositionOK(ret, dikeyPos);

            if (ret != 0)
            {
                Logger.LogError("Cafe Asansör sunum ünitesi SetPositionTask Error.");
            }

            return ret;

        }
        public int SetPosition(int ret, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (dikeyPos != null)
            {
                this.asansor.Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.asansor.Lineer);
            }

            ret = SetMotorPosition(motorList);
            if (ret != 0)
            {
                Logger.LogError("Cafe Asansör sunum ünitesi SetPosition Error.");
            }

            return ret;
        }

        public int IsPositionOK(int ret, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (dikeyPos != null)
            {
                this.asansor.Lineer.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.asansor.Lineer);
            }

            ret = IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Cafe Asansör sunum ünitesi IsPositionOK Error.");
            }

            return ret;
        }
    }
}
