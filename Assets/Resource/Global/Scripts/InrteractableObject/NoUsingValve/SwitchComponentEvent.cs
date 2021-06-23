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
        [SerializeField] private GameObject touchedGameObject;
        public bool isUsingTriggerEvent;
        [Tooltip("trigger的名稱")]public List<string> triggerName;
        public bool isUsingCollisionEvent;
        [Tooltip("collision的名稱")]public List<string> collisionName;

        public bool collapseTriggerEvent;

        [SerializeField] private UnityEvent triggerEvent;

        public bool collapseCollisionEvent;

        [SerializeField] private UnityEvent collisionEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerName.Count>0)
            {
                foreach (var s in triggerName)
                {
                    if (other.name==s)
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
            if (collisionName.Count>0)
            {
                foreach (var s in collisionName)
                {
                    if (other.gameObject.name==s)
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
            for (var i = 0; i < triggerName.Count; i++)
            {
                if (touchedGameObject.name!=triggerName[i]) continue;
                triggerName.Remove(touchedGameObject.name);
                print($"從{triggerName}刪除了{touchedGameObject.name}");
                break;
            }

            foreach (var VARIABLE in collisionName)
            {
                if (touchedGameObject.name == VARIABLE)
                {
                    collisionName.Remove(touchedGameObject.name);
                    print($"從{collisionName}刪除了{touchedGameObject.name}");
                    return;
                }
            }
        }
    }
}

