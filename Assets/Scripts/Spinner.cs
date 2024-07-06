using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spinner : MonoBehaviour
{
    [SerializeField] private Image spinner;
    [SerializeField] private Image mask;
    [SerializeField] private float spinSpeed;
    [SerializeField] private MathHelper.AlterationMethod method;

    [SerializeField] private float delayBetweenSpinnerAndMask = .1f;
    [SerializeField] private float delayAfterMask = .5f;

    private void Start()
    {
        StartCoroutine(SpinLoop());
    }

    private IEnumerator SpinLoop()
    {
        while (spinner.fillAmount < 1)
        {
            spinner.fillAmount = MathHelper.GetNextValue(spinner.fillAmount, 1, spinSpeed, method, true);

            yield return null;
        }

        yield return new WaitForSeconds(delayBetweenSpinnerAndMask);

        while (mask.fillAmount > 0)
        {
            mask.fillAmount = MathHelper.GetNextValue(mask.fillAmount, 0, spinSpeed, method, true);

            yield return null;
        }

        yield return new WaitForSeconds(delayAfterMask);

        spinner.fillAmount = 0;
        mask.fillAmount = 1;

        StartCoroutine(SpinLoop());
    }
}
