using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class KapakKapatma
    {
        /// <summary>
        /// Kapak Kapatma Unitesi
        /// </summary>

        private ushort KapakKapatma_Yatay_Target_Pos_RW_Register = 0;
        private ushort KapakKapatma_Dikey_Target_Pos_RW_Register = 1;
        private ushort KapakKapatma_Yatay_Current_Pos_R_Register = 2;
        private ushort KapakKapatma_Dikey_Current_Pos_R_Register = 3;
        private ushort KapakKapatma_Yatay_Pos_Status_R_Register = 4;
        private ushort KapakKapatma_Dikey_Pos_Status_R_Register = 5;

        public ushort FirstReadAddress = 2;
        public ushort LastReadAddress = 5;

        public Motor KapakKapatma_Dikey { get; set; }
        public Motor KapakKapatma_Yatay { get; set; }
        public KapakKapatma()
        {

            this.KapakKapatma_Dikey = new Motor(
                new RegisterWrite(KapakKapatma_Dikey_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(KapakKapatma_Dikey_Pos_Status_R_Register),
                new RegisterRead(KapakKapatma_Dikey_Current_Pos_R_Register));

            this.KapakKapatma_Yatay = new Motor(
                new RegisterWrite(KapakKapatma_Yatay_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(KapakKapatma_Yatay_Pos_Status_R_Register),
                new RegisterRead(KapakKapatma_Yatay_Current_Pos_R_Register));


        }


    }


}
