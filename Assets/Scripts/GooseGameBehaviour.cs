using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Timeline;

public class GooseGameBehaviour : MonoBehaviour
{
    [SerializeField]public GameObject[] fieldsObject = new GameObject[64];
    public static GameObject[] fields = new GameObject[64];

    public GameObject playerObject;
    public GameObject enemy1Object;
    public GameObject enemy2Object;
    public GameObject enemy3Object;
    public PlayerClass player;
    public EnemyClass enemy1;
    public EnemyClass enemy2;
    public EnemyClass enemy3;

    public GameObject menu;

    //public PlayerClass player = new PlayerClass(10);
    //public EnemyClass enemy1 = new EnemyClass(10);
    //public EnemyClass enemy2 = new EnemyClass(10);
    //public EnemyClass enemy3 = new EnemyClass(10);
    public int gameNumber;
    public int totalLosedMoney;
    public bool firstRound = false;

    public delegate void CheckNumberSignalReceivedDelegate(GamblerClass gambler);

    void Start()
    {
        gameNumber = 1;
        fields = fieldsObject;
        player = playerObject.GetComponent<PlayerClass>();
        enemy1 = enemy1Object.GetComponent<EnemyClass>();
        enemy2 = enemy2Object.GetComponent<EnemyClass>();
        enemy3 = enemy3Object.GetComponent<EnemyClass>();
        player.money = 10; enemy1.money = 10; enemy2.money = 10; enemy3.money = 10;

    }

    public void GameStart()
    {
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        
        for (int i = 0; i < gamblers.Length; i++)
        {
            gamblers[i].ThrowDices();
            gamblers[i].currentRoundField = fields[0].GetComponent<FieldClass>();
        }

        Array.Sort(gamblers, (x, y) => y.threwNumber.CompareTo(x.threwNumber));

        int order = 1;
        gamblers[0].orderNumber = order;
        while (gamblers.GroupBy(obj => obj.threwNumber).Any(g => g.Count() > 1))
        {
            for (int i = 1; i < gamblers.Length; i++)
            {

                if (gamblers[i].threwNumber == gamblers[i - 1].threwNumber)
                {
                    //Debug.Log($"{gamblers[0].threwNumber} {gamblers[1].threwNumber} {gamblers[2].threwNumber} {gamblers[3].threwNumber}");
                    gamblers[i].ThrowDices();
                    gamblers[i - 1].ThrowDices();
                    Array.Sort(gamblers, (x, y) => y.threwNumber.CompareTo(x.threwNumber));
                    //Debug.Log($"{gamblers[0].threwNumber} {gamblers[1].threwNumber} {gamblers[2].threwNumber} {gamblers[3].threwNumber}");
                }
            }
        }
        for(int i = 0; i < 4; i++)
        {
            gamblers[i].orderNumber = i+1;
            //Debug.Log($"{gamblers[i].orderNumber}:  {gamblers[i].threwNumber}");
        }
        firstRound = true;
        Debug.Log("Первая игра");
    }

    public void GameStartNextRound()
    {
        GamblerClass[] gamblersAll = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        GamblerClass[] gamblers = new GamblerClass[3];
        for (int i =0; i < 4; i++)
        {
            for(int  j = 0; j < 3; j++)
            {
                if (gamblersAll[i].won == false) { gamblers[j] = gamblersAll[i]; }
            }
            gamblers[i].currentRoundField = fields[0].GetComponent<FieldClass>();
        }

        for (int i = 0; i < gamblers.Length; i++)
        {
            gamblers[i].ThrowDices();
        }

        Array.Sort(gamblers, (x, y) => y.threwNumber.CompareTo(x.threwNumber));

        int order = 1;
        gamblers[0].orderNumber = order;
        while (gamblers.GroupBy(obj => obj.threwNumber).Any(g => g.Count() > 1))
        {
            for (int i = 1; i < gamblers.Length; i++)
            {

                if (gamblers[i].threwNumber == gamblers[i - 1].threwNumber)
                {
                    //Debug.Log($"{gamblers[0].threwNumber} {gamblers[1].threwNumber} {gamblers[2].threwNumber} {gamblers[3].threwNumber}");
                    gamblers[i].ThrowDices();
                    gamblers[i - 1].ThrowDices();
                    Array.Sort(gamblers, (x, y) => y.threwNumber.CompareTo(x.threwNumber));
                    //Debug.Log($"{gamblers[0].threwNumber} {gamblers[1].threwNumber} {gamblers[2].threwNumber} {gamblers[3].threwNumber}");
                }
            }
        }
        for (int i = 1; i < 4; i++)
        {
            gamblers[i].orderNumber = i + 1;
            gamblers[i].won = false;
            //Debug.Log($"{gamblers[i].orderNumber}:  {gamblers[i].threwNumber}");
        }
        firstRound = true;
        Debug.Log("Следующая игра");
    }


    //можно перевести в ивент. Чтобы сначала ходил первый. Потом вызывалась проверка первого. Потом все остальное. Потом ходил второй и т.д.
    public void DoOneRound()
    {

        Debug.Log("Ход начат");
        StartCoroutine(DoOneRoundForEach(0));
        Debug.Log("Ход сделан");
    }






    public static FieldClass GetField(int number)
    {
        if (number > 63) { number = 63 - Mathf.Abs(63 - number); }
        return fields[number].GetComponent<FieldClass>();
    }

    public void GameEnd()
    {
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        totalLosedMoney = player.losedMoney + enemy1.losedMoney + enemy2.losedMoney + enemy3.losedMoney;
        for (int i = 0; i < 4; i++)
        {
            gamblers[i].canPlay = false;
            gamblers[i].money += totalLosedMoney;
            gamblers[i].losedMoney = 0;
            gamblers[i].mustSkip = false;
            gamblers[i].needsHelp = false;
            gamblers[i].metSomeone = false;
        }
    }

    IEnumerator CheckCoroutine(int numberToGo, GamblerClass gambler)
    {
        //menu.SetActive(false);
        //gambler.Go(numberToGo);

        yield return new WaitForSeconds(1f);

        gambler.CheckNumber();
        //menu.SetActive(true);
    }

    IEnumerator CheckForMoving(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        if (gamblers[0].isMoving == false & gamblers[1].isMoving == false & gamblers[2].isMoving == false & gamblers[3].isMoving == false) { menu.SetActive(true); }
        //else { StartCoroutine(CheckForMoving(seconds)); }
    }

    IEnumerator DoOneRoundForEach(int number)
    {
        //yield return new WaitForSeconds(1f);
        menu.SetActive(false);
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        gamblers = gamblers.OrderBy(i => i.orderNumber).ToArray();
        if (gamblers[number].mustSkip == false & gamblers[number].needsHelp == false)
        {
            gamblers[number].ThrowDices();
            gamblers[number].isMoving = true;
            gamblers[number].currentRoundField = GetField(gamblers[number].threwNumber + gamblers[number].currentRoundField.GetComponent<FieldClass>().number);
            gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
        }
        gamblers[number].Go(gamblers[number].currentRoundField.number);
        StartCoroutine(CheckCoroutine(gamblers[number].currentRoundField.number, gamblers[number]));

        if(firstRound)
        {
            gamblers[number].firstRoundField = gamblers[number].currentRoundField;
            firstRound = false;
            gamblers[number].metSomeone = false;
        }
        if (gamblers[number].won) { GameEnd(); }
        for (int j = 0; j < 4; j++)
        {
            if (gamblers[number].currentPositionNumber == gamblers[j].currentPositionNumber & number != j)
            {
                gamblers[number].metSomeone = true;
                if (gamblers[j].needsHelp)
                {
                    if (gamblers[j].currentPositionNumber == 31) { gamblers[j].WasHelped(1); }
                    else if (gamblers[j].currentPositionNumber == 52) { gamblers[j].WasHelped(2); }
                    Debug.Log($"{j+1}-ый игрок был спасен {number+1}-ым игроком");
                }
            }
            else { }
        }
        //нужно переделать. Что если игрок попал мост, где уже кто-то стоял? Он не должен будет выбирать опции
        //нужно переделать, чтобы человек попавший на колодец не перепрыгнул того, кто в нем был
        //нужно переделать, добавить корутину, чтобы игрок попавший на другого игрока сначала в анимации переходил на него, а не сразу делал два хода
        //второй ход при попадании на игрока не всегда работает, вероятно после гуся. Возможно из-за неправильных таймингов
        //НУЖНО РАЗДЕЛИТЬ, ЧТОБЫ СНАЧАЛА ПОЛНОСТЬЮ СХОДИЛ ОДИН ИГРОК, ПОТОМ ДРУГОЙ
        if (gamblers[number].metSomeone)
        {
            gamblers[number].metSomeone = false;
            Debug.Log($"Встреча у {number + 1} игрока!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            gamblers[number].currentRoundField = GetField(gamblers[number].threwNumber + gamblers[number].currentRoundField.GetComponent<FieldClass>().number);
            gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
            gamblers[number].Go(gamblers[number].currentRoundField.number);
            StartCoroutine(CheckCoroutine(gamblers[number].currentRoundField.number, gamblers[number]));
        }
        Debug.Log($"{number + 1}-ый сделал ход");
        yield return new WaitForSeconds(1.1f);
        gamblers[number].isMoving = false;
        if ( number < 3)
        {
            number++;
            StartCoroutine(DoOneRoundForEach(number));
        }
        StartCoroutine(CheckForMoving(1.1f));
        StartCoroutine(CheckForMoving(4.6f));
        StartCoroutine(CheckForMoving(7.1f));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
