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
            // ���� �����Ͱ� ���� ���: �̾��ϱ� + ���� ����
            newGameButton.gameObject.SetActive(true);
            startButtonText.text = "���� �̾��ϱ�";
        }
        else
        {
            // ���� ������ ������: ���� �����ϱ⸸
            newGameButton.gameObject.SetActive(false);
            startButtonText.text = "���� �����ϱ�";
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll(); // ����� ������ �ʱ�ȭ
        SceneManager.LoadScene("Game");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
