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
    PlayerClass player;
    EnemyClass enemy1;
    EnemyClass enemy2;
    EnemyClass enemy3;

    public GameObject startFirstGameMenu;
    public GameObject doOneRoundMenu;
    public GameObject newGameStartMenu;

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
        startFirstGameMenu.SetActive(true);
    }

    public void GameStart()
    {
        startFirstGameMenu.SetActive(false);
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
                    gamblers[i].ThrowDices();
                    gamblers[i - 1].ThrowDices();
                    Array.Sort(gamblers, (x, y) => y.threwNumber.CompareTo(x.threwNumber));
                }
            }
        }
        for(int i = 0; i < 4; i++)
        {
            gamblers[i].orderNumber = i+1;
        }
        firstRound = true;
        Debug.Log("Первая игра");
        doOneRoundMenu.SetActive(true);
    }

    public void GameStartNextRound()
    {
        int k = 0;
        newGameStartMenu.SetActive(false);
        GamblerClass[] gamblersAll = new GamblerClass[4] { player, enemy1, enemy2, enemy3 };
        GamblerClass[] gamblers = new GamblerClass[3];

        firstRound = true;
        for (int i = 0; i < gamblersAll.Count(); i++)
        {
            gamblersAll[i].currentRoundField = fields[0].GetComponent<FieldClass>();
            gamblersAll[i].currentPositionNumber = gamblersAll[i].currentRoundField.number;
            gamblersAll[i].Go(0);
            if (!gamblersAll[i].won) { gamblers[k] = gamblersAll[i]; k++; }
            else { gamblersAll[i].orderNumber = 1; }
            gamblersAll[i].won = false;
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
                    gamblers[i].ThrowDices();
                    gamblers[i - 1].ThrowDices();
                    Array.Sort(gamblers, (x, y) => y.threwNumber.CompareTo(x.threwNumber));
                }
            }
        }
        for (int i = 0; i < gamblers.Length; i++)
        {
            gamblers[i].orderNumber = i + 2; //+2 потому что первое место уже занято
            Debug.Log("10");
        }
        
        Debug.Log("Следующая игра");
        doOneRoundMenu.SetActive(true);
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
            gamblers[i].canPlay = true;

            gamblers[i].mustSkip = false;
            gamblers[i].metSomeone = false;
            gamblers[i].needsHelp = false;
            gamblers[i].isMoving = false;
            gamblers[i].isChoising = false;
            gamblers[i].choicedToStay = false;
            gamblers[i].isInTavern = false;
            gamblers[i].readyToLeaveTheTavern = false;
            gamblers[i].isInWell = false;
            gamblers[i].readyToLeaveTheWell = false;
            gamblers[i].isInPrison = false;
            gamblers[i].readyToLeaveThePrison = false;

            gamblers[i].losedMoney = 0;
            if (gamblers[i].won) { gamblers[i].money += totalLosedMoney; Debug.Log($"{i + 1} ПОБЕДИЛ"); }
        }
        newGameStartMenu.SetActive( true );
    }

    //можно перевести в ивент. Чтобы сначала ходил первый. Потом вызывалась проверка первого. Потом все остальное. Потом ходил второй и т.д.
    public void DoOneRound()
    {

        Debug.Log("Ход начат");
        StartCoroutine(RoundThrow(0, 1f));
    }




    //СПРОСИТЬ У ФАТИМЫ Что если игрок попал мост, где уже кто-то стоял?
    //СПРОСИТЬ У ФАТИМЫ если игрок на 44, а враг на 62. Игроку выпадает 10, он попадает в гуся. Из него на 62, и начинается бесконечный цикл.
    //ПЕРЕНЕСТИ ВСЕ БУЛЫ ИГРОКОВ В GAMEEND
    //баг при попадании на мост, если остаться, ничего дальше не идет


    //БАГ ISINTAVERN И READYTOLEAVE ОСТАЮТСЯ ВКЛЮЧЕННЫМИ НАВСЕГДА
    //БАГ ДЕНЬГИ НЕПРАВИЛЬНО РАСПРЕДЕЛИЛИСЬ
    //БАГ ЕСЛИ ИГРОК ПОПАДАЕТ В ТЮРЬМУ ИЛИ, ТО КАЖДЫЙ РАЗ ИТЕРАЦИОННО СНОВА ОКАЗЫВАЕТСЯ В НЕЙ. ПРОВЕРИТЬ, РАБОТАЕТ ЛИ КОД ДЛЯ ТАВЕРНЫ И ИНТЕРПРИТИРОВАТЬ ЕГО ДЛЯ НИХ
    //БАГ ИГРОК ПОПАЛ НА ДРУГОГО ИГРОКА В ТАВЕРНЕ, УЖЕ В СЛЕДУЮЩЕМ РАУНДЕ ОН САМ СХОДИЛ. ЕМУ ВЫПАЛО 9, А ОН ПЕРЕМЕСТИЛСЯ НА 34
    // Update is called once per frame

    //при возвращении из колодца игрок не только ходит на свою изначальную клетку, но и сразу на свое выпавшее число. все происходит одновременно, при начале хода 
    //откуда-то появляются chosiedtostay

    //баг при нажатии кнопки второго раунда все зависает
    IEnumerator RoundThrow(int number, float seconds)
    {
        doOneRoundMenu.SetActive(false);
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
                gamblers[number].isChoising = true;
                gamblers[number].ChooseBridge();
                while (gamblers[number].isChoising) { yield return 0.5f; }
                yield return new WaitForSeconds(1f); 
                StartCoroutine(MetCheck(number, seconds, gamblers));
            }

            //трактир
            else if (gamblers[number].currentRoundField.number == 19)
            {
                if (gamblers[number].isInTavern)
                {
                    gamblers[number].readyToLeaveTheTavern = true;
                }
                
                if (!gamblers[number].isInTavern & !gamblers[number].readyToLeaveTheTavern)
                {
                    gamblers[number].isInTavern = true;
                    Debug.Log("Кто-то попал в трактир");
                    gamblers[number].mustSkip = true;
                    gamblers[number].money -= 2;
                    gamblers[number].losedMoney += 2;
                }

                if(gamblers[number].isInTavern & gamblers[number].readyToLeaveTheTavern)
                {
                    gamblers[number].isInTavern = false;
                    gamblers[number].readyToLeaveTheTavern = false;
                }
                
                yield return new WaitForSeconds(1f);
                StartCoroutine(MetCheck(number, seconds, gamblers));
            }

            //колодец
            else if (gamblers[number].currentRoundField.number == 31)
            {
                if (!gamblers[number].isInWell)
                {
                    gamblers[number].isInWell = true;
                    Debug.Log("Кто-то попал в колодец");
                    gamblers[number].money -= 3;
                    gamblers[number].losedMoney += 3;
                    gamblers[number].mustSkip = true;
                    gamblers[number].needsHelp = true;
                }

                yield return new WaitForSeconds(1f);
                StartCoroutine(MetCheck(number, seconds, gamblers));
            }

            //кубики 6 и 3
            else if (gamblers[number].currentRoundField.number == 26)
            {
                Debug.Log("Кто-то попал в 63");
                gamblers[number].isChoising = true;
                gamblers[number].Choose63();
                while (gamblers[number].isChoising) { yield return 0.5f;}
                if (gamblers[number].choicedToStay) { gamblers[number].choicedToStay = false; StartCoroutine(MetCheck(number, seconds, gamblers)); }
                else { StartCoroutine(NumberCheck(number, seconds, gamblers)); }
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
                yield return new WaitForSeconds(1f);
                StartCoroutine(MetCheck(number, seconds, gamblers));
            }

            //тюрьма
            else if (gamblers[number].currentRoundField.number == 52)
            {
                if (!gamblers[number].isInPrison)
                {
                    gamblers[number].isInPrison = true;
                    Debug.Log("Кто-то попал в тюрьму");
                    gamblers[number].money -= 2;
                    gamblers[number].losedMoney += 2;
                    gamblers[number].mustSkip = true;
                    gamblers[number].needsHelp = true;
                }

                yield return new WaitForSeconds(1f);
                StartCoroutine(MetCheck(number, seconds, gamblers));
            }

            //кубики 4 и 5
            else if (gamblers[number].currentRoundField.number == 53)
            {
                Debug.Log("Кто-то попал в 45");
                gamblers[number].isChoising = true;
                gamblers[number].Choose45();
                while (gamblers[number].isChoising) { yield return 0.5f; }
                StartCoroutine(NumberCheck(number, seconds, gamblers));
                if (gamblers[number].choicedToStay) { gamblers[number].choicedToStay = false; StartCoroutine(MetCheck(number, seconds, gamblers)); }
                else { StartCoroutine(NumberCheck(number, seconds, gamblers)); }
            }

            //смерть
            else if (gamblers[number].currentRoundField.number == 58)
            {
                Debug.Log("Кто-то попал в смерть");
                gamblers[number].money--;
                gamblers[number].losedMoney++;
                gamblers[number].currentRoundField = GetField(1);
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
            else { yield return new WaitForSeconds(1f); StartCoroutine(MetCheck(number, seconds, gamblers)); }
        }
    }


    IEnumerator MetCheck(int number, float seconds, GamblerClass[] gamblers)
    {
        if (firstRound)
        {
            gamblers[number].firstRoundField = gamblers[number].currentRoundField;
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
                    gamblers[j].isInWell = false;
                    gamblers[j].isInPrison = false;
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
        else
        {
            Debug.Log($"{number + 1}-ый сделал ход");
            yield return new WaitForSeconds(1f);
            gamblers[number].isMoving = false;
            if (number < 3)
            {
                Debug.Log("Переход к следующему игроку");
                number++;
                StartCoroutine(RoundThrow(number, seconds));
            }
            else { doOneRoundMenu.SetActive(true); Debug.Log("Ход сделан"); firstRound = false; }
        }
    }


    void Update()
    {
        
    }
}
