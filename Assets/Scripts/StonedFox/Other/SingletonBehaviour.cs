using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;

        protected virtual void Awake()
        {
            SingletonBehaviour<T>[] t = FindObjectsOfType<SingletonBehaviour<T>>();
            if (t != null && t.Length > 1)
            {
                Debug.LogError("Already created singleton type of " + typeof(T).ToString());
                Debug.LogError("Now Destroying this");
                Destroy(this);
                return;
            }
            Instance = GetComponent<T>();
        }
    }
}
