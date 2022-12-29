using System.Collections.Generic;


namespace Common
{
    
    public class TestCommand
    {
        public TestType testType { get; set; }
        public DeviceType deviceType { get; set; }
        public List<RegisterWrite> registerWrites { get; set; }
        public ushort readAddress { get; set; }
    }

}

