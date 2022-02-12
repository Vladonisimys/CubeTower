using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class Ads : MonoBehaviour
{
    private Coroutine showAd;

    private string gameId = "4607727", type = "video";
    private bool testMode = true, needToStop;

    private static int countLoses;

    private void Start()
    {
        Advertisement.Initialize(gameId, testMode);

        countLoses++;
        if(countLoses%3 == 0)
            showAd = StartCoroutine(ShowAd());        
    }

    public void Update()
    {
        if(needToStop)
        {
            needToStop = false;
            StopCoroutine(ShowAd());
        }
    }

    IEnumerator ShowAd()
    {
        while(true)
        {            
                Advertisement.Show(type);
                needToStop = true;
            
            yield return new WaitForSeconds(1f);
        }
    }
}
