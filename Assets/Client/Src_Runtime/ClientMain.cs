using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

namespace GameClient {

    public class ClientMain : MonoBehaviour {

        Client client;

        bool isTearDown;

        void Awake() {
            int port = 12345;
            int messageSize = 1024;
            string ip = "127.0.0.1";
            client = new Client(messageSize);

            client.Connect(ip, port);
            client.OnConnected += () => {
                Debug.Log("[client]Connected");
            };

            client.OnData += (data) => {
                Debug.Log("[client]Data: " + data.Count + " bytes");
            };

            client.OnDisconnected += () => {
                Debug.Log("[client]Disconnected");
            };

            Debug.Log("[client]Connecting to " + ip + ":" + port);

            Application.runInBackground = true;
        }

        void Update() {
            if (client != null) {
                client.Tick(10);
            }
        }

        void OnDestroy() {
            TearDown();
        }

        void OnApplicationQuit() {
            TearDown();
        }

        void TearDown() {
            if (isTearDown) return;
            isTearDown = true;

            if (client != null) {
                client.Disconnect();
            }
        }

    }
}
