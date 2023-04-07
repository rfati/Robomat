using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
   
    public class Isitici
    {
        /// <summary>
        /// Isıtıcı Unitesi
        /// </summary>
        private ushort Isitici_Yikama_Step_Target_Pos_RW_Register = 0;
        private ushort Isitici_Vakum_Servo_Target_Pos_RW_Register = 1;
        private ushort Isitici_Prob_Servo_Target_Pos_RW_Register = 2;   //
        private ushort Isitici_ONOFF_Target_Pos_RW_Register = 3;    //
        private ushort Isitici_Buhar_ONOFF_Target_Pos_RW_Register = 4;  //

        private ushort Isitici_Probe_Buhar_Target_Pos_RW_Register = 5;  //
        private ushort Isitici_Yikama_Buhar_Target_Pos_RW_Register = 6; //


        private ushort Isitici_Yikama_Step_Current_Pos_R_Register = 7;
        private ushort Isitici_Vakum_Servo_Current_Pos_R_Register = 8;
        private ushort Isitici_Prob_Servo_Current_Pos_R_Register = 9;
        private ushort Isitici_ONOFF_Current_Pos_R_Register = 10;
        private ushort Isitici_Buhar_Current_Pos_R_Register = 11;
        private ushort Isitici_Probe_Buhar_ONOFF_Current_Pos_R_Register = 12;
        private ushort Isitici_Yikama_Buhar_ONOFF_Current_Pos_R_Register = 13;


        private ushort Isitici_Yikama_Step_Pos_Status_R_Register = 14;
        private ushort Isitici_Vakum_Servo_Pos_Status_R_Register = 15;
        private ushort Isitici_Prob_Servo_Pos_Status_R_Register = 16;
        private ushort Isitici_ONOFF_Pos_Status_R_Register = 17;
        private ushort Isitici_Buhar_Pos_Status_R_Register = 18;

        private ushort Isitici_Probe_Buhar_Status_R_Register = 19;
        private ushort Isitici_Yikama_Buhar_Status_R_Register = 20;


        public ushort FirstReadAddress = 7;
        public ushort LastReadAddress = 20;

        public Motor Isitici_ProbServo { get; set; }
        public Motor Isitici_YikamaStep { get; set; }
        public Motor Isitici_VakumServo { get; set; }

        public Relay Isitici_Buhar { get; set; }
        public Relay Isitici_ONOFF{ get; set; }

        public Relay Isitici_Probe_Buhar { get; set; }
        public Relay Isitici_Yikama_Buhar { get; set; }

        public Isitici()
        {
            this.Isitici_ProbServo = new Motor(
                new RegisterWrite(Isitici_Prob_Servo_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Isitici_Prob_Servo_Pos_Status_R_Register),
                new RegisterRead(Isitici_Prob_Servo_Current_Pos_R_Register));

            this.Isitici_YikamaStep = new Motor(
                new RegisterWrite(Isitici_Yikama_Step_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Isitici_Yikama_Step_Pos_Status_R_Register),
                new RegisterRead(Isitici_Yikama_Step_Current_Pos_R_Register));

            this.Isitici_VakumServo = new Motor(
                new RegisterWrite(Isitici_Vakum_Servo_Target_Pos_RW_Register, register_Target_Value: 0),
                new RegisterRead(Isitici_Vakum_Servo_Pos_Status_R_Register),
                new RegisterRead(Isitici_Vakum_Servo_Current_Pos_R_Register));



            this.Isitici_Buhar = new Relay(
                new RegisterWrite(Isitici_Buhar_ONOFF_Target_Pos_RW_Register, register_Target_Value: 0));


            this.Isitici_ONOFF = new Relay(
                new RegisterWrite(Isitici_ONOFF_Target_Pos_RW_Register, register_Target_Value: 0));

            this.Isitici_Probe_Buhar = new Relay(
                new RegisterWrite(Isitici_Probe_Buhar_Target_Pos_RW_Register, register_Target_Value: 0));


            this.Isitici_Yikama_Buhar = new Relay(
                new RegisterWrite(Isitici_Yikama_Buhar_Target_Pos_RW_Register, register_Target_Value: 0));
        }

    }
}
