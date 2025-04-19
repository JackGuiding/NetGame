using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using System.Net.Sockets;

namespace GameServer {

    public class ServerMain : MonoBehaviour {

        bool isTearDown;

        Telepathy.Server server;

        void Awake() {

            // 监听: 开启哪个端口
            int port = 12345; // 1~65535
            int messageSize = 1024;

            // 1. new 
            server = new Server(messageSize);
            server.Start(port); // new Socket, 内部有个while

            Debug.Log("[server]Listening on port " + port);

            // 3. 事件
            server.OnConnected += (connId, str) => {
                Debug.Log("[server]Connected " + connId);
            };

            server.OnData += (connId, data) => {
                // string message = Encoding.UTF8.GetString(data);
                int message = BitConverter.ToInt32(data);
                Debug.Log("[server]Data " + connId + ": " + message);
                // server.Send(connId, data);
            };

            server.OnDisconnected += (connId) => {
                Debug.Log("[server]Disconnected " + connId);
            };

            Application.runInBackground = true;

        }

        void Update() {
            if (server != null) {
                // 2. Tick
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
                // 4. 因为是子线程，必须Stop
                server.Stop();
            }
        }

    }
}
