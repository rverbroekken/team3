using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelData", menuName = "Levels/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Skill
    }
    public CardType type;
    public Sprite image;
    public string description;

    // XXX: Hidden in inspector because it will be drawn by custom Editor.
    [HideInInspector]
    public CardAbility[] onPlayed;
}

// CardAbility.cs
public abstract class CardAbility : ScriptableObject
{
    public abstract void Resolve();
}

// DrawCards.cs
public class DrawCards : CardAbility
{
    public int numCards = 1;
    public override void Resolve()
    {
//        Deck.instance.DrawCards(numCards);
    }
}

// HealPlayer.cs
public class HealPlayer : CardAbility
{
    public int healAmount = 10;
    public override void Resolve()
    {
  //      Player.instance.Heal(healAmount);
    }
}

// CardDataEditor.cs
[CustomEditor(typeof(CardData))]
[CanEditMultipleObjects]
public class CardDataEditor : Editor
{
    private ReorderableList abilityList;

    private SerializedProperty onPlayedProp;

    private struct AbilityCreationParams
    {
        public string Path;
    }

    public void OnEnable()
    {
        onPlayedProp = serializedObject.FindProperty("onPlayed");

        abilityList = new ReorderableList(
                serializedObject,
                onPlayedProp,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true);

        abilityList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "OnPlayed Abilities");
        };

        abilityList.onRemoveCallback = (ReorderableList l) => {
            var element = l.serializedProperty.GetArrayElementAtIndex(l.index);
            var obj = element.objectReferenceValue;

            AssetDatabase.RemoveObjectFromAsset(obj);

            DestroyImmediate(obj, true);
            l.serializedProperty.DeleteArrayElementAtIndex(l.index);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };

        abilityList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            SerializedProperty element = onPlayedProp.GetArrayElementAtIndex(index);

            rect.y += 2;
            rect.width -= 10;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (element.objectReferenceValue == null)
            {
                return;
            }
            string label = element.objectReferenceValue.name;
            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

            // Convert this element's data to a SerializedObject so we can iterate
            // through each SerializedProperty and render a PropertyField.
            SerializedObject nestedObject = new SerializedObject(element.objectReferenceValue);

            // Loop over all properties and render them
            SerializedProperty prop = nestedObject.GetIterator();
            float y = rect.y;
            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script")
                {
                    continue;
                }

                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, prop);
            }

            nestedObject.ApplyModifiedProperties();

            // Mark edits for saving
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

        };

        abilityList.elementHeightCallback = (int index) => {
            float baseProp = EditorGUI.GetPropertyHeight(
                abilityList.serializedProperty.GetArrayElementAtIndex(index), true);

            float additionalProps = 0;
            SerializedProperty element = onPlayedProp.GetArrayElementAtIndex(index);
            if (element.objectReferenceValue != null)
            {
                SerializedObject ability = new SerializedObject(element.objectReferenceValue);
                SerializedProperty prop = ability.GetIterator();
                while (prop.NextVisible(true))
                {
                    // XXX: This logic stays in sync with loop in drawElementCallback.
                    if (prop.name == "m_Script")
                    {
                        continue;
                    }
                    additionalProps += EditorGUIUtility.singleLineHeight;
                }
            }

            float spacingBetweenElements = EditorGUIUtility.singleLineHeight / 2;

            return baseProp + spacingBetweenElements + additionalProps;
        };

        abilityList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            var guids = AssetDatabase.FindAssets("", new[] { "Assets/CardAbility" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var type = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                if (type.name == "CardAbility")
                {
                    continue;
                }

                menu.AddItem(
                    new GUIContent(Path.GetFileNameWithoutExtension(path)),
                    false,
                    addClickHandler,
                    new AbilityCreationParams() { Path = path });
            }
            menu.ShowAsContext();
        };
    }

    private void addClickHandler(object dataObj)
    {
        // Make room in list
        var data = (AbilityCreationParams)dataObj;
        var index = abilityList.serializedProperty.arraySize;
        abilityList.serializedProperty.arraySize++;
        abilityList.index = index;
        var element = abilityList.serializedProperty.GetArrayElementAtIndex(index);

        // Create the new Ability
        var type = AssetDatabase.LoadAssetAtPath(data.Path, typeof(UnityEngine.Object));
        var newAbility = ScriptableObject.CreateInstance(type.name);
        newAbility.name = type.name;

        // Add it to CardData
        var cardData = (CardData)target;
        AssetDatabase.AddObjectToAsset(newAbility, cardData);
        AssetDatabase.SaveAssets();
        element.objectReferenceValue = newAbility;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        abilityList.DoLayoutList();

        if (GUILayout.Button("Delete All Abilities"))
        {
            var path = AssetDatabase.GetAssetPath(target);
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is CardAbility)
                {
                    Object.DestroyImmediate(assets[i], true);
                }
            }
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
    }
}