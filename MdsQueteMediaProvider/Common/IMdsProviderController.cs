using System;
using System.Collections.Generic;
using System.Text;

namespace MdsProvider.Common
{
    public interface IMdsProviderController :IDisposable
    {
        #region <methods>

        void Run();
        void Stop();
        void AddDisposible(IDisposable disposibleObject);

        #endregion </methods>
    }
}
