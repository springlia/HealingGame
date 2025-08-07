using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public GameObject newGameButton;
    public TextMeshProUGUI startButtonText;

    private void Start()
    {
        if (PlayerPrefs.HasKey("hasStarted"))
        {
            // 저장 데이터가 있을 경우: 이어하기 + 새로 시작
            newGameButton.gameObject.SetActive(true);
            startButtonText.text = "게임 이어하기";
        }
        else
        {
            // 저장 데이터 없으면: 게임 시작하기만
            newGameButton.gameObject.SetActive(false);
            startButtonText.text = "게임 시작하기";
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll(); // 저장된 데이터 초기화
        SceneManager.LoadScene("Game");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
