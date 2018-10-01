//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class UIComItemList : MonoBehaviour {
//    public GameObject item;
//    public Transform itemList;

//    List<GameObject> items = new List<GameObject>();

//    public void SetItemList(List<GoodsItem> goods)
//    {
//        if (!isInit)
//            Init();
//        count = goods.Count;
//        for(int i = 0; i < items.Count && i < goods.Count; i++)
//        {
//            UIItem.SetGoodsItem(items[i].transform, goods[i]);
//        }
//    }


//    bool isInit = false;
//    public void Init()
//    {
//        isInit = true;
//        item = transform.FindChild("item").gameObject;
//        itemList = transform.FindChild("item_list");
//        item.SetActive(false);
//    }


//    //根据实际需求创建需要的数量
//    int curMax = 0;
//    public int count
//    {
//        set
//        {
//            if (value > curMax)
//            {
//                int num = value - curMax;
//                curMax = value;
//                for(int i = 0; i < num; i++)
//                {
//                    GameObject obj = Instantiate(item) as GameObject;
//                    obj.transform.SetParent(itemList);
//                    obj.transform.localScale = Vector3.one;
//                    obj.SetActive(true);
//                    items.Add(obj);
//                }
//            }
//            ShowListNum(itemList, value);
//        }
//    }

//    //显示子物体的数量
//    void ShowListNum(Transform trans, int num)
//    {
//        if (trans.childCount < num)
//            return;
//        for (int i = 0; i < num; i++)
//        {
//            trans.GetChild(i).gameObject.SetActive(true);
//        }
//        for (int i = num; i < trans.childCount; i++)
//        {
//            trans.GetChild(i).gameObject.SetActive(false);
//        }
//    }
//}
