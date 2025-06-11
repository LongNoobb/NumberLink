using UnityEngine;

public class WinUI : MonoBehaviour
{
    public static WinUI instance;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    public void NextLevel()
    {

    }
}
