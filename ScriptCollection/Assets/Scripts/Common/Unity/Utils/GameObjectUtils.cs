using UnityEngine;

namespace Common.Unity
{
    public static class GameObjectUtils
    {
#if UNITY_EDITOR

        public static GameObject FindGameObject(this GameObject obj, string name, bool includeInactive = true,
            bool showError = true)
        {
            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return null;
        }

        public static bool BindComponent<T>(this GameObject obj, out T component,
            string name, bool includeInactive = true, bool showError = true) where T : Component
        {
            component = null;

            T temp = null;

            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    var c = t.GetComponent<T>();
                    if (c != null)
                    {
                        if (temp != null)
                        {
                            if (showError)
                                Debug.LogErrorFormat(
                                    "ERROR: There is more than 1 Component name of {0}, Please check it",
                                    name);

                            return false;
                        }

                        temp = c;
                    }
                    else
                    {
                        if (showError)
                            Debug.LogErrorFormat("ERROR: Cannot find {0} component in {1} Obj", typeof(T), name);
                    }
                }
            }

            if (temp != null)
            {
                component = temp;
                return true;
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return false;
        }

        public static bool BindComponent(this GameObject obj, out GameObject target,
            string name, bool includeInactive = true, bool showError = true)
        {
            target = null;

            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    target = t.gameObject;
                    return true;
                }
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return false;
        }

        public static bool BindComponent(this GameObject obj, out Transform target,
            string name, bool includeInactive = true, bool showError = true)
        {
            target = null;

            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    target = t;
                    return true;
                }
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return false;
        }

#endif
    }
}