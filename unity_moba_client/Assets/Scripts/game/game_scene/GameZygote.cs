using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameZygote : MonoBehaviour
{
    //test
    public Joystick stick;
    //end
    
    public GameObject[] heroCharacters;//男、女
    
    public GameObject entryA;

    private void Start()
    {
        GameObject hero = Instantiate(this.heroCharacters[UGame.Instance.uSex]);
        hero.transform.SetParent(this.transform,false);
        hero.transform.position = this.entryA.transform.position;
        CharacterCtrl ctrl = hero.AddComponent<CharacterCtrl>();
        ctrl.isGhost = false;//自己控制
        ctrl.stick = this.stick;//测试

    }
}
