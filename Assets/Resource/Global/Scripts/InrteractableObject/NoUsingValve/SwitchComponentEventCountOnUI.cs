using System;
using UnityEngine;
using TMPro;

namespace InteractableObject
{
    public class SwitchComponentEventCount : MonoBehaviour
    {
        [SerializeField] SwitchComponentEvent switchEvent;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private string hint;

        private void Start()
        {
            if (countText==null)
            {
                countText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            int count = switchEvent.triggerOBJs.Count + switchEvent.collisionOBJs.Count;
            countText.text = $"{hint}:{count}";
        }
    }
}

