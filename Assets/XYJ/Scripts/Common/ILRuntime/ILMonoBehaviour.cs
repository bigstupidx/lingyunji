namespace xys
{
    using UnityEngine;
    using System.Reflection;
    using System.Collections.Generic;

    public class AutoILMono : System.Attribute
    {

    }

    public interface IMono
    {

    }

    public partial class ILMonoBehaviour : MonoBehaviour, ILSerialized
    {
        [SerializeField]
        string typeName; // 对应的类型

        [SerializeField]
        List<Object> objs; // 对应的unit对象

        List<Object> ILSerialized.Objs { get { return objs; } }

        [SerializeField]
        string json; // 对应的json数据字段

        object instance;// typeName创建出来的实例
        
        public object GetObject()
        {
            Init();
            return instance;
        }
       
        MethodInfo awake = null;
        MethodInfo start = null;
        MethodInfo update = null;
        MethodInfo laterUpdate = null;
        MethodInfo onEnable = null;
        MethodInfo onDisable = null;
        MethodInfo onApplicationQuit = null;
        MethodInfo onAnimatorIK = null;
        MethodInfo onAnimatorMove = null;
        MethodInfo onApplicationFocus = null;
        MethodInfo onApplicationPause = null;
        MethodInfo onBecameInvisible = null;
        MethodInfo onBeforeTransformParentChanged = null;
        MethodInfo onCanvasGroupChanged = null;
        MethodInfo onBecameVisible = null;
        MethodInfo onCollisionEnter = null;
        MethodInfo onCollisionEnter2D = null;
        MethodInfo onCollisionExit = null;
        MethodInfo onCollisionExit2D = null;
        MethodInfo onCollisionStay = null;
        MethodInfo onCollisionStay2D = null;
        MethodInfo onControllerColliderHit = null;
        MethodInfo onDestroy = null;
        MethodInfo onParticleCollision = null;
        MethodInfo onParticleTrigger = null;
        MethodInfo onTriggerEnter = null;
        MethodInfo onTriggerEnter2D = null;
        MethodInfo onTriggerExit = null;
        MethodInfo onTriggerExit2D = null;
        MethodInfo onTriggerStay = null;
        MethodInfo onTriggerStay2D = null;
        MethodInfo onCellAdding = null;
        void Init(System.Type type)
        {
            awake = type.GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            start = type.GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            update = type.GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            laterUpdate = type.GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onEnable = type.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onDisable = type.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onApplicationQuit = type.GetMethod("OnApplicationQuit", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onAnimatorIK = type.GetMethod("OnAnimatorIK", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onAnimatorMove = type.GetMethod("OnAnimatorMove", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onApplicationFocus = type.GetMethod("OnApplicationFocus", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onApplicationPause = type.GetMethod("OnApplicationPause", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onBecameInvisible = type.GetMethod("OnBecameInvisible", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onBeforeTransformParentChanged = type.GetMethod("OnBeforeTransformParentChanged", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCanvasGroupChanged = type.GetMethod("OnCanvasGroupChanged", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onBecameVisible = type.GetMethod("OnBecameVisible", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCollisionEnter = type.GetMethod("OnCollisionEnter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCollisionEnter2D = type.GetMethod("OnCollisionEnter2D", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCollisionExit = type.GetMethod("OnCollisionExit", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCollisionExit2D = type.GetMethod("OnCollisionExit2D", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCollisionStay = type.GetMethod("OnCollisionStay", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCollisionStay2D = type.GetMethod("OnCollisionStay2D", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onControllerColliderHit = type.GetMethod("OnControllerColliderHit", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onDestroy = type.GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onParticleCollision = type.GetMethod("OnParticleCollision", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onParticleTrigger = type.GetMethod("OnParticleTrigger", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onTriggerEnter = type.GetMethod("OnTriggerEnter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onTriggerEnter2D = type.GetMethod("OnTriggerEnter2D", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onTriggerExit = type.GetMethod("OnTriggerExit", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onTriggerExit2D = type.GetMethod("OnTriggerExit2D", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onTriggerStay = type.GetMethod("OnTriggerStay", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onTriggerStay2D = type.GetMethod("OnTriggerStay2D", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            onCellAdding = type.GetMethod("OnCellAdding", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        void Init()
        {
            if (instance == null)
            {
                if (string.IsNullOrEmpty(typeName))
                    return;

                var type = IL.Help.GetType(typeName);
                if (type == null)
                {
                    Log.Error("type:{0} not find!", typeName);
                    enabled = false;
                    return;
                }

                Init(type);

                // 创建实例出来
                instance = IL.Help.Create(type);
                IL.MonoSerialize.MergeFrom(instance, new IL.MonoStream(new JSONObject(json), objs));
            }
        }

        private void Awake()
        {
            Init();
            if (awake != null)
                awake.Invoke(instance, null);
        }

        private void Start()
        {
            if (start != null)
                start.Invoke(instance, null);
        }

        private void Update()
        {
            if (update != null)
                update.Invoke(instance, null);
        }

        private void LateUpdate()
        {
            if (laterUpdate != null)
                laterUpdate.Invoke(instance, null);
        }

        private void OnEnable()
        {
            if (onEnable != null)
                onEnable.Invoke(instance, null);
        }

        private void OnDisable()
        {
            if (onDisable != null)
                onDisable.Invoke(instance, null);
        }

        private void OnApplicationQuit()
        {
            if (onApplicationQuit != null)
                onApplicationQuit.Invoke(instance, null);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (onAnimatorIK != null)
                onAnimatorIK.Invoke(instance, new object[] { layerIndex });
        }

        private void OnAnimatorMove()
        {
            if (onAnimatorMove != null)
                onAnimatorMove.Invoke(instance, null);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (onApplicationFocus != null)
                onApplicationFocus.Invoke(instance, new object[] { focus });
        }

        private void OnApplicationPause(bool pause)
        {
            if (onApplicationPause != null)
                onApplicationPause.Invoke(instance, new object[] { pause });
        }

        private void OnBecameInvisible()
        {
            if (onBecameInvisible != null)
                onBecameInvisible.Invoke(instance, null);
        }

        private void OnBeforeTransformParentChanged()
        {
            if (onBeforeTransformParentChanged != null)
                onBeforeTransformParentChanged.Invoke(instance, null);
        }

        private void OnCanvasGroupChanged()
        {
            if (onCanvasGroupChanged != null)
                onCanvasGroupChanged.Invoke(instance, null);
        }

        private void OnBecameVisible()
        {
            if (onBecameVisible != null)
                onBecameVisible.Invoke(instance, null);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (onCollisionEnter != null)
                onCollisionEnter.Invoke(instance, new object[] { collision });
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (onCollisionEnter2D != null)
                onCollisionEnter2D.Invoke(instance, new object[] { collision });
        }

        private void OnCollisionExit(Collision collision)
        {
            if (onCollisionExit != null)
                onCollisionExit.Invoke(instance, new object[] { collision });
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (onCollisionExit2D != null)
                onCollisionExit2D.Invoke(instance, new object[] { collision });
        }

        private void OnCollisionStay(Collision collision)
        {
            if (onCollisionStay != null)
                onCollisionStay.Invoke(instance, new object[] { collision });
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (onCollisionStay2D != null)
                onCollisionStay2D.Invoke(instance, new object[] { collision });
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (onControllerColliderHit != null)
                onControllerColliderHit.Invoke(instance, new object[] { hit });
        }

        private void OnDestroy()
        {
            if (onDestroy != null)
                onDestroy.Invoke(instance, null);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (onParticleCollision != null)
                onParticleCollision.Invoke(instance, new object[] { other });
        }

        private void OnParticleTrigger()
        {
            if (onParticleTrigger != null)
                onParticleTrigger.Invoke(instance, null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (onTriggerEnter != null)
                onTriggerEnter.Invoke(instance, new object[] { other });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (onTriggerEnter2D != null)
                onTriggerEnter2D.Invoke(instance, new object[] { collision });
        }

        private void OnTriggerExit(Collider other)
        {
            if (onTriggerExit != null)
                onTriggerExit.Invoke(instance, new object[] { other });
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (onTriggerExit2D != null)
                onTriggerExit2D.Invoke(instance, new object[] { collision });
        }

        private void OnTriggerStay(Collider other)
        {
            if (onTriggerStay != null)
                onTriggerStay.Invoke(instance, new object[] { other });
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (onTriggerStay2D != null)
                onTriggerStay2D.Invoke(instance, new object[] { collision });
        }

        private void OnCellAdding(int index)
        {
            if (onCellAdding != null)
                onCellAdding.Invoke(instance, new object[] {index});
        }
    }
}
