using UnityEngine;
using System.Collections;

namespace Mono.Common
{
    /// <summary>
    /// Extends this class to become singleton class with MonoBehaviour
    /// </summary>
    /// <typeparam name="T"> T is a MonoSingleton class </typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString()+"_Singleton");
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                    
                }

                return _instance;
            }
        }

        public static T GetMonoInstance()
        {
            if (_instance == null)
            {
                if (Application.isPlaying)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
        
        protected virtual void Awake()
        {
            if (_instance == null)
                _instance = this as T;
        }

        public void Free()
        {
            _instance = null;
            FreeSingleton();
            DestroyObject(gameObject);
        }

        protected virtual void FreeSingleton()
        {
        }
    }
}
