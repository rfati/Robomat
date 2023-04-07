using BLL;
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

    public class OtomatUnite
    {
        public List<OtomatMotorUnite> otomatMotorUniteList;
        public OtomatAsansorUnite otomatAsansorUnite;
        public OtomatUrunAlmaUnite otomatUrunAlmaUnite;

        public OtomatUnite(List<OtomatMotorUnite> otomatMotorUniteList, OtomatAsansorUnite otomatAsansorUnite, OtomatUrunAlmaUnite otomatUrunAlmaUnite)
        {
            this.otomatMotorUniteList = otomatMotorUniteList;
            this.otomatAsansorUnite = otomatAsansorUnite;
            this.otomatUrunAlmaUnite = otomatUrunAlmaUnite;
        }

        public int DoHoming()
        {
            int ret = -1;
            ret = this.otomatAsansorUnite.DoHoming();
            if (ret != 0)
            {
                Logger.LogError("Otomat Asansör homing Error...");
                return 1;
            }



            ret =   this.otomatUrunAlmaUnite.DoHoming();
            if (ret != 0)
            {
                Logger.LogError("Otomat Urun Alma ünitesi homing Error...");
                return 1;
            }



            return 0;

        }

        public int GetHomingReadyToService()
        {
            int ret = 0;
            ret =   this.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: 180, dikeyPos: 235);
            if (ret != 0)
            {
                Logger.LogError("Otomat Asansör ünite  GetHomingReadyToService Error.");
            }
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, null, null, null, null, null);
            if (ret != 0)
            {
                Logger.LogError("Otomat Urun Alma Unite  GetHomingReadyToService Error.");
            }

            return 0;
        }

        public int GetReadyToService()
        {

            int ret = 0;

            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 120, 120, 3, 80, 80);

            ret =   this.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: 180, dikeyPos: null);
            ret =   this.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: null, dikeyPos: 235);
            if (ret != 0)
                return 1;

            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 40, 40, 3, 15, 80);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 15, 80);
            return ret;
        }

        public int DoService(Product product)
        {

            RafBolme raf = product.RafBolmes.Where(o => o.StockAdet > 0).FirstOrDefault();
            if (raf == null)
            {
                return 1;
            }
            int ret = 0;

            int MotorRunTimeDuration = 2600;
            short AtmaNoktasıDikeyPos = 235;
            short AtmaNoktasıYatayPos = 250;
            short RafDikeyPos = (short)raf.YPos;
            short RafYatayPos = (short)raf.XPos;
            short MotorSlaveAddress = raf.MotorSlaveAddress;
            ushort MotorRegNo = (ushort)raf.MotorRegNo;


            ret =   this.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: RafYatayPos, dikeyPos: RafDikeyPos);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 45, 80);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 120, 120, 3, 45, 110);

            bool isUrunReady = this.otomatUrunAlmaUnite.UrunAlindimi();

            int tryCounter = 1;
            while (isUrunReady == false)
            {
                  this.otomatMotorUniteList.Where(o => o.slaveAddress == MotorSlaveAddress).First().RunMotor(Register_Address: MotorRegNo, RunTimeDuration: (MotorRunTimeDuration / tryCounter));

                Thread.Sleep(500);

                isUrunReady = this.otomatUrunAlmaUnite.UrunAlindimi();
                tryCounter++;

                if (tryCounter >= 12)
                {
                    return 1;
                }
            }

            product.StockAdet--;
            raf.StockAdet--;
            ProductServices.Update(product, raf);

            Thread.Sleep(300);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 45, 110);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 45, 80);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 220, 80);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 120, 120, 3, 220, 80);
            Thread.Sleep(200);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 220, 80);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 40, 40, 3, 15, 80);

            ret =   this.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: 180, dikeyPos: 235);

            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 40, 40, 3, 15, 80);
            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 40, 40, 3, 80, 80);

            ret =   this.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: AtmaNoktasıYatayPos, dikeyPos: AtmaNoktasıDikeyPos);

            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 120, 120, 3, 80, 80);

            Thread.Sleep(300);

            ret =   this.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 120, 120, 80, 80, 80);



            return ret;
        }






    }

}
