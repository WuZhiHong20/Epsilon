using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

class btm_ChangeSpeaker : BasePanel
{
    private Button button;
    private Text showSpeaker;

    public override void OpenPanel()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChooseSpeaker);

        showSpeaker = GetComponentInChildren<Text>();
    }

    private void ChooseSpeaker()
    {
        UIKeeper.Instance.OpenPanel("Speakers");
    }

    public override void ClosePanel()
    {
        throw new NotImplementedException();
    }


}