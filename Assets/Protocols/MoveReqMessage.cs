using System;

namespace NetGame_Protocol {

    public struct MoveReqMessage {

        public string username;
        public float[] position; // 2D坐标

    }

}