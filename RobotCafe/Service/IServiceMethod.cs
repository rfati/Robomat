using Common;
using Model;
using RobotCafe.Devices;
using RobotCafe.xarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Service
{

    public interface IServiceMethod
    {
        Task<int> DoService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite, Product product);

        Task<int> GetReadyToService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite);

    }

}
