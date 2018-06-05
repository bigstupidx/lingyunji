#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.UI
{

    [AutoILMono]
    public class FriendSearchListItem
    {
        [SerializeField]
        Transform m_Transform;

        public FriendSearchListItem() { }



    }
}