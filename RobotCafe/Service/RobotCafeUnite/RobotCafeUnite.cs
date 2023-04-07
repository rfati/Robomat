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
        private int YikamaTaskResult = -1;
        private int DoluUrunYerlestirKaseResult = -1;
        private int DoluUrunYerlestirBardakResult = -1;

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


        public int DoHoming()
        {

            int ret = 0;

            ret = this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);
            ret = this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: 0);
            ret = this.kesiciUnite.SetPosition(ret, lineerPos: null, servoPos: 93);
            ret = this.isiticiUnite.SetPosition(ret, probPos: 35);
            ret = this.asansorUnite.SetPositionTask(ret, dikeyPos: 350);
            ret = this.asansorUnite.SetPosition(ret, dikeyPos: 0);
            ret = this.isiticiUnite.RunIsiticiONOFF(ret, run: true);

            Thread.Sleep(5000);

            ret = this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: 0);
            ret = this.kesiciUnite.SetPosition(ret, lineerPos: 2, servoPos: 93);

            Thread.Sleep(5000);

            ret = this.urunAlmaUnite.IsPositionOK(ret, donmePos: 0, lineerPos: 0);
            ret = this.kapakKapatmaUnite.IsPositionOK(ret, yatayPos: 0, dikeyPos: 0);
            ret = this.kesiciUnite.IsPositionOK(ret, lineerPos: 2, servoPos: 93);
            ret = this.isiticiUnite.IsPositionOK(ret, probPos: 35);
            ret = this.asansorUnite.SetPositionTask(ret, dikeyPos: 0);


            return ret;

        }
        public int GetReadyToService()
        {
            int ret = 0;

            ret = this.urunAlmaUnite.SetPositionTask(ret, donmePos: 0, lineerPos: null);
            ret = this.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 0);
            ret = this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret = this.KapVeKapakHazirlamaTask(ret);

            if (ret != 0)
            {
                Logger.LogError("Robot Cafe Unite  GetReadyToService Error.");
            }

            return ret;

        }

        public int GetReadyToNewService()
        {
            int ret = 0;
            ret = this.KapVeKapakHazirlamaTask(ret);

            if (ret != 0)
            {
                Logger.LogError("Robot Cafe Unite  GetReadyToService Error.");
            }

            return ret;

        }


        public int KapVeKapakHazirlamaTask(int ret)
        {
            if (ret != 0)
                return 1;
            bool is_hazir_bardak_ok = false;
            bool is_hazir_kase_ok = false;
            bool is_hazir_bardakKapak_ok = false;
            bool is_hazir_kaseKapak_ok = false;

            ret = 0;
            Thread.Sleep(1000);
            ret = this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 15, 15);

            int KapYerlestirTaskRet = -1;
            KapInfo kapInfo = this.cafeKapUnite.GetKapSensorInfo();
            if (kapInfo == null)
                return 1;

            if (kapInfo.is_hazir_bardak_ok == false)
            {
                for (int i = 0; i < kapInfo.dolu_bardak_no_list.Count; i++)
                {

                    KapYerlestirTaskRet =  this.DoBardakHazirlamaTest(bardakNo: kapInfo.dolu_bardak_no_list[i]);

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
                    KapYerlestirTaskRet =   this.DoKaseHazirlamaTest(kaseNo: kapInfo.dolu_kase_no_list[i]);
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
                for (int i = 0; i < 2; i++)
                {
                    KapakYerlestirTaskRet =   this.DoBardakKapakHazirlamaTest();
                    

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
                for (int i = 0; i < 1; i++)
                {
                    KapakYerlestirTaskRet =   this.DoKaseKapakHazirlamaTest();
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

            if (is_hazir_bardak_ok && is_hazir_kase_ok)
                return 0;
            else
                return 1;


            //if (is_hazir_bardak_ok && is_hazir_kase_ok && is_hazir_bardakKapak_ok && is_hazir_kaseKapak_ok)
            //    return 0;
            //else
            //    return 1;
        }

        private int DoBardakKapakHazirlamaTest(int tryCounter = 0)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            //ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 10, 15);

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
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos4);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            positionList.Clear();
            if (tryCounter == 0)
            {
                positionList.Add(kapakalma1);
            }
            else if (tryCounter == 1)
            {
                positionList.Add(kapakalma1);
            }

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 10);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            Thread.Sleep(1000);

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(posara);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            float[] pos31 = { 49.2f, 39f, -17.8f, -96f, 92.1f, 0 };
            float[] pos311 = { 53.7f, 61.5f, -84.6f, -69.2f, 92.1f, 0 };
            float[] pos32 = { 53.7f, 65f, -87.1f, -69.2f, 92.1f, 0 };

            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos31);
            positionList.Add(posrevize);
            positionList.Add(pos311);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos32);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 10);

            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            Thread.Sleep(1000);

            positionList.Clear();
            positionList.Add(pos311);
            positionList.Add(pos31);
            positionList.Add(pos3);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            return ret;
        }

        private int DoKaseHazirlamaTest(int kaseNo)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            //ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 10, 15);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos3 = { 127.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos40 = { 130.3f, 67.8f, -63f, -96.5f, -91.8f, 0 };
            float[] pos6 = { 0, -98.2f, -32.3f, 40.8f, 93.7f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos40);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80,wait:false);
            positionList.Clear();

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


            ret =   this.robotArm.SetPosition(ret, positionList, speed: 45);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            Thread.Sleep(1000);


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

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            positionList.Clear();
            positionList.Add(pos40);
            positionList.Add(pos3);
            positionList.Add(pos2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos6);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            positionList.Clear();
            positionList.Add(pos6x);
            positionList.Add(pos7);


            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            Thread.Sleep(1000);

            positionList.Clear();
            positionList.Add(pos6x);
            //positionList.Add(pos6);
            positionList.Add(pos1);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            return ret;
        }
 
        private int DoBardakHazirlamaTest(int bardakNo = 0)
        {

            int ret = 0;

            List<float[]> positionList = new List<float[]>();

            //ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 10, 15);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 91f, -45.6f, 9.9f, -48.7f, -91.8f, 0 };
            float[] pos3 = { 91f, 50.9f, -10.7f, -84.9f, -91.8f, 0 };
            float[] posara = { 42.3f, -54.2f, -17.3f, -19.9f, 90.6f, 0 };
            float[] poscart = { 91f, 61.9f, -55.3f, -91.9f, -91.8f, 0 };



            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);


            if (bardakNo == 0)
            {
                float[] pos4 = { 102.1f, 76.7f, -79.7f, -87.8f, -91.8f, 0 };
                float[] pos5 = { 102.1f, 53.4f, -63.1f, -81f, -91.8f, 0 };
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (bardakNo == 1)
            {
                float[] pos4 = { 89f, 94.5f, -93.8f, -89.8f, -91.8f, 0 };
                float[] pos5 = { 89, 49.5f, -53.1f, -85.8f, -91.8f, 0 };//bardak alma
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

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            Thread.Sleep(1000);

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

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 40);
            float[] posyeni = { 30.6f, -95.7f, 0, -0.3f, 88.5f, 0 };




            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(posyeni);
            positionList.Add(posara);
            positionList.Add(pos6);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos7); // bardak bırakma noktası


            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            Thread.Sleep(1000);

            positionList.Clear();
            positionList.Add(pos6);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 5);

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(pos1);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            return ret;
        }

        private int DoKaseKapakHazirlamaTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            //ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 10, 15);

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
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 10);

            float[] pos6 = { 158.4f, 49.3f, -55f, -84.6f, -92.9f, 0 };//kapak alma noktası


            positionList.Clear();
            positionList.Add(pos6);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 10);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            Thread.Sleep(1000);

            positionList.Clear();
            positionList.Add(pos5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 3);

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
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos34);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 10);


            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            Thread.Sleep(1000);


            positionList.Clear();
            positionList.Add(pos33x);
            positionList.Add(pos33);
            positionList.Add(pos32);
            positionList.Add(pos2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            return ret;
        }


        public int DoKaseYerlestirme()
        {
            int ret = 0;

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);

            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 37.5f, -58, -42.6f, 7, 89.7f, 0 };
            float[] pos3 = { 37.5f, -31.9f, -55.5f, -3.8f, 90.2f, 0 }; //kase alma posizyon
            float[] deneme = { -54.6f, -58f, -42.5f, 40.7f, -91.8f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos3);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 120, 10, 15);

            Thread.Sleep(1000);

            float[] pos4 = { 37.5f, -29.8f, -67.5f, 7.4f, 90.2f, 0 };
            float[] pos5 = { 37.5f, -76f, -37.2f, 16.9f, 90.2f, 0 };
            float[] pos4x = { -73.9f, -65.1f, -1.6f, -20.2f, -90.3f, 0 };
            float[] posara = { -73.9f, -44.3f, 6f, -50.6f, -90.3f, 0 };
            float[] pos5x = { -74f, -35.7f, 6.9f, -60f, -91.3f, 0 };//kasebırakma

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos5x);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);

            float[] pos8 = { -67.9f, -113.9f, -21.8f, 47.5f, -91.4f, 0 };
            positionList.Clear();
            positionList.Add(pos4x);
            positionList.Add(pos1);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 120, 120);


            Logger.LogInfo("DoKaseYerlestirme ret: " + ret);
            return ret;
        }

        public int DoBardakYerlestirme()
        {
            int ret = 0;
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);

            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 37.6f, -17.5f, 1f, -75.6f, 89.1f, 0 };
            float[] pos3 = { 37.6f, -8.4f, -19.2f, -61.9f, 89.9f, 0 }; //bardak alma position
            float[] deneme = { -54.6f, -58f, -42.5f, 40.7f, -91.8f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos3);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 160, 10, 15);

            Logger.Error("bekleyecek mi");
            Thread.Sleep(1000);
            Logger.Error("evet bekledi");


            float[] pos40x = { 37.6f, -12.8f, -20f, -57.9f, 89.1f, 0 };
            float[] pos40 = { 37.6f, -30.2f, -8.6f, -80.4f, 89.1f, 0 };
            float[] pos4 = { -74f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] pos41 = { -74f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] pos4x = { -74f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] posara = { -74f, -45f, 5.6f, -48.3f, -91.3f, 0 };
            float[] posara2 = { -74f, -31.4f, 6f, -62.8f, -91.3f, 0 };

            //float[] pos5 = { -74f, -23.5f, 6.3f, -69.6f, -91.3f, 0 };//bardak bırakma
            float[] pos5 = { -74f, -18.6f, 5.7f, -75.7f, -90.5f, 0 };//bardak bırakma

            positionList.Clear();
            positionList.Add(pos40x);
            positionList.Add(pos40);
            positionList.Add(pos41);
            positionList.Add(pos4x);
            positionList.Add(posara);
            positionList.Add(posara2);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);

            Thread.Sleep(1000);

            float[] son = { -74f, -113.9f, -21.8f, 47.5f, -91.4f, 0 };

            positionList.Clear();
            positionList.Add(posara2);
            positionList.Add(posara);
            positionList.Add(pos4);
            positionList.Add(son);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 120, 120);
            //ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            return ret;

        }

        public int DoUrunAlma(ServiceType serviceType)
        {
            int ret = 0;
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 120, 120);

            List<float[]> positionList = new List<float[]>();

            float[] pose4 = { -54.6f, -40.9f, -60f, 53.8f, -91.8f, 0 };
            float[] pose5 = { -54.6f, -90.2f, -48f, 53.8f, -91.8f, 0 };
            float[] deneme = { -54.6f, -58f, -42.5f, 40.7f, -91.8f, 0 };
            float[] tekrar = { -54.6f, -29.6f, -53.9f, 34.3f, -91.4f, 0 };


            positionList.Clear();

            positionList.Add(deneme);
            positionList.Add(tekrar);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 60);

            Thread.Sleep(500);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 3, 10);
            Thread.Sleep(500);

            positionList.Clear();
            positionList.Add(pose4);
            positionList.Add(pose5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 60);

            float[] vakumtut = { -54.1f, -113.2f, -6.1f, -39.6f, -91.8f, 0 };

            ret =   this.vakumUnite.RunIsiticiVakum(ret, run: true);

            positionList.Clear();
            positionList.Add(vakumtut);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 30, 30);
            Thread.Sleep(500);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 3, 10);
            ret = this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);
            if(serviceType != ServiceType.Package)
            {
                ret = this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 32, dikeyPos: null);
            }

            Thread.Sleep(300);

            return ret;
        }

        public int DoKesme()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();


            float[] pos100 = { -116.7f, -103.6f, 6, 26, -91.4f, 0 };
            float[] pos10 = { -116.6f, -27.1f, -26.4f, -0.6f, -91.4f, 0 };
            float[] pos10x = { -116.6f, -43.7f, -42.4f, 52.6f, -91.4f, 0 };//
            float[] pos4 = { -116.6f, -47.9f, -33.8f, 31.7f, -91.4f, 0 };
            float[] pos5xx = { -116.6f, -50f, -23f, 7.8f, -91.4f, 0 };
            float[] poskesme = { -116.6f, -42.6f, -19.7f, -11.6f, -91.4f, 0 };
            float[] poskesmeyeni = { -116.6f, -42.5f, -20.6f, -11.4f, -91.4f, 0 };
            float[] sikmadansonra = { -116.6f, -44.1f, -19f, -11.4f, -91.4f, 0 };
            float[] pos6 = { -116.7f, -42.7f, -4.5f, -24f, -90f, 0 };
            float[] pos7 = { -116.7f, -107f, 0f, 25.8f, -90f, 0 };
            float[] posbakalim = { -116.7f, -41.4f, -19.6f, -10.8f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos100);
            positionList.Add(pos10);
            positionList.Add(pos10x);
            positionList.Add(pos4);
            positionList.Add(pos5xx);

            ret =   this.isiticiUnite.SetPosition(ret, probPos: 170);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(posbakalim);
            positionList.Add(poskesme);
            positionList.Add(poskesmeyeni);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 30);

            ret =   this.kesiciUnite.SetPositionTask(ret, lineerPos: 50, servoPos: null);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 30, 30);

            positionList.Clear();
            positionList.Add(sikmadansonra);

            Thread.Sleep(300);
            ret =   this.kesiciUnite.SetPositionTask(ret, lineerPos: null, servoPos: 0);

            Thread.Sleep(800);
            positionList.Clear();
            positionList.Add(pos6);
            positionList.Add(pos7);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 60, wait: false);

            return ret;
        }


        public int DoBosaltmaBardak()
        {

            int ret = 0;

            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 170);
            ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: true);
            ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: true);


            ret =   this.kesiciUnite.SetPosition(ret, lineerPos: 2, servoPos: null);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 120, 120);


            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            List<float[]> positionList = new List<float[]>();


            float[] baslangic = { -68f, -104.8f, -8.2f, 25.8f, -90f, 0 };
            positionList.Clear();
            positionList.Add(baslangic);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50, wait: true);
            if (ret != 0)
                return 1;

            float[] pos10 = { -68f, -73.9f, -29.7f, 72f, -90f, 0 };
            float[] pos20 = { -68f, -54f, -48.5f, 93.7f, -90f, 0 };
            float[] pos30 = { -68f, -39.8f, -63f, 106.4f, -90f, 0 };
            float[] pos40 = { -68f, -20.5f, -82.7f, 124.4f, -90f, 0 };
            float[] pos40x = { -68f, 22.2f, -129.5f, 159.5f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos10);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 20, wait: false);
            if (ret != 0)
                return 1;




            positionList.Clear();
            positionList.Add(pos20);
            positionList.Add(pos30);
            positionList.Add(pos40);
            positionList.Add(pos40x);


            ret =   this.robotArm.SetPosition(ret, positionList, speed: 20, wait: false);
            if (ret != 0)
                return 1;

            ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
            ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: false);
            ret =   this.isiticiUnite.SetPosition(ret, probPos: 35);


            float[] pos1 = { -68f, -70.3f, -42.4f, 80.8f, -90f, 0 };
            float[] pos2 = { -68f, -111.4f, -33.7f, 47.6f, -90f, 0 };


            positionList.Clear();

            positionList.Add(pos1);
            positionList.Add(pos2);

            return   this.robotArm.SetPosition(ret, positionList, speed: 60);

        }
        public int DoBosaltmaKase()
        {

            int ret = 0;

            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 170);
            ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: true);
            ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: true);

            ret =   this.kesiciUnite.SetPosition(ret, lineerPos: 2, servoPos: null);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 120, 120);

            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            List<float[]> positionList = new List<float[]>();

            float[] baslangic = { -60.1f, -104.8f, -8.2f, 25.8f, -90f, 0 };
            positionList.Clear();
            positionList.Add(baslangic);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50, wait: true);
            if (ret != 0)
                return 1;

            float[] pos10x = { -68.8f, -107.8f, -1.8f, 21.5f, -90f, 0 };
            float[] pos10 = { -66.8f, -46.4f, -59.2f, 101.9f, -90f, 0 };
            float[] pos20 = { -68.8f, -28.8f, -77.1f, 117.5f, -90f, 0 };
            float[] pos30 = { -66.8f, -16.2f, -89.5f, 127.7f, -90f, 0 };
            float[] pos40 = { -68.8f, 18.7f, -128.5f, 159.7f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos10x);
            positionList.Add(pos10);
            positionList.Add(pos20);
            positionList.Add(pos30);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 8, wait: false);
            positionList.Clear();
            positionList.Add(pos40);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 30, wait: true);

            if (ret != 0)
                return 1;
            Thread.Sleep(2000);

            ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
            ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: false);
            ret =   this.isiticiUnite.SetPosition(ret, probPos: 35);

            float[] pos1 = { -62.7f, -70.3f, -42.4f, 80.8f, -90f, 0 };
            float[] pos2 = { -62.7f, -111.4f, -33.7f, 47.6f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);

            return   this.robotArm.SetPosition(ret, positionList, speed: 90);

        }
        public int DoCopAtma()
        {
            int ret = 0;
            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 35);
            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { -119.6f, -38.9f, -31.8f, 50.5f, -90f, 0 };
            float[] pos2 = { -66.7f, -111.4f, -33.7f, 47.6f, -90f, 0 };

            ret =   this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 24, dikeyPos: null);

            positionList.Clear();
            positionList.Add(pos1);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90);

            ret =   this.vakumUnite.RunIsiticiVakum(ret, run: false);

            positionList.Clear();
            positionList.Add(pos2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90);
            ret =   this.kesiciUnite.SetPosition(ret, lineerPos: null, servoPos: 93);
            return ret;

        }


        public int DoIsitmaBardak(int isitmaSuresi)
        {
            int ret = 0;
            List<Task<int>> paralelTasks = new List<Task<int>>();
            List<float[]> positionList = new List<float[]>();

            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);
            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 35);

            float[] pos4 = { -74f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] pos4x = { -74f, -62.1f, 3.1f, -34.8f, -90.5f, 0 };
            float[] posara = { -74f, -45f, 5.6f, -48.3f, -90.5f, 0 };
            float[] pos5 = { -74f, -18.6f, 5.7f, -75.7f, -90.5f, 0 };//bardak bırakma
            float[] posara2 = { -74f, -31.4f, 6f, -62.8f, -91.3f, 0 };


            //float[] pos5 = { -74f, -23.5f, 6.3f, -69.6f, -91.3f, 0 };//bardak bırakma


            float[] posisitmayaklasma1 = { -139f, -58.4f, 4.1f, -33.4f, -89.8f, 0 };
            float[] posisitmayaklasma2 = { -139f, 14.3f, -22.4f, -76.6f, -89.8f, 0 };
            float[] posisitmayaklasma3 = { -137.8f, 29.4f, -49.8f, -68.1f, -89.8f, 0 };
            float[] posisitma = { -140.6f, 26f, -48.5f, -62.6f, -89.8f, 0 };

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos4x);
            positionList.Add(posara);
            positionList.Add(posara2);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos5);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 155, 10, 15);
            Thread.Sleep(500);

            positionList.Clear();
            positionList.Add(posara2);
            positionList.Add(posara);
            positionList.Add(pos4x);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);


            //ret =   this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: true);
            //ret =   this.robotCafeUnite.isiticiUnite.RunIsiticiBuhar(run: 1);

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            positionList.Add(posisitmayaklasma2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            //positionList.Add(posisitmayaklasma3);
            positionList.Add(posisitma);


            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 100);
            Thread.Sleep(500);


            ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: true);
            Thread.Sleep(1000);
            ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: true);

            Thread.Sleep(isitmaSuresi);

            ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
            ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: false);

            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 35);
            Thread.Sleep(1000);
            positionList.Clear();
            //positionList.Add(posisitmayaklasma3);
            positionList.Add(posisitmayaklasma2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            YikamaVeDoluUrunYerlestirBardak();

            if (YikamaTaskResult == 0 && DoluUrunYerlestirBardakResult == 0)
            {
                ret = 0;
            }
            else
            {
                ret = 1;
            }

            return ret;
        }

        public void YikamaVeDoluUrunYerlestirBardak()
        {

            Thread t1 = new Thread(new ThreadStart(YikamaTask));
            Thread t2 = new Thread(new ThreadStart(DoluUrunYerlestirBardak));
            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();


        }

        public int DoIsitmaKase(int isitmaSuresi)
        {
            int ret = 0;
            List<Task<int>> paralelTasks = new List<Task<int>>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);
            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 35);

            List<float[]> positionList = new List<float[]>();

            float[] pos4x = { -73.9f, -65.1f, -1.6f, -20.2f, -90.3f, 0 };
            float[] posara = { -73.9f, -44.3f, 6f, -50.6f, -90.3f, 0 };
            float[] pos5 = { -73.9f, -35.7f, 6.9f, -60f, -91.3f, 0 };//kasebırakma
            float[] pos4 = { -73.9f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] posisitmayaklasma1 = { -139f, -58.4f, 4.1f, -33.4f, -89.8f, 0 };
            float[] posisitmayaklasma2 = { -139f, 14.3f, -22.4f, -76.6f, -89.8f, 0 };
            float[] posisitmayaklasma3 = { -137.8f, 29.4f, -49.8f, -68.1f, -89.8f, 0 };
            float[] posisitma = { -140.4f, 28.3f, -52.1f, -68.4f, -89.8f, 0 };

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 70, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 70);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 110, 10, 15);
            Thread.Sleep(500);

            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            positionList.Add(posisitmayaklasma1);
            positionList.Add(posisitmayaklasma2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 70,wait:false);

            positionList.Clear();
            positionList.Add(posisitma);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 40, wait:true);

            ret =   this.isiticiUnite.SetPositionTask(ret, probPos: 90);

            Thread.Sleep(500);

            ret = this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: true);
            ret = this.isiticiUnite.RunIsiticiBuhar(ret, run: true);
            Thread.Sleep((isitmaSuresi));

            ret = this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
            ret = this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: false);
            ret = this.isiticiUnite.SetPositionTask(ret, probPos: 35);

            positionList.Clear();
            positionList.Add(posisitmayaklasma2);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 30, wait: false);

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 30);

            YikamaVeDoluUrunYerlestirKase();

            if(YikamaTaskResult == 0 && DoluUrunYerlestirKaseResult == 0)
            {
                ret = 0;
            }
            else
            {
                ret = 1;
            }

            return ret;
        }

        public void YikamaVeDoluUrunYerlestirKase()
        {

            Thread t1 = new Thread(new ThreadStart(YikamaTask));
            Thread t2 = new Thread(new ThreadStart(DoluUrunYerlestirKase));
            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }




        public void DoluUrunYerlestirBardak()
        {
            List<float[]> positionList = new List<float[]>();

            float[] posisitmayaklasma1 = { -139f, -58.4f, 4.1f, -33.4f, -89.8f, 0 };
            float[] posisitmayaklasma2 = { -139f, 14.3f, -22.4f, -76.6f, -89.8f, 0 };
            float[] posurunalma = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; //yaklasma 3  

            float[] pos4 = { -74f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3

            float[] pos4x = { -74f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] posara = { -74f, -45f, 5.6f, -48.3f, -91.3f, 0 };
            float[] posara2 = { -74f, -31.4f, 6f, -62.8f, -91.3f, 0 };


            //float[] pos5 = { -74f, -23.5f, 6.3f, -69.6f, -91.3f, 0 };//bardak bırakma
            float[] pos5 = { -74f, -18.6f, 5.7f, -75.7f, -90.5f, 0 };//bardak bırakma


            int ret = 0;
            positionList.Clear();
            positionList.Add(pos4x);
            positionList.Add(posara);
            positionList.Add(posara2);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 40, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 40);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);

            positionList.Clear();
            positionList.Add(posara2);
            positionList.Add(posara);
            positionList.Add(pos4x);
            positionList.Add(pos4);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(posurunalma);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret = this.DoBardakKapakYerlestirme(ret);
            ret = this.DoKapKapatma(ret, KapType.Bardak);

            DoluUrunYerlestirBardakResult = ret;
        }

        public int DoBardakKapakYerlestirme(int ret)
        {
            if (ret != 0)
                return 1;
            List<float[]> positionList = new List<float[]>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 10, 15);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 25.2f, 0.7f, 3.8f, -69.7f, 86.2f, 0 };
            float[] pos32 = { 54.3f, 61.3f, -84.4f, -67.8f, 88.5f, 0 };
            float[] pos33 = { 54.3f, 66.2f, -87.3f, -69.9f, 88.8f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos32);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos33);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: true);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            Thread.Sleep(500);

            float[] pos5 = { 0, -98.2f, -32.3f, 40.8f, 88.5f, 0 }; // home position
            float[] pos60 = { -66.7f, -33.8f, -27.1f, -29.5f, 92f, 0 };
            float[] pos6 = { -66.7f, -32.9f, -25.8f, -31.3f, 92f, 0 };  //bardak kapağı yerleştirme noktası

            positionList.Clear();
            positionList.Add(pos32);
            positionList.Add(pos2);
            positionList.Add(pos5);
            positionList.Add(pos60);
            //positionList.Add(pos6);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(pos6);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: true);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);
            Thread.Sleep(500);

            float[] finish = { -63.7f, -112, -21.3f, 48, -88.5f, 0 };  //bardak kapağı yerleştirme noktası

            ret =   this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: null);

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(finish);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            return ret;
        }

        public int DoKaseKapakYerlestirme(int ret)
        {
            if (ret != 0)
                return 1;

            List<float[]> positionList = new List<float[]>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 10, 10);
           

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 18.5f, 29.2f, -11f, -96.5f, 91.1f, 0 };
            float[] pos3 = { 66.2f, 56.6f, -65.5f, -83.8f, 91.1f, 0 };
            float[] pos4 = { 66.2f, 61f, -69.4f, -82.8f, 91.1f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);


           
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

           

            Thread.Sleep(1000);

            float[] pos5 = { 0, -98.2f, -32.3f, 40.8f, 91.4f, 0 }; // home position j5 farklı
            float[] pos5x = { -64.9f, -34f, -24.6f, -29f, 89.7f, 0 }; // home position j5 farklı
            float[] pos6 = { -66.2f, -30.6f, -22.9f, -39.7f, 88.7f, 0 };  //kase kapağı yerleştirme noktası

            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(pos5);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            Logger.Error("adimc");


            positionList.Clear();
            positionList.Add(pos6);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            Thread.Sleep(1000);

            float[] finish = { -60.6f, -98.2f, -32.3f, 40.8f, -93.5f, 0 };  //finish

            positionList.Clear();
            positionList.Add(pos5);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            positionList.Clear();
            positionList.Add(finish);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            Thread.Sleep(1300);

            ret =   this.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: null);
            ret =   this.kapakKapatmaUnite.SetPosition(ret, yatayPos: null, dikeyPos: 17);

            return ret;

        }

        private void DoluUrunYerlestirKase()
        {
            List<float[]> positionList = new List<float[]>();

            float[] posurunalma = { -18.7f, -105f, -21.3f, 49.8f, -90.3f, 0 }; //yaklasma 3
            float[] pos4x = { -73.9f, -65.1f, -1.6f, -20.2f, -90.3f, 0 };
            float[] posara = { -73.9f, -44.3f, 6f, -50.6f, -90.3f, 0 };
            float[] pos5x = { -74f, -35.7f, 6.9f, -60f, -91.3f, 0 };//kasebırakma

            int ret = 0;
            positionList.Clear();
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 40, wait: false);

            positionList.Clear();
            positionList.Add(pos5x);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 10, 15);

            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: false);

            positionList.Clear();
            positionList.Add(posurunalma);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);

            ret = this.DoKaseKapakYerlestirme(ret);
            ret = this.DoKapKapatma(ret, KapType.Kase);

            DoluUrunYerlestirKaseResult = ret;
        }

        public int DoKapKapatma(int ret, KapType kapType)
        {
            if (ret != 0)
                return ret;

            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            if (kapType == KapType.Bardak)
            {
                ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 34);
            }
            else
            {
                ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 40);
            }

            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 0);
            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);

            return 0;
        }

        private void YikamaTask()
        {

            int ret = 0;
            ret =   this.isiticiUnite.SetPosition(ret, probPos: 170);

            Thread.Sleep(5500);

            for (int i = 0; i < 2; i++)
            {
                ret =   this.isiticiUnite.RunIsiticiYikamaBuharONOFF(ret, run: true);
                ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: true);
                Thread.Sleep(3500);

                ret =   this.isiticiUnite.RunIsiticiYikamaBuharONOFF(ret, run: false);
                ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: true);
                Thread.Sleep(3500);


                ret =   this.isiticiUnite.RunIsiticiBuhar(ret, run: false);
                ret =   this.isiticiUnite.RunIsiticiYikamaBuharONOFF(ret, run: false);
                ret =   this.isiticiUnite.RunIsiticiProbeBuharONOFF(ret, run: false);

                ret =   this.vakumUnite.RunIsitmaBuhar(ret, run: true);
                Thread.Sleep(3500);
                ret =   this.vakumUnite.RunIsitmaBuhar(ret, run: false);

            }

            ret =   this.isiticiUnite.SetPosition(ret, probPos: 35);
            YikamaTaskResult = ret;
        }

        public int DoBardakSunum()
        {
            int ret = 0;
            //ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret =   this.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);

            List<float[]> positionList = new List<float[]>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 15, 20);

            float[] start = { -63.7f, -112, -21.3f, 48, -88.5f, 0 };  //bardak kapağı yerleştirme noktası

            float[] pos4 = { -74f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] pos4x = { -74f, -62.1f, 3.1f, -34.8f, -90.5f, 0 };
            float[] posara = { -74f, -45f, 5.6f, -48.3f, -90.5f, 0 };
            float[] posara2 = { -74f, -31.4f, 6f, -62.8f, -91.3f, 0 };

            float[] pos5 = { -74f, -18.6f, 5.7f, -75.7f, -90.5f, 0 };//bardak bırakma


            positionList.Clear();
            positionList.Add(start);
            positionList.Add(pos4);
            positionList.Add(pos4x);
            positionList.Add(posara);
            positionList.Add(posara2);
            positionList.Add(pos5);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 70);
            Thread.Sleep(300);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 155, 15, 20);
            Thread.Sleep(300);

            positionList.Clear();

            positionList.Add(posara2);
            positionList.Add(posara);
            positionList.Add(pos4x);
            positionList.Add(pos4);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 45);

            float[] pos11 = { -2.9f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] pos12 = { -3.6f, 13.9f, -28.2f, -75.2f, -89.8f, 0 }; //yaklasma 3
            float[] pos13 = { -3.6f, 23.5f, -30f, -81.3f, -89.8f, 0 }; //yaklasma 3
            float[] pos14 = { -4.1f, 15f, -27.8f, -64.5f, -89.8f, 0 }; //yaklasma 3
            float[] pos15 = { -4.1f, 15.6f, -27.8f, -64.5f, -89.8f, 0 }; //yaklasma 3
            float[] pos16 = { -4.1f, 16.3f, -27.5f, -64.5f, -89.8f, 0 }; //yaklasma 3







            positionList.Clear();
            positionList.Add(pos11);
            positionList.Add(pos12);
            positionList.Add(pos13);
            positionList.Add(pos14);
            positionList.Add(pos15);
            positionList.Add(pos16);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 45);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 15, 20);


            float[] cikis = { -4.3f, -30.1f, 9f, -54.3f, -89.8f, 0 }; //yaklasma 3
                                                                      // home position

            positionList.Clear();
            positionList.Add(cikis);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);
            //ret =   this.urunAlmaUnite.IsPositionOK(ret, donmePos: null, lineerPos: 0);

            return ret;

        }

        public int DoKaseSunum()
        {


            int ret = 0;
            ret =   this.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret =   this.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);

            List<float[]> positionList = new List<float[]>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 15, 20);


            float[] pos4x = { -73.9f, -65.1f, -1.6f, -20.2f, -90.3f, 0 };
            float[] posara = { -73.9f, -44.3f, 6f, -50.6f, -90.3f, 0 };
            float[] pos5 = { -73.9f, -35.7f, 6.9f, -60f, -91.3f, 0 };//kasebırakma
            float[] pos4 = { -73.9f, -115.3f, -3f, 44.1f, -89.8f, 0 }; //yaklasma 3





            positionList.Clear();
            positionList.Add(pos4);
            //positionList.Add(pos4x);
            positionList.Add(posara);
            positionList.Add(pos5);


            ret =   this.robotArm.SetPosition(ret, positionList, speed: 70);
            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 110, 15, 20);

            Thread.Sleep(1000);
            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            //positionList.Add(pos4);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            float[] pos11 = { -4.2f, -65.7f, -12.7f, -13.8f, -89.8f, 0 };
            float[] pos12 = { -4.2f, 18.9f, -28.9f, -79.1f, -89.8f, 0 };
            float[] pos13 = { -4.2f, 22.8f, -29.5f, -79.3f, -89.8f, 0 };

            positionList.Clear();
            positionList.Add(pos11);
            positionList.Add(pos12);
            positionList.Add(pos13);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 15, 20);





            float[] cikis = { -4.3f, -30.1f, 9f, -54.3f, -89.8f, 0 }; //yaklasma 3
            positionList.Clear();

            positionList.Add(cikis);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80);



            return ret;

        }

        public int DoServisSetiHazirlama(int setNo = 0)
        {



            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 150, 15, 20);
            float[] home = { -4.3f, -91.8f, -25.1f, 26.5f, -89.8f, 0 }; //yaklasma 3

            float[] cikis = { -4.3f, -30.1f, 9f, -54.3f, -89.8f, 0 }; //yaklasma 3
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -93.5f, 0 };
            float[] pos3 = { 201.8f, 1.2f, 1.1f, -91.6f, -93.5f, 0 };


            positionList.Clear();
            positionList.Add(cikis);
            positionList.Add(pos2);
            positionList.Add(pos3);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90, wait: false);
            positionList.Clear();


            if (setNo == 0)
            {
                float[] pos4 = { 203.3f, 28.1f, -56.3f, -59.7f, -90.4f, 0 };//yaklaşma
                float[] pos4x = { 203.3f, 23.8f, -57.3f, -54.6f, -90.4f, 0 };//alma

                positionList.Add(pos4);
                positionList.Add(pos4x);

            }
            else if (setNo == 1)
            {
                float[] pos4 = { 197.6f, 24f, -48.4f, -63.6f, -90.4f, 0 };//yaklaşma
                float[] pos4x = { 197.6f, 19f, -49.9f, -56.8f, -90.4f, 0 };//alma

                positionList.Add(pos4);
                positionList.Add(pos4x);
            }
            else if (setNo == 2)
            {
                float[] pos4 = { 187.8f, 21.7f, -42.7f, -68f, -90.4f, 0 };//yaklaşma
                float[] pos4x = { 187.8f, 16.6f, -43.8f, -64.1f, -90.4f, 0 };//alma

                positionList.Add(pos4);
                positionList.Add(pos4x);
            }

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90, wait: true);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            Thread.Sleep(1000);

            if (setNo == 0)
            {
                float[] pos5 = { 206.2f, 15.2f, -45.6f, -56.8f, -90.4f, 0 };
                float[] pos6 = { 220.1f, -14.5f, -9.8f, -56.8f, -90.4f, 0 };

                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);

                ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            }
            if (setNo == 1)
            {
                float[] pos5 = { 200.3f, 9.3f, -37.9f, -56.9f, -90.4f, 0 };
                float[] pos6 = { 211.8f, -27.1f, -1.5f, -56.8f, -90.4f, 0 };

                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);

                ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            }
            if (setNo == 2)
            {
                float[] pos5 = { 189.5f, 7.1f, -31.1f, -64.7f, -90.4f, 0 };
                float[] pos6 = { 198f, -32.7f, 5.8f, -59.2f, -90.4f, 0 };

                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);

                ret =   this.robotArm.SetPosition(ret, positionList, speed: 50);

            }

            //float[] pos6x = { 224.6f, -73.5f, -32.3f, 104.8f, -91.8f, 0 };
            //float[] pos7x = { -6.7f, -73.5f, -32.3f, 104.8f, -91.8f, 0 };
            //float[] pos8x = { -6.7f, 11.3f, -58.8f, -34.7f, -182.2f, 0 };

            float[] posa = { 223.8f, -58.7f, -29.4f, 84.9f, -91.8f, 0 };
            float[] posb = { 18f, -58.7f, -29.4f, 84.9f, -91.8f, 0 };
            //float[] posc = { 0.8f, -0.9f, -82.5f, 81.9f, -91.8f, 0 };
            float[] posc = { 11.6f, 12.5f, -67.5f, 8.1f, 20.1f, 0 };


            positionList.Clear();
            positionList.Add(posa);
            positionList.Add(posb);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90, wait: false);

            positionList.Clear();
            positionList.Add(posc);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90);
            ret =   this.vakumUnite.RunRobotTutucuVakum(ret, run: false);


            positionList.Clear();
            positionList.Add(home);
            float[] urunAlmaBekleme = { -54.6f, -58f, -42.5f, 40.7f, -91.8f, 0 };
            positionList.Add(urunAlmaBekleme);
            Thread.Sleep(300);
            ret =   this.robotArm.SetPosition(ret, positionList, speed: 90);




            return ret;
        }

        public int DoUrunTeslim(ServiceType serviceType)
        {
            int ret = 0;

            if(serviceType == ServiceType.Package)
            {
                ret = this.asansorUnite.SetPositionTask(ret, dikeyPos: 350);
                Thread.Sleep(5000);
                ret = this.asansorUnite.SetPositionTask(ret, dikeyPos: 0);

                return ret;
            }

            bool isUrunVarmi = this.asansorUnite.UrunVarmi();
            if (isUrunVarmi == true)
            {
                //ret =   this.asansorUnite.SetPositionTask(ret, dikeyPos: 350);
                //for (int i = 0; i < 20; i++)
                //{
                //    isUrunVarmi = this.asansorUnite.UrunVarmi();
                //    if (isUrunVarmi == true)
                //    {
                //        Thread.Sleep(1000);
                //        continue;
                //    }
                //    else
                //    {
                //        break;

                //    }

                //}
                //Thread.Sleep(3000);
                //ret =   this.asansorUnite.SetPositionTask(ret, dikeyPos: 0);

                ret = this.asansorUnite.SetPosition(ret, dikeyPos: 350);
                Thread.Sleep(20000);
                ret = this.asansorUnite.SetPositionTask(ret, dikeyPos: 0);

            }
            else
            {
                return 1;
            }


            return ret;

        }




        public int DoColdService(KapType kapType)
        {
            int ret = 0;
            if(kapType == KapType.Bardak)
            {
                ret =   this.DoBardakKapakYerlestirme(ret);
            }
            else
            {
                ret =   this.DoKaseKapakYerlestirme(ret);
            }
            ret =   this.DoKapKapatma(ret,KapType.Bardak);

            return ret;
        }

        public int DoPackageService()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret =   this.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 45, 120, 120);
            float[] pos1 = { 2.4f, -43.8f, -81.5f, 128.7f, -87.7f, 0 }; 
            float[] pos2 = { 2.4f, 59.4f, -169.6f, 128.7f, -87.7f, 0 };
            float[] home = { -4.3f, -91.8f, -25.1f, 26.5f, -89.8f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: true);
            ret =   this.vakumUnite.RunIsiticiVakum(ret, run: false);

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(home);

            ret =   this.robotArm.SetPosition(ret, positionList, speed: 80, wait: true);
            ret = this.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: 0);
            return ret;
        }

    }

}
