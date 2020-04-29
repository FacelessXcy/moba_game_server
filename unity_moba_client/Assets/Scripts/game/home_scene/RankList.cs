using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.UI;

public class RankList : MonoBehaviour
{
    public GameObject rankOptPrefab;
    public GameObject contentRoot;
    public ScrollRect rank;
    public Sprite[] ufaceImg;
    private void Start()
    {
        EventManager.Instance.AddEventListener("get_rank_list",OnGetRankListData);
        //发送拉取排行榜请求
        SystemServiceProxy.Instance.GetWorldUChipRankInfo();
        
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("get_rank_list",OnGetRankListData);
    }

    private void OnGetRankListData(string name,object udata)
    {
        List<WorldChipRankInfo> rank_info =
            (List<WorldChipRankInfo>) udata;
        this.rank.content.sizeDelta=new Vector2(0,rank_info.Count*170);
        //获取排行榜数据
        for (int i = 0; i < rank_info.Count; i++)
        {
//            Debug.Log(rank_info[i].unick+" "+
//                      rank_info[i].uchip);
            GameObject opt = GameObject.Instantiate(this.rankOptPrefab);
            opt.transform.SetParent(contentRoot.transform,false);
            opt.transform.Find("order").GetComponent<Text>().text =
                (i + 1).ToString();
            opt.transform.Find("unick_label").GetComponent<Text>()
                .text = rank_info[i].unick;
            opt.transform.Find("uchip_label").GetComponent<Text>()
                .text = rank_info[i].uchip.ToString();
            opt.transform.Find("header/avator").GetComponent<Image>()
                .sprite = ufaceImg[rank_info[i].uface - 1];
        }
    }

    public void OnRankListClose()
    {
        Destroy(this.gameObject);
    }

}
