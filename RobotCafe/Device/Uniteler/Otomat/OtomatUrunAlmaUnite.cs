using Common;
using RobotCafe.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class OtomatUrunAlmaUnite : RTUDevice
    {
        private OtomatUrunAlma urunAlma { get; set; }
        public OtomatUrunAlmaUnite()
        {
            this.slaveAddress = 0x02;
            this.stateChangeTime = 2;

            ushort lastReg = 19;
            for (ushort regaddress = 7; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

            this.urunAlma = new OtomatUrunAlma();
        }

        public async Task<int> SetPositionTask(int ret, short? donmePos, short? kiskac1Pos, short? kiskac2Pos, short? tekmePos, short? pantiltPos, short? ileriGeriPos, bool isTogether = false)
        {
            if (ret != 0)
                return 1;

            ret = await SetPosition(donmePos, kiskac1Pos, kiskac2Pos, tekmePos, pantiltPos, ileriGeriPos, isTogether:isTogether);
            if (ret != 0)
                return 1;
            
            ret = await IsPositionOK(donmePos, kiskac1Pos, kiskac2Pos, tekmePos, pantiltPos, ileriGeriPos);
            
            return ret;
        }

        

        public async Task<int> SetPosition(short? donmePos, short? kiskac1Pos, short? kiskac2Pos, short? tekmePos, short? pantiltPos, short? ileriGeriPos, int msReadDelay=1000, bool isTogether = false)
        {

            List<Motor> motorList = new List<Motor>();

            if(donmePos != null)
            {
                this.urunAlma.Donme.TargetPosRegisterWrite.Register_Target_Value = (short)donmePos;
                motorList.Add(this.urunAlma.Donme);
            }
            if(kiskac1Pos != null)
            {
                this.urunAlma.Kiskac_Sag.TargetPosRegisterWrite.Register_Target_Value = (short)kiskac1Pos;
                motorList.Add(this.urunAlma.Kiskac_Sag);
            }
            if(kiskac2Pos != null)
            {
                this.urunAlma.Kiskac_Sol.TargetPosRegisterWrite.Register_Target_Value = (short)kiskac2Pos;
                motorList.Add(this.urunAlma.Kiskac_Sol);
            }
            if(tekmePos != null)
            {
                this.urunAlma.UrunAtma.TargetPosRegisterWrite.Register_Target_Value = (short)tekmePos;
                motorList.Add(this.urunAlma.UrunAtma);
            }
            if(pantiltPos != null)
            {
                this.urunAlma.Tilt.TargetPosRegisterWrite.Register_Target_Value = (short)pantiltPos;
                motorList.Add(this.urunAlma.Tilt);

            }
            if (ileriGeriPos != null)
            {
                this.urunAlma.IleriGeri.TargetPosRegisterWrite.Register_Target_Value = (short)ileriGeriPos;
                motorList.Add(this.urunAlma.IleriGeri);

            }

            return await SetMotorPosition(motorList, isTogether: isTogether);
        }


        public async Task<int> IsPositionOK(short? donmePos, short? kiskac1Pos, short? kiskac2Pos, short? tekmePos, short? pantiltPos, short? ileriGeriPos)
        {

            List<Motor> motorList = new List<Motor>();

            if (donmePos != null)
            {
                this.urunAlma.Donme.TargetPosRegisterWrite.Register_Target_Value = (short)donmePos;
                motorList.Add(this.urunAlma.Donme);

                //await Task.Delay(2000);
            }
            if (kiskac1Pos != null)
            {
                this.urunAlma.Kiskac_Sag.TargetPosRegisterWrite.Register_Target_Value = (short)kiskac1Pos;
                motorList.Add(this.urunAlma.Kiskac_Sag);
            }
            if (kiskac2Pos != null)
            {
                this.urunAlma.Kiskac_Sol.TargetPosRegisterWrite.Register_Target_Value = (short)kiskac2Pos;
                motorList.Add(this.urunAlma.Kiskac_Sol);
            }
            if (tekmePos != null)
            {
                this.urunAlma.UrunAtma.TargetPosRegisterWrite.Register_Target_Value = (short)tekmePos;
                motorList.Add(this.urunAlma.UrunAtma);
            }
            if (pantiltPos != null)
            {
                this.urunAlma.Tilt.TargetPosRegisterWrite.Register_Target_Value = (short)pantiltPos;
                motorList.Add(this.urunAlma.Tilt);
            }
            if (ileriGeriPos != null)
            {
                this.urunAlma.IleriGeri.TargetPosRegisterWrite.Register_Target_Value = (short)ileriGeriPos;
                motorList.Add(this.urunAlma.IleriGeri);

            }

            return await IsMotorPositionOK(motorList);
        }


        public async Task<int> DoHoming()
        {
            int ret = 0;

            ret = await this.SetPositionTask(ret, null, 45, 45, null, null, null);
            ret = await this.SetPositionTask(ret, null, null, null, 14, 30, null);
            ret = await this.SetPositionTask(ret, 0, null, null, null, null, null);
            await Task.Delay(100);
            ret = await this.SetPositionTask(ret, 55, null, null, null, null, null);

            return ret;
            

        }



        public bool UrunAlindimi()
        {
            int isSet = -1;
            var sensorOkuTask = Task.Run(() => SensorOku());
            sensorOkuTask.Wait();
            if (sensorOkuTask.Result != null)
            {
                int sensorValue = sensorOkuTask.Result.CurrentValRegisterRead.Register_Read_Value;
                isSet = sensorValue & (0x0001);
                if (isSet == 1)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }

        }

        private async Task<Sensor> SensorOku()
        {
            List<Sensor> sensorList = new List<Sensor>();
            sensorList.Clear();
            sensorList.Add(this.urunAlma.urunAlmaSensor);

            SensorCommandResult ret = await ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList.First();

        }



    }
}
