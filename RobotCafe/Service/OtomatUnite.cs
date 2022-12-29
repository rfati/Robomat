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

        public async Task<int> ServisYap(Product product)
        {
            RafBolme raf = product.RafBolmes.Where(o => o.StockAdet > 0).FirstOrDefault();
            if (raf == null)
            {
                return 1;
            }
            int ret = 0;

            int MotorRunTimeDuration = 2600;
            short AtmaNoktasıDikeyPos = 246; 
            short AtmaNoktasıYatayPos = 244; 


            //int ret = await this.otomatAsansorUnite.SetPosition(dikeyPos: (short)raf.YPos, yatayPos: (short)raf.XPos);
            //if (ret != 0)
            //    return 1;
            

            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 50, 50, 14, 60,null, isTogether:true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 120, 120, 14, 60, null, isTogether: true);

            bool isUrunReady = this.otomatUrunAlmaUnite.UrunAlindimi();

            int tryCounter = 1;
            while (isUrunReady == false)
            {
                await this.otomatMotorUniteList.Where(o => o.slaveAddress == raf.MotorSlaveAddress).First().RunMotor(Register_Address: (ushort)raf.MotorRegNo, RunTimeDuration: (MotorRunTimeDuration / tryCounter));

                await Task.Delay(500);
                isUrunReady = this.otomatUrunAlmaUnite.UrunAlindimi();
                tryCounter++;

                if (tryCounter >= 12)
                {
                    return 1;
                }
            }

            await Task.Delay(500);
           
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 50, 50, 14, 190,null, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 120, 120, 14, 190, null, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 45, 45, 14, 190, null, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 45, 45, 14, 25, null, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 45, 45, 14, 25, null, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 45, 45, 14, 100, null, isTogether: true);


            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 80, 80, 14, 120, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 45, 45, 14, 120, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 45, 45, 14, 190, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 120, 120, 14, 190, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 45, 45, 14, 190, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 45, 45, 14, 25, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 45, 45, 14, 25, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 45, 45, 14, 100, isTogether: true);

            //ret = await this.otomatAsansorUnite.SetDikeyPosition(dikeyPos: AtmaNoktasıDikeyPos);
            //if (ret != 0)
            //    return 1;

            //ret = await this.otomatAsansorUnite.SetYatayPosition(yatayPos: AtmaNoktasıYatayPos);
            //if (ret != 0)
            //    return 1;

            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 120, 120, 14, 100, null, isTogether: true);
            //if (ret != 0)
            //    return 1;

            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 120, 120, 110, 100, null, isTogether: true);
            //ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 120, 120, 14, 100, null, isTogether: true);

            //ret = await this.otomatAsansorUnite.SetYatayPosition(yatayPos: 220);
            //if (ret != 0)
            //    return 1;


            //product.StockAdet--;
            //raf.StockAdet--;
            //ProductServices.Update(product,raf);

            return ret;
        }

        public async Task<int> DoHoming()
        {
            int ret = -1;
            ret = await this.otomatAsansorUnite.DoHoming();
            if (ret != 0)
                return 1;


            ret = await this.otomatUrunAlmaUnite.DoHoming();
            if (ret != 0)
                return 1;


            return 0;

        }

        public async Task<int> GetHomingReadyToService()
        {
            int ret = -1;
            //ret = await this.otomatAsansorUnite.SetPosition(dikeyPos: 240, yatayPos:163);
            //if (ret != 0)
            //    return 1;


            return 0;
        }

        public async Task<int> GetReadyToService()
        {
            int ret = 0;

            ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 120, 120, 14, 100, null, isTogether: true);
            ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 120, 120, 14, 25, null, isTogether: true);
            ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 2, 50, 50, 14, 25, null, isTogether: true);
            ret = await this.otomatUrunAlmaUnite.SetPositionTask(ret, 55, 50, 50, 14, 25, null, isTogether: true);
            if (ret != 0)
                return 1;

            //ret = await this.otomatAsansorUnite.SetYatayPosition(yatayPos: 163);
            //if (ret != 0)
            //    return 1;

            //ret = await this.otomatAsansorUnite.SetDikeyPosition(dikeyPos: 240);
            //if (ret != 0)
            //    return 1;

            return 0;
        }




    }

}
