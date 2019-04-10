using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace MdsProvider.Diagnostics.Model
{
    public static class DiagnosticsInfo
    {
        public static Subject<List<string>> DiagnosticsInfoSubject = new Subject<List<string>>();
    }
}
