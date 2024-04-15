using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyScriptableObject))]
public class AllEnemyScriptableObjectEditor : Editor
{
    //Base Properties
    private SerializedProperty enemyTypeProp;
    private SerializedProperty moveSpeedProp;
    private SerializedProperty maxHealthProp;

    // Properties for ShootingEnemy
    private SerializedProperty collisionDamageProp;
    private SerializedProperty shootingDamageProp;
    private SerializedProperty shootingDistanceProp;
    private SerializedProperty projectilePrefabProp;
    private SerializedProperty projectileSpeedProp;
    private SerializedProperty shootingCooldownProp;
    private SerializedProperty knockbackDurationProp;

    // Properties for SplittingEnemy
    private SerializedProperty numberOfSplitsProp;
    private SerializedProperty enemySplitPrefabProp;

    // Properties for SummonerEnemy
    private SerializedProperty enemyPrefabsToSummonProp;
    private SerializedProperty summonCooldownProp;
    private SerializedProperty summoningDistanceProp;
    private SerializedProperty minSummonAmountProp;
    private SerializedProperty maxSummonAmountProp;

    // Properties for ChargingEnemy
    private SerializedProperty chargingDistanceProp;
    private SerializedProperty pauseDurationProp;
    private SerializedProperty increasedSpeedProp;
    private SerializedProperty chargeDurationProp;
    private SerializedProperty chargeCooldownProp;

    // Properties for DashEnemy
    private SerializedProperty dashDistanceProp;
    private SerializedProperty dashSpeedProp;
    private SerializedProperty dashCooldownProp;

    // Properties for ExplodingEnemy
    private SerializedProperty explosionRadiusProp;
    private SerializedProperty explosionStartDistanceProp;
    private SerializedProperty explosionDamageProp;
    private SerializedProperty movementSpeedIncreaseProp;
    private SerializedProperty flashColorProp;
    private SerializedProperty flashDurationProp;
    private SerializedProperty explosionDurationProp;
    private SerializedProperty spiralRadiusIncreaseRateProp;

    private void OnEnable()
{
    enemyTypeProp = serializedObject.FindProperty("enemyType");

    // Base properties for all enemy types
    moveSpeedProp = serializedObject.FindProperty("moveSpeed");
    maxHealthProp = serializedObject.FindProperty("maxHealth");
    collisionDamageProp = serializedObject.FindProperty("collisionDamage");

    // Properties for ShootingEnemy
    shootingDamageProp = serializedObject.FindProperty("shootingDamage");
    shootingDistanceProp = serializedObject.FindProperty("shootingDistance");
    projectilePrefabProp = serializedObject.FindProperty("projectilePrefab");
    projectileSpeedProp = serializedObject.FindProperty("projectileSpeed");
    shootingCooldownProp = serializedObject.FindProperty("shootingCooldown");
    knockbackDurationProp = serializedObject.FindProperty("knockbackDuration");

    // Properties for SplittingEnemy
    numberOfSplitsProp = serializedObject.FindProperty("numberOfSplits");
    enemySplitPrefabProp = serializedObject.FindProperty("enemySplitPrefab");

    // Properties for SummonerEnemy
    enemyPrefabsToSummonProp = serializedObject.FindProperty("enemyPrefabsToSummon");
    summonCooldownProp = serializedObject.FindProperty("summonCooldown");
    summoningDistanceProp = serializedObject.FindProperty("summoningDistance");
    minSummonAmountProp = serializedObject.FindProperty("minSummonAmount");
    maxSummonAmountProp = serializedObject.FindProperty("maxSummonAmount");

    // Properties for ChargingEnemy
    chargingDistanceProp = serializedObject.FindProperty("chargingDistance");
    pauseDurationProp = serializedObject.FindProperty("pauseDuration");
    increasedSpeedProp = serializedObject.FindProperty("increasedSpeed");
    chargeDurationProp = serializedObject.FindProperty("chargeDuration");
    chargeCooldownProp = serializedObject.FindProperty("chargeCooldown");

    // Properties for DashEnemy
    dashDistanceProp = serializedObject.FindProperty("dashDistance");
    dashSpeedProp = serializedObject.FindProperty("dashSpeed");
    dashCooldownProp = serializedObject.FindProperty("dashCooldown");

    // Properties for ExplodingEnemy
    explosionRadiusProp = serializedObject.FindProperty("explosionRadius");
    explosionStartDistanceProp = serializedObject.FindProperty("explosionStartDistance");
    explosionDamageProp = serializedObject.FindProperty("explosionDamage");
    movementSpeedIncreaseProp = serializedObject.FindProperty("movementSpeedIncrease");
    flashColorProp = serializedObject.FindProperty("flashColor");
    flashDurationProp = serializedObject.FindProperty("flashDuration");
    explosionDurationProp = serializedObject.FindProperty("explosionDuration");
    spiralRadiusIncreaseRateProp = serializedObject.FindProperty("spiralRadiusIncreaseRate");

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
            case EnemyType.ChargingEnemy: // Draw ChargingEnemy fields
                DrawChargingEnemyFields();
                break;
            case EnemyType.DashEnemy:
                DrawDashEnemyFields();
                break;
            case EnemyType.ExplodingEnemy:
                DrawExplodingEnemyFields();
                break;
            case EnemyType.BallEnemy:
                DrawBallEnemyFields();
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
        EditorGUILayout.PropertyField(enemyPrefabsToSummonProp, true);
        EditorGUILayout.PropertyField(summonCooldownProp);
        EditorGUILayout.PropertyField(summoningDistanceProp);
        EditorGUILayout.PropertyField(minSummonAmountProp);
        EditorGUILayout.PropertyField(maxSummonAmountProp);
    }

    private void DrawChargingEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("ChargingEnemy Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(chargingDistanceProp);
        EditorGUILayout.PropertyField(pauseDurationProp);
        EditorGUILayout.PropertyField(increasedSpeedProp);
        EditorGUILayout.PropertyField(chargeDurationProp);
        EditorGUILayout.PropertyField(chargeCooldownProp);
    }

    private void DrawDashEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
        EditorGUILayout.PropertyField(dashDistanceProp);
        EditorGUILayout.PropertyField(dashSpeedProp);
        EditorGUILayout.PropertyField(dashCooldownProp);
    }

    private void DrawExplodingEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
        EditorGUILayout.PropertyField(explosionRadiusProp);
        EditorGUILayout.PropertyField(explosionStartDistanceProp);
        EditorGUILayout.PropertyField(explosionDamageProp);
        EditorGUILayout.PropertyField(movementSpeedIncreaseProp);
        EditorGUILayout.PropertyField(flashColorProp);
        EditorGUILayout.PropertyField(flashDurationProp);
        EditorGUILayout.PropertyField(explosionDurationProp);
        EditorGUILayout.PropertyField(spiralRadiusIncreaseRateProp);
    }

    private void DrawBallEnemyFields()
    {
        EditorGUILayout.PropertyField(moveSpeedProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(collisionDamageProp);
    }

}

