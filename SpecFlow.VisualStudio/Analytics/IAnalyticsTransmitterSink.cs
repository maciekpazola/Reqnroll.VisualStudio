﻿using System;
using System.Composition;
using System.Linq;
using Microsoft.VisualStudio.ApplicationInsights;
using Microsoft.VisualStudio.ApplicationInsights.DataContracts;

namespace SpecFlow.VisualStudio.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        void TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
    
    [Export(typeof(IAnalyticsTransmitterSink))]
    public class AppInsightsAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly IEnableAnalyticsChecker _enableAnalyticsChecker;
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly IVsVersionProvider _vsVersionProvider;

        [ImportingConstructor]
        public AppInsightsAnalyticsTransmitterSink(IEnableAnalyticsChecker enableAnalyticsChecker, IUserUniqueIdStore userUniqueIdStore, IVsVersionProvider vsVersionProvider)
        {
            _enableAnalyticsChecker = enableAnalyticsChecker;
            _userUniqueIdStore = userUniqueIdStore;
            _vsVersionProvider = vsVersionProvider;
        }

        public void TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            if (!_enableAnalyticsChecker.IsEnabled())
                return;

            var userUniqueId = _userUniqueIdStore.GetUserId();
            var appInsightsEvent = new EventTelemetry(analyticsEvent.EventName)
            {
                Timestamp = DateTime.UtcNow,
                Properties =
                {
                    
                    { "Ide", "Microsoft Visual Studio" },
                    { "UserId", userUniqueId },
                    { "UtcDate", DateTime.UtcNow.ToString("O") },
                    { "IdeVersion", _vsVersionProvider.GetVersion() },
                    { "ExtensionVersion", "TODO" }
                }
            };
            foreach (var analyticsEventProperty in analyticsEvent.Properties)
            {
                appInsightsEvent.Properties.Add(analyticsEventProperty);
            }

            var telemetryClient = GetTelemetryClient(userUniqueId);
            telemetryClient.TrackEvent(appInsightsEvent);
            telemetryClient.Flush();
        }

        private TelemetryClient GetTelemetryClient(string userUniqueId)
        {
            return new TelemetryClient
            {
                Context =
                {
                    User =
                    {
                        Id = userUniqueId,
                        AccountId = userUniqueId
                    },
                },
            };
        }
    }
}
