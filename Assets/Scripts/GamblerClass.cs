using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GamblerClass : MonoBehaviour
{
    public GameObject gamblerObject;
    public Vector3 coords;
    public FieldClass firstRoundField;
    public FieldClass currentRoundField;// = GooseGameBehaviour.GetField(0); //инициализируй в другом месте

    public GameObject bridgeOptions;
    public GameObject dices63Options;
    public GameObject dices45Options;


    public int threwNumber;
    public int orderNumber;
    public int money;
    public int losedMoney;
    public int currentPositionNumber;

    public bool canPlay = true;
    public bool won = false;
    public bool mustSkip = false;
    public bool metSomeone = false;
    public bool needsHelp = false;

    public virtual int Choose() { return 1; }
    public virtual void ChooseBridge()
    {
        int choice = Random.Range(1,2);
        if (choice != 0)
        {
            if (choice == 1)
            {
                money--;
                losedMoney++;
            }

            if (choice == 2)
            {
                money -= 2;
                losedMoney += 2;
                currentRoundField = GooseGameBehaviour.GetField(12);
                Go(12);
            }
        }
    }
    public virtual void Choose63()
    {
        int choice = Random.Range(1, 3);
        if (choice != 0)
        {
            //если игрок не хочет ходить, то передается 3
            if (choice == 1)
            {
                currentRoundField = GooseGameBehaviour.GetField(3 + currentRoundField.number);
                Go(currentRoundField.number);

            }

            if (choice == 2)
            {
                currentRoundField = GooseGameBehaviour.GetField(6 + currentRoundField.number);
                Go(currentRoundField.number);
                CheckNumber();
            }
        }
    }
    public virtual void Choose45()
    {
        int choice = Random.Range(1, 4);
        if (choice != 0)
        {
            //если игрок не хочет ходить, то передается 4
            if (choice == 1)
            {
                currentRoundField = GooseGameBehaviour.GetField(4 + currentRoundField.number);
                Go(currentRoundField.number);

            }

            if (choice == 2)
            {
                currentRoundField = GooseGameBehaviour.GetField(5 + currentRoundField.number);
                Go(currentRoundField.number);
                CheckNumber();
            }

            if (choice == 3)
            {
                currentRoundField = GooseGameBehaviour.GetField(9 + currentRoundField.number);
                Go(currentRoundField.number);

            }
        }
    }

    void Start()
    {
        //coords = transform.position;
    }

    public void CheckNumber()
    {
        if (currentRoundField != null)
        {
            //гусь
            if (currentRoundField.number == 5 | currentRoundField.number == 9 | currentRoundField.number == 14 | currentRoundField.number == 18 | currentRoundField.number == 23 | currentRoundField.number == 27 | currentRoundField.number == 32 | currentRoundField.number == 36 | currentRoundField.number == 41 | currentRoundField.number == 45 | currentRoundField.number == 50 | currentRoundField.number == 54 | currentRoundField.number == 59)
            {
                Debug.Log("Кто-то попал в гуся");
                currentRoundField = GooseGameBehaviour.GetField(threwNumber + currentRoundField.number);
                Go(currentRoundField.number);
                CheckNumber();
            }

            //мост
            else if (currentRoundField.number == 6)
            {
                Debug.Log("Кто-то попал в мост");
                ChooseBridge();
                /*int choice = Choose();
                //while (choice != 1 | choice != 2) { choice = Choose(); }
                if (choice != 0)
                {
                    if(choice == 1)
                    {
                        money--;
                        losedMoney++;
                    }

                    if (choice == 2)
                    {
                        money -= 2;
                        losedMoney += 2;
                        currentRoundField = GooseGameBehaviour.GetField(12);
                        Go(12);
                    }
                }*/
            }

            //трактир
            else if (currentRoundField.number == 19)
            {
                Debug.Log("Кто-то попал в трактир");
                mustSkip = true;
                money -= 2;
                losedMoney += 2;
            }

            //колодец
            else if (currentRoundField.number == 31)
            {
                Debug.Log("Кто-то попал в колодец");
                money -= 3;
                losedMoney += 3;
                mustSkip = true;
                needsHelp = true;
            }

            //кубики 6 и 3
            else if (currentRoundField.number == 26)
            {
                Debug.Log("Кто-то попал в 63");
                Choose63();
                /*int choice = Choose();
                while (choice != 1 | choice != 2 | choice != 3) { choice = Choose(); }
                if (choice != 0)
                {
                    //если игрок не хочет ходить, то передается 3
                    if (choice == 1)
                    {
                        currentRoundField = GooseGameBehaviour.GetField(3 + currentRoundField.number);
                        Go(currentRoundField.number);

                    }

                    if (choice == 2)
                    {
                        currentRoundField = GooseGameBehaviour.GetField(6 + currentRoundField.number);
                        Go(currentRoundField.number);
                        CheckNumber();
                    }
                }*/
            }

            //лабиринт
            else if (currentRoundField.number == 42)
            {
                Debug.Log("Кто-то попал в лабиринт");
                money--;
                losedMoney++;
                currentRoundField = GooseGameBehaviour.GetField(39);
                Go(39);

            }

            //тюрьма
            else if (currentRoundField.number == 52)
            {
                Debug.Log("Кто-то попал в тюрьмы");
                money -= 2;
                losedMoney += 2;
                mustSkip = true;
                needsHelp = true;
            }

            //кубики 4 и 5
            else if (currentRoundField.number == 53)
            {
                Debug.Log("Кто-то попал в 45");
                Choose45();
                /*int choice = Choose();
                while (choice != 1 | choice != 2 | choice != 3 | choice != 4) { choice = Choose(); }
                if (choice != 0)
                {
                    //если игрок не хочет ходить, то передается 4
                    if (choice == 1)
                    {
                        currentRoundField = GooseGameBehaviour.GetField(4 + currentRoundField.number);
                        Go(currentRoundField.number);

                    }

                    if (choice == 2)
                    {
                        currentRoundField = GooseGameBehaviour.GetField(5 + currentRoundField.number);
                        Go(currentRoundField.number);
                        CheckNumber();
                    }

                    if (choice == 3)
                    {
                        currentRoundField = GooseGameBehaviour.GetField(9 + currentRoundField.number);
                        Go(currentRoundField.number);

                    }
                }*/
            }

            //смерть
            else if (currentRoundField.number == 58)
            {
                Debug.Log("Кто-то попал в смерть");
                money--;
                losedMoney++;
                currentRoundField = GooseGameBehaviour.GetField(1);
                Go(1);

            }

            //победа
            else if (currentRoundField.number == 63)
            {
                Debug.Log("Кто-то попал в победу");
                won = true;
            }
            else { }
        }
    }

    public void WasHelped(int situation)
    {
        //1 - колодец
        //2 - тюрьма
        if (situation == 1)
        {
            Go(firstRoundField.number);
            mustSkip = false;
            needsHelp = false;
        }
        else if (situation == 2)
        {
            mustSkip = false;
            needsHelp = false;
        }
    }

    public void ThrowDices()
    {
        //можно кидать и два кубика, но зачем...
        threwNumber = Random.Range(1, 12);
    }

    public virtual void Go(int number)
    {
        if (canPlay)
        {
            if (mustSkip)
            {
                if(!needsHelp) //Если не в колодце и не в тюрьме
                {
                    mustSkip = false;
                }
            }
            else
            {
                Debug.Log($"Перехожу на поле {number}");
                if (number > 63) { number = 63 - Mathf.Abs(63 - number); }
                FieldClass field = GooseGameBehaviour.GetField(number);
                currentRoundField = field;
                Vector3 targerCoords;
                targerCoords = field.transform.position;
                //transform.position = Vector3.Lerp(transform.position, targerCoords, 2f * Time.deltaTime);
                //transform.position = targerCoords;
                StartCoroutine(SmoothMove(targerCoords, transform.position, Time.time));
            }
        }
    }


    IEnumerator SmoothMove(Vector3 targetPosition, Vector3 startPosition, float startTime)
    {
        while (transform.position != targetPosition)
        {
            float distCovered = (Time.time - startTime) * 3f;

            float fracJourney = distCovered / Vector3.Distance(startPosition, targetPosition);

            transform.position = Vector3.Lerp(startPosition, targetPosition, fracJourney);

            yield return null;
        }
    }
}
