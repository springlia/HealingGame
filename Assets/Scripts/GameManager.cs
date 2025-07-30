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
    //�̱���
    public static GameManager Instance { get; private set; }

    //�����
    public List<Fish> fishs = new List<Fish>();

    //�κ��丮
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
    //    fishs.Add(new Fish { name = "����", difficulty = 2, price = 13.0f, minSize = 60, maxSize = 168 });
    //    fishs.Add(new Fish { name = "����", difficulty = 2, price = 15.0f, minSize = 60, maxSize = 168 });
    //    fishs.Add(new Fish { name = "����", difficulty = 3, price = 30.0f, minSize = 3, maxSize = 94 });
    //    fishs.Add(new Fish { name = "��ġ", difficulty = 2, price = 15.0f, minSize = 30, maxSize = 155 });
    //    fishs.Add(new Fish { name = "����", difficulty = 1, price = 9.0f, minSize = 30, maxSize = 79 });
    //    fishs.Add(new Fish { name = "�ػ�", difficulty = 3, price = 35.0f, minSize = 8, maxSize = 53 });
    //    fishs.Add(new Fish { name = "û��", difficulty = 2, price = 20.0f, minSize = 20, maxSize = 53 });
    //    fishs.Add(new Fish { name = "����", difficulty = 1, price = 10.0f, minSize = 20, maxSize = 58 });
    //    fishs.Add(new Fish { name = "���", difficulty = 3, price = 35.0f, minSize = 30, maxSize = 206 });
    //    fishs.Add(new Fish { name = "����", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124 });
    //    fishs.Add(new Fish { name = "���� ����", difficulty = 1, price = 9.0f, minSize = 20, maxSize = 66 });
    //    fishs.Add(new Fish { name = "��¡��", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124 });
    //    fishs.Add(new Fish { name = "��ġ", difficulty = 1, price = 6.0f, minSize = 3, maxSize = 43 });
    //    fishs.Add(new Fish { name = "���", difficulty = 1, price = 8.0f, minSize = 3, maxSize = 33 });
    //    fishs.Add(new Fish { name = "�ٴ尡��", difficulty = 1, price = 5.0f, minSize = 3, maxSize = 20 });
    //    fishs.Add(new Fish { name = "�����ٶ���", difficulty = 1, price = 10.0f, minSize = 51, maxSize = 104 });
    //    fishs.Add(new Fish { name = "����", difficulty = 1, price = 3.0f, minSize = 2, maxSize = 5 });
    //    fishs.Add(new Fish { name = "������", difficulty = 1, price = 0.0f, minSize = 1, maxSize = 5 });
    //    fishs.Add(new Fish { name = "����", difficulty = 1, price = 1.0f, minSize = 1, maxSize = 5 });
    //    fishs.Add(new Fish { name = "������ �����", difficulty = 5, price = 50.0f, minSize = 10, maxSize = 50 });
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
