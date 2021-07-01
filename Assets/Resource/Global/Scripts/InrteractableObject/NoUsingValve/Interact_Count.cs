using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class Interact_Count : MonoBehaviour
    {
        /// <summary>
        /// 目前觸碰次數
        /// </summary>
        [SerializeField] int count;
        public int Count => count;

        /// <summary>
        /// 最大觸碰次數
        /// </summary>
        [SerializeField] private int maxCount;
        public int MaxCount => maxCount;

        /// <summary>
        /// 達到指定數值後觸發事件
        /// </summary>
        [SerializeField] private UnityEvent onMaxCountEvent;

        /// <summary>
        /// 達到指定次數後再次觸碰後觸發的事件
        /// </summary>
        [SerializeField] private UnityEvent overCountEvent;


        public void ResetCount()
        {
            count = 0;
        }

        public void AddCount()
        {
            count++;
            if (count == maxCount)
            {
                onMaxCountEvent.Invoke();
            }
            else if (count>maxCount)
            {
                overCountEvent.Invoke();
            }
        }

        public void ChangeMaxCount(int value)
        {
            maxCount = value;
        }
    }
}