using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

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



    //public PlayerClass player = new PlayerClass(10);
    //public EnemyClass enemy1 = new EnemyClass(10);
    //public EnemyClass enemy2 = new EnemyClass(10);
    //public EnemyClass enemy3 = new EnemyClass(10);
    public int gameNumber;
    public int totalLosedMoney;
    public bool firstRound = false;

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

    public void DoOneRound()
    {
        Debug.Log("Ход начат");
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        gamblers = gamblers.OrderBy(i => i.orderNumber).ToArray();
        for (int i = 0; i < 4; i++)
        {
            gamblers[i].ThrowDices();
            gamblers[i].currentRoundField = GetField(gamblers[i].threwNumber + gamblers[i].currentRoundField.GetComponent<FieldClass>().number);
            gamblers[i].currentPositionNumber = gamblers[i].currentRoundField.number;
            gamblers[i].Go(gamblers[i].currentRoundField.number);
            //gamblers[i].Go(gamblers[i].currentPositionNumber + 1);
            
            gamblers[i].CheckNumber();
            /*if(firstRound)
            {
                gamblers[i].firstRoundField = gamblers[i].currentRoundField;
                firstRound = false;
                gamblers[i].metSomeone = false;
            }*/
            if (gamblers[i].won) { GameEnd(); }
            /*for (int j = 0; j < 4; j++)
            {
                if (gamblers[i].currentRoundField.number == gamblers[j].currentRoundField.number) { gamblers[i].metSomeone = true; }
                else { gamblers[i].metSomeone = false;}
            }*/
            /*if (gamblers[i].metSomeone)
            {
                Debug.Log($"Встреча у {i} игрока");
                gamblers[i].Go(gamblers[i].threwNumber + gamblers[i].currentRoundField.number);
                gamblers[i].CheckNumber();
                gamblers[i].metSomeone = false;
            }*/
            Debug.Log($"{i+1}-ый сделал ход");
        }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
