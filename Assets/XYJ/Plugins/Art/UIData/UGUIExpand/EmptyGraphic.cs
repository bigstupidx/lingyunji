using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class EmptyGraphic : Graphic
    {
        public override void SetMaterialDirty() {  }
        public override void SetVerticesDirty() {  }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }

        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            if (!isActiveAndEnabled)
                return false;

            return true;
        }
    }
}