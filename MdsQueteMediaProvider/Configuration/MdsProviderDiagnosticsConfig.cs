using MdsProvider.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MdsProvider.Configuration
{
    public class MdsProviderDiagnosticsConfig
    {
        public int HeartbeatCheckDurationInMilliseconds { get; set; }
        public List<DeadCondition> DeadCondition { get; set; }
    }
}
