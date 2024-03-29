﻿using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{

    public class CafeVakumUnite : RTUDevice
    {
        private Vakum vakum { get; set; }

        public CafeVakumUnite()
        {
            this.vakum = new Vakum();
            this.slaveAddress = 0x27;

        }

        public async Task<int> RunRobotTutucuVakum(int ret, bool run)
        {
            if (ret != 0)
                return 1;

            List<Relay> relayList = new List<Relay>();
            relayList.Clear();

            if (run == true)
            {
                this.vakum.RobotTutucu_Vakum.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.vakum.RobotTutucu_Vakum.TargetPosRegisterWrite.Register_Target_Value = 0;
            }


            relayList.Add(this.vakum.RobotTutucu_Vakum);

            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }


            return 0;

        }


        public async Task<int> RunIsiticiVakum(int ret, bool run)
        {
            if (ret != 0)
                return 1;
            List<Relay> relayList = new List<Relay>();
            if (run == true)
            {
                this.vakum.Isitici_Vakum.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.vakum.Isitici_Vakum.TargetPosRegisterWrite.Register_Target_Value = 0;
            }
            relayList.Add(this.vakum.Isitici_Vakum);
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }
            return 0;
        }

        public async Task<int> RunYikamaBuhar(int ret, bool run)
        {
            if (ret != 0)
                return 1;
            List<Relay> relayList = new List<Relay>();
            if (run == true)
            {
                this.vakum.YikamaBuhar.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.vakum.YikamaBuhar.TargetPosRegisterWrite.Register_Target_Value = 0;
            }
            relayList.Add(this.vakum.YikamaBuhar);
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }
            return 0;
        }

        public async Task<int> RunIsitmaBuhar(int ret, bool run)
        {
            if (ret != 0)
                return 1;
            List<Relay> relayList = new List<Relay>();
            if (run == true)
            {
                this.vakum.IsitmaBuhar.TargetPosRegisterWrite.Register_Target_Value = 1;
            }
            else
            {
                this.vakum.IsitmaBuhar.TargetPosRegisterWrite.Register_Target_Value = 0;
            }
            relayList.Add(this.vakum.IsitmaBuhar);
            RelayCommandResult retRelay = await WriteMultipleRelay(relayList);
            if (!retRelay.IsSuccess())
            {
                return 1;
            }
            return 0;
        }


    }
}
