using BLL;
using Common;
using Model;
using RobotCafe.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotCafe.Service
{

    public class TestService
    {
        private OtomatUnite otomatUnite;
        private RobotCafeUnite robotCafeUnite;

        KapInfo kapinfo = new KapInfo();


        public TestService(OtomatUnite otomatUnite, RobotCafeUnite robotCafeUnite)
        {
            this.otomatUnite = otomatUnite;
            this.robotCafeUnite = robotCafeUnite;
        }

        public async Task<int> DoBardakHazirlamaTest(int bardakNo = 0)
        {

            int ret = 0;

            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 100, 150, 15, 15, 35);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 93.1f, -45.6f, 9.9f, -48.7f, -91.8f, 0 };
            float[] pos3 = { 91.9f, 45.7f, -9.6f, -90f, -91.8f, 0 };
            float[] posara = { 46.3f, -54.2f, -17.3f, -19.9f, 90.6f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos3);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

            if (bardakNo == 0)
            {
                float[] pos4 = { 106.5f, 91.2f, -90.2f, -84.9f, -91.8f, 0 };
                float[] pos5 = { 106.5f, 52.6f, -62.5f, -79.7f, -92.5f, 0 };
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (bardakNo == 1)
            {
                float[] pos4 = { 93.2f, 75.5f, -75f, -90.8f, -91.8f, 0 };

                float[] pos5 = { 93f, 50.2f, -54.5f, -85.5f, -92.5f, 0 };//bardak alma
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (bardakNo == 2)
            {
                float[] pos4 = { 75.3f, 60.4f, -65f, -85.7f, -91.8f, 0 };
                float[] pos5 = { 75.6f, 53.5f, -60.9f, -82.3f, -91.8f, 0 };//bardak alma
                positionList.Clear();
                positionList.Add(pos4);
                positionList.Add(pos5);
            }

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);

            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            float[] pos60 = { 46.3f, -54.2f, -22.2f, -14f, 90.6f, 0 };
            float[] pos6 = { 46.7f, -6.7f, -54.2f, -28.9f, 90.6f, 0 };
            float[] pos7 = { 46.7f, -5.5f, -52.2f, -34.3f, 90.6f, 0 }; //bırakma noktası


            positionList.Clear();
            if (bardakNo == 0)
            {
                //float[] pos4 = { 102.8f, 97f, -106f, -77.8f, -91.4f, 0 };
                float[] pos4 = { 106.5f, 91.2f, -90.2f, -84.9f, -91.8f, 0 };


                positionList.Add(pos4);
            }
            else if (bardakNo == 1)
            {

                float[] pos4 = { 93.2f, 75.5f, -75f, -90.8f, -91.8f, 0 };

                float[] pos4x = { 93.2f, 57.5f, -16.8f, -87.7f, -91.8f, 0 };

                positionList.Add(pos4);
                positionList.Add(pos4x);

            }
            else if (bardakNo == 2)
            {

                float[] pos4 = { 74.4f, 68f, -71.9f, -87f, -91.8f, 0 };
                float[] pos4x = { 79.9f, 67.3f, -69.4f, -88.5f, -78.9f, 0 };
                float[] pos4x1 = { 83.4f, 73.9f, -70f, -85.2f, -74f, 0 };
                float[] pos4x2 = { 90.4f, 50.2f, -11.4f, -90.1f, -74f, 0 };

                positionList.Add(pos4);
                positionList.Add(pos4x);
                positionList.Add(pos4x1);
                positionList.Add(pos4x2);


            }

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);
            float[] posyeni = { 34.6f, -95.7f, 0, -0.3f, 88.5f, 0 };




            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(posyeni);
            positionList.Add(posara);
            positionList.Add(pos6);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos7); // bardak bırakma noktası


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);

            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos6);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);

            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(pos1);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);

            return ret;
        }


        public async Task<int> boyacitesti()
        {

            int ret = 0;

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);


            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 120, 120, 115);


            List<float[]> positionList = new List<float[]>();


            float[] pos1 = { -69.5f, -19.9f, 6.6f, -75.1f, -89.8f, 0 }; // urunalma
            float[] pos2 = { -69.7f, -41.4f, 7.4f, -53.1f, -89.8f, 0 }; //yaklasma 1
            float[] pos3 = { -70.2f, -58.4f, 4.1f, -33.4f, -89.8f, 0 }; //yaklasma 2
            float[] pos4 = { -70.2f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] posbos = { -4.6f, -74.2f, -73.5f, 54.9f, -89.8f, 0 }; //yaklasma 3



            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(pos1);



            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 134, 120, 120, 115);
            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            positionList.Add(posbos);
            positionList.Add(pos4);
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(pos1);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 120, 120, 115);
            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);


            return ret;
        }



        public async Task<int> DoBardakKapakHazirlamaTest(int tryCounter = 0)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 100, 150, 15, 15, 35);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] posara = { 55.5f, 54.7f, -43.1f, -95.9f, -91.8f, 0 };


            float[] pos3 = { 53.2f, 1.7f, 6f, -62.5f, -93.5f, 0 };

            float[] pos4 = { 63.4f, 61f, -68.1f, -83.4f, -91.8f, 0 };
            float[] kapakalma1 = { 63.4f, 52.6f, -63.2f, -81.4f, -91.8f, 0 }; //kapak alma pos 1
            float[] kapakalma2 = { 63.4f, 51.8f, -62.9f, -81f, -91.8f, 0 }; //kapak alma pos 2
            float[] posrevize = { 51.1f, 44.1f, -40.6f, -93.6f, 93f, 0 }; //kapak alma pos 2



            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos3);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos4);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

            positionList.Clear();
            if (tryCounter == 0)
            {
                positionList.Add(kapakalma1);
            }
            else if (tryCounter == 1)
            {
                positionList.Add(kapakalma2);
            }

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(posara);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 15);


            float[] pos31 = { 53.2f, 39f, -17.8f, -96f, 93f, 0 };

            float[] pos311 = { 58.4f, 62.7f, -85.5f, -66.9f, 88.7f, 0 };

            float[] pos32 = { 58.4f, 65f, -87.5f, -66.8f, 88.7f, 0 };



            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos31);
            positionList.Add(posrevize);
            positionList.Add(pos311);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45, wait: false);

            positionList.Clear();
            positionList.Add(pos32);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45);

            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);


            positionList.Clear();
            positionList.Add(pos311);
            positionList.Add(pos31);
            positionList.Add(pos3);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);

            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            return ret;
        }

        public async Task<int> DoKaseHazirlamaTest(int kaseNo = 0)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 99, 150, 15, 15, 35);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos3 = { 131.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos40 = { 134.3f, 67.8f, -63f, -96.5f, -91.8f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos40);

            if (kaseNo == 0)
            {
                float[] pos4 = { 147.6f, 78.2f, -89.4f, -81.6f, -91.8f, 0 };
                float[] pos5 = { 147.6f, 55.4f, -69.7f, -77.5f, -91.8f, 0 };

                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (kaseNo == 1)
            {
                float[] pos4 = { 134.1f, 81.1f, -98.8f, -72.2f, -91.8f, 0 };
                float[] pos5 = { 134.1f, 56.1f, -74.9f, -70.2f, -91.8f, 0 };
                positionList.Add(pos4);
                positionList.Add(pos5);
            }
            else if (kaseNo == 2)
            {
                float[] pos4 = { 121f, 81.6f, -100.2f, -72.8f, -91.8f, 0 };
                float[] pos5 = { 121f, 56.8f, -76.9f, -69f, -91.8f, 0 };
                positionList.Add(pos4);
                positionList.Add(pos5);
            }


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            float[] pos6 = { 0, -98.2f, -32.3f, 40.8f, 88.5f, 0 };
            float[] pos6x = { 46.5f, 6.1f, -120.5f, 25.3f, 88.5f, 0 };//yaklaşma
            float[] pos7 = { 46.5f, 2.7f, -112.9f, 19.8f, 88.5f, 0 };//kase bırakma


            positionList.Clear();

            if (kaseNo == 0)
            {
                float[] pos4 = { 147.6f, 78.2f, -89.4f, -81.6f, -91.8f, 0 };




                positionList.Add(pos4);

            }
            else if (kaseNo == 1)
            {
                float[] pos4 = { 134.1f, 81.1f, -98.8f, -72.2f, -91.8f, 0 };



                positionList.Add(pos4);

            }
            else if (kaseNo == 2)
            {
                float[] pos4 = { 121f, 81.6f, -100.2f, -72.8f, -91.8f, 0 };



                positionList.Add(pos4);

            }

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 20);

            positionList.Clear();
            positionList.Add(pos40);
            positionList.Add(pos3);
            positionList.Add(pos2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40, wait: false);

            positionList.Clear();
            positionList.Add(pos6);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);

            positionList.Clear();
            positionList.Add(pos6x);
            positionList.Add(pos7);


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos6x);
            positionList.Add(pos6);
            positionList.Add(pos1);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 25);

            return ret;
        }

        public async Task<int> DoKaseKapakHazirlamaTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 150, 15, 15, 35);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos3 = { 131.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos4 = { 134.3f, 67.8f, -63f, -96.5f, -91.8f, 0 };
            float[] pos5 = { 162.6f, 64.4f, -63.2f, -90.4f, -92.5f, 0 };// kase kapak yaklaşma

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);

            float[] pos6 = { 162.6f, 49f, -54.1f, -84.9f, -92.5f, 0 };//kapak alma noktası


            positionList.Clear();
            positionList.Add(pos6);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);

            float[] pos31 = { 131.7f, 1.2f, 1.1f, -91.6f, -91.8f, 0 };
            float[] pos32 = { 71f, 1.2f, 1.1f, -91.6f, 88.5f, 0 };
            float[] pos33 = { 71f, 16.7f, -2.2f, -78.9f, 88.5f, 0 };
            float[] pos33x = { 71f, 55.6f, -65.6f, -80.2f, 86.4f, 0 };

            float[] pos34 = { 71f, 61.1f, -69.8f, -80.4f, 86.4f, 0 };//kapak bırakma noktası

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos3);
            positionList.Add(pos31);
            positionList.Add(pos32);
            positionList.Add(pos33);
            positionList.Add(pos33x);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos34);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);


            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);


            positionList.Clear();
            positionList.Add(pos33x);
            positionList.Add(pos33);
            positionList.Add(pos32);
            positionList.Add(pos2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos1);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            return ret;
        }

        public async Task<int> DoBardakYerlestirmeTest()
        {
            int ret = 0;
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: 0, lineerPos: null);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 0);
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 15, 15, 115);

            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 42f, -17.5f, 1f, -75.6f, 89.1f, 0 };
            float[] pos3 = { 41.9f, -6.5f, -21.1f, -61.4f, 89.1f, 0 }; //bardak alma position
            float[] deneme = { -48.9f, -58f, -42.5f, 40.7f, -91.8f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 60, wait: false);

            positionList.Clear();
            positionList.Add(pos3);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 60);


            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 153, 15, 15, 115);


            await Task.Delay(1000);


            float[] pos40 = { 41.9f, -23.9f, -21.1f, -61.4f, 89.1f, 0 };
            float[] pos4 = { -67.9f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] posara = { -67.9f, -46.5f, 7.7f, -50.8f, -91.3f, 0 };

            float[] pos5 = { -67.9f, -24.8f, 7.8f, -70.4f, -91.3f, 0 };

            positionList.Clear();
            positionList.Add(pos40);
            positionList.Add(pos4);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 55, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 55);



            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 5, 5, 115);


            await Task.Delay(1000);

            float[] son = { -67.9f, -113.9f, -21.8f, 47.5f, -91.4f, 0 };


            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4);
            positionList.Add(son);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 55, wait: false);

            positionList.Clear();
            positionList.Add(deneme);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 55);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 120, 120, 118);
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            return ret;

        }

        public async Task<int> DoKaseYerlestirmeTest()
        {
            int ret = 0;

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 5, 5, 118);

            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 41.6f, -58, -42.6f, 7, 89.7f, 0 };
            float[] pos3 = { 41.6f, -31.1f, -56f, -0.8f, 89.7f, 0 }; //kase alma position
            float[] deneme = { -48.9f, -58f, -42.5f, 40.7f, -91.8f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos3);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 5, 105, 5, 5, 118);

            await Task.Delay(1000);

            float[] pos4 = { 41.6f, -26.6f, -71.4f, 9.1f, 89.7f, 0 };
            float[] pos5 = { 41.6f, -54.8f, -39.1f, -2.1f, 89.7f, 0 };
            float[] pos4x = { -68.4f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] posara = { -67.9f, -46.5f, 7.7f, -50.8f, -91.3f, 0 };
            float[] pos5x = { -67.9f, -35.7f, 7.8f, -62.3f, -91.3f, 0 };//kasebırakma

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45, wait: false);

            positionList.Clear();
            positionList.Add(pos5x);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 15, 15, 5, 5, 118);

            float[] pos8 = { -67.9f, -113.9f, -21.8f, 47.5f, -91.4f, 0 };
            positionList.Clear();
            positionList.Add(pos8);
            positionList.Add(deneme);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 120, 120, 118);
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            return ret;
        }



        public async Task<int> DoUrunAlmaTest()
        {
            int ret = 0;
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 130, 130, 118);

            List<float[]> positionList = new List<float[]>();

            float[] pose4 = { -48.9f, -40.9f, -60f, 53.8f, -91.8f, 0 };
            float[] pose5 = { -48.9f, -90.2f, -48f, 53.8f, -91.8f, 0 };
            float[] deneme = { -48.9f, -58f, -42.5f, 40.7f, -91.8f, 0 };
            float[] tekrar = { -48.9f, -29.6f, -53.9f, 34.3f, -91.4f, 0 };

            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 40);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: 33, lineerPos: null);

            positionList.Clear();

            positionList.Add(deneme);
            positionList.Add(tekrar);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 60);

            await Task.Delay(500);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 2, 2, 118);
            await Task.Delay(500);

            positionList.Clear();
            positionList.Add(pose4);
            positionList.Add(pose5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 60);

            float[] vakumtut = { -48.9f, -113.2f, -6.1f, -39.6f, -91.8f, 0 };

            ret = await this.robotCafeUnite.vakumUnite.RunIsiticiVakum(ret, run: true);

            positionList.Clear();
            positionList.Add(vakumtut);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90);
            ret = await this.robotCafeUnite.vakumUnite.RunIsiticiVakum(ret, run: true);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 30, 30, 118);
            await Task.Delay(500);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 1, 1, 118);
            await Task.Delay(500);

            ret = await this.robotCafeUnite.urunAlmaUnite.SetPosition(ret, donmePos: 0, lineerPos: null);

            return ret;
        }

        public async Task<int> DoKesmeTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            float[] urunalmabitis = { -53, -75.9f, -30f, 10.2f, -91.4f, 0 };
            float[] pos1 = { -53.2f, -103.6f, 6, 26, -91.4f, 0 };
            float[] pos100 = { -112.7f, -103.6f, 6, 26, -91.4f, 0 };
            float[] pos10 = { -112.6f, -27.1f, -26.4f, -0.6f, -91.4f, 0 };
            float[] pos10x = { -112.6f, -43.7f, -42.4f, 52.6f, -91.4f, 0 };//
            float[] pos4 = { -112.6f, -50.2f, -27.1f, 23f, -91.4f, 0 };
            float[] pos5xx = { -112.6f, -50f, -23f, 7.8f, -91.4f, 0 };
            float[] poskesme = { -112.6f, -42.6f, -19.7f, -11.6f, -91.4f, 0 };
            float[] poskesmeyeni = { -112.6f, -42.5f, -20.6f, -11.4f, -91.4f, 0 };
            float[] sikmadansonra = { -112.6f, -44.1f, -19f, -11.4f, -91.4f, 0 };
            float[] pos6 = { -112.7f, -42.7f, -4.5f, -24f, -90f, 0 };
            float[] pos7 = { -112.7f, -107f, 0f, 25.8f, -90f, 0 };
            float[] posbakalim = { -112.7f, -41.4f, -19.6f, -10.8f, -90f, 0 };



            positionList.Clear();
            positionList.Add(pos100);
            positionList.Add(pos10);
            positionList.Add(pos10x);
            positionList.Add(pos4);
            positionList.Add(pos5xx);


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);


            positionList.Clear();
            positionList.Add(posbakalim);
            positionList.Add(poskesme);
            positionList.Add(poskesmeyeni);


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: 0, lineerPos: null);

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPosition(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.robotCafeUnite.kesiciUnite.SetPositionTask(ret, lineerPos: 45, servoPos: null);


            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 30, 30, 118);

            positionList.Clear();
            positionList.Add(sikmadansonra);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);
            await Task.Delay(300);
            ret = await this.robotCafeUnite.kesiciUnite.SetPositionTask(ret, lineerPos: null, servoPos: 6);

            await Task.Delay(200);
            positionList.Clear();
            positionList.Add(pos6);
            positionList.Add(pos7);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 70, wait: false);

            return ret;
        }


        public async Task<int> DoIsitmaBardakTest()
        {
            int ret = 0;
            List<Task<int>> paralelTasks = new List<Task<int>>();
            List<float[]> positionList = new List<float[]>();

            // ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 5, 5, 115);
            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 35);

            float[] pos4x = { -67.9f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] posara = { -67.9f, -46.5f, 7.7f, -50.8f, -91.3f, 0 };
            float[] pos5 = { -67.9f, -24.8f, 7.8f, -70.4f, -91.3f, 0 };//bardak bırakma
            float[] pos4 = { -67.9f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] posisitmayaklasma1 = { -135f, -58.4f, 4.1f, -33.4f, -89.8f, 0 };
            float[] posisitmayaklasma2 = { -135f, 14.3f, -22.4f, -76.6f, -89.8f, 0 };
            float[] posisitmayaklasma3 = { -133.8f, 29.4f, -49.8f, -68.1f, -89.8f, 0 };
            float[] posisitma1 = { -133.9f, 25.4f, -49.8f, -68.1f, -89.8f, 0 };
            float[] posisitma2 = { -137.1f, 25.4f, -49.8f, -68.1f, -89.8f, 0 };

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45, wait: false);

            positionList.Clear();
            positionList.Add(pos5);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45);


            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 150, 5, 5, 115);
            await Task.Delay(500);

            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 60);


            //ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: true);
            //ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiBuhar(run: 1);

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            positionList.Add(posisitmayaklasma2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45, wait: false);

            positionList.Clear();
            positionList.Add(posisitmayaklasma3);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45);

            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 110);
            await Task.Delay(500);

            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: true);
            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiBuhar(run: 1);

            positionList.Clear();

            for (int i = 0; i < 5; i++)
            {
                positionList.Add(posisitma1);
                positionList.Add(posisitma2);
            }
            positionList.Add(posisitma1);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);

            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: false);
            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiBuhar(run: 0);

            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 35);

            positionList.Clear();
            positionList.Add(posisitmayaklasma3);
            positionList.Add(posisitmayaklasma2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45, wait: false);

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45);

            paralelTasks.Add(this.YikamaTask());
            paralelTasks.Add(this.DoluUrunYerlestirBardak());

            ret = await this.RunAsyncParalelTasks(paralelTasks);

            return ret;
        }

        public async Task<int> DoIsitmaKaseTest()
        {
            int ret = 0;
            List<Task<int>> paralelTasks = new List<Task<int>>();

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 5, 5, 115);
            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 35);

            List<float[]> positionList = new List<float[]>();

            float[] pos4x = { -68.4f, -62.1f, 3.1f, -34.8f, -90.3f, 0 };
            float[] posara = { -67.9f, -46.5f, 7.7f, -50.8f, -90.3f, 0 };
            float[] pos5 = { -67.9f, -35.8f, 8.2f, -62f, -90.3f, 0 };//kasebırakma
            float[] pos4 = { -67.9f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] posisitmayaklasma1 = { -135f, -58.4f, 4.1f, -33.4f, -89.8f, 0 };
            float[] posisitmayaklasma2 = { -135f, 14.3f, -22.4f, -76.6f, -89.8f, 0 };
            float[] posisitmayaklasma3 = { -133.8f, 29.4f, -49.8f, -68.1f, -89.8f, 0 };
            float[] posisitma1 = { -133.6f, 25f, -50f, -65.6f, -89.8f, 0 };
            float[] posisitma2 = { -137.4f, 25f, -50f, -65.6f, -89.8f, 0 };

            positionList.Clear();
            positionList.Add(pos4);
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 45);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 105, 5, 5, 115);
            await Task.Delay(500);

            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 35);

            

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            positionList.Add(posisitmayaklasma2);
            positionList.Add(posisitmayaklasma3);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 35);

            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 110);

            await Task.Delay(500);

            positionList.Clear();

            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: true);
            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiBuhar(run: 1);

            for (int i = 0; i < 6; i++)
            {
                positionList.Add(posisitma1);
                positionList.Add(posisitma2);
            }
            positionList.Add(posisitma1);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);

            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: false);
            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiBuhar(run: 0);

            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 35);

            positionList.Clear();
            positionList.Add(posisitmayaklasma3);
            positionList.Add(posisitmayaklasma2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 55, wait: false);

            positionList.Clear();
            positionList.Add(posisitmayaklasma1);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 55);

            paralelTasks.Add(this.YikamaTask());
            paralelTasks.Add(this.DoluUrunYerlestirKase());

            ret = await this.RunAsyncParalelTasks(paralelTasks);

            return ret;
        }


        private async Task<int> DoluUrunYerlestirBardak()
        {
            List<float[]> positionList = new List<float[]>();

            float[] posisitmayaklasma1 = { -135f, -58.4f, 4.1f, -33.4f, -89.8f, 0 };
            float[] posisitmayaklasma2 = { -135f, 14.3f, -22.4f, -76.6f, -89.8f, 0 };

            float[] pos4 = { -67.9f, -65.7f, -12.7f, -13.8f, -89.8f, 0 }; //yaklasma 3
            float[] posurunalma = { -14.7f, -105f, -21.3f, 49.8f, -89.8f, 0 }; //yaklasma 3

            float[] pos4x = { -67.9f, -62.1f, 3.1f, -34.8f, -91.3f, 0 };
            float[] posara = { -67.9f, -46.5f, 7.7f, -50.8f, -91.3f, 0 };

            float[] pos5 = { -67.9f, -24.8f, 7.8f, -70.4f, -91.3f, 0 };//bardak bırakma

            int ret = 0;
            positionList.Clear();
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30, wait: false);

            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 5, 5, 115);

            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            positionList.Add(pos4);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30, wait: false);

            positionList.Clear();
            positionList.Add(posurunalma);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);

            if (ret != 0)
            {
                return 1;
            }
            ret = await this.DoBardakKapakYerlestirmeTest();
            if (ret != 0)
            {
                return 1;
            }

            ret = await this.DoKapKapatmaTest(KapType.Bardak);
            if (ret != 0)
            {
                return 1;
            }

            return ret;
        }

        private async Task<int> DoluUrunYerlestirKase()
        {
            List<float[]> positionList = new List<float[]>();


            float[] posurunalma = { -14.7f, -105f, -21.3f, 49.8f, -90.3f, 0 }; //yaklasma 3

            float[] pos4x = { -68.4f, -62.1f, 3.1f, -34.8f, -90.3f, 0 };
            float[] posara = { -67.9f, -46.5f, 7.7f, -50.8f, -90.3f, 0 };
            float[] pos5x = { -67.9f, -35.8f, 8.2f, -62f, -90.3f, 0 };//kasebırakma

            int ret = 0;
            positionList.Clear();
            positionList.Add(pos4x);
            positionList.Add(posara);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(pos5x);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 12, 15, 5, 5, 115);

            positionList.Clear();
            positionList.Add(posara);
            positionList.Add(pos4x);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: false);

            positionList.Clear();
            positionList.Add(posurunalma);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

            if (ret != 0)
            {
                return 1;
            }
            ret = await this.DoKaseKapakYerlestirmeTest();
            if (ret != 0)
            {
                return 1;
            }
            ret = await this.DoKapKapatmaTest(KapType.Kase);
            if (ret != 0)
            {
                return 1;
            }
            return ret;
        }


        private async Task<int> YikamaTask()
        {

            int ret = 0;
            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 180);

            for (int i = 0; i < 2; i++)
            {
                ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: true);
                ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiYikamaBuharONOFF(run: true);
                await Task.Delay(5000);
                ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiONOFF(run: false);
                ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiYikamaBuharONOFF(run: false);

                ret = await this.robotCafeUnite.vakumUnite.RunIsitmaBuhar(ret, run: true);
                await Task.Delay(5000);
                ret = await this.robotCafeUnite.vakumUnite.RunIsitmaBuhar(ret, run: false);
            }

            ret = await this.robotCafeUnite.isiticiUnite.SetPosition(ret, yikamaPos: null, vakumPos: null, probPos: 35);




            return ret;
        }



        public async Task<int> DoBosaltmaBardakTest()
        {

            int ret = 0;
            ret = await this.robotCafeUnite.kesiciUnite.SetPosition(ret, lineerPos: 0, servoPos: null);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 130, 130, 118);


            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            List<float[]> positionList = new List<float[]>();


            float[] baslangic = { -60.1f, -104.8f, -8.2f, 25.8f, -90f, 0 };
            positionList.Clear();
            positionList.Add(baslangic);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 20, wait: true);
            if (ret != 0)
                return 1;






            float[] pos10 = { -60.7f, -73.9f, -29.7f, 72f, -90f, 0 };
            float[] pos20 = { -60.7f, -54f, -48.5f, 93.7f, -90f, 0 };
            float[] pos30 = { -60.7f, -39.8f, -63f, 106.4f, -90f, 0 };
            float[] pos40 = { -60.7f, -20.5f, -82.7f, 124.4f, -90f, 0 };
            float[] pos40x = { -60.7f, 22.2f, -129.5f, 159.5f, -90f, 0 };





            positionList.Clear();
            positionList.Add(pos10);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 10, wait: false);
            if (ret != 0)
                return 1;



            positionList.Clear();
            positionList.Add(pos20);
            positionList.Add(pos30);
            positionList.Add(pos40);
            positionList.Add(pos40x);


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 35, wait: false);
            if (ret != 0)
                return 1;



            float[] pos1 = { -60.9f, -70.3f, -42.4f, 80.8f, -90f, 0 };
            float[] pos2 = { -60.9f, -111.4f, -33.7f, 47.6f, -90f, 0 };


            positionList.Clear();


            positionList.Add(pos1);
            positionList.Add(pos2);

            return await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 30);

        }
        public async Task<int> DoBosaltmaKaseTest()
        {

            int ret = 0;
            ret = await this.robotCafeUnite.kesiciUnite.SetPosition(ret, lineerPos: 0, servoPos: null);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 70, 15, 130, 130, 118);

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            List<float[]> positionList = new List<float[]>();

            float[] baslangic = { -60.1f, -104.8f, -8.2f, 25.8f, -90f, 0 };
            positionList.Clear();
            positionList.Add(baslangic);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50, wait: true);
            if (ret != 0)
                return 1;

            float[] pos10x = { -63.8f, -107.8f, -1.8f, 21.5f, -90f, 0 };
            float[] pos10 = { -63.8f, -46.4f, -59.2f, 101.9f, -90f, 0 };
            float[] pos20 = { -63.8f, -28.8f, -77.1f, 117.5f, -90f, 0 };
            float[] pos30 = { -63.8f, -16.2f, -89.5f, 127.7f, -90f, 0 };
            float[] pos40 = { -63.8f, 18.7f, -128.5f, 159.7f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos10x);
            positionList.Add(pos10);
            positionList.Add(pos20);
            positionList.Add(pos30);
            positionList.Add(pos40);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 22, wait: false);
            if (ret != 0)
                return 1;

            float[] pos1 = { -62.7f, -70.3f, -42.4f, 80.8f, -90f, 0 };
            float[] pos2 = { -62.7f, -111.4f, -33.7f, 47.6f, -90f, 0 };

            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);

            return await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);

        }
        public async Task<int> DoCopAtmaTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            float[] pos1 = { -115.6f, -38.9f, -31.8f, 50.5f, -90f, 0 };
            float[] pos2 = { -62.7f, -111.4f, -33.7f, 47.6f, -90f, 0 };

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPosition(ret, yatayPos: 24, dikeyPos: null);

            positionList.Clear();
            positionList.Add(pos1);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90);

            ret = await this.robotCafeUnite.vakumUnite.RunIsiticiVakum(ret, run: false);

            positionList.Clear();
            positionList.Add(pos2);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90);
            ret = await this.robotCafeUnite.kesiciUnite.SetPosition(ret, lineerPos: null, servoPos: 102);
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPosition(ret, yatayPos: 24, dikeyPos: null);
            return ret;

        }

        public async Task<int> DoBardakKapakYerlestirmeTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 100, 150, 20, 10, 0);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            float[] pos2 = { 29.2f, 0.7f, 3.8f, -69.7f, 86.2f, 0 };



            float[] pos31 = { 53.2f, 39f, -17.8f, -96f, 88.5f, 0 };

            float[] pos311 = { 53.2f, 43.7f, -46.6f, -87.2f, 88.5f, 0 };

            float[] pos32 = { 58.5f, 61.3f, -84.4f, -67.8f, 88.5f, 0 };
            float[] pos33 = { 58.5f, 66.6f, -87.7f, -68.3f, 88.5f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            //positionList.Add(pos31);
            //positionList.Add(pos311);
            positionList.Add(pos32);
            //positionList.Add(pos33);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90, wait: false);


            positionList.Clear();
            positionList.Add(pos33);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 15, wait: true);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(500);


            float[] pos5 = { 0, -98.2f, -32.3f, 40.8f, 88.5f, 0 }; // home position

            float[] pos60 = { -60.2f, -37.5f, -32.3f, -12.5f, 88.8f, 0 };
            float[] pos6 = { -60.2f, -34.5f, -24.7f, -29.1f, 88.8f, 0 };  //bardak kapağı yerleştirme noktası

            positionList.Clear();
            positionList.Add(pos32);
            positionList.Add(pos2);
            positionList.Add(pos5);
            positionList.Add(pos60);
            //positionList.Add(pos6);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90, wait: false);


            positionList.Clear();
            positionList.Add(pos6);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90, wait: true);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(500);

            float[] finish = { -59.7f, -112, -21.3f, 48, -88.5f, 0 };  //bardak kapağı yerleştirme noktası

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: null);


            positionList.Clear();
            positionList.Add(pos60);
            positionList.Add(finish);


            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 90);

            return ret;

        }

        public async Task<int> DoKapKapatmaTest(KapType kapType = KapType.Bardak)
        {
            int ret = 0;


            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: null);

            if (kapType == KapType.Bardak)
            {
                ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 34);
            }
            else
            {
                ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 42);
            }

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: null, dikeyPos: 0);
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 24, dikeyPos: null);

            return 0;

        }

        public async Task<int> DoBardakSunumTest()
        {


            int ret = 0;
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);

            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 15, 138, 124, 115);



            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            //float[] pos2 = { -68.6f, -71f, 11f, -54.7f, -91.8f, 0 };//yaklaşma
            float[] pos2 = { -68.4f, -84.1f, -13.2f, -10.1f, -88.3f, 0 };//yaklaşma

            //float[] pos3 = { -68.6f, -47.9f, 11f, -54.7f, -91.8f, 0 };//bardak alma
            float[] pos3 = { -68.4f, -59.6f, 8.9f, -37.9f, -90.1f, 0 };//bardak alma


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);



            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 15, 138, 124, 10);
            await Task.Delay(300);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 100, 138, 124, 10);

            await Task.Delay(1000);

            float[] pos40 = { -68.4f, -73.4f, -3.5f, -10.1f, -88.3f, 0 };//çıkış 1
            positionList.Clear();
            positionList.Add(pos40);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);




            //float[] pos4 = { -68.8f, -73.3f, -2.9f, -14.9f, -91.8f, 0 };
            float[] pos5 = { -4.1f, -58.3f, 0.3f, -32.1f, -91.8f, 0 };
            float[] pos6 = { -4.1f, 10.3f, -34.3f, -67.2f, -91.8f, 0 };
            float[] pos7 = { -4.1f, 19.5f, -34f, -74.1f, -91.8f, 0 };
            float[] pos8 = { -4.1f, 24.3f, -35.7f, -80.5f, -91.8f, 0 };


            positionList.Clear();
            //positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos6);
            positionList.Add(pos7);
            //positionList.Add(pos8);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 100, 138, 124, 115);
            await Task.Delay(200);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 15, 138, 124, 115);




            float[] pos90 = { -4f, -69.7f, -29.9f, 10.6f, -91.8f, 0 }; // home position

            positionList.Clear();
            positionList.Add(pos90);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 40);





            ret = await this.robotCafeUnite.urunAlmaUnite.IsPositionOK(ret, donmePos: null, lineerPos: 0);

            return ret;

        }

        public async Task<int> DoKaseKapakYerlestirmeTest()
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 150, 20, 10, 0);


            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 22.5f, 29.2f, -11f, -96.5f, 88.5f, 0 };
            float[] pos3 = { 70.7f, 56.6f, -65.5f, -83.8f, 88.5f, 0 };
            float[] pos4 = { 70.7f, 63.4f, -71.3f, -83.8f, 88.5f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);
            positionList.Add(pos4);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 65);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);




            float[] pos5 = { 0, -98.2f, -32.3f, 40.8f, 91.4f, 0 }; // home position j5 farklı
            float[] pos5x = { -60.9f, -34f, -24.6f, -29f, 89.7f, 0 }; // home position j5 farklı

            float[] pos6 = { -60.8f, -31.9f, -23.2f, -32.2f, 89.7f, 0 };  //kase kapağı yerleştirme noktası

            positionList.Clear();
            positionList.Add(pos3);
            positionList.Add(pos2);
            positionList.Add(pos5);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 65, wait: false);


            positionList.Clear();
            positionList.Add(pos6);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 65);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);

            await Task.Delay(1000);

            float[] finish = { -60.6f, -98.2f, -32.3f, 40.8f, -93.5f, 0 };  //finish


            positionList.Clear();
            positionList.Add(pos5);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 65);

            positionList.Clear();
            positionList.Add(finish);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 65, wait: false);

            await Task.Delay(1300);

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPosition(ret, yatayPos: 0, dikeyPos: null);
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPosition(ret, yatayPos: null, dikeyPos: 17);





            return ret;

        }

        public async Task<int> DoServisSetiHazirlamaTest(int setNo = 0)
        {
            int ret = 0;
            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 95, 150, 20, 20, 120);

            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -93.5f, 0 }; // home position
            float[] pos2 = { 0, 1.2f, 1.1f, -91.6f, -93.5f, 0 };
            float[] pos3 = { 201.8f, 1.2f, 1.1f, -91.6f, -93.5f, 0 };


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);


            if (setNo == 0)
            {
                float[] pos4 = { 207.2f, 27f, -55.2f, -61.9f, -91.8f, 0 };//yaklaşma
                float[] pos4x = { 207.2f, 23.5f, -56.9f, -56.3f, -91.8f, 0 };//alma

                positionList.Add(pos4);
                positionList.Add(pos4x);

            }
            else if (setNo == 1)
            {
                float[] pos4 = { 201.6f, 22.2f, -48.9f, -62.5f, -91.8f, 0 };//yaklaşma
                float[] pos4x = { 201.6f, 20.3f, -49.8f, -60.4f, -91.8f, 0 };//alma

                positionList.Add(pos4);
                positionList.Add(pos4x);
            }
            else if (setNo == 2)
            {
                float[] pos4 = { 191.9f, 19.8f, -42.3f, -67.2f, -91.8f, 0 };//yaklaşma
                float[] pos4x = { 191.9f, 16.4f, -42.9f, -64.5f, -91.8f, 0 };//alma

                positionList.Add(pos4);
                positionList.Add(pos4x);
            }



            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 10);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: true);

            await Task.Delay(1000);

            if (setNo == 0)
            {
                float[] pos5 = { 211.9f, 14.2f, -41.5f, -63.4f, -91.8f, 0 };
                float[] pos6 = { 224f, -7.9f, -12f, -63.4f, -91.8f, 0 };

                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);

                ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 10);

            }
            if (setNo == 1)
            {
                float[] pos5 = { 205.6f, 10.1f, -34f, -66.1f, -91.8f, 0 };
                float[] pos6 = { 213.5f, -8.6f, -10.4f, -66.1f, -91.8f, 0 };

                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);

                ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);

            }
            if (setNo == 2)
            {
                float[] pos5 = { 193.5f, 5f, -26.9f, -68.3f, -91.8f, 0 };
                float[] pos6 = { 197.6f, -22.8f, 4.7f, -67.5f, -91.8f, 0 };

                positionList.Clear();
                positionList.Add(pos5);
                positionList.Add(pos6);

                ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);

            }

            //float[] pos6x = { 224.6f, -73.5f, -32.3f, 104.8f, -91.8f, 0 };
            //float[] pos7x = { -6.7f, -73.5f, -32.3f, 104.8f, -91.8f, 0 };
            //float[] pos8x = { -6.7f, 11.3f, -58.8f, -34.7f, -182.2f, 0 };

            float[] posa = { 223.8f, -58.7f, -29.4f, 84.9f, -91.8f, 0 };
            float[] posb = { 0.8f, -58.7f, -29.4f, 84.9f, -91.8f, 0 };
            float[] posc = { 0.8f, -0.9f, -82.5f, 81.9f, -91.8f, 0 };


            positionList.Clear();
            positionList.Add(posa);
            positionList.Add(posb);
            positionList.Add(posc);
            positionList.Add(pos1);

            await Task.Delay(300);
            ret = await this.robotCafeUnite.vakumUnite.RunRobotTutucuVakum(ret, run: false);



            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 50);


            return ret;
        }

        public async Task<int> DoKaseSunumTest()
        {


            int ret = 0;
            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 32, dikeyPos: null);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);

            List<float[]> positionList = new List<float[]>();

            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 15, 138, 124, 115);



            float[] pos1 = { 0, -98.2f, -32.3f, 40.8f, -91.8f, 0 }; // home position
            //float[] pos2 = { -68.6f, -71f, 11f, -54.7f, -91.8f, 0 };//yaklaşma
            float[] pos2 = { -68.8f, -108.8f, -2.4f, 50f, -91.8f, 0 };//yaklaşma

            //float[] pos3 = { -68.6f, -47.9f, 11f, -54.7f, -91.8f, 0 };//bardak alma
            float[] pos3 = { -68.8f, -65.8f, 8.1f, -30.2f, -91.8f, 0 };//bardak alma


            positionList.Clear();
            positionList.Add(pos1);
            positionList.Add(pos2);
            positionList.Add(pos3);



            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 50, 65, 138, 124, 118);


            await Task.Delay(1000);

            float[] pos40 = { -68.4f, -73.4f, -3.5f, -10.1f, -88.3f, 0 };//çıkış 1
            positionList.Clear();
            positionList.Add(pos40);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);




            //float[] pos4 = { -68.8f, -73.3f, -2.9f, -14.9f, -91.8f, 0 };
            float[] pos5 = { -4.1f, -58.3f, 0.3f, -32.1f, -91.8f, 0 };
            float[] pos5a = { -4.1f, 8.3f, -33.5f, -64.5f, -91.8f, 0 };
            float[] pos6 = { -4.1f, 16.3f, -33.8f, -68.9f, -91.8f, 0 };
            //kıskaç aç

            //float[] pos7 = { -4.1f, 19.5f, -34f, -74.1f, -91.8f, 0 };
            //float[] pos8 = { -4.1f, 24.3f, -35.7f, -80.5f, -91.8f, 0 };


            positionList.Clear();
            //positionList.Add(pos4);
            positionList.Add(pos5);
            positionList.Add(pos5a);
            positionList.Add(pos6);
            //positionList.Add(pos7);
            //positionList.Add(pos8);

            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);
            ret = await this.robotCafeUnite.cafeRobotTutucuKiskacUnite.WriteAllKiskac(ret, 13, 3, 138, 124, 118);




            float[] pos90 = { -4f, -69.7f, -29.9f, 10.6f, -91.8f, 0 }; // home position

            positionList.Clear();
            positionList.Add(pos90);
            ret = await this.robotCafeUnite.robotArm.SetPosition(ret, positionList, speed: 5);





            ret = await this.robotCafeUnite.urunAlmaUnite.IsPositionOK(ret, donmePos: null, lineerPos: 0);

            return ret;

        }

        public async Task<int> OtomatServisYap(Product product)
        {

            //RafBolme raf = product.RafBolmes.Where(o => o.StockAdet > 0).FirstOrDefault();
            //if (raf == null)
            //{
            //    return 1;
            //}
            int ret = 0;
            RafBolme raf = product.RafBolmes.Where(o => o.StockAdet > 0).FirstOrDefault();

            int MotorRunTimeDuration = 2600;
            short AtmaNoktasıDikeyPos = 235;
            short AtmaNoktasıYatayPos = 250;
            short RafDikeyPos = (short)raf.YPos;
            short RafYatayPos = (short)raf.XPos;
            short MotorSlaveAddress = raf.MotorSlaveAddress;
            ushort MotorRegNo = (ushort)raf.MotorRegNo;





            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 45, 30, 8, 0, 50, isTogether: false);



            ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: RafYatayPos, dikeyPos: RafDikeyPos);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 45, 30, 8, 30, 50, isTogether: true);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 120, 120, 8, 30, 120, isTogether: true);

            bool isUrunReady = this.otomatUnite.otomatUrunAlmaUnite.UrunAlindimi();

            int tryCounter = 1;
            while (isUrunReady == false)
            {
                await this.otomatUnite.otomatMotorUniteList.Where(o => o.slaveAddress == MotorSlaveAddress).First().RunMotor(Register_Address: MotorRegNo, RunTimeDuration: (MotorRunTimeDuration / tryCounter));

                await Task.Delay(500);
                isUrunReady = this.otomatUnite.otomatUrunAlmaUnite.UrunAlindimi();
                tryCounter++;

                if (tryCounter >= 12)
                {
                    return 1;
                }
            }

            await Task.Delay(500);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 50, 40, 8, 30, 120, isTogether: true);

            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 50, 40, 8, 30, 50, isTogether: true);



            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 50, 40, 8, 0, 50, isTogether: true);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 50, 40, 8, 0, 50, isTogether: true);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 50, 40, 8, 220, 50, isTogether: true);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 120, 120, 8, 220, 50, isTogether: true);
            await Task.Delay(400);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 45, 30, 8, 220, 50, isTogether: true);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 45, 30, 8, 80, 50, isTogether: true);




            ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: null, dikeyPos: AtmaNoktasıDikeyPos);
            if (ret != 0)
                return 1;

            ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: AtmaNoktasıYatayPos, dikeyPos: null);
            if (ret != 0)
                return 1;
            //
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 120, 120, 8, 80, 50, isTogether: true);
            if (ret != 0)
                return 1;

            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 120, 120, 82, 80, 50, isTogether: true);

          

            return ret;
        }

        public async Task<int> OtomatGetReadyToService()
        {
            int ret = 0;

            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 120, 120, 8, 80, 50, isTogether: true);
            //
            ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: 180, dikeyPos: null);
            ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: null, dikeyPos: 20);
            if (ret != 0)
                return 1;

            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 4, 45, 30, 8, 0, 50, isTogether: true);
            ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 45, 30, 8, 0, 50, isTogether: true);
            return ret;
        }

        public async Task<int> CafeGetReadyToService()
        {
            int ret = 0;
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPosition(ret, donmePos: null, lineerPos: 0);
            if (ret != 0)
                return 1;
            ret = await robotCafeUnite.KapVeKapakHazirlamaTask();
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 0);
            //
            return ret;
        }

        public async Task<int> DoService(Product product)
        {

            int ret = 0;
            List<Task<int>> paralelTasks = new List<Task<int>>();

            //product = ProductServices.GetById(4);

            bool isUrunCafede = false;
            for (int i = 0; i < 1; i++)
            {
                isUrunCafede = robotCafeUnite.urunAlmaUnite.UrunAlindimi();
                if (isUrunCafede == false)
                {

                    //paralelTasks.Add(this.OtomatServisYap(product));
                    if (product.KapType == KapType.Kase)
                    {
                        if (i == 0)
                            paralelTasks.Add(this.DoKaseYerlestirmeTest());
                    }
                    else if (product.KapType == KapType.Bardak)
                    {
                        if (i == 0)
                            paralelTasks.Add(this.DoBardakYerlestirmeTest());
                    }

                    ret = await this.RunAsyncParalelTasks(paralelTasks);
                    if (ret != 0)
                    {
                        return 1;
                    }
                    paralelTasks.Clear();

                }
                else
                {
                    break;
                }


            }

            int tryCounter = 0;
            while (true)
            {
                tryCounter++;
                isUrunCafede = robotCafeUnite.urunAlmaUnite.UrunAlindimi();
                if (isUrunCafede == false)
                {
                    await Task.Delay(20);
                    if (tryCounter > 200)
                        return 1;
                }
                else
                {

                    ret = await this.DoUrunAlmaTest();
                    if (ret != 0)
                    {
                        return 1;
                    }


                    ret = await this.DoKesmeTest();
                    if (ret != 0)
                    {
                        return 1;
                    }

                    if (product.KapType == KapType.Bardak)
                    {
                        ret = await this.DoBosaltmaBardakTest();
                    }
                    else if (product.KapType == KapType.Kase)
                    {
                        ret = await this.DoBosaltmaKaseTest();
                    }

                    if (ret != 0)
                    {
                        return 1;
                    }



                    ret = await this.DoCopAtmaTest();
                    if (ret != 0)
                    {
                        return 1;
                    }


                    if (product.KapType == KapType.Bardak)
                    {
                        ret = await this.DoIsitmaBardakTest();
                    }
                    else if (product.KapType == KapType.Kase)
                    {
                        ret = await this.DoIsitmaKaseTest();
                    }

                    if (ret != 0)
                    {
                        return 1;
                    }

                    //ret = await this.GetReadyToService();
                    //if (ret != 0)
                    //{
                    //    return 1;
                    //}
                    break;


                }
            }




            return 0;
        }

        public async Task<int> GetReadyToService()
        {

            int ret = 0;

            List<Task<int>> paralelTasks = new List<Task<int>>();


            paralelTasks.Add(this.CafeGetReadyToService());
            //paralelTasks.Add(this.OtomatGetReadyToService());

            ret = await this.RunAsyncParalelTasks(paralelTasks);
            return ret;
        }

        public async Task<int> DoHoming()
        {

            int ret = 0;

            //ret = await this.otomatUnite.otomatAsansorUnite.SetPosition(ret: ret, yatayPos: 0, dikeyPos: null);

            //await Task.Delay(8000);

            //ret = await this.otomatUnite.otomatAsansorUnite.SetPosition(ret: ret, yatayPos: null, dikeyPos: 0);


            //ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, null, null, null, 8, 0, 50, isTogether: false);
            //ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 0, 45, 30, 8, 0, 50, isTogether: true);
            //ret = await this.otomatUnite.otomatUrunAlmaUnite.SetPositionTask(ret, 150, 45, 30, 8, 0, 50, isTogether: true);

            //ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: null, dikeyPos: 200);
            //ret = await this.otomatUnite.otomatAsansorUnite.SetPositionTask(ret: ret, yatayPos: 150, dikeyPos: null);

            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: null, lineerPos: 40);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: 0, lineerPos: 40);
            ret = await this.robotCafeUnite.urunAlmaUnite.SetPositionTask(ret, donmePos: 0, lineerPos: 0);

            ret = await this.robotCafeUnite.kapakKapatmaUnite.SetPositionTask(ret, yatayPos: 0, dikeyPos: 0);

            ret = await this.robotCafeUnite.kesiciUnite.SetPositionTask(ret, lineerPos: 0, servoPos: 102);
            ret = await this.robotCafeUnite.isiticiUnite.SetPositionTask(ret, yikamaPos: null, vakumPos: null, probPos: 35);
            ret = await this.robotCafeUnite.isiticiUnite.RunIsiticiProbeBuharONOFF(run: true);





            return ret;
        }

        private async Task<int> RunAsyncParalelTasks(List<Task<int>> paralelTasks)
        {
            int ret = -1;
            int taskCounter = paralelTasks.Count;
            while (taskCounter > 0)
            {
                Task<int> finishedTask = await Task.WhenAny(paralelTasks);
                taskCounter--;
                //paralelTasks.Remove(finishedTask);
            }
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

            ret = 0;
            return ret;

        }

    }




}
