using System;
using System.Collections.Generic;
using UnityEngine;

public class UIKeeper : MonoBehaviour
{
    private Dictionary<string, string> nameToPath;
    private Dictionary<string, BasePanel> nameToPanel;

    private static UIKeeper uIKeeper;
    public static UIKeeper Instance
    {
        get
        {
            if (uIKeeper == null)
            {
                uIKeeper = new UIKeeper();
                Logger.Log("New a UIKeeper");
            }
            return uIKeeper;
        }
    }

    private Transform _root;

    public Transform UIRoot
    {
        get
        {
            if (_root == null)
            {
                _root = GameObject.Find("Canvas").transform;
            }
            return _root;
        }
    }

    private void Awake()
    {
        uIKeeper = this;
    }

    private UIKeeper()
    {
        Init();
    }

    private void Init()
    {
        nameToPanel = new Dictionary<string, BasePanel>();

        nameToPath = new Dictionary<string, string>
        {
            {"btm_ChangeSpeaker", "UI/btm_ChangeSpeaker" },
            {"btm_Exit", "UI/btm_Exit" },
            {"btm_Send","UI/btm_Send" },
            {"chatWindow", "UI/chatWindow" },
            {"Speakers", "UI/Speakers" }
        };
    }

    /// <summary>
    /// 打开一个UI，并返回。可能返回null
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BasePanel OpenPanel(string name)
    {
        if (nameToPanel.ContainsKey(name))
        {
            Logger.Log("UI " + name + "已存在");
            return null;
        }

        string path = "";
        if (!nameToPath.TryGetValue(name, out path))
        {
            Logger.Log("UI " + name + "不存在");
            return null;
        }

        GameObject targetUI = Resources.Load<GameObject>(path);
        targetUI.name = name;

        GameObject panelObj = Instantiate(targetUI, UIRoot, false);
        if (panelObj == targetUI)
        {
            Debug.Log("Same");
        }

        BasePanel uiPanel = panelObj.transform.GetComponent<BasePanel>();
        nameToPanel.Add(name, uiPanel);

        return uiPanel;
    }

    public bool ClosePanel(string name)
    {
        BasePanel basePanel = null;
        if (!nameToPanel.TryGetValue(name, out basePanel))
        {
            return false; 
        }
        basePanel.OpenPanel();
        return true;
    }
}
