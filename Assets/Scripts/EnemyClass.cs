using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : GamblerClass
{
    public override int Choose()
    {
        Debug.Log("Противник делает выбор");
        return Random.Range(1,4);

    }

    /*public EnemyClass(int money)
    {
        this.money = money;
    }*/

}
