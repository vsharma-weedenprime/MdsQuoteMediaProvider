using MdsProvider.Configuration;
using MdsProvider.TestService.API;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;

namespace MdsProvider.TestService.Controller
{
    public class TestService:ITestService
    {
        #region <fields>

        bool _disposed = false;
        TestServiceConfig _config;

        #endregion </fields> 

        #region <object construction and cleanup>

        public TestService(IOptions<TestServiceConfig> config)
        {
            _config = config.Value;
        }

        #endregion </object construction and cleanup>


        #region <methods> 

        public void Run()
        {
            Log.Logger.Debug(_config.MessageFromConfig);
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion </methods> 

        #region <IDisposible>

        #region <properties> 

        private List<IDisposable> Disposibles { get; set; }

        #endregion </properties>

        #region <methods>

        #region <public>

        public void AddDisposible(IDisposable disposibleObject)
        {
            if (Disposibles == null)
                Disposibles = new List<IDisposable>();

            if (disposibleObject != null)
                Disposibles.Add(disposibleObject);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion </public>

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    #region <dispose managed resources>

                    foreach (var disposible in Disposibles)
                        disposible.Dispose();

                    #endregion </dispose managed resources>
                }

                #region <delete unmanaged resources> 
                #endregion </delete unmanaged resources>

                _disposed = true;
            }
        }

        #endregion </methods>

        #endregion </IDisposible>
    }
}
    