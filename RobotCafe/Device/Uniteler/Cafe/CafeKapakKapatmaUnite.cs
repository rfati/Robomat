using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeKapakKapatmaUnite : RTUDevice
    {
        private KapakKapatma kapakKapatma { get; set; }
        public short Yatay_InitKapatma_Pos { get; set; }
        public short Yatay_Verme_Pos { get; set; }
        public short Dikey_Init_Pos { get; set; }
        public short Dikey_Bardak_Pos { get; set; }
        public short Dikey_Kase_Pos { get; set; }

        public CafeKapakKapatmaUnite(RobomatConfig robomatConfig)
        {
            this.Yatay_InitKapatma_Pos = robomatConfig.KapakKapatma_Yatay_InitKapatma_Pos;
            this.Yatay_Verme_Pos = robomatConfig.KapakKapatma_Yatay_Verme_Pos;
            this.Dikey_Init_Pos = robomatConfig.KapakKapatma_Dikey_Init_Pos;
            this.Dikey_Bardak_Pos = robomatConfig.KapakKapatma_Dikey_Bardak_Pos;
            this.Dikey_Kase_Pos = robomatConfig.KapakKapatma_Dikey_Kase_Pos;

            this.slaveAddress = 0x22;
            //this.maxResponseWaitTime = 500;
            //this.stateChangeTime = 3;
            //this.MAX_TRY_COUNTER = 20;
            this.NextReadDelay = 0;

            this.kapakKapatma = new KapakKapatma();


            for (ushort regaddress = this.kapakKapatma.FirstReadAddress; regaddress <= this.kapakKapatma.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }






        }

        public async Task<int> SetPositionTask(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;

            ret = await SetPosition(ret, yatayPos, dikeyPos);
            return await IsPositionOK(ret, yatayPos, dikeyPos);

        }

        public async Task<int> SetPosition(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (yatayPos != null)
            {
                this.kapakKapatma.KapakKapatma_Yatay.TargetPosRegisterWrite.Register_Target_Value = (short)yatayPos;
                motorList.Add(this.kapakKapatma.KapakKapatma_Yatay);
            }
            if (dikeyPos != null)
            {
                this.kapakKapatma.KapakKapatma_Dikey.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.kapakKapatma.KapakKapatma_Dikey);
            }

            return await SetMotorPosition(motorList);
        }

        public async Task<int> IsPositionOK(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (yatayPos != null)
            {
                this.kapakKapatma.KapakKapatma_Yatay.TargetPosRegisterWrite.Register_Target_Value = (short)yatayPos;
                motorList.Add(this.kapakKapatma.KapakKapatma_Yatay);
            }
            if (dikeyPos != null)
            {
                this.kapakKapatma.KapakKapatma_Dikey.TargetPosRegisterWrite.Register_Target_Value = (short)dikeyPos;
                motorList.Add(this.kapakKapatma.KapakKapatma_Dikey);
            }

            return await IsMotorPositionOK(motorList);
        }





        public async Task<int> YatayPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.kapakKapatma.KapakKapatma_Yatay.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.kapakKapatma.KapakKapatma_Yatay);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> DikeyPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.kapakKapatma.KapakKapatma_Dikey.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.kapakKapatma.KapakKapatma_Dikey);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }



    }
}
