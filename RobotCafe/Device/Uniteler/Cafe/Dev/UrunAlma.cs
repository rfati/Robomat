using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class UrunAlma
    {
        /// <summary>
        /// Urun Alma Unitesi
        /// </summary>

        private ushort UrunAlma_Servo_Target_Pos_RW_Register = 0;
        private ushort UrunAlma_Lineer_Target_Pos_RW_Register = 1;


        private ushort UrunAlma_Servo_Current_Pos_R_Register = 3;
        private ushort UrunAlma_Lineer_Current_Pos_R_Register = 4;
        private ushort Sensor_CurrentVal_R_Register = 5;

        private ushort UrunAlma_Servo_Pos_Status_R_Register = 6;
        private ushort UrunAlma_Lineer_Pos_Status_R_Register = 7;


        public ushort FirstReadAddress = 3;
        public ushort LastReadAddress = 7;

        public Motor UrunAlma_Lineer { get; set; }
        public Motor UrunAlma_Servo { get; set; }

        public Sensor urunAlmaSensor { get; set; }
        public UrunAlma()
        {
            
            this.UrunAlma_Lineer = new Motor(
                new RegisterWrite(UrunAlma_Lineer_Target_Pos_RW_Register, register_Target_Value: 0), 
                new RegisterRead(UrunAlma_Lineer_Pos_Status_R_Register),
                new RegisterRead(UrunAlma_Lineer_Current_Pos_R_Register));


            this.UrunAlma_Servo = new Motor(
                new RegisterWrite(UrunAlma_Servo_Target_Pos_RW_Register, register_Target_Value: 85),
                new RegisterRead(UrunAlma_Servo_Pos_Status_R_Register),
                new RegisterRead(UrunAlma_Servo_Current_Pos_R_Register));

            this.urunAlmaSensor = new Sensor(new RegisterRead(Sensor_CurrentVal_R_Register));

        }



    }


}
