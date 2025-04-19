using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using NetGame_Protocol;
using TinyJson;

namespace GameClient {

    public class ClientMain : MonoBehaviour {

        [SerializeField] string username;

        Client client;

        bool isTearDown;

        void Awake() {

            // 1. 连接谁? IP + Port
            int port = 12345;
            int messageSize = 1024;
            string ip = "127.0.0.1"; // 本地IP
            client = new Client(messageSize);
            client.Connect(ip, port);

            // 3. 事件
            client.OnConnected += () => {
                Debug.Log("[client]Connected");
            };

            client.OnData += (data) => {
                int typeID = MessageHelper.ReadHeader(data.Array);
                if (typeID == MessageConst.SpawnRole_Bro) {
                    // SpawnRoleBroMessage
                    var msg = MessageHelper.ReadData<SpawnRoleBroMessage>(data.Array);
                    OnSpawn(msg);
                }
            };

            client.OnDisconnected += () => {
                Debug.Log("[client]Disconnected");
            };

            Debug.Log("[client]Connecting to " + ip + ":" + port);

            Application.runInBackground = true;
        }

        void Update() {
            // 2. Tick
            if (client != null) {
                client.Tick(10);
            }

            if (Input.GetKeyUp(KeyCode.Space)) {
                // JSON序列化插件:
                // 1. Origin
                LoginReqMessage msg = new LoginReqMessage();
                msg.username = username;
                msg.password = "hello123";
                byte[] data = MessageHelper.ToData(msg);
                Debug.Log("[client]Send: " + data.Length);
                client.Send(data);
            }

            if (Input.GetKeyUp(KeyCode.R)) {
                SpawnRoleReqMessage msg = new SpawnRoleReqMessage();
                msg.username = username;
                msg.position = new float[2] { 1.0f, 2.0f };

                byte[] data = MessageHelper.ToData(msg);
                client.Send(data);
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

            // 4. Stop / Disconnect
            if (client != null) {
                client.Disconnect();
            }
        }

        // ==== Game Logic ====
        void OnSpawn(SpawnRoleBroMessage msg) {
            // 1. Spawn
            Debug.Log("[client]Spawn: " + msg.username + " at " + msg.position[0] + ", " + msg.position[1]);
        }

        void MoveTo(string username, float[] position) {
            // 2. MoveTo
            Debug.Log("[client]MoveTo: " + username + " to " + position[0] + ", " + position[1]);
        }

    }
}
