using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class Vakum
    {

        private ushort Vakum_IsiticiRelay_Target_Pos_RW_Register = 0;
        private ushort Vakum_RobotTutucuRelay_Target_Pos_RW_Register = 1;
        private ushort YikamaBuharRelay_Target_Pos_RW_Register = 2;
        private ushort IsitmaBuharRelay_Target_Pos_RW_Register = 3;

        public Relay Isitici_Vakum { get; set; }
        public Relay RobotTutucu_Vakum { get; set; }
        public Relay IsitmaBuhar { get; set; }
        public Relay YikamaBuhar { get; set; }

        public Vakum()
        {

            this.Isitici_Vakum = new Relay(
                new RegisterWrite(Vakum_IsiticiRelay_Target_Pos_RW_Register, register_Target_Value: 0));


            this.RobotTutucu_Vakum = new Relay(
                new RegisterWrite(Vakum_RobotTutucuRelay_Target_Pos_RW_Register, register_Target_Value: 0));

            this.IsitmaBuhar = new Relay(
                new RegisterWrite(IsitmaBuharRelay_Target_Pos_RW_Register, register_Target_Value: 0));

            this.YikamaBuhar = new Relay(
                new RegisterWrite(YikamaBuharRelay_Target_Pos_RW_Register, register_Target_Value: 0));
        }

    }
}
