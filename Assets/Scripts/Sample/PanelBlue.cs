using UnityEngine.UI;
using SimpleUI;
using UnityEngine;

public class PanelBlue : CommonPanel
{
    [SerializeField] Text title = null;
    [SerializeField] Text text = null;

    public override bool PreventAction
    {
        get
        {
            return true;
        }
    }

    private string data = null;
    public override void SetData<DataType>(DataType data)
    {
        if (data is string)
        {
            this.data = data as string;
        }
    }

    protected override void OnShowOver()
    {
        title.text = "I'm Blue";
        text.text = data;
    }
}
