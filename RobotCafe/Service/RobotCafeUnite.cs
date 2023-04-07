using Common;
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

    public class RobotCafeUnite
    {
        public CafeAsansorUnite asansorUnite;
        public CafeKesiciUnite kesiciUnite;
        public CafeKapakKapatmaUnite kapakKapatmaUnite;
        public CafeIsiticiUnite isiticiUnite;
        public CafeUrunAlmaUnite urunAlmaUnite;
        public CafeVakumUnite vakumUnite;
        public CafeKapUnite cafeKapUnite;
        public CafeRobotTutucuKiskacUnite cafeRobotTutucuKiskacUnite;
        public RobotArm robotArm;
        public GetReadyToServiceXArmPath readyToServicePath;
        public TestService testservice;

        public RobotCafeUnite(CafeAsansorUnite asansorUnite, CafeKesiciUnite kesiciUnite, CafeKapakKapatmaUnite kapakKapatmaUnite, CafeIsiticiUnite isiticiUnite, CafeUrunAlmaUnite urunAlmaUnite, CafeVakumUnite vakumUnite, CafeKapUnite cafeKapUnite, CafeRobotTutucuKiskacUnite cafeRobotTutucuKiskacUnite, RobotArm robotArm, GetReadyToServiceXArmPath readyToServicePath)
        {
            this.asansorUnite = asansorUnite;
            this.kesiciUnite = kesiciUnite;
            this.kapakKapatmaUnite = kapakKapatmaUnite;
            this.isiticiUnite = isiticiUnite;
            this.urunAlmaUnite = urunAlmaUnite;
            this.vakumUnite = vakumUnite;
            this.cafeKapUnite = cafeKapUnite;
            this.cafeRobotTutucuKiskacUnite = cafeRobotTutucuKiskacUnite;
            this.robotArm = robotArm;
            this.readyToServicePath = readyToServicePath;
            
        }


        public async Task<int> DoHoming()
        {
            int ret = -1;
            List<Task<int>> paralelTasks = new List<Task<int>>();
            paralelTasks.Add(this.DoDeviceHoming());
            paralelTasks.Add(this.robotArm.DoHoming());

            ret = await this.RunAsyncParalelTasks(paralelTasks);
            paralelTasks.Clear();
            if (ret != 0)
            {
                return 1;
            }
            return 0;
        }
        public async Task<int> DoDeviceHoming()
        {

            int ret = 0;

            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);
            ret = await this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: 0);
            ret = await this.asansorUnite.SetPosition(ret, dikeyPos: 0);
            ret = await this.kesiciUnite.SetPosition(ret, lineerPos: null, servoPos: 125);
            ret = await this.isiticiUnite.SetPosition(ret, yikamaPos: null, vakumPos: 130, probPos: null);

            await Task.Delay(500);

            ret = await this.isiticiUnite.IsPositionOK(ret, yikamaPos: null, vakumPos: 130, probPos: null);
            ret = await this.kesiciUnite.IsPositionOK(ret, lineerPos: null, servoPos: 125);
            ret = await this.isiticiUnite.SetPosition(ret, yikamaPos: null, vakumPos: 130, probPos: 115);
            ret = await this.kesiciUnite.SetPosition(ret, lineerPos: 0, servoPos: 125);

            await Task.Delay(500);

            ret = await this.isiticiUnite.IsPositionOK(ret, yikamaPos: null, vakumPos: 130, probPos: 115);
            ret = await this.isiticiUnite.SetPosition(ret, yikamaPos: 0, vakumPos: 130, probPos: 115);

            ret = await this.urunAlmaUnite.IsPositionOK(ret, donmePos: 0, lineerPos: null);
            ret = await this.kapakKapatmaUnite.IsPositionOK(ret, yatayPos: 0, dikeyPos: 0);
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: 0);
            ret = await this.asansorUnite.IsPositionOK(ret, dikeyPos: 0);
            ret = await this.kesiciUnite.IsPositionOK(ret, lineerPos: 0, servoPos: 125);


            ret = await this.isiticiUnite.IsPositionOK(ret, yikamaPos: 0, vakumPos: 130, probPos: 115);
            ret = await this.urunAlmaUnite.IsPositionOK(ret, donmePos: 0, lineerPos: 0);



            return ret;

        }
        public async Task<int> GetReadyToService(int ret)
        {
            if (ret != 0)
                return 1;
            return await this.KapVeKapakHazirlamaTask();
        }

        public async Task<int> KapVeKapakHazirlamaTask()
        {
            int ret = 0;
            bool is_hazir_bardak_ok = false;
            bool is_hazir_kase_ok = false;
            bool is_hazir_bardakKapak_ok = false;
            bool is_hazir_kaseKapak_ok = false;


            ret = await cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret,95,45,15,20,35);
            if (ret != 0)
                return 1;

            int KapYerlestirTaskRet = -1;
            KapInfo kapInfo = this.cafeKapUnite.GetKapSensorInfo();
            if (kapInfo == null)
                return 1;

            if (kapInfo.is_hazir_bardak_ok == false)
            {
                for (int i = 0; i < kapInfo.dolu_bardak_no_list.Count; i++)
                {
                   
                   KapYerlestirTaskRet = await this.DoBardakHazirlamaTest(bardakNo: kapInfo.dolu_bardak_no_list[i]);

                    if (KapYerlestirTaskRet != 0)
                        return 1;
                    kapInfo = this.cafeKapUnite.GetKapSensorInfo();
                    if (kapInfo == null)
                        return 1;
                    if (kapInfo.is_hazir_bardak_ok == true)
                    {
                        is_hazir_bardak_ok = true;
                        break;
                    }
                }

            }
            else
            {
                is_hazir_bardak_ok = true;
            }

            if (kapInfo.is_hazir_kase_ok == false)
            {
                for (int i = 0; i < 1; i++)
                {
                    KapYerlestirTaskRet = await this.DoKaseHazirlamaTest(kaseNo: 0);
                    if (KapYerlestirTaskRet != 0)
                        return -1;
                    kapInfo = this.cafeKapUnite.GetKapSensorInfo();
                    if (kapInfo == null)
                        return -1;
                    if (kapInfo.is_hazir_kase_ok == true)
                    {
                        is_hazir_kase_ok = true;
                        break;
                    }
                }
            }
            else
            {
                is_hazir_kase_ok = true;
            }

            int KapakYerlestirTaskRet = -1;

            if (kapInfo.is_hazir_bardakKapak_ok == false)
            {
                for (int i = 0; i < kapInfo.dolu_bardakKapak_no_list.Count + 1; i++)
                {
                    KapakYerlestirTaskRet = await this.DoBardakKapakHazirlamaTest();
                    

                    if (KapakYerlestirTaskRet != 0)
                        return -1;
                    kapInfo = this.cafeKapUnite.GetKapSensorInfo();
                    if (kapInfo == null)
                        return -1;
                    if (kapInfo.is_hazir_bardakKapak_ok == true)
                    {
                        is_hazir_bardakKapak_ok = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

            }
            else
            {
                is_hazir_bardakKapak_ok = true;
            }

            if (kapInfo.is_hazir_kaseKapak_ok == false)
            {
                for (int i = 0; i < kapInfo.dolu_kaseKapak_no_list.Count; i++)
                {
                    KapakYerlestirTaskRet = await this.DoKaseKapakHazirlamaTest();
                    if (KapakYerlestirTaskRet != 0)
                        return -1;
                    kapInfo = this.cafeKapUnite.GetKapSensorInfo();
                    if (kapInfo == null)
                        return -1;
                    if (kapInfo.is_hazir_kaseKapak_ok == true)
                    {
                        is_hazir_kaseKapak_ok = true;
                        break;
                    }
                }
            }
            else
            {
                is_hazir_kaseKapak_ok = true;
            }




            if (is_hazir_bardak_ok && is_hazir_kase_ok && is_hazir_bardakKapak_ok && is_hazir_kaseKapak_ok)
                return 0;
            else
                return 1;
        }

        public async Task<int> DoBardakKapakHazirlamaTest(int tryCounter = 0)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 100, 150, 10, 15, 35);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] posara = { 51.5f, 54.7f, -43.1f, -95.9f, -91.8f, 0 };


            float[] pos3 = { 49.2f, 1.7f, 6f, -62.5f, -93.5f, 0 };

            float[] pos4 = { 59f, 61.7f, -68.4f, -83.9f, -91.8f, 0 };
            float[] kapakalma1 = { 59f, 52.9f, -62.4f, -81.2f, -91.8f, 0 }; //kapak alma pos 1
            float[] kapakalma2 = { 59.4f, 51.8f, -62.9f, -81f, -91.8f, 0 }; //kapak alma pos 2
            float[] posrevize = { 47.1f, 44.1f, -40.6f, -93.6f, 93f, 0 }; //kapak alma pos 2



            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos3);
            positionList.Add(posara);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);

            positionList.Clear();
            positionList.Add(pos4);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            positionList.Clear();
            if (tryCounter == 0)
            {
                positionList.Add(kapakalma1);
            }
            else if (tryCounter == 1)
            {
                positionList.Add(kapakalma2);
            }

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(posara);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            float[] pos31 = { 49.2f, 39f, -17.8f, -96f, 92.1f, 0 };
            float[] pos311 = { 53.7f, 61.5f, -84.6f, -69.2f, 92.1f, 0 };
            float[] pos32 = { 53.7f, 65f, -87.1f, -69.2f, 92.1f, 0 };

            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos31);
            positionList.Add(posrevize);
            positionList.Add(pos311);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);

            positionList.Clear();
            positionList.Add(pos32);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos311);
            positionList.Add(pos31);
            positionList.Add(pos3);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            return ret;
        }

        public async Task<int> DoKaseHazirlamaTest(int kaseNo)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 99, 150, 10, 15, 35);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos3 = { 127.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos40 = { 130.3f, 67.8f, -63f, -96.5f, -91.8f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos40);

            if (kaseNo == 0)
            {
                float[] pos4 = { 143.8f, 78f, -84f, -80.3f, -89.7f, 0 };
                float[] pos5 = { 143.8f, 52.5f, -66f, -74.7f, -89.7f, 0 };

                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (kaseNo == 1)
            {
                float[] pos4 = { 130.5f, 81.3f, -94.2f, -74.1f, -89.7f, 0 };
                float[] pos5 = { 130.5f, 58.1f, -77.6f, -70.7f, -89.7f, 0 };
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (kaseNo == 2)
            {
                float[] pos4 = { 117f, 82.3f, -96.3f, -73f, -89.7f, 0 };
                float[] pos5 = { 117.1f, 59.2f, -79.9f, -70.3f, -89.7f, 0 };
                positionList.Add(pos4);
                positionList.Add(pos5);
            }


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 30);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            float[] pos6 = { 0, -98.2f, -32.3f, 40.8f, 93.7f, 0 };
            float[] pos6x = { 42.2f, 4.7f, -122.1f, 26.4f, 93.7f, 0 };//yaklaşma
            float[] pos7 = { 42.2f, 0.5f, -111.4f, 19.1f, 93.7f, 0 };//kase bırakma


            positionList.Clear();

            if (kaseNo == 0)
            {
                float[] pos4 = { 143.8f, 78f, -84f, -80.3f, -89.7f, 0 };
                positionList.Add(pos4);

            }
            else if (kaseNo == 1)
            {
                float[] pos4 = { 130.5f, 81.3f, -94.2f, -74.1f, -89.7f, 0 };
                positionList.Add(pos4);

            }
            else if (kaseNo == 2)
            {
                float[] pos4 = { 117f, 82.3f, -96.3f, -73f, -89.7f, 0 };
                positionList.Add(pos4);

            }

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 50);

            positionList.Clear();
            positionList.Add(pos40);
            positionList.Add(pos3);
            positionList.Add(pos2);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos6);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 50);

            positionList.Clear();
            positionList.Add(pos6x);
            positionList.Add(pos7);


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 50);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos6x);
            positionList.Add(pos6);
            positionList.Add(pos1);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            return ret;
        }
        public async Task<int> DoBardakHazirlamaTest(int bardakNo)
        {

            int ret = 0;

            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 100, 150, 10, 15, 35);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 91f, -45.6f, 9.9f, -48.7f, -91.8f, 0 };
            float[] pos3 = { 91f, 50.9f, -10.7f, -84.9f, -91.8f, 0 };
            float[] posara = { 42.3f, -54.2f, -17.3f, -19.9f, 90.6f, 0 };
            float[] poscart = { 91f, 61.9f, -55.3f, -91.9f, -91.8f, 0 };



            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70, wait: false);


            if (bardakNo == 0)
            {
                float[] pos4 = { 102.1f, 76.7f, -79.7f, -87.8f, -91.8f, 0 };
                float[] pos5 = { 102.1f, 53.1f, -62.4f, -81f, -91.8f, 0 };
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (bardakNo == 1)
            {
                float[] pos4 = { 89.2f, 94.5f, -93.8f, -89.8f, -91.8f, 0 };
                float[] pos5 = { 89.2f, 49.6f, -53.7f, -84.6f, -91.8f, 0 };//bardak alma
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (bardakNo == 2)
            {
                float[] pos4 = { 74.1f, 57.5f, -57f, -89.4f, -91.8f, 0 };
                float[] pos5 = { 75.1f, 49.9f, -53.8f, -86.3f, -91.8f, 0 };//bardak alma
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 50);

            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            float[] pos60 = { 42.7f, -54.2f, -22.2f, -14f, 92.4f, 0 };
            float[] pos6 = { 42.7f, -8f, -53.9f, -27.9f, 92.4f, 0 };

            //float[] pos7 = { 42.7f, -7.4f, -50.6f, -34.5f, 92.4f, 0 }; //bırakma noktası
            float[] pos7 = { 42.7f, -6.8f, -50.1f, -34.4f, 92.4f, 0 }; //bırakma noktası


            positionList.Clear();
            if (bardakNo == 0)
            {
                //float[] pos4 = { 102.8f, 97f, -106f, -77.8f, -91.4f, 0 };
                float[] pos4 = { 102.1f, 76.7f, -79.7f, -87.8f, -91.8f, 0 };
                positionList.Add(pos4);
            }
            else if (bardakNo == 1)
            {

                float[] pos4 = { 89.2f, 94.5f, -93.8f, -89.8f, -91.8f, 0 };
                float[] pos4x = { 89.2f, 57.5f, -16.8f, -87.7f, -91.8f, 0 };

                positionList.Add(pos4);
                //positionList.Add(pos4x);

            }
            else if (bardakNo == 2)
            {

                float[] pos4 = { 74.6f, 56.7f, -55.9f, -89.2f, -91.8f, 0 };
                float[] pos4x = { 77.4f, 57.4f, -54.9f, -88f, -84.4f, 0 };
                float[] pos4x1 = { 80f, 59.9f, -54.9f, -88f, -79.2f, 0 };
                float[] pos4x2 = { 84.6f, 67.9f, -54.8f, -88f, -68f, 0 };

                positionList.Add(pos4);
                positionList.Add(pos4x);
                positionList.Add(pos4x1);
                positionList.Add(pos4x2);


            }

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 30);
            float[] posyeni = { 30.6f, -95.7f, 0, -0.3f, 88.5f, 0 };




            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(posyeni);
            positionList.Add(posara);
            positionList.Add(pos6);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 60, wait: false);

            positionList.Clear();
            positionList.Add(pos7); // bardak bırakma noktası


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);

            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos6);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 5);

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(pos1);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);


            return ret;
        }
        public async Task<int> DoKaseKapakHazirlamaTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 150, 10, 15, 35);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos3 = { 131.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos4 = { 134.3f, 67.8f, -63f, -96.5f, -91.8f, 0 };
            float[] pos5 = { 158.4f, 53.1f, -55.1f, -84.6f, -92.5f, 0 };// kase kapak yaklaşma

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            float[] pos6 = { 158.4f, 49.3f, -55f, -84.6f, -92.9f, 0 };//kapak alma noktası


            positionList.Clear();
            positionList.Add(pos6);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 3);

            float[] pos31 = { 131.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos32 = { 66.4f, 1.2f, 1.1f, -91.6f, 89.5f, 0 };
            float[] pos33 = { 66.4f, 16.7f, -2.2f, -78.9f, 89.5f, 0 };
            float[] pos33x = { 66.4f, 54.4f, -63.9f, -82.6f, 89.9f, 0 };

            float[] pos34 = { 66.4f, 60.6f, -68.6f, -84.6f, 89.9f, 0 };//kapak bırakma noktası

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos3);
            positionList.Add(pos31);
            positionList.Add(pos32);
            positionList.Add(pos33);
            positionList.Add(pos33x);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);

            positionList.Clear();
            positionList.Add(pos34);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);


            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);


            positionList.Clear();
            positionList.Add(pos33x);
            positionList.Add(pos33);
            positionList.Add(pos32);
            positionList.Add(pos2);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);

            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            return ret;
        }


        public async Task<int> DoKaseKapakYerlestirmeTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 95, 0, 0, 35);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 22.5f, 29.2f, -11f, -96.5f, 87.5f, 0 };
            float[] pos3 = { 66.5f, 55.8f, -70.2f, -75.1f, 87.5f, 0 };
            float[] pos4 = { 66.5f, 60.1f, -73.1f, -76.5f, 87.5f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);


            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);



            float[] pos5 = { 0, -98.2f, -32.3f, 40.8f, 87.5f, 0 }; // home position
            float[] pos6 = { -60.4f, -38f, -21.1f, -31.1f, 87.5f, 0 };  //kase kapağı yerleştirme noktası

            positionList.Add(pos5);
            positionList.Add(pos6);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos5);
            positionList.Add(pos1);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);

            return ret;

        }
        public async Task<int> DoBardakKapakYerlestirmeTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 95, 0, 0, 35);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 22.5f, 29.2f, -11f, -96.5f, 87.5f, 0 };

            float[] pos3 = { 54f, 60.5f, -88.4f, -62.1f, 87.5f, 0 };
            float[] pos4 = { 54f, 65.3f, -91.8f, -62.1f, 87.5f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);


            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);



            float[] pos5 = { 0, -98.2f, -32.3f, 40.8f, 87.5f, 0 }; // home position

            float[] pos60 = { -60.3f, -39.6f, -24.7f, -24.7f, 86.5f, 0 };  //bardak kapağı yerleştirme noktası
            float[] pos6 = { -59.2f, -39.9f, -25.1f, -20.4f, 86.5f, 0 };  //bardak kapağı yerleştirme noktası

            positionList.Add(pos5);
            positionList.Add(pos60);
            positionList.Add(pos6);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 60);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(pos5);
            positionList.Add(pos1);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);

            return ret;

        }
        public async Task<int> DoBardakYerlestirmeTest()
        {
            int ret = 0;

            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 5, 100, 100 , 120);

            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 38.2f, -17.5f, 1f, -75.6f, 91f, 0 };
            float[] pos3 = { 38.2f, 12f, -28.3f, -75.2f, 91f, 0 }; //bardak alma position

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 75, 100, 100, 120);


            await Task.Delay(1000);


            float[] pos40 = { 38.2f, 8.1f, -28f, -72.5f, 91f, 0 };
            float[] pos4 = { 38.2f, -11.8f, -5.9f, -75.8f, 91f, 0 };
            float[] pos5 = { 38.2f, -17.5f, 1f, -75.6f, 91f, 0 };
            float[] pos6 = { 38.2f, -63.3f, 4.6f, -30.3f, 91f, 0 };
            float[] pos8 = { -69.7f, -73.9f, 5.7f, -30.4f, -89f, 0 };
            float[] pos80 = { -69.7f, -62.6f, 5.7f, -30.5f, -89f, 0 };
            positionList.Clear();
            positionList.Add(pos40);
            //positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos6);
            positionList.Add(pos8);
            positionList.Add(pos80);


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 5, 100, 100, 120);

            await Task.Delay(1000);

            float[] pos9 = { -69.7f, -63.3f, -11.1f, -30.3f, -89f, 0 };
            float[] pos10 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position

            positionList.Clear();
            positionList.Add(pos9);
            positionList.Add(pos10);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            return ret;

        }
        public async Task<int> DoKaseYerlestirmeTest()
        {
            int ret = 0;

            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 5, 100, 100 , 120);

            List<float[]> positionList = new List<float[]>();


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 37.9f, -58, -42.6f, 7, 91f, 0 };
            float[] pos3 = { 37.9f, -25.5f, -57.4f, -7.3f, 91f, 0 }; //kase alma position

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 34, 50, 50, 120);

            await Task.Delay(1000);

            float[] pos4 = { 37.9f, -25.2f, -58.8f, -8.1f, 91f, 0 };
            float[] pos5 = { 37.9f, -56.8f, -39.2f, 1.2f, 91f, 0 };
            float[] pos6 = { -69.3f, -67.2f, 4.8f, -25.5f, -89f, 0 };
            float[] pos7 = { -69.3f, -62.8f, 7.1f, -31.7f, -89f, 0 };//kase bırakma position


            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos6);
            positionList.Add(pos7);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 5, 100, 100 , 120);

            await Task.Delay(1000);

            float[] pos9 = { -69.3f, -115.8f, 5.6f, 32.5f, -89f, 0 };
            float[] pos10 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position

            positionList.Clear();
            positionList.Add(pos9);
            positionList.Add(pos10);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            return ret;

        }


        public async Task<int> DoUrunAlmaTest(AmbalajType PackageType, bool packagedService = false)
        {

            int ret = 0;

            ret = await this.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 40);
            ret = await this.urunAlmaUnite.SetPositionTask(ret, donmePos: 33, lineerPos: null);


            List<float[]> positionList = new List<float[]>();
            float[] home = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position

            float[] pose1 = { -53.9f, -82.4f, -3.7f, -3.1f, -93f, 0 };
            float[] pose2 = { -53.9f, -51f, -42.7f, 42.5f, -93f, 0 };
            float[] pose3 = { -53.9f, -37.9f, -40.8f, 22.3f, -93f, 0 };
            float[] pose4 = { -53.9f, -34.2f, -47.7f, 23.2f, -93f, 0 };
            float[] pose5 = { -53.9f, -35.7f, -47.3f, 21.7f, -93f, 0 };
            float[] pose6 = { -53.9f, -42f, -50.2f, 36, -93f, 0 };
            float[] pose7 = { -53.9f, -75f, -50.2f, 36, -93f, 0 };

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 100, 100, 100, 120);


            positionList.Clear();
            positionList.Add(pose1);
            positionList.Add(pose2);
            positionList.Add(pose3);
            positionList.Add(pose4);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);

            if (PackageType == AmbalajType.Size_12)
            {
                ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 30, 100, 5, 5 , 120);
            }
            else
            {
                ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 5, 5 , 120);
            }


            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pose5);
            positionList.Add(pose6);
            positionList.Add(pose7);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);

            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 3, 5, 2);
            //await Task.Delay(200);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 3, 50, 50);
            //await Task.Delay(200);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 10, 3, 50, 50);
            //await Task.Delay(200);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 10, 3, 15, 13);
            //await Task.Delay(200);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 3, 50, 50);
            //await Task.Delay(200);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 3, 5, 2);
            return ret;

        }

        public async Task<int> DoKesmeTest(AmbalajType ambalajType)
        {
            int ret = 0;


            List<float[]> positionList = new List<float[]>();
            float[] pos1 = { -53.2f, -103.6f, 6, 26, -90f, 0 };
            float[] pos100 = { -117.2f, -103.6f, 6, 26, -90f, 0 };
            float[] pos10 = { -117.2f, -35.3f, -7.3f, -29.3f, -90f, 0 };
            float[] pos11 = { -117.2f, -17.2f, -21.7f, -29.3f, -90f, 0 };
            float[] pos2 = { -115.8f, -65.6f, 1.3f, -10.2f, -90f, 0 };
            float[] pos3 = { -115.8f, -37.6f, -13.7f, -10.2f, -90f, 0 };
            float[] pos4 = { -115.8f, -34.9f, -17.7f, -31.8f, -90f, 0 };
            float[] pos5 = { -115.6f, -39f, -12.1f, -31.8f, -90f, 0 };
            float[] pos5size12pack = { -116.5f, -39f, -12.3f, -31.8f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos100);
            positionList.Add(pos10);
            positionList.Add(pos11);
            //positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);

            if(ambalajType == AmbalajType.Size_12)
                positionList.Add(pos5);
            if (ambalajType == AmbalajType.Size_13) { 
                //positionList.Add(pos5size12pack);
                positionList.Add(pos5);

            }

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 50);
            ret = await this.kesiciUnite.SetPositionTask(ret,lineerPos: 50, servoPos:null);
            ret = await this.kesiciUnite.SetPositionTask(ret, lineerPos: null, servoPos: 45);


            float[] pos6 = { -117f, -42.7f, -4.5f, -24f, -90f, 0 };
            float[] pos7 = { -117f, -107f, 0f, 25.8f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos6);
            positionList.Add(pos7);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);


            return ret;
        }

        public async Task<int> DoAmbalajAcmaTest()
        {
            int ret = 0;

            ret = await this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);



            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { -117f, -78.7f, -39.2f, 29.7f, -90f, 0 };
            positionList.Clear();
            positionList.Add(pos1);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 15, 15, 120);

            ret = await this.vakumUnite.RunIsiticiVakum(ret, run: true);


            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 60, probPos: null);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 50, probPos: null);

            await Task.Delay(200);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 100, probPos: null);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 15, 15, 120);
            await Task.Delay(500);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 10, 10, 120);
            await Task.Delay(500);

            await this.vakumUnite.RunIsiticiVakum(ret, run: false);

            await Task.Delay(500);

            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 130, probPos: null);


            float[] pos2 = { -67.7f, -78.7f, -39.2f, 29.7f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos2);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 35);

            ret = await this.urunAlmaUnite.IsPositionOK(ret, donmePos: 0, lineerPos: null);
            ret = await this.kapakKapatmaUnite.IsPositionOK(ret, yatayPos: 32, dikeyPos: null);

            return ret;
        }


        public async Task<int> DoIsitmaTest(int isitmaSuresi, AmbalajType ambalajType)
        {
            int ret = 0;

            ret = await this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);



            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { -117f, -80.5f, -34.1f, 24.4f, -90f, 0 };
            positionList.Clear();
            positionList.Add(pos1);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 35);
            if(ambalajType == AmbalajType.Size_12)
                ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 20, 100, 10, 10 , 120);
            else if (ambalajType == AmbalajType.Size_13)
                ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 15, 15, 120);

            ret = await this.vakumUnite.RunIsiticiVakum(ret, run: true);


            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 60, probPos: null);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 50, probPos: null);


            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 85, probPos: null);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 20);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 70, probPos: null);

            ret = await this.isiticiUnite.RunIsiticiBuhar(ret, run: true);

            await Task.Delay(isitmaSuresi);



            ret = await this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
            if (ambalajType == AmbalajType.Size_12)
            {

                ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 110, probPos: null);
                ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 25, 100, 5, 5, 120);
                await Task.Delay(200);

            }
            else if (ambalajType == AmbalajType.Size_13)
            {
                ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 110, probPos: null);
                ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 10, 10, 120);
                await Task.Delay(200);

                //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 80, 6, 20, 20);
                //await Task.Delay(500);
                //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 85, 6, 5, 2);
            }




            await this.vakumUnite.RunIsiticiVakum(ret, run: false);
            await Task.Delay(500);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 130, probPos: null);




            //ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 95);
            //ret = await this.isiticiUnite.RunIsiticiBuhar(ret, run: true);
            //await Task.Delay(500);
            //ret = await this.isiticiUnite.RunIsiticiBuhar(ret, run: false);

            await Task.Delay(1000);

            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: 130, probPos: null);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 115);



            float[] pos2 = { -67.7f, -78.7f, -39.2f, 29.7f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos2);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 35);

            ret = await this.urunAlmaUnite.IsPositionOK(ret, donmePos: 0, lineerPos: null);
            ret = await this.kapakKapatmaUnite.IsPositionOK(ret, yatayPos: 32, dikeyPos: null);

            return ret;
        }


        public async Task<int> DoProbeTemizlemeTest(int temizlemeSuresi)
        {
            int ret = 0;
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: 69, vakumPos: null, probPos: null);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 20);

            ret = await this.isiticiUnite.RunIsiticiBuhar(ret, run: true);

            await Task.Delay(temizlemeSuresi);

            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 55);
            ret = await this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 115);
            ret = await this.isiticiUnite.SetPositionTask(ret, yikamaPos: 0, vakumPos: null, probPos: null);

            return ret;
        }
        public async Task<int> DoBosaltmaBardakTest()
        {

            int ret = 0;

            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 100, 5, 5, 120);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 100, 5, 5, 120);

            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 6, 10, 10);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 60, 6, 10, 10);
            //ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 60, 6, 8, 3);

            float[] pos10 = { -68.3f, -100f, -9.3f, 57.9f, -90f, 0 };
            float[] pos20 = { -69.5f, -61.3f, -43.1f, 90.8f, -90f, 0 };
            float[] pos30 = { -69.5f, -41.3f, -63f, 109f, -90f, 0 };
            float[] pos40 = { -69.5f, -19.9f, -85.9f, 128.3f, -90f, 0 };
            float[] pos50 = { -69.8f, -0.5f, -104.3f, 143.5f, -90f, 0 };
            float[] pos60 = { -69.8f, 13.9f, -121.3f, 158.1f, -90f, 0 };//*s1
            float[] pos70 = { -69.8f, 9.3f, -121.6f, 150.5f, -90f, 0 };//*s2

            //float[] pos10 = { -68.4f, -62.5f, -41.2f, 85.3f, -90f, 0 };
            //float[] pos20 = { -68.4f, -49.4f, -52.9f, 94.3f, -90f, 0 };
            //float[] pos30 = { -68.4f, -34.6f, -68.1f, 108.7f, -90f, 0 };
            //float[] pos40 = { -68.4f, -25.4f, -78.3f, 121.3f, -90f, 0 };
            //float[] pos50 = { -68.4f, -13.3f, -93.3f, 130.4f, -90f, 0 };
            //float[] pos60 = { -68.4f, -1.1f, -108f, 142.9f, -90f, 0 };
            //float[] pos70 = { -68.4f, 17.4f, -128.4f, 160f, -90f, 0 };


            //await Task.Delay(500);

            positionList.Clear();
            positionList.Add(pos10);
            positionList.Add(pos20);
            positionList.Add(pos30);
            positionList.Add(pos40);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 12, wait: false);
            if (ret != 0)
                return 1;

            positionList.Clear();
            positionList.Add(pos50);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 20);
            if (ret != 0)
                return 1;

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(pos70);
            positionList.Add(pos60);
            positionList.Add(pos70);



            ret = await this.robotArm.SetPosition(ret, positionList, speed: 20);
            if (ret != 0)
                return 1;

            float[] pos1 = { -67.7f, -60.7f, -41.9f, 81f, -90f, 0 };
            float[] pos2 = { -67.7f, -55.6f, -48.1f, 91f, -90f, 0 };
            float[] pos3 = { -67.7f, -42.7f, -61f, 101.9f, -90f, 0 };
            float[] pos4 = { -67.7f, -25.6f, -77.6f, 116.7f, -90f, 0 };
            float[] pos5 = { -67.7f, -7.6f, -98.9f, 135.3f, -90f, 0 };
            float[] pos6 = { -67.7f, 10f, -118.6f, 152.5f, -90f, 0 };
            float[] pos7 = { -67.7f, 29f, -144.1f, 171.7f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos7);
            positionList.Add(pos6);
            positionList.Add(pos5);
            positionList.Add(pos4);
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(pos1);

            return await this.robotArm.SetPosition(ret, positionList, speed: 50);

        }

        public async Task<int> DoBosaltmaKaseTest()
        {

            int ret = 0;

            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 100, 5, 5, 120);

            float[] pos10 = { -68.3f, -100f, -9.3f, 57.9f, -90f, 0 };
            float[] pos20 = { -69.5f, -61.3f, -43.1f, 90.8f, -90f, 0 };
            float[] pos30 = { -69.5f, -41.3f, -63f, 109f, -90f, 0 };
            float[] pos40 = { -69.5f, -19.9f, -85.9f, 128.3f, -90f, 0 };
            float[] pos50 = { -69.8f, -0.5f, -104.3f, 143.5f, -90f, 0 };
            float[] pos60 = { -69.8f, 13.9f, -121.3f, 158.1f, -90f, 0 };//*s1
            float[] pos70 = { -69.8f, 9.3f, -121.6f, 150.5f, -90f, 0 };//*s2

            //await Task.Delay(500);

            positionList.Clear();
            positionList.Add(pos10);
            positionList.Add(pos20);
            positionList.Add(pos30);
            positionList.Add(pos40);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 15, wait: false);
            if (ret != 0)
                return 1;

            positionList.Clear();
            positionList.Add(pos50);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 20);
            if (ret != 0)
                return 1;

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(pos70);
            positionList.Add(pos60);
            positionList.Add(pos70);



            ret = await this.robotArm.SetPosition(ret, positionList, speed: 20);
            if (ret != 0)
                return 1;

            float[] pos1 = { -67.7f, -60.7f, -41.9f, 81f, -90f, 0 };
            float[] pos2 = { -67.7f, -55.6f, -48.1f, 91f, -90f, 0 };
            float[] pos3 = { -67.7f, -42.7f, -61f, 101.9f, -90f, 0 };
            float[] pos4 = { -67.7f, -25.6f, -77.6f, 116.7f, -90f, 0 };
            float[] pos5 = { -67.7f, -7.6f, -98.9f, 135.3f, -90f, 0 };
            float[] pos6 = { -67.7f, 10f, -118.6f, 152.5f, -90f, 0 };
            float[] pos7 = { -67.7f, 29f, -144.1f, 171.7f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos7);
            positionList.Add(pos6);
            positionList.Add(pos5);
            positionList.Add(pos4);
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(pos1);

            return await this.robotArm.SetPosition(ret, positionList, speed: 50);

        }
        public async Task<int> DoUrunBosaltmaVeProbeYikamaTest(KapType kapType, int temizlemeSuresi)
        {
            int ret = -1;
            List<Task<int>> paralelTasks = new List<Task<int>>();
            paralelTasks.Add(this.DoProbeTemizlemeTest(temizlemeSuresi));

            if (kapType == KapType.Bardak)
            {
                paralelTasks.Add(this.DoBosaltmaBardakTest());
            }
            else
            {
                paralelTasks.Add(this.DoBosaltmaKaseTest());
            }

            ret = await this.RunAsyncParalelTasks(paralelTasks);
            paralelTasks.Clear();
            if (ret != 0)
            {
                return 1;
            }
            return 0;

        }


        public async Task<int> DoCopAtmaTest()
        {
            int ret = 0;
            ret = await this.kesiciUnite.SetPosition(ret, lineerPos: 0, servoPos: null);

            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { -67.7f, -78.7f, -39.2f, 29.7f, -90f, 0 };
            float[] pos2 = { -67.7f, -115.5f, 0.6f, 25.3f, -90f, 0 };
            float[] pos3 = { -119.3f, -115.5f, 0.6f, 25.3f, -90f, 0 };
            float[] pos4 = { -120.9f, -33.5f, -4.2f, -29.3f, -90f, 0 };
            float[] pos5 = { -117f, 6.3f, -19.1f, -67.8f, -90f, 0 };
            float[] pos6 = { -117f, 6.3f, -19.1f, -67.8f, 90f, 0 };
            //reverse
            float[] pos7 = { -117f, 6.3f, -19.1f, -67.8f, -90f, 0 };
            float[] pos8 = { -120.9f, -33.5f, -4.2f, -29.3f, -90f, 0 };
            float[] pos9 = { -119.3f, -115.5f, 0.6f, 25.3f, -90f, 0 };
            float[] pos10 = { -67.7f, -115.5f, 0.6f, 25.3f, -90f, 0 };
            float[] pos11 = { -67.7f, -78.7f, -39.2f, 29.7f, -90f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos6);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 100, 100, 90, 90, 120);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos7);
            positionList.Add(pos8);
            positionList.Add(pos9);
            positionList.Add(pos10);
            positionList.Add(pos11);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 70);

            ret = await this.kesiciUnite.SetPosition(ret, lineerPos: null, servoPos: 125);
            return ret;
        }




        public async Task<int> DoKapKapatmaTest(KapType kapType)
        {
            int ret = 0;

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 95, 5, 5, 120);
            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos:null);

            if (kapType == KapType.Bardak)
            {
                ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 33);
            }
            else
            {
                ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 42);
            }

            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 0);
            ret = await this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);

            return 0;

        }

        public async Task<int> DoAmbalajSunumTest()
        {


            int ret = 0;
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);
            List<float[]> positionList = new List<float[]>();


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { -11.4f, 3.4f, -73.3f, 50.1f, -90, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 15, 100, 100, 120);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos1);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80);

            return ret;

        }

        public async Task<int> DoKaseSunumTest()
        {


            int ret = 0;
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);
            List<float[]> positionList = new List<float[]>();


            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 15, 100, 100, 120);
            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -87.8f, 0 }; // home position
            float[] pos2 = { -69.2f, -116.6f, 2.5f, 39f, -87.8f, 0 };
            float[] pos3 = { -69.2f, -90.4f, 7.5f, 12.6f, -87.8f, 0 };
            float[] pos4 = { -69.2f, -36.8f, 7.9f, -59.7f, -87.8f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);
            if (ret != 0)
                return 1;

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 40, 100, 100, 120);
            await Task.Delay(500);

            float[] pos5 = { -69.2f, -55f, 6.3f, -39.9f, -87.8f, 0 };

            positionList.Clear();
            positionList.Add(pos5);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 20);
            if (ret != 0)
                return 1;

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 45, 100, 100, 120);
            await Task.Delay(500);

            float[] pos6 = { -4.1f, -61f, -5.1f, -22.7f, -93.2f, 0 };
            float[] pos7 = { -4.1f, 22.5f, -37.1f, -76.1f, -93.2f, 0 };

            positionList.Clear();
            positionList.Add(pos6);
            positionList.Add(pos7);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 20);
            if (ret != 0)
                return 1;

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 40, 100, 100, 120);
            await Task.Delay(500);
            float[] pos8 = { -4.1f, 25.7f, -37.3f, -75.3f, -93.2f, 0 };

            positionList.Clear();
            positionList.Add(pos8);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);
            if (ret != 0)
                return 1;

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 15, 100, 100, 120);
            await Task.Delay(500);


            float[] pos9 = { -4.1f, 12.6f, -31.6f, -56f, -93.2f, 0 };
            float[] pos10 = { -4.1f, -39.3f, -11.7f, -0.6f, -93.2f, 0 };

            positionList.Clear();
            positionList.Add(pos9);
            positionList.Add(pos10);
            positionList.Add(pos1);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);
            if (ret != 0)
                return 1;

            ret = await this.urunAlmaUnite.IsPositionOK(ret, donmePos: null, lineerPos: 0);

            return 0;

        }


        public async Task<int> DoBardakSunumTest()
        {
            

            int ret = 0;
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);

            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 15, 100, 100, 120);



            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { -68.4f, -77f, 4.3f, -38.5f, -90, 0 };
            float[] pos3 = { -68.4f, -18.1f, 7.5f, -78, -90, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);



            ret = await this.robotArm.SetPosition(ret, positionList, speed: 60);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 100, 25, 25, 120);

            await Task.Delay(1000);

            float[] pos40 = { -68.4f, -33.4f, 7.6f, -66.1f, -90, 0 };
            float[] pos4 = { -68.4f, -65.2f, 7.4f, -32.3f, -90, 0 };
            float[] pos5 = { 0, -59.4f, 5.1f, -35.5f, -90, 0 };
            float[] pos6 = { -4.3f, 17.5f, -35.4f, -73f, -90, 0 };//*
            //float[] pos7 = { -4.6f, 25.6f, -36f, -71.2f, -90, 0 };

            float[] pos7 = { -4.3f, 26.4f, -37.5f, -77.5f, -90, 0 };
            float[] pos8 = { -4.3f, 26.4f, -37.5f, -73.4f, -90, 0 };
            float[] pos9 = { -4.3f, 26.2f, -37f, -70.3f, -90, 0 };

            positionList.Clear();
            positionList.Add(pos40);
            positionList.Add(pos4);
            positionList.Add(pos5);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);

            positionList.Clear();
            positionList.Add(pos6);
            positionList.Add(pos7);
            positionList.Add(pos8);


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);

            positionList.Clear();
            positionList.Add(pos9);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 10);
            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 15, 100, 100, 120);
            await Task.Delay(1000);


            float[] pos81 = { -4.3f, 22.1f, -30.5f, -70.4f, -90, 0 };
            float[] pos82 = { -4.2f, -1.6f, -19.8f, -70.4f, -90, 0 };
            float[] pos90 = { -4.6f, -98.2f, -32.3f, 40.8f, -90, 0 }; // home position



            positionList.Clear();
            positionList.Add(pos81);
            positionList.Add(pos82);
            positionList.Add(pos90);

            ret = await this.robotArm.SetPosition(ret, positionList, speed: 40);

            ret = await this.urunAlmaUnite.IsPositionOK(ret, donmePos: null, lineerPos: 0);

            return ret;

        }


        public async Task<int> DoServisSetiHazirlamaTest(int setNo)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95,95, 5, 5, 120);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -93.5f, 0 };
            float[] pos3 = { 201.8f, 1.2f, 1.1f, -91.6f, -93.5f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);


            if (setNo == 0)
            {
                float[] pos4 = { 203.6f, 31.9f, -64.2f, -58f, -93.5f, 0 };
                positionList.Add(pos4);
            }
            else if (setNo == 1)
            {
                float[] pos4 = { 203.6f, 31.9f, -64.2f, -58f, -93.5f, 0 };
                positionList.Add(pos4);
            }
            else if (setNo == 2)
            {
                float[] pos4 = { 203.6f, 31.9f, -64.2f, -58f, -93.5f, 0 };
                positionList.Add(pos4);
            }
            else if (setNo == 3)
            {
                float[] pos4 = { 203.6f, 31.9f, -64.2f, -58f, -93.5f, 0 };
                positionList.Add(pos4);
            }


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 90);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            if (setNo == 0)
            {
                float[] pos5 = { 203.6f, 24.6f, -61.2f, -49.3f, -93.5f, 0 };
                float[] pos6 = { 205.7f, 20.6f, -53.4f, -53.3f, -93.5f, 0 };
                float[] pos7 = { 207.1f, 17.5f, -48.2f, -55.6f, -93.5f, 0 };
                float[] pos8 = { 224.2f, 0.2f, -25.4f, -62f, -93.5f, 0 };

                float[] pos9 = { 201.8f, 1.2f, 1.1f, -91.6f, -93.5f, 0 };


                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);
                positionList.Add(pos7);
                positionList.Add(pos8);
                positionList.Add(pos9);

                ret = await this.robotArm.SetPosition(ret, positionList, speed: 15);

            }

            float[] pos10 = { 0, 1.2f, 1.1f, -91.6f, -93.5f, 0 };
            float[] pos11 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos12 = { -8, 20.2f, -61.3f, -48.5f, 90, 0 }; // home position

            positionList.Clear();
            positionList.Add(pos10);
            positionList.Add(pos11);
            positionList.Add(pos12);


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 90);
            ret = await this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos11);


            ret = await this.robotArm.SetPosition(ret, positionList, speed: 90);


            float[] pos13 = { 0, 21.7f, -126f, -58, -90, 0 };
            float[] pos14 = { 15, 21.7f, -126f, -58, -90, 0 };
            float[] pos15 = { -15, 21.7f, -126f, -58, -90, 0 };
            float[] pos16 = { 15, 21.7f, -126f, -58, -90, 0 };
            float[] pos17 = { -15, 21.7f, -126f, -58, -90, 0 };
            float[] pos18 = { 0, 21.7f, -126f, -58, -90, 0 };
            float[] pos19 = { 0, -98.2f, -32.3f, 40.8f, -90, 0 }; // home position

            positionList.Clear();

            positionList.Add(pos13);
            positionList.Add(pos14);
            positionList.Add(pos15);
            positionList.Add(pos16);
            positionList.Add(pos17);
            positionList.Add(pos18);
            positionList.Add(pos19);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            return ret;
        }


        public async Task<int> DoSelamlamaTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();


            float[] pos13 = { 0, 21.7f, -126f, -58, -90, 0 };
            float[] pos14 = { 15, 21.7f, -126f, -58, -90, 0 };
            float[] pos15 = { -15, 21.7f, -126f, -58, -90, 0 };
            float[] pos16 = { 15, 21.7f, -126f, -58, -90, 0 };
            float[] pos17 = { -15, 21.7f, -126f, -58, -90, 0 };
            float[] pos18 = { 0, 21.7f, -126f, -58, -90, 0 };
            float[] pos19 = { 0, -98.2f, -32.3f, 40.8f, -90, 0 }; // home position

            positionList.Clear();

            positionList.Add(pos13);
            positionList.Add(pos14);
            positionList.Add(pos15);
            positionList.Add(pos16);
            positionList.Add(pos17);
            positionList.Add(pos18);
            positionList.Add(pos19);
            ret = await this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            return ret;
        }

        public async Task<int> DoAmbalajTeslimTest()
        {
            int ret = 0;
            ret = await this.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);
            ret = await this.asansorUnite.SetPositionTask(ret, dikeyPos: 350);
            await Task.Delay(5000);
            ret = await this.asansorUnite.SetPositionTask(ret, dikeyPos: 0);
            return ret;

        }
        public async Task<int> DoUrunTeslimTest()
        {
            int ret = 0;
            ret = await this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: null);
            bool isUrunVarmi = this.asansorUnite.UrunVarmi();
            if (isUrunVarmi == true)
            {
                ret = await this.asansorUnite.SetPositionTask(ret, dikeyPos: 350);

                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1000);

                    isUrunVarmi = this.asansorUnite.UrunVarmi();
                    if (isUrunVarmi == true)
                    {
                        continue;
                    }
                    else
                    {
                        await Task.Delay(5000);
                        break;

                    }

                }

                ret = await this.asansorUnite.SetPositionTask(ret, dikeyPos: 0);

            }
            else
            {
                return 1;
            }

            ret = await this.kapakKapatmaUnite.IsPositionOK(ret, yatayPos: 0, dikeyPos: null);
            return ret;

        }

        private async Task<int> RunAsyncParalelTasks(List<Task<int>> paralelTasks)
        {
            //int ret = -1;
            //int taskCounter = paralelTasks.Count;
            //while (taskCounter > 0)
            //{
            //    Task<int> finishedTask = await Task.WhenAny(paralelTasks);
            //    taskCounter--;
            //    //paralelTasks.Remove(finishedTask);
            //}
            //foreach (var task in paralelTasks)
            //{
            //    if (task.Result == 0)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        return -1;
            //    }
            //}

            //ret = 0;
            //return ret;


            await Task.WhenAll(paralelTasks);
            foreach (var task in paralelTasks)
            {
                if (task.Result == 0)
                {
                    continue;
                }
                else
                {
                    return -1;
                }
            }

            return 0;


        }

    }

}
