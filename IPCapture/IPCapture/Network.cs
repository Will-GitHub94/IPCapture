using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCapture
{
    public class Network
    {
        public string DefaultGateway { get; set; }
        public string ExternalIP { get; set; }
        public string Name { get; set; }

        public Network() { }
    }
}
