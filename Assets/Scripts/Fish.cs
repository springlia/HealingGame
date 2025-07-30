using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private FishData fishData;
    public FishData FishData { set { fishData = value; } }
    public void WatchFishInfo()
    {
        Debug.Log("�̸� : " + fishData.FishName);
        Debug.Log("���̵� : " + fishData.Difficulty);
        Debug.Log("���� : " + fishData.Price);
        Debug.Log("�ּ� ������ : " + fishData.MinSize);
        Debug.Log("�ִ� ������ : " + fishData.MaxSize);
    }
}
