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
            //this.maxResponseWaitTime = 500;
            //this.stateChangeTime = 3;
            //this.MAX_TRY_COUNTER = 20;
            this.NextReadDelay = 0;
            this.isitici = new Isitici();


            for (ushort regaddress = this.isitici.FirstReadAddress; regaddress <= this.isitici.LastReadAddress; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

        }


        public async Task<int> SetPositionTask(int ret, short? yikamaPos, short? vakumPos, short? probPos)
        {
            if (ret != 0)
                return 1;
            ret = await SetPosition(ret, yikamaPos, vakumPos, probPos);
            return await IsPositionOK(ret, yikamaPos, vakumPos, probPos);
        }

        public async Task<int> SetPosition(int ret, short? yikamaPos, short? vakumPos, short? probPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (yikamaPos != null)
            {
                this.isitici.Isitici_YikamaStep.TargetPosRegisterWrite.Register_Target_Value = (short)yikamaPos;
                motorList.Add(this.isitici.Isitici_YikamaStep);
            }
            if (vakumPos != null)
            {
                this.isitici.Isitici_VakumServo.TargetPosRegisterWrite.Register_Target_Value = (short)vakumPos;
                motorList.Add(this.isitici.Isitici_VakumServo);
            }
            if (probPos != null)
            {
                this.isitici.Isitici_ProbServo.TargetPosRegisterWrite.Register_Target_Value = (short)probPos;
                motorList.Add(this.isitici.Isitici_ProbServo);
            }

            return await SetMotorPosition(motorList);
        }

        public async Task<int> IsPositionOK(int ret, short? yikamaPos, short? vakumPos, short? probPos)
        {
            if (ret != 0)
                return 1;
            List<Motor> motorList = new List<Motor>();
            if (yikamaPos != null)
            {
                this.isitici.Isitici_YikamaStep.TargetPosRegisterWrite.Register_Target_Value = (short)yikamaPos;
                motorList.Add(this.isitici.Isitici_YikamaStep);
            }
            if (vakumPos != null)
            {
                this.isitici.Isitici_VakumServo.TargetPosRegisterWrite.Register_Target_Value = (short)vakumPos;
                motorList.Add(this.isitici.Isitici_VakumServo);
            }
            if (probPos != null)
            {
                this.isitici.Isitici_ProbServo.TargetPosRegisterWrite.Register_Target_Value = (short)probPos;
                motorList.Add(this.isitici.Isitici_ProbServo);
            }

            return await IsMotorPositionOK(motorList);
        }



        public async Task<int> VakumServoPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.isitici.Isitici_VakumServo.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.isitici.Isitici_VakumServo);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> ProbServoPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.isitici.Isitici_ProbServo.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.isitici.Isitici_ProbServo);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> YikamaStepPosAyarla(short Pos)
        {
            List<Motor> motorList = new List<Motor>();
            MotorCommandResult retMotor = null;

            this.isitici.Isitici_YikamaStep.TargetPosRegisterWrite.Register_Target_Value = Pos;
            motorList.Add(this.isitici.Isitici_YikamaStep);
            retMotor = await WriteReadMultipleMotor(motorList);
            if (!retMotor.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> RunIsiticiBuhar(int run)
        {
            List<Relay> relayList = new List<Relay>();
            relayList.Clear();
            this.isitici.Isitici_Buhar.TargetPosRegisterWrite.Register_Target_Value = ((short)run);
            relayList.Add(this.isitici.Isitici_Buhar);
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }

            //await Task.Delay(RunTimeDuration);

            //relayList.Clear();
            //this.isitici.Isitici_Buhar.TargetPosRegisterWrite.Register_Target_Value = 0;
            //relayList.Add(this.isitici.Isitici_Buhar);
            //retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> RunIsiticiBuhar(int ret, bool run)
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
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }

            return 0;
        }


        public async Task<int> RunIsiticiONOFF(bool run)
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
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }


            return 0;
        }

        public async Task<int> RunIsiticiProbeBuharONOFF(bool run)
        {
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
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }

            return 0;
        }

        public async Task<int> RunIsiticiYikamaBuharONOFF(bool run)
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
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }


            return 0;
        }

    }
}
