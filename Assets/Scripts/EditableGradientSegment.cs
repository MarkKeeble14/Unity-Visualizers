using UnityEngine;
using UnityEngine.UI;

public class EditableGradientSegment : GradientSegment
{
    public void ListSegmentClicked(Transform clicked)
    {
        float percentPos = (float)clicked.GetSiblingIndex() / transform.parent.childCount;
        GradientEditor._Instance.AddKey(percentPos);
    }
}