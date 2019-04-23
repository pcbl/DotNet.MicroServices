using System;
using System.Collections.Generic;
using System.Text;

namespace WebClient.BusHelper
{
    public class ServerResponse
    {
        public int payload_bytes;
        public bool redelivered;
        public string exchange;
        public string routing_key;
        public int message_count;
        public string[] properties;
        public string payload;
        public string payload_encoding;
    }
}


