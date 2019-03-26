using System;
using System.Reflection;
using System.Text;
using Common.CSharp;
using UnityEngine;

namespace Common.Unity
{
    public class BindBase : MonoBehaviour
    {
#if UNITY_EDITOR
        private UnityEngine.Object FindComponentInObj(string name, Type type, bool showError = true)
        {
            var obj = gameObject.FindGameObject(name, showError: showError);
            if (obj != null)
            {
                if (type == typeof(GameObject))
                {
                    return obj;
                }

                if (type == typeof(Transform))
                {
                    return obj.transform;
                }

                try
                {
                    return obj.GetComponent(type);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return null;
        }

        protected bool BC(out GameObject obj, string name, bool showError = true)
        {
            return BindComponent(out obj, name, showError);
        }

        protected bool BC(out Transform tf, string name, bool showError = true)
        {
            return BindComponent(out tf, name, showError);
        }

        protected bool BC<T>(out T component, string name, bool showError = true) where T : Component
        {
            return BindComponent(out component, name, showError);
        }

        protected bool BC<T>(out T component, bool includeInactive = true, bool showError = true) where T : Component
        {
            return BindComponent(out component, includeInactive, showError);
        }

        protected bool BCS<T>(out T[] components, bool includeInactive = true, bool showError = true)
            where T : Component
        {
            return BindComponents(out components, includeInactive, showError);
        }

        protected bool BindComponent(out GameObject obj, string name, bool showError = true)
        {
            return gameObject.BindComponent(out obj, name, showError: showError);
        }

        protected bool BindComponent(out Transform tf, string name, bool showError = true)
        {
            return gameObject.BindComponent(out tf, name, showError: showError);
        }

        protected bool BindComponent<T>(out T component, string name, bool showError = true) where T : Component
        {
            return gameObject.BindComponent(out component, name, showError: showError);
        }

        protected bool BindComponent<T>(out T component, bool includeInactive = true, bool showError = true)
            where T : Component
        {
            var c = GetComponentInChildren<T>(includeInactive);
            if (c == null)
            {
                component = null;

                if (showError)
                    Debug.LogErrorFormat("ERROR: Cannot find {0} component in {1} Obj's child", typeof(T), name);

                return false;
            }

            component = c;
            return true;
        }

        protected bool BindComponents<T>(out T[] components, bool includeInactive = true, bool showError = true)
            where T : Component
        {
            var c = GetComponentsInChildren<T>(includeInactive);
            if (c == null)
            {
                components = null;

                if (showError)
                    Debug.LogErrorFormat("ERROR: Cannot find {0} components in {1} Obj's child", typeof(T), name);

                return false;
            }

            components = c;
            return true;
        }

        [ContextMenu("Validate")]
        public void ValidateMenu()
        {
            OnValidate();
            ValidateInEditor();
        }

        protected virtual void OnValidate()
        {
        }

        protected virtual void ValidateInEditor()
        {
        }

        [ContextMenu("自动绑定")]
        public void AutoBind()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<color=Orange>Auto Bind Result：</color>\n---------------------\n");

            int suc = 0;
            int fail = 0;

            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (!CommonUtils.IsTypeEqual(field.FieldType, typeof(UnityEngine.Object)) || field.FieldType.IsArray)
                    continue;

                var attrs = field.GetCustomAttributes(false);

                if (field.IsPrivate)
                {
                    if (CommonUtils.ContainType(attrs, typeof(SerializeField)))
                    {
                        sb.AppendLine(BindFieldWithComponent(field, ref suc, ref fail));
                    }
                }
                else
                {
                    if (!CommonUtils.ContainType(attrs, typeof(HideInInspector)))
                    {
                        sb.AppendLine(BindFieldWithComponent(field, ref suc, ref fail));
                    }
                }
            }

            sb.AppendLine(string.Format("\n<color=red>Total:</color>\nSucc Count {0}\nSkip Count {1}", suc, fail));

            Debug.Log(sb.ToString());
        }

        [ContextMenu("根据脚本引用自动更改 Transform 名字")]
        public void AutoChangeName()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<color=Orange>Auto Bind Result：</color>\n---------------------\n");

            var suc = 0;
            var fail = 0;

            var type = this.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (!CommonUtils.IsTypeEqual(field.FieldType, typeof(UnityEngine.Object)) || field.FieldType.IsArray)
                    continue;

                var attrs = field.GetCustomAttributes(false);

                if (field.IsPrivate)
                {
                    if (CommonUtils.ContainType(attrs, typeof(SerializeField)))
                    {
                        sb.AppendLine(ChangeNameWithFieldName(this, field, ref suc, ref fail));
                    }
                }
                else
                {
                    if (!CommonUtils.ContainType(attrs, typeof(HideInInspector)))
                    {
                        sb.AppendLine(ChangeNameWithFieldName(this, field, ref suc, ref fail));
                    }
                }
            }

            sb.AppendLine(string.Format("\n<color=red>Total:</color>\nSucc Count {0}\nSkip Count {1}", suc, fail));

            Debug.Log(sb.ToString());
        }

        private string ChangeNameWithFieldName(object obj, FieldInfo fieldInfo, ref int suc, ref int fail)
        {
            var value = fieldInfo.GetValue(obj);
            if (value is Component)
            {
                var comp = value as Component;

                if (comp.transform == transform)
                {
                    fail++;
                    return string.Format("{0}\t\t\t\t\tSkip", fieldInfo.Name);
                }

                comp.transform.name = fieldInfo.Name;
                suc++;
                return string.Format("<color=green>{0} \t----->\t {1}</color>", GetObjUrl(comp.transform, name),
                    fieldInfo.Name);
            }
            else if (value is GameObject)
            {
                var gameObj = value as GameObject;

                if (gameObj == gameObject)
                {
                    fail++;
                    return string.Format("{0}\t\t\t\t\tSkip", fieldInfo.Name);
                }

                gameObj.name = fieldInfo.Name;
                suc++;

                return string.Format("<color=green>{0} \t----->\t {1}</color>", GetObjUrl(gameObj.transform, name),
                    fieldInfo.Name);
            }
            else
            {
                fail++;

                return string.Format("{0}\t\t\t\t\tSkip", fieldInfo.Name);
            }
        }

        private string BindFieldWithComponent(FieldInfo field, ref int suc, ref int fail)
        {
            var c = FindComponentInObj(field.Name, field.FieldType, showError: false);
            if (c != null)
            {
                field.SetValue(this, c);
                suc++;

                Transform tf = null;

                if (c is GameObject)
                    tf = (c as GameObject).transform;
                else if (c is Transform)
                    tf = (c as Transform);
                else if (c is Component)
                    tf = (c as Component).transform;

                return string.Format("<color=green>{0} \t----->\t {1}</color>", field.Name, GetObjUrl(tf, name));
            }
            else
            {
                fail++;

                return string.Format("{0}\t\t\t\t\tSkip", field.Name);
            }
        }

        public static string GetObjUrl(Transform tf, string rootName = "")
        {
            if (tf == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Insert(0, tf.name);

            Transform p = tf.parent;
            while (p != null)
            {
                sb.Insert(0, p.name + "/");
                p = p.parent;

                if (p == null)
                    break;

                if (p.name == rootName)
                {
                    sb.Insert(0, p.name + "/");
                    break;
                }
            }

            return sb.ToString();
        }

#endif
    }
}