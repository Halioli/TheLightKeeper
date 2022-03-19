using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjective : MonoBehaviour
{
    /*
    1. Return to the Spaceship
    2. Bring 6 Coal to the Core
    3. Upgrade the Pickaxe
    4. Bring Luxinite to the Core
    */


    [SerializeField] string messege;
    [SerializeField] bool isLast = false;


    public delegate void TutorialObjectiveStartAction(string messege);
    public static event TutorialObjectiveStartAction OnObjectiveStart;

    public delegate void TutorialObjectiveEndAction();
    public static event TutorialObjectiveEndAction OnObjectiveEnd;


    protected void InvokeOnObjectiveStart()
    {
        if (OnObjectiveStart != null) OnObjectiveStart(messege);
    }

    protected void InvokeOnObjectiveEnd()
    {
        if (OnObjectiveEnd != null) OnObjectiveEnd();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isLast)
            {
                InvokeOnObjectiveEnd();
            }
            else
            {
                InvokeOnObjectiveStart();
            }
        }
    }


}