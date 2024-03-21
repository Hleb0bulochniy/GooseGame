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

    //можно перевести в ивент. Чтобы сначала ходил первый. Потом вызывалась проверка первого. Потом все остальное. Потом ходил второй и т.д.
    public void DoOneRound()
    {

        Debug.Log("Ход начат");
        StartCoroutine(RoundThrow(0, 1f));
    }

    IEnumerator CheckCoroutine(int numberToGo, GamblerClass gambler)
    {
        //menu.SetActive(false);
        //gambler.Go(numberToGo);

        yield return new WaitForSeconds(1f);

        //gambler.CheckNumber();
        //menu.SetActive(true);
    }

    IEnumerator CheckForMoving(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        if (gamblers[0].isMoving == false & gamblers[1].isMoving == false & gamblers[2].isMoving == false & gamblers[3].isMoving == false) { menu.SetActive(true); }
        //else { StartCoroutine(CheckForMoving(seconds)); }
    }



    //нужно переделать. Что если игрок попал мост, где уже кто-то стоял? Он не должен будет выбирать опции
    //нужно переделать, чтобы человек попавший на колодец не перепрыгнул того, кто в нем был
    //нужно переделать, добавить корутину, чтобы игрок попавший на другого игрока сначала в анимации переходил на него, а не сразу делал два хода
    //второй ход при попадании на игрока не всегда работает, вероятно после гуся. Возможно из-за неправильных таймингов
    //НУЖНО РАЗДЕЛИТЬ, ЧТОБЫ СНАЧАЛА ПОЛНОСТЬЮ СХОДИЛ ОДИН ИГРОК, ПОТОМ ДРУГОЙ
    //можно вручную для каждого особого поля выставлять, сколько секунд ждать

    //баг, игрок пошел на 4 и потом сразу на 12

    IEnumerator RoundThrow(int number, float seconds)
    {
        menu.SetActive(false);
        GamblerClass[] gamblers = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        gamblers = gamblers.OrderBy(i => i.orderNumber).ToArray();
        if (gamblers[number].mustSkip == false & gamblers[number].needsHelp == false)
        {
            gamblers[number].ThrowDices();
            Debug.Log($"Кубики были брошены, выпало {gamblers[number].threwNumber}");
            gamblers[number].isMoving = true;
            gamblers[number].currentRoundField = GetField(gamblers[number].threwNumber + gamblers[number].currentRoundField.GetComponent<FieldClass>().number);
            gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
        }
        gamblers[number].Go(gamblers[number].currentRoundField.number);

        yield return new WaitForSeconds(1);
        StartCoroutine(NumberCheck(number, seconds, gamblers));
    }


    IEnumerator NumberCheck(int number, float seconds, GamblerClass[] gamblers)
    {
        if (gamblers[number].currentRoundField != null)
        {
            //гусь
            if (gamblers[number].currentRoundField.number == 5 | gamblers[number].currentRoundField.number == 9 | gamblers[number].currentRoundField.number == 14 | gamblers[number].currentRoundField.number == 18 | gamblers[number].currentRoundField.number == 23 | gamblers[number].currentRoundField.number == 27 | gamblers[number].currentRoundField.number == 32 | gamblers[number].currentRoundField.number == 36 | gamblers[number].currentRoundField.number == 41 | gamblers[number].currentRoundField.number == 45 | gamblers[number].currentRoundField.number == 50 | gamblers[number].currentRoundField.number == 54 | gamblers[number].currentRoundField.number == 59)
            {
                Debug.Log("Кто-то попал в гуся");
                gamblers[number].currentRoundField = GooseGameBehaviour.GetField(gamblers[number].threwNumber + gamblers[number].currentRoundField.number);
                gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
                gamblers[number].Go(gamblers[number].currentRoundField.number);
                yield return new WaitForSeconds(1f);
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }
            //мост
            else if (gamblers[number].currentRoundField.number == 6)
            {
                Debug.Log("Кто-то попал в мост");
                gamblers[number].ChooseBridge();
                gamblers[number].isChoising = true;
                while (gamblers[number].isChoising) { yield return 0.5f; ; }
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //трактир
            else if (gamblers[number].currentRoundField.number == 19)
            {
                Debug.Log("Кто-то попал в трактир");
                gamblers[number].mustSkip = true;
                gamblers[number].money -= 2;
                gamblers[number].losedMoney += 2;
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //колодец
            else if (gamblers[number].currentRoundField.number == 31)
            {
                Debug.Log("Кто-то попал в колодец");
                gamblers[number].money -= 3;
                gamblers[number].losedMoney += 3;
                gamblers[number].mustSkip = true;
                gamblers[number].needsHelp = true;
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //кубики 6 и 3
            else if (gamblers[number].currentRoundField.number == 26)
            {
                Debug.Log("Кто-то попал в 63");
                gamblers[number].Choose63();
                gamblers[number].isChoising = true;
                while (gamblers[number].isChoising) { yield return 0.5f;}
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //лабиринт
            else if (gamblers[number].currentRoundField.number == 42)
            {
                Debug.Log("Кто-то попал в лабиринт");
                gamblers[number].money--;
                gamblers[number].losedMoney++;
                gamblers[number].currentRoundField = GooseGameBehaviour.GetField(39);
                gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
                gamblers[number].Go(39);
                yield return new WaitForSeconds(seconds);
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //тюрьма
            else if (gamblers[number].currentRoundField.number == 52)
            {
                Debug.Log("Кто-то попал в тюрьму");
                gamblers[number].money -= 2;
                gamblers[number].losedMoney += 2;
                gamblers[number].mustSkip = true;
                gamblers[number].needsHelp = true;
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //кубики 4 и 5
            else if (gamblers[number].currentRoundField.number == 53)
            {
                Debug.Log("Кто-то попал в 45");
                gamblers[number].Choose45();
                gamblers[number].isChoising = true;
                while (gamblers[number].isChoising) { yield return 0.5f; }
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //смерть
            else if (gamblers[number].currentRoundField.number == 58)
            {
                Debug.Log("Кто-то попал в смерть");
                gamblers[number].money--;
                gamblers[number].losedMoney++;
                gamblers[number].currentRoundField = GooseGameBehaviour.GetField(1);
                gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
                gamblers[number].Go(1);
                yield return new WaitForSeconds(seconds);
                StartCoroutine(NumberCheck(number, seconds, gamblers));
            }

            //победа
            else if (gamblers[number].currentRoundField.number == 63)
            {
                Debug.Log("Кто-то попал в победу");
                gamblers[number].won = true;
                GameEnd();
            }
            else { StartCoroutine(MetCheck(number, seconds, gamblers)); }
        }
    }

    //при повторном вызове внутри, корутины будут дублироваться. Надо предусмотреть это

    IEnumerator MetCheck(int number, float seconds, GamblerClass[] gamblers)
    {
        if (firstRound)
        {
            gamblers[number].firstRoundField = gamblers[number].currentRoundField;
            firstRound = false;
            gamblers[number].metSomeone = false;
        }
        for (int j = 0; j < 4; j++)
        {
            if (gamblers[number].currentPositionNumber == gamblers[j].currentPositionNumber & number != j)
            {
                gamblers[number].metSomeone = true;
                if (gamblers[j].needsHelp)
                {
                    if (gamblers[j].currentPositionNumber == 31) { gamblers[j].WasHelped(1); }
                    else if (gamblers[j].currentPositionNumber == 52) { gamblers[j].WasHelped(2); }
                    Debug.Log($"{j + 1}-ый игрок был спасен {number + 1}-ым игроком");
                }
            }
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(AfterMetCheck(number, seconds, gamblers));
    }
    IEnumerator AfterMetCheck(int number, float seconds, GamblerClass[] gamblers)
    {
        if (gamblers[number].metSomeone)
        {
            gamblers[number].metSomeone = false;
            Debug.Log($"Встреча у {number + 1} игрока!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            gamblers[number].currentRoundField = GetField(gamblers[number].threwNumber + gamblers[number].currentRoundField.GetComponent<FieldClass>().number);
            gamblers[number].currentPositionNumber = gamblers[number].currentRoundField.number;
            gamblers[number].Go(gamblers[number].currentRoundField.number);
            yield return new WaitForSeconds(1);
            StartCoroutine(NumberCheck(number, seconds, gamblers));
        }

        Debug.Log($"{number + 1}-ый сделал ход");
        yield return new WaitForSeconds(1f);
        gamblers[number].isMoving = false;
        if (number < 3)
        {
            Debug.Log("Переход к следующему игроку");
            number++;
            StartCoroutine(RoundThrow(number, seconds));
        }
        else { menu.SetActive(true); Debug.Log("Ход сделан"); }
        //StartCoroutine(CheckForMoving(1.1f));
        //StartCoroutine(CheckForMoving(4.6f));
        //StartCoroutine(CheckForMoving(7.1f));
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
