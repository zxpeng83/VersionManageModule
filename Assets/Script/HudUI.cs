using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    public GameObject dropDown;
    // Start is called before the first frame update
    void Start()
    {
        this.dropDown.GetComponent<TMP_Dropdown>().onValueChanged.RemoveAllListeners();
        this.dropDown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(this.onDropdownSelect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onDropdownSelect(int idx)
    {
        Debug.Log(idx);

        switch (idx)
        {
            
        }
    }
}
