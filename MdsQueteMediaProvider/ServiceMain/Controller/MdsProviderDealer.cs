using MdsProvider.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MdsProvider.ServiceMain.Controller
{
    public class MdsProviderDealer
    {
        #region <fields>
        MdsProviderDealerConfig _config;

        #endregion</fields>

        #region <object construction and cleanup>
        public MdsProviderDealer(IOptions<MdsProviderDealerConfig> config)
        {
            _config = config.Value;
        }
        #endregion </object construction and cleanup>

        #region <methods>
        public void Run() { }

        #endregion </methods>
    }
}

