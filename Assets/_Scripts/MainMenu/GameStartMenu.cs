using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public Button startButton;

    void Start()
    {
        mainMenu.SetActive(true);
        startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }
}
