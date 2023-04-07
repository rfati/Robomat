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

            this.kapakKapatma = new KapakKapatma();


            for (ushort regaddress = this.kapakKapatma.FirstReadAddress; regaddress <= this.kapakKapatma.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }






        }

        public int SetPositionTask(int ret, short? yatayPos, short? dikeyPos)
        {
            if (ret != 0)
                return 1;

            ret = SetPosition(ret, yatayPos, dikeyPos);
            ret = IsPositionOK(ret, yatayPos, dikeyPos);

            if (ret != 0)
            {
                Logger.LogError("Kapak Kapatma ünitesi SetPositionTask Error.");
            }

            return ret;

        }

        public int SetPosition(int ret, short? yatayPos, short? dikeyPos)
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

            ret = SetMotorPosition(motorList);
            if (ret != 0)
            {
                Logger.LogError("Kapak Kapatma ünitesi SetPosition Error.");
            }

            return ret;
        }

        public int IsPositionOK(int ret, short? yatayPos, short? dikeyPos)
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

            ret = IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Kapak Kapatma ünitesi IsPositionOK Error.");
            }

            return ret;
        }




    }
}
