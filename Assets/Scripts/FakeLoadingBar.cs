using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FakeLoadingBar : MonoBehaviour
{
    [SerializeField] private Image fillFromLeft;
    [SerializeField] private Image fillFromRight;
    [SerializeField] private float increment;
    [SerializeField] private float delayBetweenMoves;

    [SerializeField] private float startFillFromLeftAt = 0;
    [SerializeField] private float startFillFromRightAt = 0.8f;

    private void Start()
    {
        StartCoroutine(Cycle());
    }

    private IEnumerator Cycle()
    {
        fillFromLeft.fillAmount = startFillFromLeftAt;
        fillFromRight.fillAmount = startFillFromRightAt;

        while (fillFromLeft.fillAmount < 1)
        {
            fillFromLeft.fillAmount += increment;
            fillFromRight.fillAmount -= increment;

            yield return new WaitForSeconds(delayBetweenMoves);
        }

        fillFromLeft.fillAmount = startFillFromLeftAt;
        fillFromRight.fillAmount = startFillFromRightAt;

        yield return new WaitForSeconds(delayBetweenMoves);

        StartCoroutine(Cycle());
    }
}
