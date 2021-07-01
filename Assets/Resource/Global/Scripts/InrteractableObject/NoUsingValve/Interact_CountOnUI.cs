using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InteractableObject
{
    public class Interact_CountOnUI : MonoBehaviour
    {
        [SerializeField] Interact_Count count;
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
            countText.text = $"{hint}:{count.MaxCount}";
        }
    }
}

