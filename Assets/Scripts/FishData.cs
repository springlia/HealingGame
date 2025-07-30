using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish Data", menuName = "Scriptable Object/Fish Data", order = int.MaxValue)]
public class FishData : ScriptableObject
{
    [SerializeField]
    private string fishName;
    public string FishName { get { return fishName; } }

    [SerializeField] private int difficulty;
    public int Difficulty { get { return difficulty; } }

    [SerializeField] private float price;
    public float Price { get { return price; } }

    [SerializeField] private float minSize;
    public float MinSize { get { return minSize; } }

    [SerializeField] private float maxSize;
    public float MaxSize { get { return maxSize; } }

}
