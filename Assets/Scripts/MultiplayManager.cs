using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.RenderStreaming;

namespace Tree.Multiplay
{
    class MultiplayManager : MonoBehaviour
    {
        [SerializeField] SignalingManager renderStreaming;
        [SerializeField] GameObject prefabHost;
        [SerializeField] GameObject prefabGuest;
        [SerializeField] GameObject prefabPlayer;
        [SerializeField] RawImage videoImage;

        private bool serverStarted = false;

        enum Role
        {
            Host = 0,
            Guest = 1
        }

        private RenderStreamingSettings settings;

        void Start()
        {
            if( !serverStarted ){
                SetUpHost();
                serverStarted = true;
            } else {
                var username =  UnityEngine.Random.Range(0, 99999).ToString("00000");
                var connectionId = Guid.NewGuid().ToString();
                // StartCoroutine(SetUpGuest(username, connectionId));
            }
        }

        void SetUpHost()
        {
            var instance = GameObject.Instantiate(prefabHost);
            var handler = instance.GetComponent<Multiplay>();

            // host player
            var hostPlayer = GameObject.Instantiate(prefabPlayer);
            var playerInput = hostPlayer.GetComponent<InputReceiver>();
            playerInput.PerformPairingWithAllLocalDevices();

            if (settings != null)
                renderStreaming.useDefaultSettings = settings.UseDefaultSettings;
            if (settings?.SignalingSettings != null)
                renderStreaming.SetSignalingSettings(settings.SignalingSettings);
            renderStreaming.Run(handlers: new SignalingHandlerBase[] { handler });
        }

        IEnumerator SetUpGuest(string username, string connectionId)
        {
            var guestPlayer = GameObject.Instantiate(prefabGuest);
            var handler = guestPlayer.GetComponent<SingleConnection>();

            if (settings != null)
                renderStreaming.useDefaultSettings = settings.UseDefaultSettings;
            if (settings?.SignalingSettings != null)
                renderStreaming.SetSignalingSettings(settings.SignalingSettings);
            renderStreaming.Run(handlers: new SignalingHandlerBase[] { handler });

            videoImage.gameObject.SetActive(true);
            var receiveVideoViewer = guestPlayer.GetComponent<VideoStreamReceiver>();
            receiveVideoViewer.OnUpdateReceiveTexture += texture => videoImage.texture = texture;

            var channel = guestPlayer.GetComponent<MultiplayChannel>();
            channel.OnStartedChannel += _ => { StartCoroutine(ChangeLabel(channel, username)); };

            // todo(kazuki):
            yield return new WaitForSeconds(1f);

            handler.CreateConnection(connectionId);
            yield return new WaitUntil(() => handler.IsConnected(connectionId));
        }

        IEnumerator ChangeLabel(MultiplayChannel channel, string username)
        {
            yield return new WaitUntil(() => channel.IsConnected);
            channel.ChangeLabel(username);
        }
    }
}
