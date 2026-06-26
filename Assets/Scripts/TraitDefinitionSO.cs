using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrait", menuName = "MonsterCombo/Trait Definition")]
public class TraitDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    public string traitName;
    [TextArea] public string description;

    [Header("Active Triggers")]
    [Tooltip("Actions that happen at specific moments (e.g., Start of Turn).")]
    public List<BuffTriggerData> triggeredActions = new List<BuffTriggerData>();
}