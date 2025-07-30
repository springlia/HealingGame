using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private FishData fishData;
    public FishData FishData { set { fishData = value; } }
    public void WatchFishInfo()
    {
        Debug.Log("이름 : " + fishData.FishName);
        Debug.Log("난이도 : " + fishData.Difficulty);
        Debug.Log("가격 : " + fishData.Price);
        Debug.Log("최소 사이즈 : " + fishData.MinSize);
        Debug.Log("최대 사이즈 : " + fishData.MaxSize);
    }
}
