using UnityEngine;

public class OffsetAnchoredPositionByHeightFollowup : AttachParameterFollowup
{
    [SerializeField] private RectTransform myRect;
    [SerializeField] private RectTransform matchRect;
    [SerializeField] private float matchingFactor = 1f;
    private Vector2 v = Vector2.zero;

    protected override void Followup(float v)
    {
        this.v.y = matchRect.sizeDelta.y * matchingFactor;
        myRect.anchoredPosition = this.v;
    }
}
