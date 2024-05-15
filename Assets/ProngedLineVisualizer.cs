using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class ProngedLineVisualizer : MonoBehaviour
{
    [SerializeField] private bool updateComponents;
    [SerializeField] private int size = 1;
    [SerializeField] private GameObject linePrefab;

    [Header("Settings")]
    [SerializeField] private float multiplier = 25;
    [SerializeField] private float defaultHeight = 0;
    [SerializeField] private float adjustSpeed = 25;
    [SerializeField] private float spacing = 0;
    [SerializeField] private bool useSlowAdjust;

    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private List<AttachHeightToBand> attachHeightToBandList = new List<AttachHeightToBand>();

    private void Awake()
    {
        layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (size <= 0) size = 1;
        MakeLineVisualizer();
    }

    private void Update()
    {
        layoutGroup.spacing = spacing;

        if (updateComponents)
        {
            foreach (AttachHeightToBand htb in attachHeightToBandList)
            {
                htb.Set(multiplier, defaultHeight, adjustSpeed);
                htb.SlowAdjust = useSlowAdjust;
            }
            updateComponents = false;
        }
    }

    private void MakeLineVisualizer()
    {
        int currentBand = 0;
        int count = 0;
        for (int i = 0; i < size * 8; i++)
        {
            GameObject spawned = Instantiate(linePrefab, transform);
            AttachHeightToBand htb = spawned.AddComponent<AttachHeightToBand>();
            attachHeightToBandList.Add(htb);
            htb.Set(currentBand, multiplier, defaultHeight, adjustSpeed);
            htb.SlowAdjust = useSlowAdjust;

            htb.GetComponent<Image>().color =  VisualizerManager._Instance.UserDefinedGradient.Evaluate(currentBand / 8f);

            count++;
            if (count >= size)
            {
                count = 0;
                currentBand++;
            }
        }
    }
}
