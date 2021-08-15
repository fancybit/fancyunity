using UnityEngine;

namespace FancyUnity
{
	/// <summary>
	/// Singleton pattern.
	/// </summary>
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		protected static T _inst;

		/// <summary>
		/// Singleton design pattern
		/// </summary>
		/// <value>The instance.</value>
		public static T Inst
		{
			get
			{
				if (_inst == null)
				{
					if (_inst == null)
					{
						GameObject obj = new GameObject(typeof(T).Name);
						DontDestroyOnLoad(obj);
						_inst = obj.AddComponent<T>();
					}
				}
				return _inst;
			}
		}

		/// <summary>
		/// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			_inst = this as T;
			DontDestroyOnLoad(gameObject);
		}

		public virtual void RemoveSelf()
        {
			_inst = null;
			DestroyImmediate(gameObject);
        }
	}
}
