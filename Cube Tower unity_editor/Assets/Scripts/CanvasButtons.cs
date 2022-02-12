using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    public Sprite musicOn, musicOff;

    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name == "Music")
            GetComponent<Image>().sprite = musicOff;
    }
    public void RestartGame()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadShop()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("Shop");
    }

    public void CloseShop()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("Main");
    }

    public void Share()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();
        Application.OpenURL("https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLUhqbF92MjlQa1dzSHVoeUluRmlndUpIczVBSWhEQXxBQ3Jtc0tuZml2MlM4bVNra096MGJiZXFqY2oyc01KUExzdG5RS0dtTUQwaGhEUWx3THZPblFsNTR6ejR2VXJFSFZnYWoxNzU1aGU5UVdjZzA4T181alpiZUowQzFyYjhqWFZvcDV6NEc2VGlOdUVzUVNIbEFnUQ&q=https%3A%2F%2Fitproger.com%2Fcourse%2Funity-gamedev%2F5");
    }

    public void MusicWork()
    {
        if( PlayerPrefs.GetString("music") == "No")
        {
            GetComponent<AudioSource>().Play();
            PlayerPrefs.SetString("music", "Yes");
            GetComponent<Image>().sprite = musicOn;
        }
        else
        {
            PlayerPrefs.SetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
        }
    }
}
