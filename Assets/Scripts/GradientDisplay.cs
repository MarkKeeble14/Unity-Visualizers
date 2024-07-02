using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientDisplay : MonoBehaviour
{
    [SerializeField] private GradientSegment segmentPrefab;
    [SerializeField] private Transform segmentList;
    [SerializeField] private GradientKeyDisplay keyPrefab;
    [SerializeField] private Transform keyList;
    [SerializeField] private int segments = 128;

    private bool hasConstructed;

    private Gradient displayedGradient;

    public void DestroyKeys()
    {
        foreach (Transform child in keyList)
        {
            Destroy(child.gameObject);
        }

        keyList.gameObject.SetActive(false);
    }

    public void CreateKeys()
    {
        keyList.gameObject.SetActive(true);

        Dictionary<int, GradientColorKey> keyedIndexes = new();
        foreach (GradientColorKey key in displayedGradient.colorKeys)
        {
            keyedIndexes.Add(GetIndexFromTime(key.time), key);
        }

        int assignedKeys = 0;
        for (int i = 0; i < segments; ++i)
        {
            GradientKeyDisplay keyDisplay = Instantiate(keyPrefab, keyList);
            if (keyedIndexes.ContainsKey(i))
            {
                keyDisplay.Set(assignedKeys, keyedIndexes[i].color, GetTimeFromIndex(i));
                keyDisplay.SetVisible(true);
                assignedKeys++;
            }
        }
    }

    private int GetIndexFromTime(float time)
    {
        return Mathf.FloorToInt(time * (segments - 1));
    }

    private float GetTimeFromIndex(int index)
    {
        return (float)index / segments;
    }

    private void Construct()
    {
        for (int i = 0; i < segments; ++i)
        {
            Image image = Instantiate(segmentPrefab, segmentList);
            image.color = Color.white;
        }
        hasConstructed = true;
    }

    public void UpdateColors(Gradient g)
    {
        // if the display hasn't yet been constructed, do so
        if (!hasConstructed) Construct();

        // set the displayed gradient
        displayedGradient = g;

        // set the color of individual segments based on their child index
        for (int i = 0; i < segmentList.childCount; ++i)
        {
            segmentList.GetChild(i).GetComponent<Image>().color = displayedGradient.Evaluate((float)i / segments);
        }

        // if the key list is active, we also need to remake those
        if (keyList.gameObject.activeSelf)
        {
            RemakeKeys();
        }
    }

    public void RemakeKeys()
    {
        DestroyKeys();
        CreateKeys();
    }
}
