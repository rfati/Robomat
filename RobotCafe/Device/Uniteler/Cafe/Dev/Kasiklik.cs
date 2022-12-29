using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class Kasiklik
    {
        /// <summary>
        /// Kaşıklık Unitesi 
        /// </summary>

        private ushort Kasiklik_DikeyStep_Target_Pos_RW_Register = 1;
        private ushort Kasiklik_YatayStep_Target_Pos_RW_Register = 2;
        private ushort Kasiklik_Vakum_Target_Pos_RW_Register = 11;
        private ushort Kasiklik_DikeyStep_Current_Pos_R_Register = 20;
        private ushort Kasiklik_YatayStep_Current_Pos_R_Register = 21;
        private ushort Kasiklik_DikeyStep_Pos_Status_R_Register = 31;
        private ushort Kasiklik_YatayStep_Pos_Status_R_Register = 32;

        public Motor Kasiklik_DikeyStep { get; set; }
        public Motor Kasiklik_YatayStep { get; set; }
        public Relay Kasiklik_Vakum { get; set; }
        public Kasiklik()
        {
            this.Kasiklik_DikeyStep = new Motor(
                new RegisterWrite(Kasiklik_DikeyStep_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Kasiklik_DikeyStep_Pos_Status_R_Register),
                new RegisterRead(Kasiklik_DikeyStep_Current_Pos_R_Register));

            this.Kasiklik_YatayStep = new Motor(
                new RegisterWrite(Kasiklik_YatayStep_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Kasiklik_YatayStep_Pos_Status_R_Register),
                new RegisterRead(Kasiklik_YatayStep_Current_Pos_R_Register));

           this.Kasiklik_Vakum = new Relay(
                new RegisterWrite(Kasiklik_Vakum_Target_Pos_RW_Register, register_Target_Value: 0));
        }


    }


}
