using System;
using TMPro;
using UnityEngine;

namespace Firebase
{
    public class PlayerDataItemUI : MonoBehaviour
    {

        [field: SerializeField] public string DataText { get; set; }

        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = DataText;
        }
    }
}