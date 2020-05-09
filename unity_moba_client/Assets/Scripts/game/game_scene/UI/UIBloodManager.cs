using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBloodManager : MonoBehaviour
{
    public GameObject uiHeroBloodPrefab;
    public GameObject uiTowerBloodPrefab;
    public GameObject uiMonsterBloodPrefab;

    public UIShowBlood PlaceUIBloodOnHero(int side)
    {
        GameObject ui = Instantiate(this.uiHeroBloodPrefab);
        ui.transform.SetParent(this.transform,false);

        UIShowBlood blood = ui.GetComponent<UIShowBlood>();
        blood.Init(side);
        return blood;
    }
    
    public UIShowBlood PlaceUIBloodOnTower(int side)
    {
        GameObject ui = Instantiate(this.uiTowerBloodPrefab);
        ui.transform.SetParent(this.transform,false);

        UIShowBlood blood = ui.GetComponent<UIShowBlood>();
        blood.Init(side);
        return blood;
    }
    
    public UIShowBlood PlaceUIBloodOnMonster(int side)
    {
        GameObject ui = Instantiate(this.uiMonsterBloodPrefab);
        ui.transform.SetParent(this.transform,false);

        UIShowBlood blood = ui.GetComponent<UIShowBlood>();
        blood.Init(side);
        return blood;
    }
}
