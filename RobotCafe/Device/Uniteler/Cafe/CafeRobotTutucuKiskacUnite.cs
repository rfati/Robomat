using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeRobotTutucuKiskacUnite : RTUDevice
    {

        public ushort UstKiskac_Servo_Target_Pos_RW_Register = 0;
        public ushort AltKiskac_Servo_Target_Pos_RW_Register = 1;
        public ushort SagKiskac_Servo_Target_Pos_RW_Register = 2;
        public ushort SolKiskac_Servo_Target_Pos_RW_Register = 3;
        public ushort OrtaKiskac_Servo_Target_Pos_RW_Register = 4;


        public CafeRobotTutucuKiskacUnite()
        {
            this.slaveAddress = 0x02;
            this.maxResponseWaitTime = 500;
            this.stateChangeTime = 3;
            this.MAX_TRY_COUNTER = 20;
            this.NextReadDelay = 0;
            this.state = State.Idle;

        }


        public async Task<int> WriteAllKiskac(int ret, short ustKisKacTarget, short altKisKacTarget, short sagKisKacTarget, short solKisKacTarget, short ortaKisKacTarget, bool sirayla=false)
        {
            if (ret != 0)
                return 1;

            MotorCommandResult result = new MotorCommandResult();

            List<RegisterWrite> RegisterWriteList = new List<RegisterWrite>();
            RegisterWrite ustKiskac = new RegisterWrite(UstKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: ustKisKacTarget);
            RegisterWrite altKiskac = new RegisterWrite(AltKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: altKisKacTarget);
            RegisterWrite sagKiskac = new RegisterWrite(SagKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: sagKisKacTarget);
            RegisterWrite solKiskac = new RegisterWrite(SolKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: solKisKacTarget);
            RegisterWrite ortaKiskac = new RegisterWrite(OrtaKiskac_Servo_Target_Pos_RW_Register, register_Target_Value: ortaKisKacTarget);

            RegisterWriteList.Add(ustKiskac);
            RegisterWriteList.Add(altKiskac);
            RegisterWriteList.Add(sagKiskac);
            RegisterWriteList.Add(solKiskac);
            RegisterWriteList.Add(ortaKiskac);


            for (int i = 0; i < MAX_WRITE_TRY_COUNTER; i++)
            {
                if (sirayla == false)
                {
                    result.retWriteRegisterResult = await this.WriteMultipleRegisterTogether(RegisterWriteList);
                }
                else
                {
                    result.retWriteRegisterResult = await this.WriteMultipleRegister(RegisterWriteList);
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
                return 1;
            }


        }
    }
}
