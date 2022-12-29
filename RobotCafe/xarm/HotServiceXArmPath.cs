using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCafe.xarm
{
    //public class HotServiceXArmPath : IXArmPath
    //{
    //    public List<float[]> Home { get; set; }
    //    public List<float[]> Hazır_Bardak_Alma_Path { get; set; }
    //    public List<float[]> Hazır_Kase_Alma_Path { get; set; }

    //    public List<float[]> Hazır_Bardak_Yerlestir_Path { get; set; }
    //    public List<float[]> Hazır_Kase_Yerlestir_Path { get; set; }

    //    public List<float[]> Hazır_Bardak_Yerlestir_Finish_Path { get; set; }
    //    public List<float[]> Hazır_Kase_Yerlestir_Finish_Path { get; set; }

    //    public List<float[]> Urun_Almaya_Git_Path { get; set; }
    //    public List<float[]> Urun_Al_Path { get; set; }
    //    public List<float[]> Urun_Isit_Path { get; set; }
    //    public List<float[]> Urun_Bosalt_Path { get; set; }
    //    public List<float[]> Cop_At_Path { get; set; }

    //    public List<float[]> Hazır_BardakKapak_Alma_Path { get; set; }
    //    public List<float[]> Hazır_KaseKapak_Alma_Path { get; set; }

    //    public List<float[]> Hazır_BardakKapak_Kapat_Path { get; set; }
    //    public List<float[]> Hazır_KaseKapak_Kapat_Path { get; set; }

    //    public List<float[]> BardakKapat_Uzaklas_Path { get; set; }
    //    public List<float[]> KaseKapat_Uzaklas_Path { get; set; }

    //    public List<float[]> DoluBardak_KaldirmayaGit_Path { get; set; }
    //    public List<float[]> DoluKase_KaldirmayaGit_Path { get; set; }

    //    public List<float[]> DoluBardak_Servis_Path { get; set; }
    //    public List<float[]> DoluKase_Servis_Path { get; set; }

    //    public List<float[]> From_Bardak_To_Kasiklik_Path { get; set; }
    //    public List<float[]> From_Kase_To_Kasiklik_Path { get; set; }

    //    public List<float[]> Kasik_ServisEt_Path { get; set; }

    //    public List<float[]> Finish { get; set; }

    //}


    public class Pos
    {
        public int Speed { get; set; }
        public float[] Path { get; set; }
    }

    public class HotServiceXArmPath : IXArmPath
    {
        public List<Pos> Home { get; set; }
        public List<Pos> Hazır_Bardak_Alma_Path { get; set; }
        public List<Pos> Hazır_Kase_Alma_Path { get; set; }

        public List<Pos> Hazır_Bardak_Yerlestir_Path { get; set; }
        public List<Pos> Hazır_Kase_Yerlestir_Path { get; set; }

        public List<Pos> Hazır_Bardak_Yerlestir_Finish_Path { get; set; }
        public List<Pos> Hazır_Kase_Yerlestir_Finish_Path { get; set; }

        public List<Pos> Urun_Almaya_Git_Path { get; set; }
        public List<Pos> Urun_Al_Path { get; set; }
        public List<Pos> Urun_Isit_Path { get; set; }
        public List<Pos> Urun_Bosalt_Path { get; set; }
        public List<Pos> Cop_At_Path { get; set; }

        public List<Pos> Hazır_BardakKapak_Alma_Path { get; set; }
        public List<Pos> Hazır_KaseKapak_Alma_Path { get; set; }

        public List<Pos> Hazır_BardakKapak_Kapat_Path { get; set; }
        public List<Pos> Hazır_KaseKapak_Kapat_Path { get; set; }

        public List<Pos> BardakKapat_Uzaklas_Path { get; set; }
        public List<Pos> KaseKapat_Uzaklas_Path { get; set; }

        public List<Pos> DoluBardak_KaldirmayaGit_Path { get; set; }
        public List<Pos> DoluKase_KaldirmayaGit_Path { get; set; }

        public List<Pos> DoluBardak_Servis_Path { get; set; }
        public List<Pos> DoluKase_Servis_Path { get; set; }

        public List<Pos> From_Bardak_To_Kasiklik_Path { get; set; }
        public List<Pos> From_Kase_To_Kasiklik_Path { get; set; }

        public List<Pos> Kasik_ServisEt_Path { get; set; }
        public List<Pos> Kasik_Al_Path { get; set; }
        public List<Pos> Kasik_Birak_Path { get; set; }
        public List<Pos> Finish { get; set; }

    }
}
