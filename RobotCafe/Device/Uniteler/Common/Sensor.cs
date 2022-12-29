using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RobotCafe.Devices
{
    public class Sensor
    {
        public RegisterRead CurrentValRegisterRead { get; set; }

        public Sensor(RegisterRead CurrentValRegisterRead)
        {
            this.CurrentValRegisterRead = CurrentValRegisterRead;
        }

    }
}
