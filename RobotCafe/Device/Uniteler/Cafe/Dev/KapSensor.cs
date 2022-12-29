using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class KapSensor
    {
        private ushort Set1Sensor_CurrentVal_R_Register = 0;
        private ushort Set2Sensor_CurrentVal_R_Register = 1;
        private ushort Set3Sensor_CurrentVal_R_Register = 2;
        private ushort Set4Sensor_CurrentVal_R_Register = 3;

        private ushort Kase3Sensor_CurrentVal_R_Register = 4;
        private ushort Kase2Sensor_CurrentVal_R_Register = 5;
        private ushort Kase1Sensor_CurrentVal_R_Register = 6;

        private ushort KaseKapakSensor_CurrentVal_R_Register = 7;

        private ushort Bardak1Sensor_CurrentVal_R_Register = 8;
        private ushort Bardak2Sensor_CurrentVal_R_Register = 9;
        private ushort Bardak3Sensor_CurrentVal_R_Register = 10;

        private ushort BardakKapakSensor_CurrentVal_R_Register = 11;
        private ushort HazirKaseSensor_CurrentVal_R_Register = 12;
        private ushort HazirBardakSensor_CurrentVal_R_Register = 13;
        private ushort HazirKaseKapakSensor_CurrentVal_R_Register = 14;
        private ushort HazirBardakKapakSensor_CurrentVal_R_Register = 15;


        public Sensor Set1_Sensor { get; set; }
        public Sensor Set2_Sensor { get; set; }
        public Sensor Set3_Sensor { get; set; }
        public Sensor Set4_Sensor { get; set; }

        public Sensor Kase1_Sensor { get; set; }
        public Sensor Kase2_Sensor { get; set; }
        public Sensor Kase3_Sensor { get; set; }


        public Sensor Bardak1_Sensor { get; set; }
        public Sensor Bardak2_Sensor { get; set; }
        public Sensor Bardak3_Sensor { get; set; }
        public Sensor KaseKapak_Sensor { get; set; }
        public Sensor BardakKapak_Sensor { get; set; }
        public Sensor HazirKase_Sensor { get; set; }
        public Sensor HazirBardak_Sensor { get; set; }
        public Sensor HazirKaseKapak_Sensor { get; set; }
        public Sensor HazirBardakKapak_Sensor { get; set; }
        public KapSensor()
        {

            this.Set1_Sensor = new Sensor(new RegisterRead(Set1Sensor_CurrentVal_R_Register));
            this.Set2_Sensor = new Sensor(new RegisterRead(Set2Sensor_CurrentVal_R_Register));
            this.Set3_Sensor = new Sensor(new RegisterRead(Set3Sensor_CurrentVal_R_Register));



            this.Kase1_Sensor = new Sensor(new RegisterRead(Kase1Sensor_CurrentVal_R_Register));
            this.Kase2_Sensor = new Sensor(new RegisterRead(Kase2Sensor_CurrentVal_R_Register));
            this.Kase3_Sensor = new Sensor(new RegisterRead(Kase3Sensor_CurrentVal_R_Register));

            this.Bardak1_Sensor = new Sensor(new RegisterRead(Bardak1Sensor_CurrentVal_R_Register));
            this.Bardak2_Sensor = new Sensor(new RegisterRead(Bardak2Sensor_CurrentVal_R_Register));
            this.Bardak3_Sensor = new Sensor(new RegisterRead(Bardak3Sensor_CurrentVal_R_Register));


            this.KaseKapak_Sensor = new Sensor(new RegisterRead(KaseKapakSensor_CurrentVal_R_Register));
            this.BardakKapak_Sensor = new Sensor(new RegisterRead(BardakKapakSensor_CurrentVal_R_Register));

            this.HazirKase_Sensor = new Sensor(new RegisterRead(HazirKaseSensor_CurrentVal_R_Register));
            this.HazirKaseKapak_Sensor = new Sensor(new RegisterRead(HazirKaseKapakSensor_CurrentVal_R_Register));
            this.HazirBardak_Sensor = new Sensor(new RegisterRead(HazirBardakSensor_CurrentVal_R_Register));
            this.HazirBardakKapak_Sensor = new Sensor(new RegisterRead(HazirBardakKapakSensor_CurrentVal_R_Register));
        }

    }

}
