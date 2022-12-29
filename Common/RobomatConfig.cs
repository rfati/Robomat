using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RobomatConfig
    {
        public string cafeUniteSerialManager_port { get; set; }
        public string cafeKapUniteSerialManager_port { get; set; }
        public string cafeRobotTutucuKiskacUniteSerialManager_port { get; set; }
        public string cafeOtomatUniteSerialManager_port { get; set; }
        public string IM20_port { get; set; }
        public string XArm_Ip { get; set; }
        public string ModeSelector_port { get; set; }


        public short Kesici_Lineer_Init_Pos { get; set; }
        public short Kesici_Lineer_Kesme_Pos { get; set; }
        public short Kesici_Servo_Init_Pos { get; set; }
        public short Kesici_Servo_Kesme_Pos { get; set; }

        public short KapakKapatma_Yatay_InitKapatma_Pos { get; set; }
        public short KapakKapatma_Yatay_Verme_Pos { get; set; }
        public short KapakKapatma_Dikey_Init_Pos { get; set; }
        public short KapakKapatma_Dikey_Bardak_Pos { get; set; }
        public short KapakKapatma_Dikey_Kase_Pos { get; set; }

        public short Isitici_YikamaStep_Init_Pos { get; set; }
        public short Isitici_YikamaStep_Yikama_Pos { get; set; }
        public short Isitici_VakumServo_Acik_Pos { get; set; }
        public short Isitici_VakumServo_Kapali_Pos { get; set; }
        public short Isitici_ProbServo_Yukari_Pos { get; set; }
        public short Isitici_ProbServo_Asagi_Pos { get; set; }


        public short UrunAlma_Servo_Init_Pos { get; set; }
        public short UrunAlma_Servo_Verme_Pos { get; set; }
        public short UrunAlma_Lineer_Init_Pos { get; set; }
        public short UrunAlma_Lineer_Verme_Pos { get; set; }
   

    }

}
