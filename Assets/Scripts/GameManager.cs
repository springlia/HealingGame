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
    //싱글톤
    public static GameManager Instance { get; private set; }

    //물고기
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
        fishs.Add(new Fish { name = "고등어", difficulty = 2, price = 13.0f });
        fishs.Add(new Fish { name = "연어", difficulty = 2, price = 15.0f });
        fishs.Add(new Fish { name = "복어", difficulty = 3, price = 30.0f });
        fishs.Add(new Fish { name = "참치", difficulty = 2, price = 15.0f });
        fishs.Add(new Fish { name = "도미", difficulty = 1, price = 9.0f });
        fishs.Add(new Fish { name = "해삼", difficulty = 3, price = 35.0f });
        fishs.Add(new Fish { name = "청어", difficulty = 2, price = 20.0f });
        fishs.Add(new Fish { name = "숭어", difficulty = 1, price = 10.0f });
        fishs.Add(new Fish { name = "장어", difficulty = 3, price = 35.0f });
        fishs.Add(new Fish { name = "문어", difficulty = 4, price = 30.0f });
        fishs.Add(new Fish { name = "붉은 퉁돔", difficulty = 1, price = 9.0f });
        fishs.Add(new Fish { name = "오징어", difficulty = 4, price = 30.0f });
        fishs.Add(new Fish { name = "멸치", difficulty = 1, price = 6.0f });
        fishs.Add(new Fish { name = "정어리", difficulty = 1, price = 8.0f });
        fishs.Add(new Fish { name = "바닷가재", difficulty = 1, price = 5.0f });
        fishs.Add(new Fish { name = "날개다랑어", difficulty = 1, price = 10.0f });
        fishs.Add(new Fish { name = "조개", difficulty = 1, price = 3.0f });
        fishs.Add(new Fish { name = "쓰레기", difficulty = 1, price = 0.0f });
        fishs.Add(new Fish { name = "해초", difficulty = 1, price = 1.0f });
        fishs.Add(new Fish { name = "전설의 물고기", difficulty = 5, price = 50.0f });
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
