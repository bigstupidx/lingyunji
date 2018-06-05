#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.UI
{

    [AutoILMono]
    public class FriendApplyListItem
    {
        [SerializeField]
        Transform m_Transform;

        public FriendApplyListItem() { }



    }
}