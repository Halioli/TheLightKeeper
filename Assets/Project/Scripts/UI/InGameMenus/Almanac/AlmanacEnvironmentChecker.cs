using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmanacEnvironmentChecker : MonoBehaviour
{
    Dictionary<int, AlmanacScriptableObject> materialsChecklist;

    public int[] IDs;
    public AlmanacScriptableObject[] almanacScriptableObjectMaterials;

    private void Awake()
    {
        materialsChecklist = new Dictionary<int, AlmanacScriptableObject>();
        for (int i = 0; i < IDs.Length; ++i)
        {
            materialsChecklist.Add(IDs[i], almanacScriptableObjectMaterials[i]);
        }
    }


    private void OnEnable()
    {
        AlmanacEnvironmentTrigger.OnEnvironmentTrigger += CheckItemID;
    }

    private void OnDisable()
    {
        AlmanacEnvironmentTrigger.OnEnvironmentTrigger -= CheckItemID;
    }


    private void CheckItemID(int ID)
    {
        bool isNew = !materialsChecklist[ID].hasBeenFound;

        if (isNew)
        {
            materialsChecklist[ID].hasBeenFound = true;
        }

    }


}
