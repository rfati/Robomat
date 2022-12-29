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
    public class OtomatMotorUnite : RTUDevice
    {
        private short RunMotorValue = 256;
        private short StopMotorValue = 728;
        public OtomatMotorUnite(byte slaveAddress)
        {
            this.slaveAddress = slaveAddress;
        }

        public async Task<int> RunMotor(ushort Register_Address, int RunTimeDuration)
        {
            var motorRunTask = Task.Run(() => this.ActuateMotor(Register_Address, Run: true));
            motorRunTask.Wait();
            if (motorRunTask.Result != 0)
            {
                return 1;
            }

            await Task.Delay(RunTimeDuration);

            var motorStopTask = Task.Run(() => this.ActuateMotor(Register_Address, Run: false));
            motorStopTask.Wait();
            if (motorStopTask.Result != 0)
            {
                return 1;
            }

            return 0;

        }

        private async Task<int> ActuateMotor(ushort Register_Address, bool Run)
        {
            RegisterWrite registerWrite;
            if (Run == true)
            {
                registerWrite = new RegisterWrite(Register_Address, RunMotorValue);
            }
            else
            {
                registerWrite = new RegisterWrite(Register_Address, StopMotorValue);
            }
            List<RegisterWrite> RegisterWriteList = new List<RegisterWrite>();
            RegisterWriteList.Add(registerWrite);

            int ret = await this.WriteMultipleRegister(RegisterWriteList);
            if (ret != 0)
            {
                Logger.LogError("OtomatMotorUnite--> ActuateMotor(ushort Register_Address, bool Run)- Register_Address: " + Register_Address.ToString() + "Run: " + Run.ToString() + "return: " + ret.ToString());
                return 1;
            }



            return 0;

        }







    }
}
