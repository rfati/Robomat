
namespace Common
{
    public class RegisterRead
    {
        public ushort Register_Address { get; set; }
        public short Register_Read_Value { get; set; }

        public RegisterRead(ushort register_Address, short register_Read_Value = 0)
        {
            this.Register_Address = register_Address;
            this.Register_Read_Value = register_Read_Value;
        }
    }
}
