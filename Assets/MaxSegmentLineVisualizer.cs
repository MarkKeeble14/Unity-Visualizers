using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class MaxSegmentLineVisualizer : MonoBehaviour
{
    [SerializeField] private bool updateComponents;
    [SerializeField] private GameObject linePrefab;

    [Header("Settings")]
    [SerializeField] private float multiplier = 25;
    [SerializeField] private float defaultHeight = 0;
    [SerializeField] private float adjustSpeed = 25;
    [SerializeField] private float spacing = 0;
    [SerializeField] private bool useSlowAdjust;

    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private List<AttachHeightToSample> attachHeightToSampleList = new List<AttachHeightToSample>();

    private void Awake()
    {
        layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeLineVisualizer();
    }

    private void Update()
    {
        layoutGroup.spacing = spacing;

        if (updateComponents)
        {
            foreach (AttachHeightToSample hts in attachHeightToSampleList)
            {
                hts.Set(multiplier, defaultHeight, adjustSpeed);
                hts.SlowAdjust = useSlowAdjust;
            }
            updateComponents = false;
        }
    }

    private void MakeLineVisualizer()
    {
        for (int i = 0; i < 512; i++)
        {
            GameObject spawned = Instantiate(linePrefab, transform);
            AttachHeightToSample hts = spawned.AddComponent<AttachHeightToSample>();
            attachHeightToSampleList.Add(hts);
            hts.Set(i, multiplier, defaultHeight, adjustSpeed);
            hts.SlowAdjust = useSlowAdjust;

            hts.GetComponent<Image>().color = VisualizerManager._Instance.UserDefinedGradient.Evaluate(i / 512f);
        }
    }
}