using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    public static HudUI instance;

    public GameObject dropDown;
    public GameObject resetBtn;
    public GameObject curVersionTxt;

    private void Awake()
    {
        HudUI.instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.dropDown.GetComponent<TMP_Dropdown>().onValueChanged.RemoveAllListeners();
        this.dropDown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(this.onDropdownSelect);

        ///重置按钮
        this.resetBtn.GetComponent<Button>().onClick.AddListener(this.reset);
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void reset()
    {
        GameControllMgr.instance.reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 刷新ui信息
    /// </summary>
    public void freshUI()
    {
        int versionIdx = PersistentSegmentTree2D.instance.getVersionIdx();
        int showVersionIdx = PersistentSegmentTree2D.instance.getshowVersionIdx();

        this.dropDown.GetComponent<TMP_Dropdown>().options.Clear();
        for(int i=0;i<= versionIdx;i++)
        {
            this.dropDown.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData("Version:" + i));
        }

        this.dropDown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(showVersionIdx);
        this.dropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();

        this.curVersionTxt.GetComponent<TMP_Text>().text = "Version:" + showVersionIdx;
    }

    /// <summary>
    /// 转跳至指定版本
    /// </summary>
    /// <param name="idx"></param>
    public void onDropdownSelect(int idx)
    {
        GameControllMgr.instance.jump2Version(idx);
    }
}
