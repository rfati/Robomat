using System;
using System.Collections.Generic;

namespace RobotCafe.Serial
{
    public enum RTUPackStatus
    {
        Empty,
        Function_Code_Wait,
        Count_Wait,
        Data_Wait,
        CRC0_Wait,
        CRC1_Wait,
        Packet_Ready
    }
    public class RTUResponsePacket
    {
        public RTUPackStatus PackStatus = RTUPackStatus.Empty;
        public byte Slaveaddress;
        public byte FunctionCode;
        public byte Count;
        public List<byte> Data = new List<byte>();
        public List<byte> Frame = new List<byte>();
        public byte[] CRC = new byte[2];
        public byte counter;

        public void Parse(byte readByte)
        {
            switch(PackStatus)
            {
                case RTUPackStatus.Empty:
                    this.Slaveaddress = readByte;
                    this.Frame.Add(readByte);
                    this.PackStatus = RTUPackStatus.Function_Code_Wait;
                    break;
                case RTUPackStatus.Function_Code_Wait:
                    this.FunctionCode = readByte;
                    this.Frame.Add(readByte);
                    if (this.FunctionCode == 0x03)
                    {
                        this.PackStatus = RTUPackStatus.Count_Wait;
                    }
                    else
                    {
                        this.PackStatus = RTUPackStatus.Data_Wait;
                    }
                    break;
                case RTUPackStatus.Count_Wait:
                    this.Count = readByte;
                    this.Frame.Add(readByte);
                    this.PackStatus = RTUPackStatus.Data_Wait;
                    break;
                case RTUPackStatus.Data_Wait:
                    this.Frame.Add(readByte);
                    this.Data.Add(readByte);
                    this.counter++;
                    if (this.FunctionCode == 0x03)
                    {
                        if(this.counter >= this.Count)
                        {
                            this.PackStatus = RTUPackStatus.CRC0_Wait;
                        }

                    }
                    else
                    {
                        if (this.counter >= 4)
                        {
                            this.PackStatus = RTUPackStatus.CRC0_Wait;
                        }
                    }

                    break;
                case RTUPackStatus.CRC0_Wait:
                    this.CRC[0] = readByte;
                    this.Frame.Add(readByte);
                    PackStatus = RTUPackStatus.CRC1_Wait;
                    break;

                case RTUPackStatus.CRC1_Wait:
                    this.CRC[1] = readByte;
                    this.Frame.Add(readByte);
                    byte[] CRC = this.CalculateCRC(this.Frame.ToArray());
                    this.Frame.Clear();
                    this.counter = 0;
                    if (CRC[0] == this.CRC[0] && CRC[1] == this.CRC[1])
                    {
                        this.PackStatus = RTUPackStatus.Packet_Ready;
                    }
                    else
                    {
                        this.PackStatus = RTUPackStatus.Packet_Ready;
                        //this.PackStatus = RTUPackStatus.Empty;
                    }
                    break;
                default:
                    this.PackStatus = RTUPackStatus.Empty;
                    break;
            }
        }

        private byte[] CalculateCRC(byte[] data)
        {
            ushort CRCFull = 0xFFFF;
            char CRCLSB;
            byte[] CRC = new byte[2];
            for (int i = 0; i < (data.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]); // 

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
            return CRC;
        }

        public void Reset()
        {
            this.counter = 0;
            this.Data.Clear();
            this.Frame.Clear();
            this.PackStatus = RTUPackStatus.Empty;
        }
    }
}