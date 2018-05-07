using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Tool
{
    public class SSH
    {
        private SshClient client = null;

        public SSH(string ip, int port, string username, string password)
        {
            client = new SshClient(ip, port, username, password);
        }

        public void RunCommand(string cmd)
        {
            client.Connect();
            client.RunCommand(cmd);
            client.Disconnect();
        }
    }
}
