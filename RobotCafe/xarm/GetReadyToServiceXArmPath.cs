using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.xarm
{
    public class HazirlamaPath
    {
        public int No { get; set; }
        public List<float[]> Path { get; set; }
    }
    public class GetReadyToServiceXArmPath : IXArmPath
    {
        public List<HazirlamaPath> Bardak_Al_Path { get; set; }
        public List<HazirlamaPath> Kase_Al_Path { get; set; }
        public List<HazirlamaPath> Bardak_Yerlestir_Path { get; set; }
        public List<HazirlamaPath> Kase_Yerlestir_Path { get; set; }
        public List<HazirlamaPath> BardakKapak_Al_Path { get; set; }
        public List<HazirlamaPath> KaseKapak_Al_Path { get; set; }
        public List<HazirlamaPath> BardakKapak_Yerlestir_Path { get; set; }
        public List<HazirlamaPath> KaseKapak_Yerlestir_Path { get; set; }

        public List<float[]> FromBardakKapak_To_Home_Path { get; set; }
        public List<float[]> FromKaseKapak_To_Home_Path { get; set; }
        public List<float[]> FromHazirBardak_To_Home_Path { get; set; }
        public List<float[]> FromHazirKase_To_Home_Path { get; set; }


    }
}
