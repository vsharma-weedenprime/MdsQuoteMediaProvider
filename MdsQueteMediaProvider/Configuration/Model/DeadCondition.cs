using System;
using System.Collections.Generic;
using System.Text;

namespace MdsProvider.Configuration.Model
{
    public class DeadCondition
    {
        public string ObjectName { get; set; }
        public int HeartbeatCheckElapseCount { get; set; }
    }
}
