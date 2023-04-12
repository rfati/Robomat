using RobotCafe.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using RobotCafe.xarm;
using System.Threading;

namespace RobotCafe.Service
{
    public class HomingService
    {
        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;
        private int OtomatHomingResult;
        private int CafeHomingResult;
        private int OtomatReadyResult;
        private int CafeReadyResult;

        public int HomingResult = 1;
        public int GetReadyToServiceResult = 1;

        public HomingService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            this.otomatUnite = otomatUnite;
            this.robotCafeUnite = robotCafeUnite;
        }


        public void DoHoming()
        {


            DoOtomatHoming();
            //Thread t2 = new Thread(new ThreadStart(DoCafeHoming));
            //t1.Start();
            //t2.Start();

            //t1.Join();
            //t2.Join();

            if (CafeHomingResult == 0)
            {
                HomingResult = 0;
            }
            else
            {
                HomingResult = 1;
            }

        }

        public void GetReadyToService()
        {
            Thread t1 = new Thread(new ThreadStart(DoOtomatReadyToService));
            Thread t2 = new Thread(new ThreadStart(DoCafeReadyToService));
            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            if (OtomatReadyResult == 0 && CafeReadyResult == 0)
            {
                GetReadyToServiceResult = 0;
            }
            else
            {
                GetReadyToServiceResult = 1;
            }

        }

        private void DoOtomatHoming()
        {
            OtomatHomingResult = otomatUnite.DoHoming();
        }

        private void DoCafeHoming()
        {
            CafeHomingResult = robotCafeUnite.DoHoming();
        }

        private void DoOtomatReadyToService()
        {
            OtomatReadyResult = otomatUnite.GetHomingReadyToService();
        }

        private void DoCafeReadyToService()
        {
            CafeReadyResult = robotCafeUnite.GetReadyToService();
        }
    }
}
