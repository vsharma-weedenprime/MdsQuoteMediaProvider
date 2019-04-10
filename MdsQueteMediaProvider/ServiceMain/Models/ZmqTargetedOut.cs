using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace MdsProvider.ServiceMain.Models
{
    public static class ZmqTargetedOut
    {
        public static Subject<List<string>> ZmqTargetedOutSubject = new Subject<List<string>>();
    }
}
