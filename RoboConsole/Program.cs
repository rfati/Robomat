
using Common;
using Manager;
using RobotCafe.Service;
using System;

namespace RobomatConsole
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int sayac = 0;
            try
            {
                OtomatManager.GetInstance();
                OtomatManager.GetInstance().Init();
                OtomatManager.GetInstance().Start();
                OtomatManager.GetInstance().SetMode(Mode.SaleService);
            }
            catch (Exception ex)
            {
                Logger.LogError("exception:  " + ex.Message);
            }
            while (true)
            {
                sayac = sayac + 1;
                Console.WriteLine("Test Sayısı: "+ sayac);
                Console.WriteLine("Homing : 0");
                Console.WriteLine("Test : 1");
                Console.WriteLine("Get Ready To Service: 2");
                string input = Console.ReadLine();
                if (input.Equals("1"))
                {
                    OtomatManager.GetInstance().DoTestServisIslem(4);
                }
                if (input.Equals("0"))
                {

                }
            }

        }
    }
}
