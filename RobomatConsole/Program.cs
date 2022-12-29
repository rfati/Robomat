using BLL;
using Common;
using DAL;
using Manager;
using Model;
using RESTApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobomatConsole
{
    class Program
    {
        static void Main(string[] args)
        {


            using (RobomatUnitOfWork worker = new RobomatUnitOfWork())
            {

                var datas = worker.CategoryRepository.FindById(1);

            }



            RESTApiServer rESTApiServer = RESTApiServer.GetInstance();
            rESTApiServer.Start();

            OtomatManager.GetInstance();
            OtomatManager.GetInstance().Init();
            OtomatManager.GetInstance().Start();

            Thread.Sleep(1000);
            //OtomatManager.GetInstance().SetMode(Mode.SaleService);


            //Thread.Sleep(6000);
            //OtomatManager.GetInstance().DoTest(5);

            //int motorRunDuration;
            int userInput = 0;
            do
            {
                userInput = DisplayMenu();
                OtomatManager.GetInstance().DoTest(userInput);

            } while (userInput != 999);



            //OtomatManager otomatManager = new OtomatManager();
            //otomatManager.Init();
            //otomatManager.Start();

            //Thread.Sleep(2000);
            //otomatManager.DoHoming();
            //otomatManager.DoServiceTest();
            //otomatManager.DoTest();

            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }

        static public int DisplayMenu()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine();
                Console.WriteLine("1. DoBardakHazirlamaTest");
                Console.WriteLine("2. DoBardakKapakHazirlamaTest");
                Console.WriteLine("3. DoKaseHazirlamaTest");
                Console.WriteLine("4. DoKaseKapakHazirlamaTest");
                Console.WriteLine("5. DoBardakYerlestirmeTest");
                Console.WriteLine("6. DoKaseYerlestirmeTest");
                Console.WriteLine("7. DoUrunAlmaTest");
                Console.WriteLine("8. DoKesmeTest");
                Console.WriteLine("9. DoIsitmaTest");
                Console.WriteLine("10. DoBosaltmaBardakTest");
                Console.WriteLine("11. DoCopAtmaTest");
                Console.WriteLine("12. DoBardakKapakYerlestirmeTest");
                Console.WriteLine("13. DoKapKapatmaTest");
                Console.WriteLine("14. DoBardakSunumTest");
                Console.WriteLine("15. DoKaseKapakYerlestirmeTest");
                Console.WriteLine("16. DoKaseSunumTest"); 
                Console.WriteLine("17. DoServisSetiHazirlamaTest");
                Console.WriteLine("18. DoBosaltmaKaseTest");
                Console.WriteLine("19. ServisYap");

                Console.WriteLine("999. çıkış");
                int result;
                if (Int32.TryParse(Console.ReadLine(), out result))
                    return result;
                else
                    Console.WriteLine("Fonksiyon seçiniz. ");
            }
        }
    }
}
