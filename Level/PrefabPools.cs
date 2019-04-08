using UnityEngine;
using System.Collections.Generic;

public class PrefabPools : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject matchTextPrefab;
    public GameObject explosionPrefab;
    public GameObject armamentSecondaryPrefab;
    private int armamentSecondaryTotal = 0;
    public GameObject armamentPrimaryPrefab;
    public GameObject enemyProjectilePrefab;
    public GameObject abilityRocketBarragePrefab;
    public GameObject moduleMinesPrefab;
    public GameObject moduleShatterPrefab;
    public GameObject textDamagePrefab;
    public GameObject asteroidPrefab;
    public GameObject iconPhaseOutPrefab;
    public GameObject iconBombPrefab;
    public GameObject enemyGnatPrefab;
    public GameObject enemyExploderPrefab;
    public Stack<MatchText> stackMatchText = new Stack<MatchText>();
    public Stack<Explosion> stackExplosion = new Stack<Explosion>();
    public Stack<ArmamentSecondary> stackArmamentSecondary = new Stack<ArmamentSecondary>();
    public Stack<ArmamentPrimary> stackArmamentPrimary = new Stack<ArmamentPrimary>();
    public Stack<EnemyProjectile> stackEnemyProjectile = new Stack<EnemyProjectile>();
    public Stack<AbilityRocketBarrage> stackAbilityRocketBarrage = new Stack<AbilityRocketBarrage>();
    public Stack<ModuleMines> stackModuleMines = new Stack<ModuleMines>();
    public Stack<ModuleShatter> stackModuleShatter = new Stack<ModuleShatter>();
    public Stack<TextDamage> stackTextDamage = new Stack<TextDamage>();
    public Stack<Asteroid> stackAsteroid = new Stack<Asteroid>();
    public Stack<IconPhaseOut> stackIconPhaseOut = new Stack<IconPhaseOut>();
    public Stack<IconBomb> stackIconBomb = new Stack<IconBomb>();
    public Stack<Enemy> stackEnemyGnat = new Stack<Enemy>();
    public Stack<Enemy> stackEnemyExploder = new Stack<Enemy>();
    private Explosion explosion;

    public void GeneratePrefabPools()
    {
        GenerateMatchTextPool(15);
        GenerateExplosionPool(15);
        GenerateArmamentSecondaryPool(4);
        GenerateArmamentPrimaryPool(4);
        GenerateEnemyProjectilePool(5);
        GenerateTextDamagePool(10);
        GenerateAsteroidPool(5);
        GenerateIconPhaseOutPool(10);
        GenerateIconBombPool(2);
    }

    public void GenerateAbilityPrefabPools(int whichLevel)
    {
        if (PlayerStats.playerStats.moduleSelection[0] == 0 && stackAbilityRocketBarrage.Count <= 0)
        {
            GenerateAbilityRocketBarragePool(5);
        }
        else if (PlayerStats.playerStats.moduleSelection[0] == 4 && stackModuleMines.Count <= 0)
        {
            GenerateModuleMinesPool(5);
        }
        if (PlayerStats.playerStats.moduleSelection[2] == 4 && stackModuleShatter.Count <= 0)
        {
            GenerateModuleShatterPool(7);
        }
        if (whichLevel == 5 && stackEnemyExploder.Count <= 0)
        {
            GenerateEnemyExploderPool(2);
        }
        else if (whichLevel >= 7 && stackEnemyGnat.Count <= 0)
        {
            GenerateEnemyGnatPool(7);
        }
    }

    public void GenerateMatchTextPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(matchTextPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            MatchText matchText = instantiatedObject.GetComponent<MatchText>();
            matchText.prefabPools = this;
            matchText.SetSprites();
            matchText.PushOnStack();
        }
    }

    void GenerateArmamentSecondaryPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(armamentSecondaryPrefab, transform, 0.0f, 30.0f, 0.0f, 0.4f, 0.4f, 1.0f);
            ArmamentSecondary armamentSecondary = instantiatedObject.GetComponent<ArmamentSecondary>();
            armamentSecondary.prefabPools = this;
            armamentSecondary.StartSetup(armamentSecondaryTotal);
            //armamentSecondary.armamentId = armamentSecondaryTotal;
            armamentSecondaryTotal++;
            //armamentSecondary.PushOnStack();
        }
    }

    void GenerateArmamentPrimaryPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(armamentPrimaryPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            ArmamentPrimary armamentPrimary = instantiatedObject.GetComponent<ArmamentPrimary>();
            armamentPrimary.prefabPools = this;
            armamentPrimary.StartSetup();
            //armamentPrimary.PushOnStack();
        }
    }

    void GenerateEnemyProjectilePool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(enemyProjectilePrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            EnemyProjectile enemyProjectile = instantiatedObject.GetComponent<EnemyProjectile>();
            enemyProjectile.prefabPools = this;
            enemyProjectile.enabled = true;
            enemyProjectile.StartSetup();
            stackEnemyProjectile.Push(enemyProjectile);
            enemyProjectile.enabled = false;
            //enemyProjectile.PushOnStack();
        }
    }

    void GenerateAbilityRocketBarragePool(int quantity)
    {
        print("Created " + quantity + " AbilityRocketBarrage");

        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(abilityRocketBarragePrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            AbilityRocketBarrage abilityRocketBarrage = instantiatedObject.GetComponent<AbilityRocketBarrage>();
            abilityRocketBarrage.prefabPools = this;
            abilityRocketBarrage.gameManager = gameManager;
            abilityRocketBarrage.StartSetup();
            //abilityRocketBarrage.PushOnStack();
        }
    }

    void GenerateModuleMinesPool(int quantity)
    {
        print("Created " + quantity + " ModuleMines");

        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(moduleMinesPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            ModuleMines moduleMines = instantiatedObject.GetComponent<ModuleMines>();
            moduleMines.prefabPools = this;
            moduleMines.StartSetup();
        }
    }

    void GenerateModuleShatterPool(int quantity)
    {
        print("Created " + quantity + " ModuleShatter");

        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(moduleShatterPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            ModuleShatter moduleShatter = instantiatedObject.GetComponent<ModuleShatter>();
            moduleShatter.prefabPools = this;
            //moduleShatter.StartSetup();
            moduleShatter.PushOnStack();
        }
    }

    void GenerateExplosionPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(explosionPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            Explosion explosion = instantiatedObject.GetComponent<Explosion>();
            explosion.prefabPools = this;
            explosion.PushOnStack();
        }
    }

    void GenerateTextDamagePool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(textDamagePrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            TextDamage textDamage = instantiatedObject.GetComponent<TextDamage>();
            textDamage.prefabPools = this;
            textDamage.StartSetup();
            textDamage.enabled = false;
            stackTextDamage.Push(textDamage);
        }
    }

    void GenerateAsteroidPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(asteroidPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            Asteroid asteroid = instantiatedObject.GetComponent<Asteroid>();
            asteroid.space = gameManager.space;
            asteroid.StartSetup();
        }
    }

    void GenerateIconPhaseOutPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(iconPhaseOutPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            IconPhaseOut iconPhaseOut = instantiatedObject.GetComponent<IconPhaseOut>();
            iconPhaseOut.prefabPools = this;
            iconPhaseOut.enabled = false;
            stackIconPhaseOut.Push(iconPhaseOut);
        }
    }

    void GenerateIconBombPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(iconBombPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            IconBomb iconBomb = instantiatedObject.GetComponent<IconBomb>();
            iconBomb.prefabPools = this;
            iconBomb.ship = gameManager.ship;
            iconBomb.board = gameManager.board;
            iconBomb.enabled = false;
            stackIconBomb.Push(iconBomb);
        }
    }

    void GenerateEnemyGnatPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(enemyGnatPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            Enemy enemyGnat = instantiatedObject.GetComponent<Enemy>();
            //enemyGnat.prefabPools = this;
            enemyGnat.space = gameManager.space;
            enemyGnat.StartSetup();
            enemyGnat.gameObject.SetActive(false);
            stackEnemyGnat.Push(enemyGnat);
        }
    }

    void GenerateEnemyExploderPool(int quantity)
    {
        for (int currentObject = 0; currentObject < quantity; currentObject++)
        {
            GameObject instantiatedObject = gameManager.InstantiateObject(enemyExploderPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
            Enemy enemyExploder = instantiatedObject.GetComponent<Enemy>();
            enemyExploder.space = gameManager.space;
            enemyExploder.StartSetup();
            enemyExploder.gameObject.SetActive(false);
            stackEnemyExploder.Push(enemyExploder);
        }
    }

    public MatchText PopMatchText()
    {
        if (stackMatchText.Count <= 0)
        {
            print("Created " + 1 + " MatchText");
            GenerateMatchTextPool(1);
        }

        return stackMatchText.Pop();
    }

    public ArmamentSecondary PopArmamentSecondary()
    {
        if (stackArmamentSecondary.Count <= 0)
        {
            print("Created " + 1 + " ArmamentSecondary");
            GenerateArmamentSecondaryPool(1);
        }

        return stackArmamentSecondary.Pop();
    }

    public ArmamentPrimary PopArmamentPrimary()
    {
        if (stackArmamentPrimary.Count <= 0)
        {
            print("Created " + 1 + " ArmamentPrimary");
            GenerateArmamentPrimaryPool(1);
        }

        return stackArmamentPrimary.Pop();
    }

    public EnemyProjectile PopEnemyProjectile()
    {
        if (stackEnemyProjectile.Count <= 0)
        {
            print("Created " + 1 + " EnemyProjectile");
            GenerateEnemyProjectilePool(1);
        }

        EnemyProjectile poppedProjectile;
        do
        {
            poppedProjectile = stackEnemyProjectile.Pop();
        } while (poppedProjectile.enabled);
        return poppedProjectile;
    }

    public AbilityRocketBarrage PopAbilityRocketBarrage()
    {
        if (stackAbilityRocketBarrage.Count <= 0)
        {
            GenerateAbilityRocketBarragePool(1);
        }

        return stackAbilityRocketBarrage.Pop();
    }

    public ModuleMines PopModuleMines()
    {
        if (stackModuleMines.Count <= 0)
        {
            GenerateModuleMinesPool(1);
        }
        else if (stackModuleMines.Count == 1)
        {
            gameManager.missionTracker.IncrementMissionProgress(22, 1);
        }
        return stackModuleMines.Pop();
    }

    public ModuleShatter PopModuleShatter()
    {
        if (stackModuleShatter.Count <= 0)
        {
            GenerateModuleShatterPool(1);
        }

        return stackModuleShatter.Pop();
    }

    public TextDamage PopTextDamage()
    {
        if (stackTextDamage.Count <= 0)
        {
            print("Created " + 1 + " TextDamage");
            GenerateTextDamagePool(1);
        }

        return stackTextDamage.Pop();
    }

    public Asteroid PopAsteroid()
    {
        if (stackAsteroid.Count <= 0)
        {
            print("Created " + 1 + " Asteroid");
            GenerateAsteroidPool(1);
        }

        return stackAsteroid.Pop();
    }

    public IconPhaseOut PopIconPhaseOut()
    {
        if (stackIconPhaseOut.Count <= 0)
        {
            print("Created " + 1 + " IconPhaseOut");
            GenerateIconPhaseOutPool(1);
        }

        return stackIconPhaseOut.Pop();
    }

    public IconBomb PopIconBomb()
    {
        if (stackIconBomb.Count <= 0)
        {
            print("Created " + 1 + " IconBomb");
            GenerateIconBombPool(1);
        }

        return stackIconBomb.Pop();
    }

    public Enemy PopEnemyGnat()
    {
        if (stackEnemyGnat.Count <= 0)
        {
            print("Created " + 1 + " EnemyGnat");
            GenerateEnemyGnatPool(1);
        }

        return stackEnemyGnat.Pop();
    }

    public Enemy PopEnemyExploder()
    {
        if (stackEnemyExploder.Count <= 0)
        {
            print("Created " + 1 + " EnemyExploder");
            GenerateEnemyExploderPool(1);
        }

        return stackEnemyExploder.Pop();
    }

    public void CreateExplosion(Transform explodingObject, float explosionSizeMultiplier)
    {
        if (stackExplosion.Count <= 0)
        {
            print("Created " + 1 + " Explosion");
            GenerateExplosionPool(1);
        }
        explosion = stackExplosion.Pop();
        if (explosion != null)
        {
            explosion.Explode(explodingObject, explosionSizeMultiplier);
        }
    }
}