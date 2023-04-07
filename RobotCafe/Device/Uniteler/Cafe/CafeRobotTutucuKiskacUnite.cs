using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeRobotTutucuKiskacUnite : RTUDevice
    {
        public ushort AltKiskac_Servo_Target_Pos_RW_Register = 1;
        public ushort SagKiskac_Servo_Target_Pos_RW_Register = 2;
        public ushort SolKiskac_Servo_Target_Pos_RW_Register = 3;

        public CafeRobotTutucuKiskacUnite()
        {
            this.slaveAddress = 0x02;

        }


        public int WriteAllKiskac(int ret, short altKisKacTarget, short sagKisKacTarget, short solKisKacTarget, bool sirayla=false)
        {
            if (ret != 0)
                return 1;

            MotorCommandResult result = new MotorCommandResult();

            List<RegisterWrite> RegisterWriteList = new List<RegisterWrite>();
            RegisterWrite altKiskac = new RegisterWrite(AltKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: altKisKacTarget);
            RegisterWrite sagKiskac = new RegisterWrite(SagKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: sagKisKacTarget);
            RegisterWrite solKiskac = new RegisterWrite(SolKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: solKisKacTarget);

            RegisterWriteList.Add(altKiskac);
            RegisterWriteList.Add(sagKiskac);
            RegisterWriteList.Add(solKiskac);


            for (int i = 0; i < MAX_WRITE_TRY_COUNTER; i++)
            {
                if (sirayla == false)
                {
                    result.retWriteRegisterResult = this.WriteMultipleRegisterTogether(RegisterWriteList);
                }
                else
                {
                    result.retWriteRegisterResult = this.WriteMultipleRegister(RegisterWriteList);
                }

                if (result.retWriteRegisterResult == 0)
                {
                    break;
                }
            }

            if (result.retWriteRegisterResult == 0)
            {
                return 0;
            }
            else
            {
                Logger.LogError("Cafe Robot tutucu WriteKiskac Error.");
                return 1;
            }


        }
    }
}
