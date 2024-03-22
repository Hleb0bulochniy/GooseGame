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
    public GameObject menu;
    public Vector3 coords;
    public FieldClass firstRoundField;
    public FieldClass currentRoundField;// = GooseGameBehaviour.GetField(0); //инициализируй в другом месте

    public GameObject bridgeOptions;
    public GameObject dices63Options;
    public GameObject dices45Options;


    public int threwNumber;
    public int orderNumber;
    public int money;
    public int losedMoney = 0;
    public int currentPositionNumber = 0;
    public int GoToCheckNumber = 0;

    public bool canPlay = true;
    public bool won = false;
    public bool mustSkip = false;
    public bool metSomeone = false;
    public bool needsHelp = false;
    public bool isMoving = false;
    public bool isChoising = false;
    public bool choicedToStay = false;
    public bool isInTavern = false;
    public bool readyToLeaveTheTavern = false;
    public bool isInWell = false;
    public bool readyToLeaveTheWell = false;
    public bool isInPrison = false;
    public bool readyToLeaveThePrison = false;

    public void GoToCheckButton()
    {
        currentRoundField = GooseGameBehaviour.GetField(GoToCheckNumber);
        currentPositionNumber = currentRoundField.number;
        Go(GoToCheckNumber);
        //CheckNumber();
    }


    public virtual int Choose() { return 1; }
    public virtual void ChooseBridge()
    {
    }
    public virtual void Choose63()
    {
    }
    public virtual void Choose45()
    {
    }

    void Start()
    {
        //coords = transform.position;
    }

    
    public void WasHelped(int situation)
    {
        //1 - колодец
        //2 - тюрьма
        if (situation == 1)
        {
            currentRoundField = firstRoundField;
            currentPositionNumber = firstRoundField.number;
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
                //currentRoundField = GooseGameBehaviour.GetField(threwNumber + currentRoundField.number);
                //currentPositionNumber = currentRoundField.number;
                Debug.Log($"Перехожу на поле {number}");
                if (number > 63) { number = 63 - Mathf.Abs(63 - number); }
                FieldClass field = GooseGameBehaviour.GetField(number);
                //currentRoundField = field;
                Vector3 targerCoords = currentRoundField.transform.position;
                //transform.position = Vector3.Lerp(transform.position, targerCoords, 2f * Time.deltaTime);
                //transform.position = targerCoords;
                StartCoroutine(SmoothMove(targerCoords, transform.position, 0));
            }
        }
    }


    IEnumerator SmoothMove(Vector3 targetPosition, Vector3 startPosition, float startTime)
    {
        while (transform.position != targetPosition)
        {
            //float distCovered = (Time.time - startTime) * 3f;

            //float fracJourney = distCovered / Vector3.Distance(startPosition, targetPosition);

            transform.position = Vector3.Lerp(startPosition, targetPosition, startTime);
            startTime += Time.deltaTime;

            yield return null;
        }
    }


}