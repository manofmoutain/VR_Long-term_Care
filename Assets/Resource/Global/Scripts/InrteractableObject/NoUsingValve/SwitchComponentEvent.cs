using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponentEvent : MonoBehaviour
    {
        public bool isRemoveKeyword;

        /// <summary>
        /// 是否在打開物件時啟用事件
        /// </summary>
        public bool isUsingOnEnableEvent;

        /// <summary>
        /// 打開物件時要啟用的事件
        /// </summary>
        [SerializeField] private UnityEvent onEnableEvent;


        [SerializeField] private GameObject touchedGameObject;

        /// <summary>
        /// 是否打開trigger事件(Editor用)
        /// </summary>
        public bool isUsingTriggerEvent;

        [Tooltip("trigger的名稱")] public List<string> triggerName;
        [Tooltip("trigger的物件")] public List<GameObject> triggerOBJs;
        [SerializeField] private UnityEvent triggerEvent;

        /// <summary>
        /// 是否打開碰撞事件(Editor用)
        /// </summary>
        public bool isUsingCollisionEvent;

        [Tooltip("collision的名稱")] public List<string> collisionName;
        [Tooltip("collision的物件")] public List<GameObject> collisionOBJs;
        [SerializeField] private UnityEvent collisionEvent;


        private void OnEnable()
        {
            onEnableEvent.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            touchedGameObject = other.gameObject;
            if (triggerOBJs.Count > 0)
            {
                foreach (var go in triggerOBJs)
                {
                    if (other.gameObject == go)
                    {
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }

                        triggerEvent.Invoke();
                        return;
                    }
                }
            }
            else if (triggerOBJs.Count == 0 && triggerName.Count > 0)
            {
                foreach (var s in triggerName)
                {
                    if (other.gameObject.name == s)
                    {
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }

                        triggerEvent.Invoke();
                        return;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            touchedGameObject = null;
        }

        private void OnCollisionEnter(Collision other)
        {
            touchedGameObject = other.gameObject;
            if (collisionOBJs.Count > 0)
            {
                foreach (var go in collisionOBJs)
                {
                    if (other.gameObject == go)
                    {
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }

                        collisionEvent.Invoke();
                        return;
                    }
                }
            }
            else if (collisionOBJs.Count == 0 && collisionName.Count > 0)
            {
                foreach (var s in collisionName)
                {
                    if (other.gameObject.name == s)
                    {
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }

                        collisionEvent.Invoke();
                        return;
                    }
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            touchedGameObject = null;
        }

        void RemoveKeywords()
        {
            for (int i = 0; i < triggerOBJs.Count; i++)
            {
                if (touchedGameObject == triggerOBJs[i])
                {
                    triggerName.Remove(touchedGameObject.name);
                    triggerOBJs.Remove(touchedGameObject);
                }
            }

            for (int i = 0; i < triggerName.Count; i++)
            {
                if (touchedGameObject.name == triggerName[i])
                {
                    triggerName.Remove(touchedGameObject.name);
                }
            }

            for (int i = 0; i < collisionOBJs.Count; i++)
            {
                if (touchedGameObject == collisionOBJs[i])
                {
                    collisionName.Remove(touchedGameObject.name);
                    collisionOBJs.Remove(touchedGameObject);
                }
            }

            for (int i = 0; i < collisionName.Count; i++)
            {
                if (touchedGameObject.name == collisionName[i])
                {
                    collisionName.Remove(touchedGameObject.name);
                }
            }
        }
    }
}