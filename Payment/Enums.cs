using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment
{
    public enum CommandType
    {
        OdemeAlmaIstek,
        KaydetIstek,
        GunSonu,
        Restart,
        None
    }
    public enum OdemeControllerNotifyEvent
    {
        ResponseTimeOut,
        IslemBaslatildi,
        IslemOnaylandi,
        IslemOnaylanmadi,
        Islem_Kaydedildi,
        Islem_Kaydedilmedi,
        GunSonu_Tamamlandi,
        GunSonu_Tamamlanmadi
    }
    public enum OdemeControllerState
    {
        Idle,
        Satis_Istek_Gonderildi,
        Kart_Okunuyor,
        Kart_Okundu,
        Kart_Cikartildi,
        Pin_Giriniz,
        Pin_Girildi,
        Pin_Girilmedi,
        Online_Baglanti_Kuruluyor,
        Online_Cevap_Bekleniyor,
        Online_Cevap_Alindi,
        Islem_Tamamlandi,
        Islem_Kaydedildi,
        Islem_Kaydedilmedi,
        Kaydet_Istek_Gonderildi,
        Gun_Sonu_Gonderildi,
        GunSonu_Tamamlandi,
        GunSonu_Tamamlanmadi,
    }

    public class UygulamaDurumBilgiKodu
    {
        public const int SistemBosta = 0;
        public const int Kart_Okundu = 1;
        public const int Kart_Cikartildi = 2;
        public const int Pin_Giriniz = 3;
        public const int Pin_Girildi = 4;
        public const int Pin_Girilmedi = 5;
        public const int Online_Baglanti_Kuruluyor = 6;
        public const int Online_Cevap_Bekleniyor = 7;
        public const int Online_Cevap_Alindi = 8;
        public const int Islem_Tamamlandi = 9;
    }


}
