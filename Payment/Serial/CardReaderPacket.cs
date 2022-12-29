using System;
using System.Collections.Generic;

namespace Payment.Serial
{
    public enum CardReaderPacketStatus
    {
        Empty = 0,
        LEN0_Wait = 1,
        LEN1_wait = 2,
        Data_Wait = 3,
        LRC_Wait = 4,
        ETX_Wait = 5,
        Packet_Ready = 6,
        Packet_Fail = 7,
        Packet_ACK_Waiting = 8



    }
    public class CardReaderPacket
    {

        public CardReaderPacketStatus PackStatus = CardReaderPacketStatus.Empty;
        public byte FirstByte;
        public byte STX;
        public byte Len0;
        public byte Len1;
        public List<byte> RawData = new List<byte>();
        public byte LRC;
        public byte ETX;

        public void Parse(byte readByte)
        {
            switch(PackStatus)
            {
                case CardReaderPacketStatus.Empty:
                    this.FirstByte = readByte;
                    if (readByte == 0x02)
                    {
                        this.STX = readByte;
                        this.PackStatus = CardReaderPacketStatus.LEN0_Wait;
                    }
                    else if((readByte == 0x06) || (readByte == 0x15))
                    {
                        this.PackStatus = CardReaderPacketStatus.Packet_ACK_Waiting;
                    }                    
                    else if((readByte == 0x04))
                    {
                        this.PackStatus = CardReaderPacketStatus.Packet_Ready;
                    }
                    break;
                case CardReaderPacketStatus.Packet_ACK_Waiting:
                    if(readByte == 0x00)
                    {
                        this.PackStatus = CardReaderPacketStatus.Packet_Ready;
                    }
                    else
                    {
                        this.FirstByte = readByte;
                        if (readByte == 0x02)
                        {
                            this.STX = readByte;
                            this.PackStatus = CardReaderPacketStatus.LEN0_Wait;
                        }
                    }
                    break;
                case CardReaderPacketStatus.LEN0_Wait:
                    this.Len0 = readByte;
                    this.PackStatus = CardReaderPacketStatus.LEN1_wait;
                    break;
                case CardReaderPacketStatus.LEN1_wait:
                    this.Len1 = readByte;
                    this.PackStatus = CardReaderPacketStatus.Data_Wait;
                    break;
                case CardReaderPacketStatus.Data_Wait:

                    this.LRC ^= readByte;
                    int len = 256 * this.Len0 + this.Len1;
                    this.RawData.Add(readByte);
                    if (this.RawData.Count == (len - 2))
                    {
                        this.PackStatus = CardReaderPacketStatus.LRC_Wait;
                    }
                    else
                    {
                        this.PackStatus = CardReaderPacketStatus.Data_Wait;
                    }

                    break;
                case CardReaderPacketStatus.LRC_Wait:
                    if(this.LRC == readByte)
                    {
                        this.PackStatus = CardReaderPacketStatus.ETX_Wait;
                    }
                    else
                    {
                        this.PackStatus = CardReaderPacketStatus.Packet_Fail;
                    }
                    break;
                case CardReaderPacketStatus.ETX_Wait:
                    this.PackStatus = CardReaderPacketStatus.Packet_Ready;
                    break;
                default:
                    this.PackStatus = CardReaderPacketStatus.Empty;
                    break;
            }
        }

        public void Reset()
        {
            this.LRC = 0;
            this.PackStatus = CardReaderPacketStatus.Empty;
            this.RawData.Clear();
        }
    }
}