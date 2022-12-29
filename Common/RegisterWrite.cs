
namespace Common
{
    public class RegisterWrite
    {
        public ushort Register_Address { get; set; }
        public short Register_Target_Value { get; set; }

        public RegisterWrite(ushort register_Address, short register_Target_Value)
        {
            this.Register_Address = register_Address;
            this.Register_Target_Value = register_Target_Value;
        }
    }
}
