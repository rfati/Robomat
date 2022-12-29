using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class Kesici
    {
        /// <summary>
        /// Kesici Unitesi
        /// </summary>

        private ushort Kesici_Lineer_Target_Pos_RW_Register = 0;
        private ushort Kesici_Servo_Target_Pos_RW_Register = 1;
        private ushort Kesici_Lineer_Current_Pos_R_Register = 2;
        private ushort Kesici_Servo_Current_Pos_R_Register = 3;
        private ushort Kesici_Lineer_Pos_Status_R_Register = 4;
        private ushort Kesici_Servo_Pos_Status_R_Register = 5;

        public ushort FirstReadAddress = 2;
        public ushort LastReadAddress = 5;

        public Motor Kesici_Servo { get; set; }
        public Motor Kesici_Lineer { get; set; }
        public Kesici()
        {

            this.Kesici_Servo = new Motor(
                new RegisterWrite(Kesici_Servo_Target_Pos_RW_Register, register_Target_Value: 5), 
                new RegisterRead(Kesici_Servo_Pos_Status_R_Register),
                new RegisterRead(Kesici_Servo_Current_Pos_R_Register));


            this.Kesici_Lineer = new Motor(
                new RegisterWrite(Kesici_Lineer_Target_Pos_RW_Register, register_Target_Value: 0), 
                new RegisterRead(Kesici_Lineer_Pos_Status_R_Register),
                new RegisterRead(Kesici_Lineer_Current_Pos_R_Register));

        }

    }


}
