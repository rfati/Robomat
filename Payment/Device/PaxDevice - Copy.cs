using Newtonsoft.Json.Linq;
using Payment.Commands;
using Payment.Responses;
using Payment.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

//namespace Payment.Device
//{

//    public enum PaxState
//    {
//        Idle,
//        Satis_Istek_Sent,
//        Satis_Istek_Isleme_Alindi,
//        //Kart_Okundu,
//        Satis_Islem_Basarili,
//        Satis_Islem_Basarisiz,
//        Satis_Kaydet_Sent,
//        Satis_Kaydet_Isleme_Alindi,
//        Satis_Kaydet_Basarili,
//        Satis_Kaydet_Basarisiz,
//        Satis_Kaydet_Tamamlandi,

//        TimeOut,
//        GunSonu_Baslatildi,
//        GunSonu_Basarili,
//        GunSonu_Basarisiz,
//        GunSonu_Tamamlandi,


//        Iptal_Istek_Sent,
//        Iptal_Istek_Isleme_Alindi,
//        Iptal_Islem_Basarili,
//        Iptal_Islem_Basarisiz,
//        //Kart_Cikarildi,
//        //PinGiriniz,
//        //PinGirildi,
//        //PinGirilmedi,
//        //BaglantiKuruluyor,
//        //CevapBekleniyor,
//        //CevapAlindi,
//        //IslemTamamlandi,
//        DurumBilgilendirmeAlindi,
//    }

//    public class PaxDevice
//    {

//        private int batchNo;
//        private int stan;
//        private string refNo;

//        private SerialManager serialManager;
//        private PaxState state { get; set; }

//        public PaxDevice(SerialManager serialManager)
//        {
//            this.serialManager = serialManager;
//        }



//        public void Update(CardReaderPacket packet)
//        {
//            this.HandleResponse(packet);
//        }

//        private void HandleResponse(CardReaderPacket packet)
//        {
//            if (packet.FirstByte == 0x06)
//                this.HandleACK();
//            else if (packet.FirstByte == 0x15)
//                this.HandleNACK();
//            else if (packet.FirstByte == 0x04)
//                this.HandleEOT();
//            else if (packet.FirstByte == 0x02)
//                this.HandleResponseMessage(packet.RawData.ToArray());

//        }



//        private void HandleResponseMessage(byte[] rawdata)
//        {
//            string jsonStr = Encoding.UTF8.GetString(rawdata);
//            this.ParseSerialResponse(jsonStr);
//        }


//        private void HandleEOT()
//        {
//            if (this.state == PaxState.Satis_Kaydet_Basarili || this.state == PaxState.Satis_Kaydet_Basarisiz)
//            {
//                this.state = PaxState.Satis_Kaydet_Tamamlandi;
//            }
//            else if (this.state == PaxState.GunSonu_Basarili || this.state == PaxState.GunSonu_Basarisiz)
//            {
//                this.state = PaxState.GunSonu_Tamamlandi;
//            }
//        }

//        private void HandleNACK()
//        {
//        }

//        private void HandleACK()
//        {
//            if (this.state == PaxState.Satis_Istek_Sent)
//            {
//                this.state = PaxState.Satis_Istek_Isleme_Alindi;
//            }
//            else if (this.state == PaxState.Satis_Kaydet_Sent)
//            {
//                this.state = PaxState.Satis_Kaydet_Isleme_Alindi;
//            }
//        }

//        private void ParseSerialResponse(string jsonStr)
//        {
//            CardReaderCevap response = Newtonsoft.Json.JsonConvert.DeserializeObject<CardReaderCevap>(jsonStr);
//            if (response.MessageType.Equals("FF8E7F"))
//            {
//                this.HandleDurumBilgilendirmeResponse(jsonStr);
//            }
//            else if (response.MessageType.Equals("FF8E72"))
//            {
//                this.HandleOdemeAlmaResponse(jsonStr);
//            }
//            else if (response.MessageType.Equals("FF8E7B"))
//            {
//                this.HandleKaydetResponse(jsonStr);
//            }
//            else if (response.MessageType.Equals("FF8E75"))
//            {
//                this.HandleGunSonuResponse(jsonStr);
//            }
//            else if (response.MessageType.Equals("FF8E21"))
//            {
//                this.HandleBaglantiMesajResponse(jsonStr);
//            }
//            else if (response.MessageType.Equals("FF8E23"))
//            {
//                this.HandleRestartResponse(jsonStr);
//            }
//            else if (response.MessageType.Equals("FF8E7D"))
//            {
//                this.HandleBankaListesiResponse(jsonStr);
//            }
//        }


//        private void HandleDurumBilgilendirmeResponse(string jsonStr)
//        {

//            DurumBilgilendirmeCevap response = Newtonsoft.Json.JsonConvert.DeserializeObject<DurumBilgilendirmeCevap>(jsonStr);
//            this.state = PaxState.DurumBilgilendirmeAlindi;

//            //if (response.AppstateInfo == 1)
//            //{
//            //    this.state = PaxState.DurumBilgilendirmeAlindi;
//            //}
//            //else if(response.AppstateInfo == 2)
//            //{
//            //    this.state = PaxState.Kart_Cikarildi;
//            //}
//            //else if (response.AppstateInfo == 3)
//            //{
//            //    this.state = PaxState.PinGiriniz;
//            //}
//            //else if (response.AppstateInfo == 4)
//            //{
//            //    this.state = PaxState.PinGirildi;
//            //}
//            //else if (response.AppstateInfo == 5)
//            //{
//            //    this.state = PaxState.PinGirilmedi;
//            //}
//            //else if (response.AppstateInfo == 6)
//            //{
//            //    this.state = PaxState.BaglantiKuruluyor;
//            //}
//            //else if (response.AppstateInfo == 7)
//            //{
//            //    this.state = PaxState.CevapBekleniyor;
//            //}
//            //else if (response.AppstateInfo == 8)
//            //{
//            //    this.state = PaxState.CevapAlindi;
//            //}
//            //else if (response.AppstateInfo == 9)
//            //{
//            //    this.state = PaxState.IslemTamamlandi;
//            //}
//        }

//        private void HandleOdemeAlmaResponse(string jsonStr)
//        {
//            OdemeAlmaCevap response = Newtonsoft.Json.JsonConvert.DeserializeObject<OdemeAlmaCevap>(jsonStr);
//            if (response.AuthRespCode.Equals("00") || response.AuthRespCode.Equals("08") || response.AuthRespCode.Equals("11"))
//            {
//                this.batchNo = response.BatchNo;
//                this.stan = response.Stan;
//                this.refNo = response.RefNo;
//                this.state = PaxState.Satis_Islem_Basarili;
//            }
//            else
//            {
//                this.state = PaxState.Satis_Islem_Basarisiz;
//            }

//        }

//        private void HandleKaydetResponse(string jsonStr)
//        {
//            KaydetCevap response = Newtonsoft.Json.JsonConvert.DeserializeObject<KaydetCevap>(jsonStr);
//            if (response.InternalErrCode.Equals("0") || response.InternalErrCode.Equals("00"))
//            {
//                this.state = PaxState.Satis_Kaydet_Basarili;
//            }
//            else
//            {
//                this.state = PaxState.Satis_Kaydet_Basarisiz;
//            }

//        }

//        private void HandleGunSonuResponse(string jsonStr)
//        {
//            GunSonuCevap response = Newtonsoft.Json.JsonConvert.DeserializeObject<GunSonuCevap>(jsonStr);
//            if (response.InternalErrCode.Equals("00"))
//            {
//                this.state = PaxState.GunSonu_Basarili;
//            }
//            else
//            {
//                this.state = PaxState.GunSonu_Basarisiz;
//            }
//            Thread.Sleep(20);
//            this.SendACKCommand();
//        }

//        private void HandleBaglantiMesajResponse(string jsonStr)
//        {
//            throw new NotImplementedException();
//        }

//        private void HandleRestartResponse(string jsonStr)
//        {
//            throw new NotImplementedException();
//        }

//        private void HandleBankaListesiResponse(string jsonStr)
//        {
//            throw new NotImplementedException();
//        }




//        private void SendCommand(CardReaderCommand command)
//        {
//            this.serialManager.SendCommand(command);
//        }
//        private void SendACKCommand()
//        {
//            this.serialManager.SendACK();
//        }
//        private void SendEOTCommand()
//        {
//            this.serialManager.SendEOT();
//        }

//        public async Task<int> SendSatisIstekCommand(int price)
//        {
//            int ret = -1;
//            int ack_resp = -1;
//            int command_resp = -1;


//            SatisIstekCommand command = new SatisIstekCommand(price);
//            this.state = PaxState.Satis_Istek_Sent;
//            this.SendCommand(command);

//            await Task.Delay(500);
//            ack_resp = this.serialManager.GetResponse(length: 2);
//            if (ack_resp != 0)
//            {
//                return -1;
//            }

//            if (this.state != PaxState.Satis_Istek_Isleme_Alindi)
//            {
//                return -1;
//            }

//            command_resp = this.serialManager.GetResponse();
//            if (command_resp != 0)
//            {
//                return -1;
//            }

//            while (true)
//            {
//                if (this.state == PaxState.DurumBilgilendirmeAlindi)
//                {
//                    this.SendACKCommand();
//                    command_resp = this.serialManager.GetResponse();
//                    if (command_resp != 0)
//                    {
//                        return -1;
//                    }
//                }
//                else if ()
//            }




//            while (this.state == PaxState.Kart_Okundu)
//            {
//                await Task.Delay(10);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 1;
//                ret.returnDesc = "Odeme Response alinamadi.. Time out oluştu.";
//                return ret;
//            }
//            else if (this.state == PaxState.Satis_Islem_Basarisiz)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 2;
//                ret.returnDesc = "Satış islem Başarısız";
//                return ret;
//            }
//            else if (this.state == PaxState.Satis_Islem_Basarili)
//            {
//                this.SendACKCommand();
//                await Task.Delay(2000);
//                ret = await this.SendSatisKaydetIstekCommand();
//            }

//            return ret;

//        }


//        public async Task<PaymentIslemResult> SendSatisKaydetIstekCommand()
//        {
//            PaymentIslemResult ret = new PaymentIslemResult();
//            SatisKaydetIstekCommand command = new SatisKaydetIstekCommand(this.batchNo, this.stan, 1);
//            this.state = PaxState.Satis_Kaydet_Sent;
//            this.StartResponseTimeoutTimer(ms: 100000);
//            this.SendCommand(command);

//            while (this.state == PaxState.Satis_Kaydet_Sent)
//            {
//                await Task.Delay(50);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 1;
//                ret.returnDesc = "Satış kaydet gönderildi.. Time out oluştu.";
//                return ret;
//            }
//            else if (this.state == PaxState.Satis_Kaydet_Isleme_Alindi)
//            {
//                this.StartResponseTimeoutTimer(ms: 2000);
//                while (this.state == PaxState.Satis_Kaydet_Isleme_Alindi)
//                {
//                    await Task.Delay(10);
//                }
//                if (this.state == PaxState.TimeOut)
//                {
//                    this.SendEOTCommand();
//                    ret.returnCode = 1;
//                    ret.returnDesc = "Satış kaydet istek gönderildi uzak cihaz ACK göndermedi.";
//                    return ret;
//                }
//                else if (this.state == PaxState.Satis_Kaydet_Basarisiz)
//                {
//                    this.SendEOTCommand();
//                    ret.returnCode = 3;
//                    ret.returnDesc = "Satış Kaydet İşlem Başarısız..";
//                    return ret;
//                }
//                else if (this.state == PaxState.Satis_Kaydet_Basarili)
//                {
//                    this.SendACKCommand();
//                    this.StartResponseTimeoutTimer(ms: 1000);
//                    ret.returnCode = 0;
//                    ret.returnDesc = "Satış Kaydet İşlem Başarılı..";
//                }
//                while (this.state == PaxState.Satis_Kaydet_Basarili || this.state == PaxState.Satis_Kaydet_Basarisiz)
//                {
//                    await Task.Delay(50);
//                }
//                if (this.state == PaxState.TimeOut)
//                {
//                    this.SendEOTCommand();
//                    return ret;
//                }
//                else if (this.state == PaxState.Satis_Kaydet_Tamamlandi)
//                {
//                    return ret;
//                }
//            }

//            else if (this.state == PaxState.Satis_Kaydet_Basarisiz)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 3;
//                ret.returnDesc = "Satış Kaydet İşlem Başarısız..";
//                return ret;
//            }
//            else if (this.state == PaxState.Satis_Kaydet_Basarili)
//            {
//                this.StartResponseTimeoutTimer(ms: 5000);
//                this.SendACKCommand();
//                ret.returnCode = 0;
//                ret.returnDesc = "Satış Kaydet İşlem Başarılı..";
//            }
//            while (this.state == PaxState.Satis_Kaydet_Basarili || this.state == PaxState.Satis_Kaydet_Basarisiz)
//            {
//                await Task.Delay(150);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                return ret;
//            }
//            else if (this.state == PaxState.Satis_Kaydet_Tamamlandi)
//            {
//                return ret;
//            }

//            return ret;
//        }


//        public async Task<PaymentIslemResult> SendGunSonuCommand()
//        {
//            PaymentIslemResult ret = new PaymentIslemResult();
//            if (this.state == PaxState.Idle)
//            {
//                GunSonuCommand command = new GunSonuCommand();
//                this.state = PaxState.GunSonu_Baslatildi;
//                this.SendCommand(command);
//            }
//            while (this.state == PaxState.GunSonu_Baslatildi)
//            {
//                await Task.Delay(10);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 1;
//                ret.returnDesc = "Gun Sonu islem Time out oluştu.";
//                return ret;
//            }
//            else if (this.state == PaxState.GunSonu_Basarili)
//            {
//                this.StartResponseTimeoutTimer(ms: 60000);
//                ret.returnCode = 0;
//                ret.returnDesc = "GunSonu İşlem Başarılı Tamamlandı";
//            }
//            else if (this.state == PaxState.GunSonu_Basarisiz)
//            {
//                this.StartResponseTimeoutTimer(ms: 60000);
//                ret.returnCode = 3;
//                ret.returnDesc = "Gunsonu İşlem Başarısız Tamamlandı";
//            }
//            while (this.state == PaxState.GunSonu_Basarili || this.state == PaxState.GunSonu_Basarisiz)
//            {
//                await Task.Delay(10);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//            }
//            else if (this.state == PaxState.GunSonu_Tamamlandi)
//            {
//                return ret;
//            }
//            this.state = PaxState.Idle;
//            return ret;

//        }



//        public async Task<PaymentIslemResult> SendIptalIstekCommand(int price)
//        {
//            PaymentIslemResult ret = new PaymentIslemResult();
//            IptalIstekCommand command = new IptalIstekCommand(1, 73, 16, "0003215018886107");
//            this.state = PaxState.Iptal_Istek_Sent;
//            this.StartResponseTimeoutTimer(ms: 100000);
//            this.SendCommand(command);

//            while (this.state == PaxState.Iptal_Istek_Sent)
//            {
//                await Task.Delay(10);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 1;
//                ret.returnDesc = "Satış istek gönderildi uzak cihaz ACK göndermedi.";
//                return ret;
//            }
//            else if (this.state == PaxState.Iptal_Istek_Isleme_Alindi)
//            {
//                this.StartResponseTimeoutTimer(ms: 10000);
//            }
//            while (this.state == PaxState.Iptal_Istek_Isleme_Alindi)
//            {
//                await Task.Delay(10);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 1;
//                ret.returnDesc = "Kart okunmadi.. Time out oluştu.";
//                return ret;
//            }
//            else if (this.state == PaxState.Kart_Okundu)
//            {
//                this.StartResponseTimeoutTimer(ms: 40000);
//                this.SendACKCommand();

//            }
//            while (this.state == PaxState.Kart_Okundu)
//            {
//                await Task.Delay(10);
//            }
//            if (this.state == PaxState.TimeOut)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 1;
//                ret.returnDesc = "Odeme Response alinamadi.. Time out oluştu.";
//                return ret;
//            }
//            else if (this.state == PaxState.Iptal_Islem_Basarisiz)
//            {
//                this.SendEOTCommand();
//                ret.returnCode = 2;
//                ret.returnDesc = "Satış islem Başarısız";
//                return ret;
//            }
//            else if (this.state == PaxState.Iptal_Islem_Basarili)
//            {
//                this.SendACKCommand();
//                await Task.Delay(2000);
//                ret = await this.SendSatisKaydetIstekCommand();
//            }

//            return ret;

//        }

//    }
//}
