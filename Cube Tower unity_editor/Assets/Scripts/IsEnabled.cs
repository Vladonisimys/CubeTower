using UnityEngine;

public class IsEnabled : MonoBehaviour
{
    public int needToUnlock;
    public Material closedMaterial;

    public void Start()
    {
        if (PlayerPrefs.GetInt("score") < needToUnlock)
            GetComponent<MeshRenderer>().material = closedMaterial;
    }
}
