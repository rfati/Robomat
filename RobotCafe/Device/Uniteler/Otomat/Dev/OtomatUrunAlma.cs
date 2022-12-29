using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class OtomatUrunAlma
    {
        /// <summary>
        /// Urun Alma Unitesi
        /// </summary>
        private ushort Donme_Target_Pos_RW_Register = 0;
        private ushort Kiskac_Sag_Target_Pos_RW_Register = 1;
        private ushort Kiskac_Sol_Target_Pos_RW_Register = 2;
        private ushort UrunAtma_Target_Pos_RW_Register = 3;
        private ushort Tilt_Target_Pos_RW_Register = 4;
        private ushort IleriGeri_Target_Pos_RW_Register = 5;


        private ushort Donme_Current_Pos_R_Register = 7;
        private ushort Kiskac_Sag_Current_Pos_R_Register = 8;
        private ushort Kiskac_Sol_Current_Pos_R_Register = 9;
        private ushort UrunAtma_Current_Pos_R_Register = 10;
        private ushort Tilt_Current_Pos_R_Register = 11;
        private ushort IleriGeri_Current_Pos_R_Register = 12;


        private ushort Donme_Pos_Status_R_Register = 13;
        private ushort Kiskac_Sag_Pos_Status_R_Register = 14;
        private ushort Kiskac_Sol_Pos_Status_R_Register = 15;
        private ushort UrunAtma_Pos_Status_R_Register = 16;
        private ushort Tilt_Pos_Status_R_Register = 17;
        private ushort IleriGeri_Status_Pos_R_Register = 18;

        private ushort Sensor_CurrentVal_R_Register = 19;


        public Motor Donme { get; set; }
        public Motor Kiskac_Sag { get; set; }
        public Motor Kiskac_Sol { get; set; }
        public Motor UrunAtma { get; set; }
        public Motor Tilt { get; set; }
        public Sensor urunAlmaSensor { get; set; }
        public Motor IleriGeri { get; set; }
        public OtomatUrunAlma()
        {
            
            this.Donme = new Motor(
                new RegisterWrite(Donme_Target_Pos_RW_Register, register_Target_Value: 0), 
                new RegisterRead(Donme_Pos_Status_R_Register),
                new RegisterRead(Donme_Current_Pos_R_Register));


            this.Kiskac_Sag = new Motor(
                new RegisterWrite(Kiskac_Sag_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Kiskac_Sag_Pos_Status_R_Register),
                new RegisterRead(Kiskac_Sag_Current_Pos_R_Register));

            this.Kiskac_Sol = new Motor(
                new RegisterWrite(Kiskac_Sol_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Kiskac_Sol_Pos_Status_R_Register),
                new RegisterRead(Kiskac_Sol_Current_Pos_R_Register));

            this.UrunAtma = new Motor(
                new RegisterWrite(UrunAtma_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(UrunAtma_Pos_Status_R_Register),
                new RegisterRead(UrunAtma_Current_Pos_R_Register));

            this.Tilt = new Motor(
                new RegisterWrite(Tilt_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Tilt_Pos_Status_R_Register),
                new RegisterRead(Tilt_Current_Pos_R_Register));
                Task.Delay(500);

            this.IleriGeri = new Motor(
                new RegisterWrite(IleriGeri_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(IleriGeri_Status_Pos_R_Register),
                new RegisterRead(IleriGeri_Current_Pos_R_Register));

            this.urunAlmaSensor = new Sensor(new RegisterRead(Sensor_CurrentVal_R_Register));

        }



    }


}
