using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//[System.Serializable]
//public class Fish 
//{
//    //public string name;
//    //public int difficulty;
//    //public float price;
//    //public float minSize;
//    //public float maxSize;


//}


// https://wergia.tistory.com/189

public class Inventory
{
    public string name;
    public int count;
}

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager Instance { get; private set; }

    //물고기
    public List<Fish> fishs = new List<Fish>();

    //인벤토리
    public List<Inventory> inventory = new List<Inventory>();

    //UI
    [SerializeField] GameObject invUI;
    [SerializeField] GameObject fishGameUI;
    [SerializeField] TextMeshProUGUI logText;


    private void Awake()
    {
        Instance = this;

        //AddFish();
    }

    void GetItem(string item, int count)
    {
        inventory.Add(new Inventory { name = item, count = count });
    }

    //void AddFish()
    //{
    //    fishs.Add(new Fish { name = "고등어", difficulty = 2, price = 13.0f, minSize = 60, maxSize = 168 });
    //    fishs.Add(new Fish { name = "연어", difficulty = 2, price = 15.0f, minSize = 60, maxSize = 168 });
    //    fishs.Add(new Fish { name = "복어", difficulty = 3, price = 30.0f, minSize = 3, maxSize = 94 });
    //    fishs.Add(new Fish { name = "참치", difficulty = 2, price = 15.0f, minSize = 30, maxSize = 155 });
    //    fishs.Add(new Fish { name = "도미", difficulty = 1, price = 9.0f, minSize = 30, maxSize = 79 });
    //    fishs.Add(new Fish { name = "해삼", difficulty = 3, price = 35.0f, minSize = 8, maxSize = 53 });
    //    fishs.Add(new Fish { name = "청어", difficulty = 2, price = 20.0f, minSize = 20, maxSize = 53 });
    //    fishs.Add(new Fish { name = "숭어", difficulty = 1, price = 10.0f, minSize = 20, maxSize = 58 });
    //    fishs.Add(new Fish { name = "장어", difficulty = 3, price = 35.0f, minSize = 30, maxSize = 206 });
    //    fishs.Add(new Fish { name = "문어", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124 });
    //    fishs.Add(new Fish { name = "붉은 퉁돔", difficulty = 1, price = 9.0f, minSize = 20, maxSize = 66 });
    //    fishs.Add(new Fish { name = "오징어", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124 });
    //    fishs.Add(new Fish { name = "멸치", difficulty = 1, price = 6.0f, minSize = 3, maxSize = 43 });
    //    fishs.Add(new Fish { name = "정어리", difficulty = 1, price = 8.0f, minSize = 3, maxSize = 33 });
    //    fishs.Add(new Fish { name = "바닷가재", difficulty = 1, price = 5.0f, minSize = 3, maxSize = 20 });
    //    fishs.Add(new Fish { name = "날개다랑어", difficulty = 1, price = 10.0f, minSize = 51, maxSize = 104 });
    //    fishs.Add(new Fish { name = "조개", difficulty = 1, price = 3.0f, minSize = 2, maxSize = 5 });
    //    fishs.Add(new Fish { name = "쓰레기", difficulty = 1, price = 0.0f, minSize = 1, maxSize = 5 });
    //    fishs.Add(new Fish { name = "해초", difficulty = 1, price = 1.0f, minSize = 1, maxSize = 5 });
    //    fishs.Add(new Fish { name = "전설의 물고기", difficulty = 5, price = 50.0f, minSize = 10, maxSize = 50 });
    //}
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
        invUI.SetActive(true);
    }

    public void ClickBagExitButton()
    {
        invUI.SetActive(false);
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
