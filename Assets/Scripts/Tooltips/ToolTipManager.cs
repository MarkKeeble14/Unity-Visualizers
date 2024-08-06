using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    [SerializeField] private ToolTip toolTipPrefab;
    [SerializeField] private float yOffset;

    public static ToolTipManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    public ToolTip RequestToolTip(RectTransform spawningFor, RectTransform container, string text)
    {
        ToolTip spawned = Instantiate(toolTipPrefab, spawningFor);
        RectTransform spawnedRect = spawned.TipContainer;

        Vector2 spawnedPos = spawnedRect.anchoredPosition;
        spawnedPos.y += container.sizeDelta.y / 2 + spawnedRect.sizeDelta.y / 2 + yOffset;
        spawnedRect.anchoredPosition = spawnedPos;

        spawned.transform.position = spawningFor.position;
        spawned.SetText(text);

        return spawned;
    }
}
