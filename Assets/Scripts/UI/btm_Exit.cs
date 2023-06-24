using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class btm_Exit : BasePanel
{
    private Button button;

    public override void OpenPanel()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NetWorker.Instance.OnExitGame);
    }

    public override void ClosePanel()
    {
        throw new NotImplementedException();
    }
}
