using System;
using System.Collections.Generic;
using System.Text;
using TinyJson;
using UnityEngine;

namespace NetGame_Protocol {

    public static class MessageHelper {

        static Dictionary<Type, int> typeIDMap = new Dictionary<Type, int>() {
            { typeof(LoginReqMessage), MessageConst.Login_Req },
            { typeof(LoginResMessage), MessageConst.Login_Res },
            { typeof(SpawnRoleReqMessage), MessageConst.SpawnRole_Req },
            { typeof(SpawnRoleBroMessage), MessageConst.SpawnRole_Bro },
            { typeof(MoveReqMessage), MessageConst.Move_Req },
            { typeof(MoveBroMessage), MessageConst.Move_Bro },
        };

        public static int GetTypeID<T>() {
            Type type = typeof(T);
            if (typeIDMap.ContainsKey(type)) {
                return typeIDMap[type];
            } else {
                throw new Exception("MessageHelper: Type not found");
            }
        }

        public static byte[] ToData<T>(T msg) {
            string str = JsonUtility.ToJson(msg);

            int typeID = GetTypeID<T>();

            byte[] msg_header = BitConverter.GetBytes(typeID);
            byte[] msg_data = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] msg_length = BitConverter.GetBytes(msg_data.Length);

            byte[] msg_dst = new byte[msg_header.Length + 4 + msg_data.Length];

            // header 写入 dst
            Buffer.BlockCopy(msg_header, 0, msg_dst, 0, msg_header.Length);

            // length 写入 dst
            Buffer.BlockCopy(msg_length, 0, msg_dst, msg_header.Length, msg_length.Length);

            // data 写入 dst
            Buffer.BlockCopy(msg_data, 0, msg_dst, msg_header.Length + msg_length.Length, msg_data.Length);

            UnityEngine.Debug.Log("[MessageHelper]ToData: " + typeID + " " + str);

            return msg_dst;
        }

        public static int ReadHeader(byte[] data) {
            if (data.Length < 4) {
                return -1;
            } else {
                int typeID = BitConverter.ToInt32(data, 0);
                return typeID;
            }
        }

        public static T ReadData<T>(byte[] data) where T : struct {
            if (data.Length < 4) {
                return default;
            } else {
                int typeID = ReadHeader(data);
                if (typeID != GetTypeID<T>()) {
                    throw new Exception("MessageHelper: Type mismatch");
                } else {
                    int length = BitConverter.ToInt32(data, 4);
                    string str = Encoding.UTF8.GetString(data, 8, length);
                    T msg = JsonUtility.FromJson<T>(str);
                    return msg;
                }
            }
        }
    }
}