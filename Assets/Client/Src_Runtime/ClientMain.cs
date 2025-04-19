using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

namespace GameClient {

    public class ClientMain : MonoBehaviour {

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
                Debug.Log("[client]Data: " + data.Count + " bytes");
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
                // string message = "Hello111" + Time.time;
                // byte[] data = Encoding.UTF8.GetBytes(message);
                int a = Time.frameCount;
                byte[] data = BitConverter.GetBytes(a);
                client.Send(data);
                Debug.Log("[client]Send: " + a);
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

    }
}
