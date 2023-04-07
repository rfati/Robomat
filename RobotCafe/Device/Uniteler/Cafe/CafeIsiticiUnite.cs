using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeIsiticiUnite : RTUDevice
    {


        private Isitici isitici { get; set; }
        public short YikamaStep_Init_Pos { get; set; }
        public short YikamaStep_Yikama_Pos { get; set; }
        public short VakumServo_Acik_Pos { get; set; }
        public short VakumServo_Kapali_Pos { get; set; }
        public short ProbServo_Yukari_Pos { get; set; }
        public short ProbServo_Asagi_Pos { get; set; }
        public CafeIsiticiUnite(RobomatConfig robomatConfig)
        {

            this.YikamaStep_Init_Pos = robomatConfig.Isitici_YikamaStep_Init_Pos;
            this.YikamaStep_Yikama_Pos = robomatConfig.Isitici_YikamaStep_Yikama_Pos;
            this.VakumServo_Acik_Pos = robomatConfig.Isitici_VakumServo_Acik_Pos;
            this.VakumServo_Kapali_Pos = robomatConfig.Isitici_VakumServo_Kapali_Pos;
            this.ProbServo_Yukari_Pos = robomatConfig.Isitici_ProbServo_Yukari_Pos;
            this.ProbServo_Asagi_Pos = robomatConfig.Isitici_ProbServo_Asagi_Pos;

            this.slaveAddress = 0x25;
            this.isitici = new Isitici();


            for (ushort regaddress = this.isitici.FirstReadAddress; regaddress <= this.isitici.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

        }


        public int SetPositionTask(int ret, short? probPos)
        {
            if (ret != 0)
                return 1;
            ret = SetPosition(ret, probPos);
            ret = IsPositionOK(ret, probPos);


            if (ret != 0)
            {
                Logger.LogError("Isıtıcı ünitesi SetPositionTask Error.");
            }

            return ret;
        }

        public int SetPosition(int ret, short? probPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            
            if (probPos != null)
            {
                this.isitici.Isitici_ProbServo.TargetPosRegisterWrite.Register_Target_Value = (short)probPos;
                motorList.Add(this.isitici.Isitici_ProbServo);
            }

            ret =  SetMotorPosition(motorList);
            if (ret != 0)
            {
                Logger.LogError("Isıtıcı ünitesi SetPosition Error.");
            }

            return ret;
        }

        public int IsPositionOK(int ret, short? probPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
           
            if (probPos != null)
            {
                this.isitici.Isitici_ProbServo.TargetPosRegisterWrite.Register_Target_Value = (short)probPos;
                motorList.Add(this.isitici.Isitici_ProbServo);
            }

            ret = IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Isıtıcı ünitesi IsPositionOK Error.");
            }

            return ret;
        }




        public int RunIsiticiBuhar(int ret, bool run)
        {
            if (ret != 0)
                return 1;
            List<Relay> relayList = new List<Relay>();
            relayList.Clear();

            if(run == true)
            {
                this.isitici.Isitici_Buhar.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.isitici.Isitici_Buhar.TargetPosRegisterWrite.Register_Target_Value = 0;
            }

            relayList.Add(this.isitici.Isitici_Buhar);
            RelayCommandResult retRelay = WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                Logger.LogError("Isıtıcı ünitesi RunIsiticiBuhar Error.");
                return 1;
            }

            return 0;
        }


        public int RunIsiticiONOFF(int ret, bool run)
        {
            List<Relay> relayList = new List<Relay>();
            relayList.Clear();

            if(run == true)
            {
                
                this.isitici.Isitici_ONOFF.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.isitici.Isitici_ONOFF.TargetPosRegisterWrite.Register_Target_Value = 0;
            }

            relayList.Add(this.isitici.Isitici_ONOFF);
            RelayCommandResult retRelay = WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                Logger.LogError("Isıtıcı ünitesi RunIsiticiONOFF Error.");
                return 1;
            }


            return 0;
        }

        public int RunIsiticiProbeBuharONOFF(int ret,bool run)
        {
            if (ret != 0)
                return 1;
            List<Relay> relayList = new List<Relay>();
            relayList.Clear();

            if (run == true)
            {   
                this.isitici.Isitici_Probe_Buhar.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.isitici.Isitici_Probe_Buhar.TargetPosRegisterWrite.Register_Target_Value = 0;
            }

            relayList.Add(this.isitici.Isitici_Probe_Buhar);
            RelayCommandResult retRelay = WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                Logger.LogError("Isıtıcı ünitesi RunIsiticiProbeBuharONOFF Error.");
                return 1;
            }

            return 0;
        }

        public int RunIsiticiYikamaBuharONOFF(int ret, bool run)
        {
            List<Relay> relayList = new List<Relay>();
            relayList.Clear();

            if (run == true)        
            {
                this.isitici.Isitici_Yikama_Buhar.TargetPosRegisterWrite.Register_Target_Value = 1;
                
            }
            else
            {
                this.isitici.Isitici_Yikama_Buhar.TargetPosRegisterWrite.Register_Target_Value =0;

            }

            relayList.Add(this.isitici.Isitici_Yikama_Buhar);
            RelayCommandResult retRelay = WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                Logger.LogError("Isıtıcı ünitesi RunIsiticiYikamaBuharONOFF Error.");
                return 1;
            }


            return 0;
        }

    }
}
