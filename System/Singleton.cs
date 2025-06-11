using UnityEngine;

namespace FancyUnity
{
    /// <summary>
    /// Singleton pattern.
    /// </summary>
    public class Singleton<T>: MonoBehaviour where T : Component
    {
        public bool Perpetual = false;

        protected static T _inst;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Inst
        {
            get
            {
                return _inst;
            }
        }

        /// <summary>
        /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Start()
        {
            if (_inst != null) throw new System.Exception("duplicate singleton");
            if (!Application.isPlaying)
            {
                return;
            }
            _inst = this as T;
            if (Perpetual)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public virtual void RemoveSelf()
        {
            _inst = null;
            DestroyImmediate(gameObject);
        }
    }
}
