using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace MdsProvider.Diagnostics.Model
{
    public static class MissingHeartbeat
    {
        public static Subject<string> MissingHeartbeatSubject = new Subject<string>();
    }
}
