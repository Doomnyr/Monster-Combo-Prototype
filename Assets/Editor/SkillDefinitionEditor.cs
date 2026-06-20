#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDefinitionSO))]
public class SkillDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Draw the normal inspector fields (Name, Description, Mana Cost, etc.)
        DrawDefaultInspector();

        SkillDefinitionSO skill = (SkillDefinitionSO)target;

        GUILayout.Space(20);
        GUILayout.Label("Data-Driven Effect Factory", EditorStyles.boldLabel);

        // 2. Add custom buttons to safely inject instances into your list
        if (GUILayout.Button("＋ Add Damage Effect to List"))
        {
            Undo.RecordObject(skill, "Add Damage Effect");
            skill.ExecutionEffects.Add(new DamageEffect());
            EditorUtility.SetDirty(skill);
        }

        if (GUILayout.Button("＋ Add Gain Mana Effect to List"))
        {
            Undo.RecordObject(skill, "Add Gain Mana Effect");
            skill.ExecutionEffects.Add(new ModifyManaEffect());
            EditorUtility.SetDirty(skill);
        }
    }
}
#endif