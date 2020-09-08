using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishingBot {
    class Offset {
        //CTM
        public static int CTM_Base = 0x00CA11D8;
        public static int CTM_X = CTM_Base + 0x90;
        public static int CTM_Y = CTM_Base + 0x8C;
        public static int CTM_Z = CTM_Base + 0x94;
        public static int CTM_Distance = CTM_Base + 0xC;
        public static int CTM_Action = CTM_Base + 0x1C;
        public static int CTM_GUID = CTM_Base + 0x20;

        //MOUSEOVER
        public static int MOUSE_OVER_GUID = 0x00BD07A0;

        //OBJ értékek

        public static int TYPE_ID = 0x224;

    }
}
