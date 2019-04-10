using MdsProvider.Configuration;
using MdsProvider.Diagnostics.API;
using MdsProvider.Diagnostics.Model;
using MdsProvider.Diagnostics.Utils;
using MdsProvider.ServiceMain.Models;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdsProvider.Diagnostics.Controller
{
    class MdsProviderDiagnostics : IMdsProviderDiagnostics
    {
        #region <fields>

        bool _disposed = false;
        int _logCounter = 1;
        MdsProviderDiagnosticsConfig _config;
        HashSet<Tuple<string, int>> _deadCounter = new HashSet<Tuple<string, int>>();
        object _deadCounterMonitor = new object();
        object _heartbeatMonitor = new object();

        IObservable<IGroupedObservable<string, List<string>>> _heartbeats =
            from s in Heartbeat.LogHeartbeatSubject where s != null group s by s.FirstOrDefault();

        #endregion </fields>

        #region <object creation and cleanup>

        public MdsProviderDiagnostics(IOptions<MdsProviderDiagnosticsConfig> config)
        {
            _config = config.Value;
        }

        #endregion </object creation and cleanup>

        #region <methods>

        #region <public>
        public void Run()
        {
            SubscribeDiagnosticsInfo(); // Subscribe to diagnostics messages 
            SubscribeHeartbeatLogger(); // Subscribe to internal heartbeat logger
            SubscribeHeartbeats(); // Subscribe to heartbeat reports 
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion </public>

        private void SubscribeHeartbeats()
        {
            AddDisposible(
            Heartbeat.HeartbeatSubject.Subscribe((reportingObjectName)=> 
            {
                LogHeartbeat(reportingObjectName);
            }));
        }

        private void SubscribeHeartbeatLogger()
        {
            _heartbeats.ForEachAsync(group =>
            {
                AddDisposible(
                group.Throttle(TimeSpan.FromMilliseconds(_config.HeartbeatCheckDurationInMilliseconds)).Subscribe((hbData) =>
                {
                    LogDiagnosticsAsync($"Last tick from '{hbData[0]}' was at {hbData[1]}.");
                    lock (_deadCounterMonitor)
                    {
                        var deadCheck = _config.DeadCondition.FirstOrDefault(o => o.ObjectName == hbData[0]);
                        if (deadCheck != null)
                        {
                            if (deadCheck.HeartbeatCheckElapseCount <= 1)
                            {
                                LogDiagnosticsAsync($"'{hbData[0]}' has stopped working!");
                                return;
                            }
                            var dCount = _deadCounter.FirstOrDefault(dc => dc.Item1 == hbData[0]);
                            if (dCount == null)
                            {
                                dCount = new Tuple<string, int>(hbData[0], 1);
                                _deadCounter.Add(dCount);
                            }
                            else
                            {
                                _deadCounter.Remove(dCount);
                                if (dCount.Item2 >= deadCheck.HeartbeatCheckElapseCount)
                                {
                                    LogDiagnosticsAsync($"'{hbData[0]}' has stopped working!");
                                    MissingHeartbeat.MissingHeartbeatSubject.OnNext(hbData[0]); // notify subscriber of the object that has been missing hartbeats
                                    return;
                                }
                                _deadCounter.Add(new Tuple<string, int>(dCount.Item1, dCount.Item2 + 1));

                            }
                            LogDiagnosticsAsync($"Deadcounting '{hbData[0]}', {dCount.Item2} of {deadCheck.HeartbeatCheckElapseCount}");
                            LogHeartbeat(hbData[0], true);
                        }
                    }
                }));
            });
        }

        private void LogHeartbeat(string objectName, bool rePost = false)
        {
            var timeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");

            Task.Run(() =>
            {
                if (!rePost)
                {
                    lock (_deadCounterMonitor)
                    {
                        // reset the dead counter since the message orginated from source 
                        var counter = _deadCounter.FirstOrDefault(t => t.Item1 == objectName);
                        if (counter != null)
                        {
                            _deadCounter.Remove(counter);
                        }
                    }
                }
                lock (_heartbeatMonitor)
                {
                    Heartbeat.LogHeartbeatSubject.OnNext(new List<string>() { objectName, timeStamp });
                }

            });
        }


        private void LogDiagnosticsAsync(string message)
        {
            var msg = new List<string>() { "[" + DateTime.Now.Ticks.ToString() + " " + _logCounter++ + "]", message };
            Task.Run(() =>
            {
                DiagnosticsInfo.DiagnosticsInfoSubject.OnNext(msg);
            });
        }


        private void SubscribeDiagnosticsInfo()
        {
            AddDisposible(
            DiagnosticsInfo.DiagnosticsInfoSubject.Subscribe((msg) =>
            {
                if (msg == null)
                    return;
                var output = Constants.DiagnosticsPrefix + msg.Aggregate((i, j) => i + "\t::\t" + j);
                Log.Logger.Information(output);

                if (msg[0].Trim() == Constants.ErrorMessage)
                {
                    ZmqTargetedOut.ZmqTargetedOutSubject.OnNext(msg);
                }
            }));

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
