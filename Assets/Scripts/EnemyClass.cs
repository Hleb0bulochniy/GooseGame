using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : GamblerClass
{
    public override void ChooseBridge()
    {
        int i = Random.Range(1, 2);
        Debug.Log($"Враг, попавший на мост выбрал {i}");
        if (i == 1)
        {
            money--;
            losedMoney++;
            isChoising = false;
        }
        if (i == 2)
        {
            isChoising = false;
            money -= 2;
            losedMoney += 2;
            currentRoundField = GooseGameBehaviour.GetField(12);
            currentPositionNumber = currentRoundField.number;
            Go(12);
        }

    }

    public override void Choose63()
    {
        int i = Random.Range(1, 3);
        Debug.Log($"Враг, попавший в 63 выбрал {i}");
        if (i == 1)
        {
            currentRoundField = GooseGameBehaviour.GetField(3 + currentRoundField.number);
            currentPositionNumber = currentRoundField.number;
            Go(currentRoundField.number);
        }
        if (i == 2)
        {
            currentRoundField = GooseGameBehaviour.GetField(6 + currentRoundField.number);
            currentPositionNumber = currentRoundField.number;
            Go(currentRoundField.number);
            //CheckNumber();
        }
        isChoising = false;
    }

    public override void Choose45()
    {
        int i = Random.Range(1, 4);
        Debug.Log($"Враг, попавший в 45 выбрал {i}");
        if (i == 1)
        {
            currentRoundField = GooseGameBehaviour.GetField(4 + currentRoundField.number);
            currentPositionNumber = currentRoundField.number;
            Go(currentRoundField.number);
        }
        if (i == 2)
        {
            currentRoundField = GooseGameBehaviour.GetField(5 + currentRoundField.number);
            currentPositionNumber = currentRoundField.number;
            Go(currentRoundField.number);
            //CheckNumber();
        }
        if (i == 3)
        {
            currentRoundField = GooseGameBehaviour.GetField(9 + currentRoundField.number);
            currentPositionNumber = currentRoundField.number;
            Go(currentRoundField.number);
        }
        isChoising = false;
    }


    /*public EnemyClass(int money)
    {
        this.money = money;
    }*/

}
