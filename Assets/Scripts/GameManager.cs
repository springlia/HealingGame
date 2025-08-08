using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string name;
    public int difficulty;
    public float price;
    public float minSize;
    public float maxSize;
    public string lore;
}


[System.Serializable]
public class InventoryItem
{
    public Item itemData;
    public int count;
}

[System.Serializable]
public class InventoryWrapper
{
    public List<InventoryItem> inventory;
}

[System.Serializable]
public class TileMapWrapper
{
    public List<string> tileNames;
}


public class GameManager : MonoBehaviour
{
    //�̱���
    public static GameManager Instance { get; private set; }

    //������
    public List<Item> items = new List<Item>();


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
    [SerializeField] GameObject itemInfoUI;

    [SerializeField] GameObject optionUI;

    //����
    public GameObject sellButton;
    private InventoryItem selectedItem = null;

    public float money = 0;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GameObject player;
    Player p;

    //����
    public AudioClip walking;
    public AudioClip farming;
    public AudioClip fishing;
    public AudioClip shopping;
    public AudioClip hoeing;
    public AudioClip no;
    public AudioSource audioSource;
    AudioSource bgmAudio;

    private void Awake()
    {
        Instance = this;

        p = player.GetComponent<Player>();
        AddItem();
        audioSource = GetComponent<AudioSource>();
        LoadGameData();

        for (int i = 0; i < InventorySize; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            slotUIs.Add(slotObj.GetComponent<InventorySlot>());
        }

        // ����� ������ ���ٸ� ���� �������� �Ǵ�
        if (!PlayerPrefs.HasKey("hasStarted"))
        {
            // ���� 1�� ����
            Item seed = null;
            foreach (Item item in GameManager.Instance.items)
            {
                if (item.name == "����")
                {
                    seed = item;
                    break;
                }
            }
            AddToInventory(seed);

            // ���� ���� ��ũ ����
            PlayerPrefs.SetInt("hasStarted", 1);
            PlayerPrefs.Save();
        }

        //���� �� ������ ������Ʈ
        UpdateMoney(0);
        clothUnlock[0] = true;
        UpdateBuyFishRodUI();
        UpdateInventoryUI();
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "WALK":
                audioSource.clip = walking;
                break;
            case "FARM":
                audioSource.clip = farming;
                break;
            case "FISH":
                audioSource.clip = fishing;
                break;
            case "SHOP":
                audioSource.clip = shopping;
                break;
            case "HOE":
                audioSource.clip = hoeing;
                break;
            case "NO":
                audioSource.clip = no;
                break;
        }
        audioSource.Play();
    }

    //�κ��丮 ������ �߰� (���÷�)
    public void AddToInventory(Item i)
    {
        // �̹� �ִ� �׸��̸� ������ ����
        foreach (var item in inventory)
        {
            if (item.itemData != null && item.itemData.name == i.name)
            {
                item.count++;
                UpdateInventoryUI();
                return;
            }
        }
        // ���� �߰�
        if (inventory.Count < InventorySize)
        {
            inventory.Add(new InventoryItem { itemData = i, count = 1 });
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

                if (inventory[i].itemData != null)
                {
                    name = inventory[i].itemData.name;
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

    void AddItem()
    {
        items.Add(new Item { name = "����", difficulty = 2, price = 13.0f, minSize = 60, maxSize = 168, lore = "�ٴٸ� ������ ���ġ�� ��ܹ� ����." });
        items.Add(new Item { name = "����", difficulty = 2, price = 15.0f, minSize = 60, maxSize = 168, lore = "������ �ٴٷ�, �ٽ� ������ ���ƿ��� ������ ������ ���ΰ�." });
        items.Add(new Item { name = "����", difficulty = 3, price = 30.0f, minSize = 3, maxSize = 94, lore = "�͵��� ���� �ź�ο� ����, ������ �ٷ�� �Ѵ�." });
        items.Add(new Item { name = "��ġ", difficulty = 2, price = 15.0f, minSize = 30, maxSize = 155, lore = "���� �ٴٸ� ����� ������ ��ɲ�." });
        items.Add(new Item { name = "����", difficulty = 1, price = 9.0f, minSize = 30, maxSize = 79, lore = "��� �丮�� ���� ���̴� ������ ����." });
        items.Add(new Item { name = "�ػ�", difficulty = 3, price = 35.0f, minSize = 8, maxSize = 53, lore = "�ٴ� �عٴ��� û�Һ�, ���簡 ���� �����." });
        items.Add(new Item { name = "û��", difficulty = 2, price = 20.0f, minSize = 20, maxSize = 53, lore = "�������� �ٴϸ� �ٴ� ���°��� �߿��� ��." });
        items.Add(new Item { name = "����", difficulty = 1, price = 10.0f, minSize = 20, maxSize = 58, lore = "����� �ؼ��� ������ �������� �پ �����." });
        items.Add(new Item { name = "���", difficulty = 3, price = 35.0f, minSize = 30, maxSize = 206, lore = "�� ���� ���� ���༺ ��ɲ�." });
        items.Add(new Item { name = "����", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124, lore = "�ȶ��� �γ��� �ȷ� ���� ������ �����Ѵ�." });
        items.Add(new Item { name = "���� ����", difficulty = 1, price = 9.0f, minSize = 20, maxSize = 66, lore = "���� �ٴٿ��� �ڶ�� �ҹ��� ���� ����." });
        items.Add(new Item { name = "��¡��", difficulty = 4, price = 30.0f, minSize = 30, maxSize = 124, lore = "���߷°� ��α�� �ٴٿ��� ��Ƴ��� ��." });
        items.Add(new Item { name = "��ġ", difficulty = 1, price = 6.0f, minSize = 3, maxSize = 43, lore = "������ �ٴ��� ���� �¿��ϴ� �߿��� ���." });
        items.Add(new Item { name = "���", difficulty = 1, price = 8.0f, minSize = 3, maxSize = 33, lore = "���簡 ǳ���� ���� ��Ȱ�� ����." });
        items.Add(new Item { name = "�ٴ尡��", difficulty = 1, price = 5.0f, minSize = 3, maxSize = 20, lore = "���� �丮�� ���ΰ�, �ܴ��� ���Թ��� Ư¡." });
        items.Add(new Item { name = "�����ٶ���", difficulty = 1, price = 10.0f, minSize = 51, maxSize = 104, lore = "���� �ӵ��� �ٴٸ� ������ �ٶ����� ����." });
        items.Add(new Item { name = "����", difficulty = 1, price = 3.0f, minSize = 2, maxSize = 5, lore = "������ �ٴ� �ٴڿ��� �ڶ�� �ػ깰." });
        items.Add(new Item { name = "������", difficulty = 1, price = 0.0f, minSize = 1, maxSize = 5, lore = "�ٴ��� ��û��, ������ ó���ؾ� �Ѵ�." });
        items.Add(new Item { name = "����", difficulty = 1, price = 1.0f, minSize = 1, maxSize = 5, lore = "�ٴ��� ��Ҹ� ����� �߿��� �Ĺ�." });
        items.Add(new Item { name = "������ �����", difficulty = 5, price = 50.0f, minSize = 10, maxSize = 50, lore = "������ ���� �ӿ��� �����Ѵٴ� �ź�ο� ����ü." });
        items.Add(new Item { name = "����", difficulty = 0, price = 5.0f, minSize = 0, maxSize = 0, lore = "�̷��� ���� ������ ��� �ڶ󳯱��?" });
        items.Add(new Item { name = "�丶��", difficulty = 0, price = 35.0f, minSize = 0, maxSize = 0, lore = "�����ϸ鼭�� ������ ���� ��ȭ�� �̷�� �α� ������ ä��." });
    }
    public Item GetRandomFish()
    {
        return items[Random.Range(0, items.Count-2)];
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
        itemInfoUI.SetActive(false);
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
        if (item.itemData != null)
        {
            itemInfoImage.sprite = GetItemIcon(item.itemData.name);
            itemInfoNameText.text = item.itemData.name;
            itemInfoLoreText.text = item.itemData.lore;
        }
        itemInfoUI.SetActive(true);
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
            PlaySound("SHOP");
            //�ʱ� ���� 1�� ����
            Item seed = null;
            foreach (Item item in GameManager.Instance.items)
            {
                if (item.name == "����")
                {
                    seed = item;
                    break;
                }
            }
            AddToInventory(seed);
        }
        else
        {
            PlaySound("NO");
            WriteLog("���� �����մϴ�.");
        }
    }

    public void UpdateBuyFishRodUI()
    {
        if (fishRodLv == 1)
        {
            buyFishRodButtonText.text = $"\n\n\n\n\n���˴� ���׷��̵�\n����: Lv{fishRodLv}\n{Lv2UpPrice}";
        }
        else if (fishRodLv == 2)
        {
            buyFishRodButtonText.text = $"\n\n\n\n\n���˴� ���׷��̵�\n����: Lv{fishRodLv}\n{Lv3UpPrice}";
        }
        else if (fishRodLv == 3)
        {
            buyFishRodButtonText.text = $"\n\n\n\n\n���˴� ���׷��̵�\n����: {fishRodLv}";
        }
    }

    public void BuyFishRod()
    {
        if (fishRodLv == 1 && money >= Lv2UpPrice)
        {
            fishRodLv++;
            UpdateMoney(-Lv2UpPrice);
            WriteLog("���˴� ������ �ö����ϴ�!");
            UpdateBuyFishRodUI();
            PlaySound("SHOP");
        }
        else if (fishRodLv == 2 && money >= Lv3UpPrice)
        {
            fishRodLv++;
            UpdateMoney(-Lv3UpPrice);
            WriteLog("���˴� ������ �ö����ϴ�!");
            UpdateBuyFishRodUI();
            PlaySound("SHOP");
        }
        else if (fishRodLv == 3)
        {
            WriteLog("�̹� �ִ� ������ ���ô��Դϴ�.");
            PlaySound("NO");
        }
        else
        {
            WriteLog("���� �����մϴ�.");
            PlaySound("NO");
        }
    }

    public void SellItem()
    {
        if (selectedItem == null)
        {
            WriteLog("�Ǹ��� �������� �������ּ���.");
            PlaySound("NO");
            return;
        }

        string name = "";
        float price = 0;

        //������ ���� �ҷ�����
        if (selectedItem.itemData != null)
        {
            name = selectedItem.itemData.name;
            price = selectedItem.itemData.price;
        }

        UpdateMoney(price);
        PlaySound("SHOP");
        // ������ �ϳ� ����
        selectedItem.count--;
        if (selectedItem.count <= 0)
        {
            inventory.Remove(selectedItem);
            selectedItem = null;
            itemInfoUI.SetActive(false);
        }

        UpdateInventoryUI();
        WriteLog($"{name}��(��) {price}���� �Ǹ��߽��ϴ�.");
        PlaySound("SHOP");
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
            PlaySound("SHOP");
            p.ChangeClothNow();
        }
        else
        {
            WriteLog("�����ϰ� ���� ���� �ǻ��Դϴ�.");
            PlaySound("NO");
        }
        
    }

    public void BuyCloth(int cloth)
    {
        if (clothUnlock[cloth])
        {
            WriteLog("�̹� �����ϰ� �ִ� �ǻ��Դϴ�.");
            PlaySound("NO");
            return;
        }
        if (money >= clothPrice)
        {
            WriteLog("�ǻ� ���� �Ϸ�!");
            PlaySound("SHOP");
            cloths[cloth].color = Color.white; //���忡�� �������
            clothUnlock[cloth] = true;
            UpdateMoney(-clothPrice);
            
        }
        else
        {
            WriteLog("���� �����մϴ�.");
            PlaySound("NO");
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


    void SaveGameData()
    {
        // �⺻ ������ ����
        PlayerPrefs.SetInt("clothIndex", clothIndex);
        PlayerPrefs.SetFloat("money", money);
        PlayerPrefs.SetInt("fishRodLv", fishRodLv);

        // �ǻ� �ر� ���� ����
        for (int i = 0; i < clothUnlock.Length; i++)
        {
            PlayerPrefs.SetInt($"clothUnlock_{i}", clothUnlock[i] ? 1 : 0);
        }

        // �κ��丮 ���� (JSON ����ȭ)
        string invJson = JsonUtility.ToJson(new InventoryWrapper { inventory = inventory });
        PlayerPrefs.SetString("inventory", invJson);

        // �÷��̾� ��ġ ����
        Vector3 pos = player.transform.position;
        PlayerPrefs.SetFloat("playerPosX", pos.x);
        PlayerPrefs.SetFloat("playerPosY", pos.y);
        PlayerPrefs.SetFloat("playerPosZ", pos.z);

        // ���� Ÿ�ϸ� ���� (�����ϰ� Ÿ�� �̸��� �����ϴ� ��)
        List<string> tileNames = new List<string>();
        BoundsInt bounds = p.groundTilemap.cellBounds;
        foreach (Vector3Int posTile in bounds.allPositionsWithin)
        {
            TileBase tile = p.groundTilemap.GetTile(posTile);
            tileNames.Add(tile != null ? tile.name : "null");
        }
        PlayerPrefs.SetString("groundTilemap", JsonUtility.ToJson(new TileMapWrapper { tileNames = tileNames }));

        PlayerPrefs.Save();
    }
    void LoadGameData()
    {
        clothIndex = PlayerPrefs.GetInt("clothIndex", 0);
        money = PlayerPrefs.GetFloat("money", 0);
        fishRodLv = PlayerPrefs.GetInt("fishRodLv", 1);

        for (int i = 0; i < clothUnlock.Length; i++)
        {
            clothUnlock[i] = PlayerPrefs.GetInt($"clothUnlock_{i}", 0) == 1;
        }

        // �κ��丮 �ε�
        string invJson = PlayerPrefs.GetString("inventory", "");
        if (!string.IsNullOrEmpty(invJson))
        {
            InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(invJson);
            inventory = wrapper.inventory;
        }

        // �÷��̾� ��ġ
        Vector3 pos = new Vector3(
            PlayerPrefs.GetFloat("playerPosX", player.transform.position.x),
            PlayerPrefs.GetFloat("playerPosY", player.transform.position.y),
            PlayerPrefs.GetFloat("playerPosZ", player.transform.position.z));
        player.transform.position = pos;

        // Ÿ�ϸ� �ε�
        string tileJson = PlayerPrefs.GetString("groundTilemap", "");
        if (!string.IsNullOrEmpty(tileJson))
        {
            TileMapWrapper mapWrapper = JsonUtility.FromJson<TileMapWrapper>(tileJson);
            BoundsInt bounds = p.groundTilemap.cellBounds;
            int i = 0;
            foreach (Vector3Int posTile in bounds.allPositionsWithin)
            {
                if (i >= mapWrapper.tileNames.Count) break;
                string tileName = mapWrapper.tileNames[i++];
                if (tileName == "null")
                {
                    p.groundTilemap.SetTile(posTile, null);
                }
                else
                {
                    TileBase found = System.Array.Find(p.farmTiles, t => t.name == tileName);
                    if (found != null)
                        p.groundTilemap.SetTile(posTile, found);
                }
            }
        }

        UpdateInventoryUI();

        p.ResumeCropsAfterLoad();
    }

    public void GameExit()
    {
        SaveGameData();
        //�̰����� �����ϱ� PlayerPrefs
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
