using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace MdsProvider.Diagnostics.Model
{
    public static class Heartbeat
    {
        internal static Subject<List<string>> LogHeartbeatSubject = new Subject<List<string>>();
        public static Subject<string/*Name of the object reporting*/> HeartbeatSubject = new Subject<string>();
    }
}
