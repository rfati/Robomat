using Common;
using Model;
using RobotCafe.Devices;
using RobotCafe.Serial;
using RobotCafe.xarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotCafe.Service
{

    public class SaleService
    {

        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;


        private IServiceMethod method;
        public SaleService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            this.otomatUnite = otomatUnite;
            this.robotCafeUnite = robotCafeUnite;
        }

        public int DoService(Product product)
        {
            int ret = this.method.DoService(this.otomatUnite, this.robotCafeUnite, product);
            return ret;
        }

        public void SetServiceMethod(IServiceMethod method)
        {
            this.method = method;
        }

    }

}
