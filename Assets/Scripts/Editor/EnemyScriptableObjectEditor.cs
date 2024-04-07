using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyScriptableObject))]
public class AllEnemyScriptableObjectEditor : Editor
{
    private SerializedProperty enemyTypeProp;
    private SerializedProperty moveSpeedProp;
    private SerializedProperty maxHealthProp;
    private SerializedProperty collisionDamageProp;
    private SerializedProperty shootingDamageProp;
    private SerializedProperty shootingDistanceProp;
    private SerializedProperty projectilePrefabProp;
    private SerializedProperty projectileSpeedProp;
    private SerializedProperty shootingCooldownProp;
    private SerializedProperty knockbackDurationProp;
    private SerializedProperty numberOfSplitsProp;
    private SerializedProperty enemySplitPrefabProp;
    private SerializedProperty enemyPrefabsToSummonProp;
    private SerializedProperty summoningEffectProp;///
    private SerializedProperty summonCooldownProp;
    private SerializedProperty summoningDistanceProp;
    private SerializedProperty minSummonAmountProp;
    private SerializedProperty maxSummonAmountProp;

    private void OnEnable()
    {
        enemyTypeProp = serializedObject.FindProperty("enemyType");
        moveSpeedProp = serializedObject.FindProperty("moveSpeed");
        maxHealthProp = serializedObject.FindProperty("maxHealth");
        collisionDamageProp = serializedObject.FindProperty("collisionDamage");
        shootingDamageProp = serializedObject.FindProperty("shootingDamage");
        shootingDistanceProp = serializedObject.FindProperty("shootingDistance");
        projectilePrefabProp = serializedObject.FindProperty("projectilePrefab");
        projectileSpeedProp = serializedObject.FindProperty("projectileSpeed");
        shootingCooldownProp = serializedObject.FindProperty("shootingCooldown");
        knockbackDurationProp = serializedObject.FindProperty("knockbackDuration");
        numberOfSplitsProp = serializedObject.FindProperty("numberOfSplits");
        enemySplitPrefabProp = serializedObject.FindProperty("enemySplitPrefab");
        enemyPrefabsToSummonProp = serializedObject.FindProperty("enemyPrefabsToSummon");
        summoningEffectProp = serializedObject.FindProperty("summoningEffect");
        summonCooldownProp = serializedObject.FindProperty("summonCooldown");
        summoningDistanceProp = serializedObject.FindProperty("summoningDistance");
        minSummonAmountProp = serializedObject.FindProperty("minSummonAmount");
        maxSummonAmountProp = serializedObject.FindProperty("maxSummonAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(enemyTypeProp);

        EnemyType enemyType = (EnemyType)enemyTypeProp.enumValueIndex;

        switch (enemyType)
        {
            case EnemyType.CollisionEnemy:
                DrawCollisionEnemyFields();
                break;
            case EnemyType.ShootingEnemy:
                DrawShootingEnemyFields();
                break;
            case EnemyType.SplittingEnemy:
                DrawSplittingEnemyFields();
                break;
            case EnemyType.SummonerEnemy:
                DrawSummonerEnemyFields();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCollisionEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
    }

    private void DrawShootingEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
        EditorGUILayout.PropertyField(shootingDamageProp);
        EditorGUILayout.PropertyField(shootingDistanceProp);
        EditorGUILayout.PropertyField(projectilePrefabProp);
        EditorGUILayout.PropertyField(projectileSpeedProp);
        EditorGUILayout.PropertyField(shootingCooldownProp);
        EditorGUILayout.PropertyField(knockbackDurationProp);
    }

    private void DrawSplittingEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
        EditorGUILayout.PropertyField(numberOfSplitsProp);
        EditorGUILayout.PropertyField(enemySplitPrefabProp);
    }

    private void DrawSummonerEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
        EditorGUILayout.PropertyField(enemyPrefabsToSummonProp, true); // true for array children
        EditorGUILayout.PropertyField(summoningEffectProp);
        EditorGUILayout.PropertyField(summonCooldownProp);
        EditorGUILayout.PropertyField(summoningDistanceProp);
        EditorGUILayout.PropertyField(minSummonAmountProp);
        EditorGUILayout.PropertyField(maxSummonAmountProp);
    }
}
