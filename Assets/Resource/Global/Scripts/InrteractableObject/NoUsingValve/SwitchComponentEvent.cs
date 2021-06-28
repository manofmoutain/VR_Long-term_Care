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
        /// <summary>
        /// 是否在打開物件時啟用事件
        /// </summary>
        public bool isUsingOnEnableEvent;

        /// <summary>
        /// 打開物件時要啟用的事件
        /// </summary>
        [SerializeField] private UnityEvent onEnableEvent;


        [SerializeField] private GameObject touchedGameObject;
        public bool isUsingTriggerEvent;
        [Tooltip("trigger的名稱")] public List<string> triggerName;
        public List<GameObject> triggerOBJs;
        public bool isUsingCollisionEvent;
        [Tooltip("collision的名稱")] public List<string> collisionName;
        public List<GameObject> collisionOBJs;

        public bool collapseTriggerEvent;

        [SerializeField] private UnityEvent triggerEvent;

        public bool collapseCollisionEvent;

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
                    if (other == go)
                    {
                        touchedGameObject = other.gameObject;
                        triggerEvent.Invoke();
                        break;
                    }
                }
            }
            else if (triggerOBJs.Count == 0 && triggerName.Count > 0)
            {
                foreach (var s in triggerName)
                {
                    if (other.name == s)
                    {
                        touchedGameObject = other.gameObject;
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

        public void RemoveKeywords()
        {
            for (var i = 0; i < triggerOBJs.Count; i++)
            {
                if (touchedGameObject != triggerOBJs[i]) continue;
                triggerOBJs.Remove(touchedGameObject);
                return;
            }

            for (var i = 0; i < triggerName.Count; i++)
            {
                if (touchedGameObject.name != triggerName[i]) continue;
                triggerName.Remove(touchedGameObject.name);
                return;
            }

            foreach (var go in collisionOBJs)
            {
                if (touchedGameObject == go) continue;
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