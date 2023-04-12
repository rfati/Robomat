using Common;
using RobotCafe.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace RobotCafe.Devices
{

    public enum State
    {
        Idle,
        CommandSent,
        CommandACKReceived,
        StatusSent,
        StatusResponseReceived,
        TimeOut
    }
    public class FunctionCode
    {
        public const byte FC3 = 0x03;
        public const byte FC6 = 0x06;
        public const byte FC16 = 0x10;
    }

    public class RTUDevice
    {
        private bool isAttachedToCOM = false;

        protected int MAX_WRITE_TRY_COUNTER = 10;


        public State state;
        public byte slaveAddress { get; set; }
        public List<RegisterRead> RegisterReadList;

        public SerialManager serialManager;

        System.Timers.Timer ResponseTimer = new System.Timers.Timer();
        int ResponseTimeOutInterval = 3000;



        public RTUDevice()
        {
            ResponseTimer.Interval = ResponseTimeOutInterval;
            ResponseTimer.Elapsed += ResponseTimer_Elapsed;
            ResponseTimer.AutoReset = false;
            ResponseTimer.Enabled = false;

            this.state = State.Idle;
            this.RegisterReadList = new List<RegisterRead>();

        }

        private void ResponseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.LogInfo("Timer_Elapsed.....");
            this.state = State.TimeOut;
        }




        public bool Attach(SerialManager serialManager)
        {
            try
            {
                if (this.isAttachedToCOM == false)
                {
                    this.serialManager = serialManager;
                    this.serialManager.Attach(this);

                    //this.serialManager.PacketReceived += Update;
                }

                this.isAttachedToCOM = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Detach()
        {
            try
            {
                if (this.isAttachedToCOM == true)
                {
                    this.serialManager.Detach(this);
                    //this.serialManager.PacketReceived -= Update;
                }
                this.isAttachedToCOM = false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        protected int SetMotorPosition(List<Motor> motorList, bool isTogether = false)
        {
            MotorCommandResult ret = new MotorCommandResult();
            List<RegisterWrite> RegisterWriteList = new List<RegisterWrite>();
            foreach (var motor in motorList)
            {
                RegisterWriteList.Add(motor.TargetPosRegisterWrite);
            }

            for (int i = 0; i < MAX_WRITE_TRY_COUNTER; i++)
            {
                if (isTogether == true)
                {
                    ret.retWriteRegisterResult = this.WriteMultipleRegisterTogether(RegisterWriteList);
                }
                else
                {
                    ret.retWriteRegisterResult = this.WriteMultipleRegister(RegisterWriteList);
                }

                if (ret.retWriteRegisterResult == 0)
                {
                    break;
                }
                else
                {
                    Logger.LogError("SetMotorPosition ret.retWriteRegisterResult != 0 ret.retWriteRegisterResult: " + ret.retWriteRegisterResult + "  slave: " + this.slaveAddress);
                }
            }

            if (ret.retWriteRegisterResult == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }



        protected int IsMotorPositionOK(List<Motor> motorList)
        {
            MotorCommandResult ret = new MotorCommandResult();

            for (int i = 0; i < 400; i++)
            {
                ret.retReadRegisterResult = this.ReadAllRegisters();
                if (ret.retReadRegisterResult == 0)
                {
                    foreach (var motor in motorList)
                    {
                        short motor_pos_status_value = this.RegisterReadList.First(o => o.Register_Address == motor.PosStatusRegisterRead.Register_Address).Register_Read_Value;
                        short motor_current_pos_value = this.RegisterReadList.First(o => o.Register_Address == motor.CurrentPosRegisterRead.Register_Address).Register_Read_Value;

                        motor.PosStatusRegisterRead.Register_Read_Value = motor_pos_status_value;
                        motor.CurrentPosRegisterRead.Register_Read_Value = motor_current_pos_value;

                    }
                    if (motorList.Any(o => o.PosStatusRegisterRead.Register_Read_Value != 0))
                    {
                        ret.retPosStatusResult = 1;
                        Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        ret.retPosStatusResult = 0;
                    }

                    if (motorList.Any(o => o.CurrentPosRegisterRead.Register_Read_Value != o.TargetPosRegisterWrite.Register_Target_Value))
                    {
                        ret.retCurrentPosResult = 1;
                        Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        ret.retCurrentPosResult = 0;
                        break;
                    }

                }
                else
                {
                    Logger.LogError("Read response gelmedi.. slaveAddress: " + this.slaveAddress);
                    continue;
                }


            }

            if (ret.retReadRegisterResult == 0 && ret.retCurrentPosResult == 0 && ret.retPosStatusResult == 0)
                return 0;
            else
                return 1;


        }

        protected RelayCommandResult WriteMultipleRelay(List<Relay> RelayList)
        {
            RelayCommandResult ret = new RelayCommandResult();


            List<Relay> relays = new List<Relay>();
            relays.AddRange(RelayList);
            List<RegisterWrite> RegisterWriteList = new List<RegisterWrite>();
            foreach (var relay in relays)
            {
                RegisterWriteList.Add(relay.TargetPosRegisterWrite);
            }


            int counter = 0;

            for (counter = 0; counter <= MAX_WRITE_TRY_COUNTER; counter++)
            {
                ret.retWriteRegisterResult = this.WriteMultipleRegister(RegisterWriteList);

                if (ret.retWriteRegisterResult == 0)
                {
                    break;
                }
            }

            return ret;

        }



        protected SensorCommandResult ReadMultipleSensor(List<Sensor> sensorList)
        {
            SensorCommandResult ret = new SensorCommandResult();
            List<Sensor> sensors = new List<Sensor>();
            sensors.AddRange(sensorList);

            for (int counter = 0; counter <= 2; counter++)
            {
                ret.retReadRegisterResult = this.ReadAllRegisters();
                if (ret.retReadRegisterResult == 0)
                {
                    foreach (var sensor in sensors)
                    {
                        short sensor_current_value = this.RegisterReadList.First(o => o.Register_Address == sensor.CurrentValRegisterRead.Register_Address).Register_Read_Value;

                        sensor.CurrentValRegisterRead.Register_Read_Value = sensor_current_value;

                    }

                    break;


                }
            }


            return ret;

        }





        protected int WriteMultipleRegister(List<RegisterWrite> registerWrites)
        {
            int ret = -1;

            foreach (var item in registerWrites)
            {
                short[] values = { item.Register_Target_Value };
                bool write = this.SendMultipleWriteRegister(item.Register_Address, 1, values);
                if (write != true)
                {
                    Logger.LogError("WriteMultipleRegister bool write= false");
                    return -1;
                }



                ResponseTimer.Enabled = true;

                while (true)
                {
                    if (this.state == State.CommandACKReceived)
                    {
                        ret = 0; //success
                        break;
                    }
                    else if (this.state == State.TimeOut)
                    {
                        Logger.LogError("WriteMultipleRegister this.state == State.CommandSent TimeOut oluştu SlaveAdd: " + this.slaveAddress);
                        ret = 1;
                        break;
                    }

                    Thread.Sleep(10);
                }




            }

            return ret;
        }


        protected int WriteMultipleRegisterTogether(List<RegisterWrite> registerWrites)
        {
            int ret = -1;


            ushort Start_reg_address = registerWrites.First().Register_Address;
            ushort reg_count = (ushort)registerWrites.Count;
            List<short> valueList = new List<short>();
            foreach (var item in registerWrites)
            {
                valueList.Add(item.Register_Target_Value);
            }
            bool write = this.SendMultipleWriteRegister(Start_reg_address, reg_count, valueList.ToArray());
            if (write != true)
            {
                Logger.LogError("WriteMultipleRegisterTogether bool write= false");
                return -1;
            }



            ResponseTimer.Enabled = true;

            while (true)
            {
                if (this.state == State.CommandACKReceived)
                {
                    ret = 0; //success
                    break;
                }
                else if (this.state == State.TimeOut)
                {
                    Logger.LogError("WriteMultipleRegisterTogether this.state == State.CommandSent TimeOut oluştu SlaveAdd: " + this.slaveAddress);
                    ret = 1;
                    break;
                }

                Thread.Sleep(10);
            }


            return ret;
        }


        protected int ReadAllRegisters()
        {
            int ret = 1;

            bool read = this.SendMultipleReadRegister(RegisterReadList.First().Register_Address, (ushort)(RegisterReadList.Last().Register_Address - RegisterReadList.First().Register_Address + 1), (RegisterReadList.Count * 2 + 5));
            if (read != true)
            {
                return -1;
            }




            ResponseTimer.Enabled = true;

            while (true)
            {
                if (this.state == State.StatusResponseReceived)
                {
                    ret = 0; //success
                    break;
                }
                else if (this.state == State.TimeOut)
                {
                    Logger.LogError("ReadAllRegisters this.state == State.CommandSent TimeOut oluştu SlaveAdd: " + this.slaveAddress);
                    ret = 1;
                    break;
                }

                Thread.Sleep(10);
            }


            return ret;
        }


        public void Update(RTUResponsePacket packet)
        {
            ResponseTimer.Enabled = false;
            if (packet == null)
            {
                Logger.LogError("RTU Device state= Timeout geldi................ slaveAdd: " + slaveAddress);
                this.state = State.TimeOut;
                return;
            }
            this.HandleResponse(packet);
        }

        public void Update(object sender, PacketReceivedEventArgs args)
        {

            if (args.SerialPacket.Slaveaddress == this.slaveAddress)
            {
                this.HandleResponse(args.SerialPacket);
            }

        }

        private void HandleResponse(RTUResponsePacket packet)
        {
            if (packet.FunctionCode == FunctionCode.FC16 || packet.FunctionCode == FunctionCode.FC6)
            {
                this.HandleCommandACK();
            }
            else if (packet.FunctionCode == FunctionCode.FC3)
            {
                this.HandleStatusResponse(packet);
            }
        }

        private void HandleCommandACK()
        {
            this.state = State.CommandACKReceived;
        }



        private void HandleStatusResponse(RTUResponsePacket packet)
        {

            int devCount = packet.Data.Count / 2;
            for (int i = 0; i < devCount; i++)
            {
                byte x1 = packet.Data[i * 2];
                byte x2 = packet.Data[i * 2 + 1];
                this.RegisterReadList[i].Register_Read_Value = (short)(x1 * 256 + x2);
            }
            this.state = State.StatusResponseReceived;

        }



        private bool SendMultipleWriteRegister(ushort Start_Reg_Address, ushort registerCount, short[] values)
        {

            byte[] data = BuildWriteMultipleRegisterMsg(this.slaveAddress, FunctionCode.FC16, Start_Reg_Address, registerCount, values);

            this.state = State.CommandSent;
            TXRXData tXRXData = new TXRXData();
            tXRXData.data = data;
            tXRXData.receiveSize = 8;
            tXRXData.SlaveAddress = this.slaveAddress;

            //if(this.serialManager.portName.Equals("COM2") && this.slaveAddress == 2)
            //{
            //    for(int i=0;i<tXRXData.data.Length; i++)
            //    {
            //        Logger.Error("kiskac data: " + tXRXData.data[i]);
            //    }

            //}

            return this.serialManager.SendReq(tXRXData);

        }


        private bool SendMultipleReadRegister(ushort Start_Reg_Address, ushort registerCount, int receiveSize)
        {

            byte[] data = BuildReadMultipleRegisterMsg(this.slaveAddress, FunctionCode.FC3, Start_Reg_Address, registerCount);
            this.state = State.StatusSent;
            TXRXData tXRXData = new TXRXData();
            tXRXData.data = data;
            tXRXData.receiveSize = receiveSize;
            tXRXData.SlaveAddress = this.slaveAddress;
            return this.serialManager.SendReq(tXRXData);

        }



        private byte[] BuildWriteMultipleRegisterMsg(byte slaveAddress, byte function, ushort startAddress, ushort registerCount, short[] values)
        {
            byte[] frame = new byte[9 + 2 * registerCount];
            frame[0] = slaveAddress;                        // Slave address
            frame[1] = function;                            // Function code            
            frame[2] = (byte)(startAddress >> 8);           // start Address High
            frame[3] = (byte)startAddress;                  // start Address Low
            frame[4] = (byte)(registerCount >> 8);           // registerCount High
            frame[5] = (byte)registerCount;                  // registerCount Low
            frame[6] = (byte)(registerCount * 2);
            for (int i = 0; i < registerCount; i++)
            {
                frame[7 + 2 * i] = (byte)(values[i] >> 8);
                frame[8 + 2 * i] = (byte)(values[i]);
            }
            byte[] crc = CalculateCRC(frame);          // Calculate CRC
            frame[frame.Length - 2] = crc[0];               //CRC low
            frame[frame.Length - 1] = crc[1];               //CRC high
            return frame;
        }

        private byte[] BuildReadMultipleRegisterMsg(byte slaveAddress, byte function, ushort startAddress, ushort registerCount)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveAddress;                        // Slave address
            frame[1] = function;                            // Function code            
            frame[2] = (byte)(startAddress >> 8);           // start Address High
            frame[3] = (byte)startAddress;                  // start Address Low
            frame[4] = (byte)(registerCount >> 8);           // registerCount High
            frame[5] = (byte)registerCount;                  // registerCount Low
            byte[] crc = CalculateCRC(frame);          // Calculate CRC
            frame[frame.Length - 2] = crc[0];               //CRC low
            frame[frame.Length - 1] = crc[1];               //CRC high
            return frame;
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




    }
}
