using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe
{
    public class FunctionCode
    {
        public const byte Read_Multiple_Holding_Registers = 0x03;
        public const byte Write_Single_Holding_Register = 0x06;
        public const byte Write_Multiple_Holding_Registers = 0x16;

    }

    public enum SalesType
    {
        PackageSale,
        ColdServe,
        HotServe
    }

    public enum DeviceState
    {
        Idle,
        CommandSent,
        CommandACKReceived,
        GetStatusSent,
        StatusResponseReceived
    }

}
