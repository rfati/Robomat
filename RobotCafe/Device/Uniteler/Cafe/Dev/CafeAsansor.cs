using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class CafeAsansor
    {

        private ushort Lineer_Target_Pos_RW_Register = 0;

        private ushort Lineer_Current_Pos_R_Register = 2;

        private ushort Lineer_Pos_Status_R_Register = 1;

        private ushort Sensor_CurrentVal_R_Register = 3;


        public Motor Lineer { get; set; }
        public Sensor urunTeslimSensor { get; set; }
        public CafeAsansor()
        {
            
            this.Lineer = new Motor(
                new RegisterWrite(Lineer_Target_Pos_RW_Register, register_Target_Value: 0), 
                new RegisterRead(Lineer_Pos_Status_R_Register),
                new RegisterRead(Lineer_Current_Pos_R_Register));

            this.urunTeslimSensor = new Sensor(new RegisterRead(Sensor_CurrentVal_R_Register));

        }



    }


}
