using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public interface GM_IPage
    {
        void OnOpen();
        void OnClose();
        string GetTitle();
        void OnGUI( Rect beginArea );
    }

}
