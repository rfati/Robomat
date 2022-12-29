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
    public class KapInfo
    {
        public List<int> dolu_kase_no_list = new List<int>();
        public List<int> dolu_bardak_no_list = new List<int>();
        public List<int> dolu_kaseKapak_no_list = new List<int>();
        public List<int> dolu_bardakKapak_no_list = new List<int>();
        public List<int> dolu_set_no_list = new List<int>();

        public bool is_hazir_kase_ok = false;
        public bool is_hazir_bardak_ok = false;
        public bool is_hazir_kaseKapak_ok = false;
        public bool is_hazir_bardakKapak_ok = false;

        public void Set(List<Sensor> sensorList, KapSensor kapSensor)
        {
            int isSet = -1;
            foreach(var item in sensorList)
            {
                if(item.CurrentValRegisterRead.Register_Address == kapSensor.HazirKase_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        is_hazir_kase_ok = true;
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.HazirBardak_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        is_hazir_bardak_ok = true;
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.HazirKaseKapak_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        is_hazir_kaseKapak_ok = true;
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.HazirBardakKapak_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        is_hazir_bardakKapak_ok = true;
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Kase1_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_kase_no_list.Add(0);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Kase2_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_kase_no_list.Add(1);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Kase3_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_kase_no_list.Add(2);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Bardak1_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_bardak_no_list.Add(0);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Bardak2_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_bardak_no_list.Add(1);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Bardak3_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_bardak_no_list.Add(2);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.BardakKapak_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_bardakKapak_no_list.Add(0);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.KaseKapak_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_kaseKapak_no_list.Add(0);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Set1_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_set_no_list.Add(0);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Set2_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_set_no_list.Add(1);
                }
                else if (item.CurrentValRegisterRead.Register_Address == kapSensor.Set3_Sensor.CurrentValRegisterRead.Register_Address)
                {
                    if (item.CurrentValRegisterRead.Register_Read_Value == 0)
                        dolu_set_no_list.Add(2);
                }

            }

        }
    }
    public class CafeKapUnite : RTUDevice
    {
        private KapSensor kapSensor { get; set; }

        public CafeKapUnite()
        {
            this.slaveAddress = 0x51;
            this.state = State.Idle;
            ushort lastReg = 15;
            for (ushort regaddress = 0; regaddress <= lastReg; regaddress++)
            {
                RegisterRead registerRead = new RegisterRead(regaddress);
                this.RegisterReadList.Add(registerRead);
            }


            this.kapSensor = new KapSensor();

        }

        public KapInfo GetKapSensorInfo()
        {
            KapInfo kapInfo = new KapInfo();
            var kapSensorOkuTask = Task.Run(() => KapSensorOku());
            kapSensorOkuTask.Wait();
            if (kapSensorOkuTask.Result != null)
            {
                //kapSensorOkuTask.Result[4].CurrentValRegisterRead.Register_Read_Value;
                //int sensorValue = kapSensorOkuTask.Result.CurrentValRegisterRead.Register_Read_Value;
                kapInfo.Set(kapSensorOkuTask.Result, kapSensor);
                return kapInfo;
            }
            else
            {
                return null;
            }

        }



        private async Task<List<Sensor>> KapSensorOku()
        {
            List<Sensor> sensorList = new List<Sensor>();
            sensorList.Clear();
            sensorList.Add(this.kapSensor.Set1_Sensor);
            sensorList.Add(this.kapSensor.Set2_Sensor);
            sensorList.Add(this.kapSensor.Set3_Sensor);
            sensorList.Add(this.kapSensor.Kase3_Sensor);
            sensorList.Add(this.kapSensor.Kase2_Sensor);
            sensorList.Add(this.kapSensor.Kase1_Sensor);

            sensorList.Add(this.kapSensor.KaseKapak_Sensor);
            sensorList.Add(this.kapSensor.Bardak1_Sensor);
            sensorList.Add(this.kapSensor.Bardak2_Sensor);
            sensorList.Add(this.kapSensor.Bardak3_Sensor);

            sensorList.Add(this.kapSensor.BardakKapak_Sensor);
            sensorList.Add(this.kapSensor.HazirKase_Sensor);
            sensorList.Add(this.kapSensor.HazirBardak_Sensor);
            sensorList.Add(this.kapSensor.HazirKaseKapak_Sensor);
            sensorList.Add(this.kapSensor.HazirBardakKapak_Sensor);


            SensorCommandResult ret = await ReadMultipleSensor(sensorList);
            if (!ret.IsSuccess())
            {
                return null;
            }

            return sensorList;

        }


    }
}
