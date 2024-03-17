using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : GamblerClass
{



    public int chosenNumber;
    public void Choose1() { chosenNumber = 1; }
    public void Choose2() { chosenNumber = 2; }
    public void Choose3() { chosenNumber = 3; }
    public void Choose4() { chosenNumber = 4; }

    public override void ChooseBridge()
    {
        bridgeOptions.SetActive(true);
    }

    public void ChooseBridgeButton1()
    {
        bridgeOptions.SetActive(false);
        money--;
        losedMoney++;
    }
    public void ChooseBridgeButton2()
    {
        bridgeOptions.SetActive(false);
        money -= 2;
        losedMoney += 2;
        currentRoundField = GooseGameBehaviour.GetField(12);
        currentPositionNumber = currentRoundField.number;
        Go(12);
    }


    //переделать игрок успевает перебросить кубики и сходить второй раз на другое число
    public override void Choose63()
    {
        dices63Options.SetActive(true);
    }

    public void Choose63Button1()
    {
        dices63Options.SetActive(false);
        currentRoundField = GooseGameBehaviour.GetField(3 + currentPositionNumber);
        currentPositionNumber = currentRoundField.number;
        Go(currentRoundField.number);
    }
    public void Choose63Button2()
    {
        dices63Options.SetActive(false);
        currentRoundField = GooseGameBehaviour.GetField(6 + currentPositionNumber);
        currentPositionNumber = currentRoundField.number;
        Go(currentRoundField.number);
        CheckNumber();
    }
    public void Choose63Button3()
    {
        dices63Options.SetActive(false);
    }


    public override void Choose45()
    {
        dices63Options.SetActive(true);
    }

    public void Choose45Button1()
    {
        dices63Options.SetActive(false);
        currentRoundField = GooseGameBehaviour.GetField(4 + currentPositionNumber);
        currentPositionNumber = currentRoundField.number;
        Go(currentRoundField.number);
    }
    public void Choose45Button2()
    {
        dices63Options.SetActive(false);
        currentRoundField = GooseGameBehaviour.GetField(5 + currentPositionNumber);
        currentPositionNumber = currentRoundField.number;
        Go(currentRoundField.number);
        CheckNumber();
    }
    public void Choose45Button3()
    {
        dices63Options.SetActive(false);
        currentRoundField = GooseGameBehaviour.GetField(9 + currentPositionNumber);
        currentPositionNumber = currentRoundField.number;
        Go(currentRoundField.number);
    }
    public void Choose45Button4()
    {
        dices63Options.SetActive(false);
    }


    public override int Choose()
    {
        Debug.Log("Игрок делает выбор");
        if (currentRoundField.GetComponent<FieldClass>().number == 12) { bridgeOptions.SetActive(true); }
        if (currentRoundField.GetComponent<FieldClass>().number == 26) { dices63Options.SetActive(true); }
        if (currentRoundField.GetComponent<FieldClass>().number == 53) { dices45Options.SetActive(true); }
        Debug.Log("player goes");
        bridgeOptions.SetActive(false);
        dices63Options.SetActive(false);
        dices45Options.SetActive(false);
        return chosenNumber;
    }

    /*public PlayerClass(int money)
    {
        this.money = money;
    }*/
}
