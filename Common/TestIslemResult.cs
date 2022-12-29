
using System.Collections.Generic;

namespace Common
{
    public class TestIslemResult
    {
        public TestCommand testCommand { get; set; }
        public int writeResponseValue { get; set; }
        public string writeResponseDesc { get; set; }
        public List<RegisterRead> readResponseValue { get; set; }
        public string readResponseDesc { get; set; }

    }
}
