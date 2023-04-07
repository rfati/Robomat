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
    public class OtomatMotorUnite : RTUDevice
    {
        private short RunMotorValue = 256;
        private short StopMotorValue = 728;
        public OtomatMotorUnite(byte slaveAddress)
        {
            this.slaveAddress = slaveAddress;
        }

        public int RunMotor(ushort Register_Address, int RunTimeDuration)
        {
            var motorRunResult = this.ActuateMotor(Register_Address, Run: true);
            if (motorRunResult != 0)
            {
                Logger.LogError("Otomat Motor Run:True Error.");
                return 1;
            }

            Thread.Sleep(RunTimeDuration);

            var motorStopResult = this.ActuateMotor(Register_Address, Run: false);
            if (motorStopResult != 0)
            {
                Logger.LogError("Otomat Motor Run:False Error.");
                return 1;
            }

            return 0;

        }

        private int ActuateMotor(ushort Register_Address, bool Run)
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

            int ret = this.WriteMultipleRegister(RegisterWriteList);
            if (ret != 0)
            {
                Logger.LogError("OtomatMotorUnite--> ActuateMotor(ushort Register_Address, bool Run)- Register_Address: " + Register_Address.ToString() + "Run: " + Run.ToString() + "return: " + ret.ToString());
                return 1;
            }



            return 0;

        }







    }
}
