using Common;
using RobotCafe.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotCafe.Devices
{
    public class OtomatUrunAlmaUnite : RTUDevice
    {
        private OtomatUrunAlma urunAlma { get; set; }
        public OtomatUrunAlmaUnite()
        {
            this.slaveAddress = 0x02;

            ushort lastReg = 19;
            for (ushort regaddress = 7; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }

            this.urunAlma = new OtomatUrunAlma();
        }

        public int SetPositionTask(int ret, short? donmePos, short? kiskac1Pos, short? kiskac2Pos, short? tekmePos, short? pantiltPos, short? ileriGeriPos)
        {
            if (ret != 0)
                return 1;


            ret =   SetPosition(donmePos, kiskac1Pos, kiskac2Pos, tekmePos, pantiltPos, ileriGeriPos);
            if (ret != 0)
                return 1;
            
            ret =   IsPositionOK(donmePos, kiskac1Pos, kiskac2Pos, tekmePos, pantiltPos, ileriGeriPos);

            if (ret != 0)
            {
                Logger.LogError("Otomat Ürün Alma ünitesi SetPositionTask Error.");
            }

            return ret;

        }

        

        public int SetPosition(short? donmePos, short? kiskac1Pos, short? kiskac2Pos, short? tekmePos, short? pantiltPos, short? ileriGeriPos)
        {
            bool isTogether = false;
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

            if(donmePos != null && kiskac1Pos != null && kiskac2Pos != null && tekmePos != null && pantiltPos != null && ileriGeriPos != null)
            {
                isTogether = true;
            }

            int ret =   SetMotorPosition(motorList, isTogether: isTogether);
            if (ret != 0)
            {
                Logger.LogError("Otomat Ürün Alma ünitesi SetPosition Error.");
            }

            return ret;
        }


        public int IsPositionOK(short? donmePos, short? kiskac1Pos, short? kiskac2Pos, short? tekmePos, short? pantiltPos, short? ileriGeriPos)
        {

            List<Motor> motorList = new List<Motor>();

            if (donmePos != null)
            {
                this.urunAlma.Donme.TargetPosRegisterWrite.Register_Target_Value = (short)donmePos;
                motorList.Add(this.urunAlma.Donme);

                //Thread.Sleep(2000);
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

            int ret =   IsMotorPositionOK(motorList);
            if (ret != 0)
            {
                Logger.LogError("Otomat Ürün Alma ünitesi IsMotorPositionOK Error.");
            }

            return ret;
        }


        public int DoHoming()
        {
            int ret = 0;

            ret =   this.SetPositionTask(ret, null, null, null, 3, null, null);
            ret =   this.SetPositionTask(ret, null, 40, 40, null, null, 80);
            ret =   this.SetPositionTask(ret, null, null, null, null, 15, null);
            ret =   this.SetPositionTask(ret, 0, null, null, null, null, null);

            return ret;
            

        }



        public bool UrunAlindimi()
        {
            int isSet = -1;
            var sensorOkuResult = SensorOku();

            if (sensorOkuResult != null)
            {
                int sensorValue = sensorOkuResult.CurrentValRegisterRead.Register_Read_Value;
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

        private Sensor SensorOku()
        {
            List<Sensor> sensorList = new List<Sensor>();
            sensorList.Clear();
            sensorList.Add(this.urunAlma.urunAlmaSensor);

            SensorCommandResult ret = ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList.First();

        }



    }
}
