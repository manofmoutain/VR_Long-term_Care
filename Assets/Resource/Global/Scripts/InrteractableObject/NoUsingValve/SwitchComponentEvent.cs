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
            if (triggerOBJs.Count > 0)
            {
                foreach (var go in triggerOBJs)
                {
                    if (other.gameObject == go)
                    {
                        touchedGameObject = other.gameObject;
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }
                        triggerEvent.Invoke();
                        break;
                    }
                }
            }
            else if (triggerOBJs.Count == 0 && triggerName.Count > 0)
            {
                foreach (var s in triggerName)
                {
                    if (other.gameObject.name == s)
                    {
                        touchedGameObject = other.gameObject;
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }
                        triggerEvent.Invoke();
                        break;
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
            if (collisionOBJs.Count > 0)
            {
                foreach (var go in collisionOBJs)
                {
                    if (other.gameObject == go)
                    {
                        touchedGameObject = other.gameObject;
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }
                        collisionEvent.Invoke();
                        break;
                    }
                }
            }
            else if (collisionOBJs.Count == 0 && collisionName.Count > 0)
            {
                foreach (var s in collisionName)
                {
                    if (other.gameObject.name == s)
                    {
                        touchedGameObject = other.gameObject;
                        if (isRemoveKeyword)
                        {
                            RemoveKeywords();
                        }
                        collisionEvent.Invoke();
                        break;
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
            foreach (GameObject go in triggerOBJs)
            {
                if (touchedGameObject != go) continue;
                triggerName.Remove(touchedGameObject.name);
                triggerOBJs.Remove(touchedGameObject);
                return;
            }

            foreach (string s in triggerName)
            {
                if (touchedGameObject.name != s) continue;
                triggerName.Remove(touchedGameObject.name);
                return;
            }

            foreach (var go in collisionOBJs)
            {
                if (touchedGameObject == go) continue;
                collisionName.Remove(touchedGameObject.name);
                collisionOBJs.Remove(touchedGameObject);
                return;
            }
            foreach (var s in collisionName)
            {
                if (touchedGameObject.name == s) continue;
                collisionName.Remove(touchedGameObject.name);
                return;
            }
        }
    }
}