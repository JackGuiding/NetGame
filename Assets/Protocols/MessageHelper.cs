using System;
using System.Text;
using TinyJson;

namespace NetGame_Protocol {

    public static class MessageHelper {

        public static byte[] ToData(int typeID, object msg) {
            string str = msg.ToJson();

            byte[] msg_header = BitConverter.GetBytes(typeID);
            byte[] msg_data = System.Text.Encoding.UTF8.GetBytes(str);

            byte[] msg_dst = new byte[msg_header.Length + msg_data.Length];

            // header 写入 dst
            Buffer.BlockCopy(msg_header, 0, msg_dst, 0, msg_header.Length);

            // data 写入 dst
            Buffer.BlockCopy(msg_data, 0, msg_dst, msg_header.Length, msg_data.Length);

            return msg_dst;
        }
    }
}