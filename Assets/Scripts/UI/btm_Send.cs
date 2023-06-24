using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

class btm_Send : BasePanel
{
    Button button;

    public override void OpenPanel()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NetWorker.Instance.Send);
    }

    public override void ClosePanel()
    {
        throw new NotImplementedException();
    }
}

