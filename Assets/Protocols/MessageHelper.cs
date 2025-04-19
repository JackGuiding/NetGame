using System;
using System.Collections.Generic;
using System.Text;
using TinyJson;

namespace NetGame_Protocol {

    public static class MessageHelper {

        static Dictionary<Type, int> typeIDMap = new Dictionary<Type, int>() {
            { typeof(LoginReqMessage), MessageConst.Login_Req },
            { typeof(LoginResMessage), MessageConst.Login_Res },
            { typeof(SpawnRoleReqMessage), MessageConst.SpawnRole_Req },
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
            string str = msg.ToJson();

            int typeID = GetTypeID<T>();

            byte[] msg_header = BitConverter.GetBytes(typeID);
            byte[] msg_data = System.Text.Encoding.UTF8.GetBytes(str);

            byte[] msg_dst = new byte[msg_header.Length + msg_data.Length];

            // header 写入 dst
            Buffer.BlockCopy(msg_header, 0, msg_dst, 0, msg_header.Length);

            // data 写入 dst
            Buffer.BlockCopy(msg_data, 0, msg_dst, msg_header.Length, msg_data.Length);

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
    }
}