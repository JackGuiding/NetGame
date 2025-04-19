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

        Dictionary<string, RoleEntity> players = new Dictionary<string, RoleEntity>();

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
                } else if (typeID == MessageConst.Move_Bro) {
                    // MoveBroMessage
                    var msg = MessageHelper.ReadData<MoveBroMessage>(data.Array);
                    OnMove(msg);
                } else {
                    Debug.LogError("[client]Unknown typeID " + typeID);
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

            Vector3 move = Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up;
            bool has = players.TryGetValue(username, out var me);
            if (has && move != Vector3.zero) {
                Vector3 pos = me.transform.position + move * Time.deltaTime * 5.0f;
                MoveReqMessage req = new MoveReqMessage();
                req.username = username;
                req.position = new float[2] { pos.x, pos.y };
                MoveSend(req);
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
            RoleEntity role = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<RoleEntity>();
            role.name = msg.username;
            role.transform.position = new Vector3(msg.position[0], 0, msg.position[1]);
            players.TryAdd(msg.username, role);
        }

        void MoveSend(MoveReqMessage req) {
            // 1. MoveSend
            byte[] data = MessageHelper.ToData(req);
            client.Send(data);
        }

        void OnMove(MoveBroMessage msg) {
            bool has = players.TryGetValue(msg.username, out var role);
            if (has) {
                role.transform.position = new Vector3(msg.position[0], 0, msg.position[1]);
            } else {
                Debug.Log("[client]Move: " + msg.username + " not found");
            }
        }

    }
}
