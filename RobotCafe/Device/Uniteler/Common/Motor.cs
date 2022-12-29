using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RobotCafe.Devices
{
    public class Motor
    {
        public RegisterWrite TargetPosRegisterWrite { get; set; }
        public RegisterRead PosStatusRegisterRead { get; set; }
        public RegisterRead CurrentPosRegisterRead { get; set; }

        public Motor(RegisterWrite TargetPosRegisterWrite, RegisterRead PosStatusRegisterRead, RegisterRead CurrentPosRegisterRead)
        {
            this.TargetPosRegisterWrite = TargetPosRegisterWrite;
            this.PosStatusRegisterRead = PosStatusRegisterRead;
            this.CurrentPosRegisterRead = CurrentPosRegisterRead;
        }

    }
}
