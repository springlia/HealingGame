using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public string name;
    public int difficulty;
    public float price;
}

public class GameManager : MonoBehaviour
{
    //�̱���
    public static GameManager Instance { get; private set; }

    //�����
    public List<Fish> fishs = new List<Fish>();

    //UI
    [SerializeField] GameObject inv;
    [SerializeField] GameObject fishGameUI;
    [SerializeField] TextMeshProUGUI logText;

    private void Awake()
    {
        Instance = this;

        AddFish();
    }

    void AddFish()
    {
        fishs.Add(new Fish { name = "����", difficulty = 2, price = 13.0f });
        fishs.Add(new Fish { name = "����", difficulty = 2, price = 15.0f });
        fishs.Add(new Fish { name = "����", difficulty = 3, price = 30.0f });
        fishs.Add(new Fish { name = "��ġ", difficulty = 2, price = 15.0f });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 9.0f });
        fishs.Add(new Fish { name = "�ػ�", difficulty = 3, price = 35.0f });
        fishs.Add(new Fish { name = "û��", difficulty = 2, price = 20.0f });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 10.0f });
        fishs.Add(new Fish { name = "���", difficulty = 3, price = 35.0f });
        fishs.Add(new Fish { name = "����", difficulty = 4, price = 30.0f });
        fishs.Add(new Fish { name = "���� ����", difficulty = 1, price = 9.0f });
        fishs.Add(new Fish { name = "��¡��", difficulty = 4, price = 30.0f });
        fishs.Add(new Fish { name = "��ġ", difficulty = 1, price = 6.0f });
        fishs.Add(new Fish { name = "���", difficulty = 1, price = 8.0f });
        fishs.Add(new Fish { name = "�ٴ尡��", difficulty = 1, price = 5.0f });
        fishs.Add(new Fish { name = "�����ٶ���", difficulty = 1, price = 10.0f });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 3.0f });
        fishs.Add(new Fish { name = "������", difficulty = 1, price = 0.0f });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 1.0f });
        fishs.Add(new Fish { name = "������ �����", difficulty = 5, price = 50.0f });
    }
    public Fish GetRandomFish()
    {
        return fishs[Random.Range(0, fishs.Count)];
    }

    public void StartFishGame()
    {
        fishGameUI.SetActive(true);
    }
    public void StopFishGame()
    {
        fishGameUI.SetActive(false);
    }

    public void ClickBagButton()
    {
        inv.SetActive(true);
    }

    public void ClickBagExitButton()
    {
        inv.SetActive(false);
    }

    public void WriteLog(string Log)
    {
        logText.text = Log;
        StartCoroutine(RemoveLog());
    }

    IEnumerator RemoveLog()
    {
        yield return new WaitForSeconds(3);
        logText.text = "";
    }


}
