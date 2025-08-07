using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Fish
{
    public string name;
    public int difficulty;
    public float price;
    public float minSize;
    public float maxSize;
    public string lore;
}

public class Crop
{
    public string name;
    public float price;
    public string lore;
}

[System.Serializable]
public class InventoryItem
{
    public Fish fishData;
    public Crop cropData;
    public int count;
}

public class GameManager : MonoBehaviour
{
    //�̱���
    public static GameManager Instance { get; private set; }

    //������
    public List<Fish> fishs = new List<Fish>();
    public List<Crop> crops = new List<Crop>();


    //���� ����
    public int fishRodLv = 1;
    const int Lv2UpPrice = 100;
    const int Lv3UpPrice = 300;
    [SerializeField] TextMeshProUGUI buyFishRodButtonText;

    //�κ��丮
    public List<InventoryItem> inventory = new List<InventoryItem>();
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject inventorySlotPrefab;
    private List<InventorySlot> slotUIs = new List<InventorySlot>();
    const int InventorySize = 36;

    public Image itemInfoImage;
    public TextMeshProUGUI itemInfoNameText;
    public TextMeshProUGUI itemInfoLoreText;

    //�ǻ�
    public int clothIndex = 0;
    [SerializeField] bool[] clothUnlock;
    const int clothPrice = 500;
    [SerializeField] Image[] cloths;

    //UI
    [SerializeField] GameObject invUI;
    [SerializeField] GameObject fishGameUI;
    [SerializeField] TextMeshProUGUI logText;
    public GameObject buyShopUI;
    [SerializeField] GameObject closetUI;
    public GameObject clothShopUI;

    [SerializeField] GameObject optionUI;

    //����
    public GameObject sellButton;
    private InventoryItem selectedItem = null;

    public float money = 0;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GameObject player;
    Player p;

    private void Awake()
    {
        Instance = this;

        p = player.GetComponent<Player>();
        AddFish();
        AddCrop();
    }

    private void Start()
    {
        for (int i = 0; i < InventorySize; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            slotUIs.Add(slotObj.GetComponent<InventorySlot>());
        }

        //���� 1�� ����
        Crop seed = null;
        foreach (Crop crop in GameManager.Instance.crops)
        {
            if (crop.name == "����")
            {
                seed = crop;
                break;
            }
        }
        AddToInventory(seed);
    }

    //�κ��丮 ������ �߰� (����)
    public void AddToInventory(Crop crop)
    {
        foreach (var item in inventory)
        {
            if (item.cropData != null && item.cropData.name == crop.name)
            {
                item.count++;
                UpdateInventoryUI();
                return;
            }
        }

        if (inventory.Count < InventorySize)
        {
            inventory.Add(new InventoryItem { cropData = crop, count = 1 });
            UpdateInventoryUI();
        }
        else
        {
            WriteLog("�κ��丮�� ���� á���ϴ�.");
        }
    }

    //�κ��丮 ������ �߰� (���÷�)
    public void AddToInventory(Fish fish)
    {
        // �̹� �ִ� �׸��̸� ������ ����
        foreach (var item in inventory)
        {
            if (item.fishData != null && item.fishData.name == fish.name)
            {
                item.count++;
                UpdateInventoryUI();
                return;
            }
        }
        // ���� �߰�
        if (inventory.Count < InventorySize)
        {
            inventory.Add(new InventoryItem { fishData = fish, count = 1 });
            UpdateInventoryUI();
        }
        else
        {
            WriteLog("�κ��丮�� ���� á���ϴ�.");
        }
    }
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < inventory.Count)
            {
                Sprite itemIcon = null;
                string name = "";

                if (inventory[i].fishData != null)
                {
                    name = inventory[i].fishData.name;
                }
                else if (inventory[i].cropData != null)
                {
                    name = inventory[i].cropData.name;
                }

                itemIcon = GetItemIcon(name);
                slotUIs[i].AddItem(itemIcon, inventory[i].count, inventory[i]);
            }
            else
            {
                slotUIs[i].RemoveItem();
            }
        }
    }
    public Sprite GetItemIcon(string name)
    {
        return Resources.Load<Sprite>($"Icons/{name}"); // Resources/Icons ������ ����
    }


    void AddCrop()
    {
        crops.Add(new Crop { name = "����", price = 5.0f, lore = "�̷��� ���� ������ ��� �ڶ󳯱��?" });
        crops.Add(new Crop { name = "�丶��", price = 35.0f, lore = "�����ϸ鼭�� ������ ���� ��ȭ�� �̷�� �α� ������ ä��." });
    }

    void AddFish()
    {
        fishs.Add(new Fish { name = "����", difficulty = 2, price = 13.0f, minSize = 60, maxSize = 168, lore = "�ٴٸ� ������ ���ġ�� ��ܹ� ����." });
        fishs.Add(new Fish { name = "����", difficulty = 2, price = 15.0f, minSize = 60, maxSize = 168, lore = "������ �ٴٷ�, �ٽ� ������ ���ƿ��� ������ ������ ���ΰ�." });
        fishs.Add(new Fish { name = "����", difficulty = 3, price = 30.0f, minSize = 3, maxSize = 94, lore = "�͵��� ���� �ź�ο� ����, ������ �ٷ�� �Ѵ�." });
        fishs.Add(new Fish { name = "��ġ", difficulty = 2, price = 15.0f, minSize = 30, maxSize = 155, lore = "���� �ٴٸ� ����� ������ ��ɲ�." });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 9.0f, minSize = 30, maxSize = 79, lore = "��� �丮�� ���� ���̴� ������ ����." });
        fishs.Add(new Fish { name = "�ػ�", difficulty = 3, price = 35.0f, minSize = 8, maxSize = 53, lore = "�ٴ� �عٴ��� û�Һ�, ���簡 ���� �����." });
        fishs.Add(new Fish { name = "û��", difficulty = 2, price = 20.0f, minSize = 20, maxSize = 53, lore = "�������� �ٴϸ� �ٴ� ���°��� �߿��� ��." });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 10.0f, minSize = 20, maxSize = 58, lore = "����� �ؼ��� ������ �������� �پ �����." });
        fishs.Add(new Fish { name = "���", difficulty = 3, price = 35.0f, minSize = 30, maxSize = 206, lore = "�� ���� ���� ���༺ ��ɲ�." });
        fishs.Add(new Fish { name = "����", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124, lore = "�ȶ��� �γ��� �ȷ� ���� ������ �����Ѵ�." });
        fishs.Add(new Fish { name = "���� ����", difficulty = 1, price = 9.0f, minSize = 20, maxSize = 66, lore = "���� �ٴٿ��� �ڶ�� �ҹ��� ���� ����." });
        fishs.Add(new Fish { name = "��¡��", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124, lore = "���߷°� ��α�� �ٴٿ��� ��Ƴ��� ��." });
        fishs.Add(new Fish { name = "��ġ", difficulty = 1, price = 6.0f, minSize = 3, maxSize = 43, lore = "������ �ٴ��� ���� �¿��ϴ� �߿��� ���." });
        fishs.Add(new Fish { name = "���", difficulty = 1, price = 8.0f, minSize = 3, maxSize = 33, lore = "���簡 ǳ���� ���� ��Ȱ�� ����." });
        fishs.Add(new Fish { name = "�ٴ尡��", difficulty = 1, price = 5.0f, minSize = 3, maxSize = 20, lore = "���� �丮�� ���ΰ�, �ܴ��� ���Թ��� Ư¡." });
        fishs.Add(new Fish { name = "�����ٶ���", difficulty = 1, price = 10.0f, minSize = 51, maxSize = 104, lore = "���� �ӵ��� �ٴٸ� ������ �ٶ����� ����." });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 3.0f, minSize = 2, maxSize = 5, lore = "������ �ٴ� �ٴڿ��� �ڶ�� �ػ깰." });
        fishs.Add(new Fish { name = "������", difficulty = 1, price = 0.0f, minSize = 1, maxSize = 5, lore = "�ٴ��� ��û��, ������ ó���ؾ� �Ѵ�." });
        fishs.Add(new Fish { name = "����", difficulty = 1, price = 1.0f, minSize = 1, maxSize = 5, lore = "�ٴ��� ��Ҹ� ����� �߿��� �Ĺ�." });
        fishs.Add(new Fish { name = "������ �����", difficulty = 5, price = 50.0f, minSize = 10, maxSize = 50, lore = "������ ���� �ӿ��� �����Ѵٴ� �ź�ο� ����ü." });
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
        invUI.SetActive(true);
    }

    public void ClickBagExitButton()
    {
        itemInfoLoreText.enabled = false;
        itemInfoImage.enabled = false;
        itemInfoNameText.enabled = false;
        closetUI.SetActive(false);
        
        invUI.SetActive(false);
        sellButton.SetActive(false);
        selectedItem = null;
    }

    public void ClickShopExitButton()
    {
        buyShopUI.SetActive(false);
        clothShopUI.SetActive(false);
    }

    public void ItemInfo(InventoryItem item)
    {
        if (item == null)
        {
            return;
        }
        selectedItem = item;
        if (item.fishData != null)
        {
            itemInfoImage.sprite = GetItemIcon(item.fishData.name);
            itemInfoNameText.text = item.fishData.name;
            itemInfoLoreText.text = item.fishData.lore;
        }
        else if (item.cropData != null)
        {
            itemInfoImage.sprite = GetItemIcon(item.cropData.name);
            itemInfoNameText.text = item.cropData.name;
            itemInfoLoreText.text = item.cropData.lore;
        }
        itemInfoImage.enabled = true;
        itemInfoNameText.enabled = true;
        itemInfoLoreText.enabled = true;
    }

    //�α� ǥ�� �Լ�
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

    public void UpdateMoney(float plusMoney) //�� ������Ʈ
    {
        money += plusMoney;
        moneyText.text = money + " ��";
    }

    public void BuySeed()
    {
        if (money >= 7)
        {
            UpdateMoney(-7);
            WriteLog("������ �����߽��ϴ�.");
            //�ʱ� ���� 1�� ����
            Crop seed = null;
            foreach (Crop crop in GameManager.Instance.crops)
            {
                if (crop.name == "����")
                {
                    seed = crop;
                    break;
                }
            }
            AddToInventory(seed);
        }
        else
        {
            WriteLog("���� �����մϴ�.");
        }
    }

    public void BuyFishRod()
    {
        if (fishRodLv == 1 && money >= Lv2UpPrice)
        {
            fishRodLv++;
            UpdateMoney(-Lv2UpPrice);
            buyFishRodButtonText.text = $"\n\n\n\n\n���˴� ���׷��̵�\n����: Lv{fishRodLv}\n{Lv3UpPrice}";
            WriteLog("���˴� ������ �ö����ϴ�!");
        }
        else if (fishRodLv == 2 && money >= Lv3UpPrice)
        {
            fishRodLv++;
            UpdateMoney(-Lv3UpPrice);
            buyFishRodButtonText.text = $"\n\n\n\n\n���˴� ���׷��̵�\n����: {fishRodLv}";
            WriteLog("���˴� ������ �ö����ϴ�!");
        }
        else if (fishRodLv == 3)
        {
            WriteLog("�̹� �ִ� ������ ���ô��Դϴ�.");
        }
        else
        {
            WriteLog("���� �����մϴ�.");
        }
    }

    public void SellItem()
    {
        if (selectedItem == null)
        {
            WriteLog("�Ǹ��� �������� �������ּ���.");
            return;
        }

        string name = "";
        float price = 0;

        //������ ���� �ҷ�����
        if (selectedItem.fishData != null)
        {
            name = selectedItem.fishData.name;
            price = selectedItem.fishData.price;
        }
        else if (selectedItem.cropData != null)
        {
            name = selectedItem.cropData.name;
            price = selectedItem.cropData.price;
        }

        UpdateMoney(price);
        // ������ �ϳ� ����
        selectedItem.count--;
        if (selectedItem.count <= 0)
        {
            inventory.Remove(selectedItem);
            selectedItem = null;
            itemInfoImage.enabled = false;
            itemInfoLoreText.enabled = false;
            itemInfoNameText.enabled = false;
        }

        UpdateInventoryUI();
        WriteLog($"{name}��(��) {price}���� �Ǹ��߽��ϴ�.");
    }

    public void OpenCloset()
    {
        for (int i = 1; i < 4; i++)
        {
            if (!clothUnlock[i])
            {
                cloths[i].color = Color.gray;
            }
        }
        closetUI.SetActive(true);
    }

    //�� ���� �Լ�
    public void ChangeCloth(int cloth)
    {
        if (clothUnlock[cloth])
        {
            clothIndex = cloth;
            WriteLog("�ǻ� ���� �Ϸ�!");
            p.ChangeClothNow();
        }
        else
        {
            WriteLog("�����ϰ� ���� ���� �ǻ��Դϴ�.");
        }
        
    }

    public void BuyCloth(int cloth)
    {
        if (clothUnlock[cloth])
        {
            WriteLog("�̹� �����ϰ� �ִ� �ǻ��Դϴ�.");
            return;
        }
        if (money >= clothPrice)
        {
            WriteLog("�ǻ� ���� �Ϸ�!");
            cloths[cloth].color = Color.white; //���忡�� �������
            clothUnlock[cloth] = true;
            UpdateMoney(-clothPrice);
            
        }
        else
        {
            WriteLog("���� �����մϴ�.");
        }
        
    }

    public void ClickOptionButton(string option)
    {
        if (option == "Exit") //�ɼǹ�ư �ݱ�
        {
            optionUI.SetActive(false);
        }
        if (option == "Open") //�ɼǹ�ư ����
        {
            optionUI.SetActive(true);
        }
    }

    public void GameExit()
    {
        //�̰����� �����ϱ� PlayerPrefs
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
