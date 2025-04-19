using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using TinyJson;
using NetGame_Protocol;

namespace GameServer {

    public class ServerMain : MonoBehaviour {

        bool isTearDown;

        Telepathy.Server server;

        List<int/*connID*/> clients = new List<int>();

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
                clients.Add(connId);
            };

            server.OnData += (connId, data) => {
                int typeID = MessageHelper.ReadHeader(data.Array);
                if (typeID == MessageConst.Login_Req) {
                    // LoginMessage
                } else if (typeID == MessageConst.Login_Res) {
                    // SpawnRoleMessage
                }
            };

            server.OnDisconnected += (connId) => {
                Debug.Log("[server]Disconnected " + connId);
                clients.Remove(connId);
            };

            Application.runInBackground = true;

        }

        void Update() {
            if (server != null) {
                // 2. Tick
                server.Tick(10);
            }

            if (Input.GetKeyUp(KeyCode.Space)) {
                // 广播
                for (int i = 0; i < clients.Count; i++) {
                    int connId = clients[i];
                    SpawnRoleReqMessage msg = new SpawnRoleReqMessage();
                    msg.position = new float[] { 1, 2 };
                    byte[] data = MessageHelper.ToData(msg);
                    server.Send(connId, data);
                }
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
