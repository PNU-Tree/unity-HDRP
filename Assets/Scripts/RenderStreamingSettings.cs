using System;
using Unity.RenderStreaming;

#if URS_USE_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace Tree.Multiplay
{
    internal enum SignalingType
    {
        WebSocket,
        Http,
    }

    internal class RenderStreamingSettings
    {
        private bool useDefaultSettings = false;
        private SignalingType signalingType = SignalingType.WebSocket;
        private string signalingAddress = "localhost";
        private int signalingInterval = 5000;
        private bool signalingSecured = false;

        public bool UseDefaultSettings
        {
            get { return useDefaultSettings; }
            set { useDefaultSettings = value; }
        }

        public SignalingType SignalingType
        {
            get { return signalingType; }
            set { signalingType = value; }
        }

        public string SignalingAddress
        {
            get { return signalingAddress; }
            set { signalingAddress = value; }
        }

        public bool SignalingSecured
        {
            get { return signalingSecured; }
            set { signalingSecured = value; }
        }

        public int SignalingInterval
        {
            get { return signalingInterval; }
            set { signalingInterval = value; }
        }

        public SignalingSettings SignalingSettings
        {
            get
            {
                switch (signalingType)
                {
                    case SignalingType.WebSocket:
                        {
                            var schema = signalingSecured ? "wss" : "ws";
                            return new WebSocketSignalingSettings
                            (
                                url: $"{schema}://{signalingAddress}"
                            );
                        }
                    case SignalingType.Http:
                        {
                            var schema = signalingSecured ? "https" : "http";
                            return new HttpSignalingSettings
                            (
                                url: $"{schema}://{signalingAddress}",
                                interval: signalingInterval
                            );
                        }
                }
                throw new InvalidOperationException();
            }
        }
    }
}
