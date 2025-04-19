using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

namespace GameServer {

    public class ServerMain : MonoBehaviour {

        bool isTearDown;

        Server server;

        void Awake() {
            int port = 12345;
            int messageSize = 1024;
            server = new Server(messageSize);
            server.Start(port);

            Debug.Log("[server]Listening on port " + port);

            server.OnConnected += (connId, str) => {
                Debug.Log("[server]Connected " + connId + ": " + str);
                server.Send(connId, new byte[] { 1, 2, 3, 4, 5 });
            };

            server.OnData += (connId, data) => {
                Debug.Log("[server]Data " + connId + ": " + data.Count + " bytes");
                server.Send(connId, data);
            };

            server.OnDisconnected += (connId) => {
                Debug.Log("[server]Disconnected " + connId);
            };

            Application.runInBackground = true;

        }

        void Update() {
            if (server != null) {
                server.Tick(10);
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

            if (server != null) {
                server.Stop();
            }
        }

    }
}
