
namespace Common
{
    public class MotorCommandResult
    {

        public int retWriteRegisterResult { get; set; }
        public int retReadRegisterResult { get; set; }
        public int retPosStatusResult { get; set; }
        public int retCurrentPosResult { get; set; }

        public MotorCommandResult()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.retWriteRegisterResult = -1;
            this.retReadRegisterResult = -1;
            this.retPosStatusResult = -1;
            this.retCurrentPosResult = -1;
        }

        public bool IsSuccess()
        {
            if (retWriteRegisterResult == 0 && retReadRegisterResult == 0 && retPosStatusResult == 0 && retCurrentPosResult == 0)
            {
                return true;
            }
            else
                return false;
        }

    }





    public class RelayCommandResult
    {

        public int retWriteRegisterResult { get; set; }

        public RelayCommandResult()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.retWriteRegisterResult = -1;

        }

        public bool IsSuccess()
        {
            if (retWriteRegisterResult == 0)
            {
                return true;
            }
            else
                return false;
        }

    }

    public class SensorCommandResult
    {

        public int retReadRegisterResult { get; set; }

        public SensorCommandResult()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.retReadRegisterResult = -1;

        }

        public bool IsSuccess()
        {
            if (this.retReadRegisterResult == 0)
            {
                return true;
            }
            else
                return false;
        }

    }
}
