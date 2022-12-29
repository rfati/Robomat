using System;

namespace RobotCafe.Serial
{
    public enum PackStatus
    {
        Empty = 0,
        Source_Address_Wait = 1,
        Dest_Address_wait = 2,
        Packet_Type_Wait = 3,
        CRC_Wait = 4,
        Packet_Ready = 5
    }
    public class SerialPacket
    {
        public PackStatus PackStatus = PackStatus.Empty;
        public byte Start_Byte;
        public byte Source_Address;
        public byte Dest_Address;
        public byte Packet_Type;
        public byte CRC;

        public void Parse(byte readByte)
        {
            switch(PackStatus)
            {
                case PackStatus.Empty:
                    this.Start_Byte = readByte;
                    this.CRC = readByte;
                    this.PackStatus = PackStatus.Source_Address_Wait;
                    break;
                case PackStatus.Source_Address_Wait:
                    this.Source_Address = readByte;
                    this.CRC ^= readByte;
                    this.PackStatus = PackStatus.Dest_Address_wait;
                    break;
                case PackStatus.Dest_Address_wait:
                    this.Dest_Address = readByte;
                    this.CRC ^= readByte;
                    this.PackStatus = PackStatus.Packet_Type_Wait;
                    break;
                case PackStatus.Packet_Type_Wait:
                    Packet_Type = readByte;
                    CRC ^= readByte;
                    PackStatus = PackStatus.CRC_Wait;
                    break;
                case PackStatus.CRC_Wait:
                    if(CRC == readByte)
                    {
                        this.PackStatus = PackStatus.Packet_Ready;
                    }
                    else
                    {
                        this.PackStatus = PackStatus.Empty;
                    }
                    break;
                default:
                    this.PackStatus = PackStatus.Empty;
                    break;
            }
        }
    }
}