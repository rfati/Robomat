using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class Asansor
    {

        private ushort Asansor_YatayStep_Target_Pos_RW_Register = 0;
        private ushort Asansor_DikeyStep_Target_Pos_RW_Register = 1;

        private ushort Asansor_YatayStep_Current_Pos_R_Register = 2;
        private ushort Asansor_DikeyStep_Current_Pos_R_Register = 3;
        private ushort Asansor_YatayStep_Pos_Status_R_Register = 4;
        private ushort Asansor_DikeyStep_Pos_Status_R_Register = 5;


        public Motor Asansor_DikeyStep { get; set; }
        public Motor Asansor_YatayStep { get; set; }
        public Asansor()
        {
            this.Asansor_DikeyStep = new Motor(
                new RegisterWrite(Asansor_DikeyStep_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Asansor_DikeyStep_Pos_Status_R_Register),
                new RegisterRead(Asansor_DikeyStep_Current_Pos_R_Register));

            this.Asansor_YatayStep = new Motor(
                new RegisterWrite(Asansor_YatayStep_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Asansor_YatayStep_Pos_Status_R_Register),
                new RegisterRead(Asansor_YatayStep_Current_Pos_R_Register));

        }


    }


}
