using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RobotCafe.Devices
{
    public class Relay
    {
        public RegisterWrite TargetPosRegisterWrite { get; set; }

        public Relay(RegisterWrite TargetPosRegisterWrite)
        {
            this.TargetPosRegisterWrite = TargetPosRegisterWrite;
        }

    }
}
