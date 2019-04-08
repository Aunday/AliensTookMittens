using UnityEngine;
using System.Collections;

public class Icon : MonoBehaviour 
{
    public Board board;
    public int iconRow;
    public int iconColumn;
    public bool iconMobile;
    public SpriteRenderer[] IconFace;
    public SpriteRenderer IconFace0;
    public SpriteRenderer IconFace1;
    public SpriteRenderer IconFace2;
    public SpriteRenderer IconFace3;
    public SpriteRenderer IconFace4;
    public SpriteRenderer IconFace5;
    public SpriteRenderer IconFace6;
    public Animator[] animator;
    public Animator animator0;
    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
    public Animator animator4;
    public Animator animator5;
    public Animator animator6;
    public int iconType;
    public int savedIconType;
    //public SpriteRenderer spriteBomb;
    //private Animator bombAnimator;
    public IconBomb iconBomb = null;
    public GameObject spriteFire;
    public SpriteRenderer spriteAcid;
    public Animator animatorAcid;
    public SpriteRenderer spriteLock;
    public Animator animatorLock;
    public SpriteRenderer spriteArrow;
    public Animator animatorHalf;
    new Transform transform;
    private Ship ship;
    private GameManager gameManager;
    public ParticleScaler particleScaler;
    private PrefabPools prefabPools;
    public float currentEffectMultiplier { get; private set; }
    private int numUniqueIcons;

    public void StartSetup()
    {
        IconFace = new SpriteRenderer[] { IconFace0, IconFace1, IconFace2, IconFace3, IconFace4, IconFace5, IconFace6 };
        animator = new Animator[] { animator0, animator1, animator2, animator3, animator4, animator5, animator6 };
        gameManager = board.gameManager;
        prefabPools = gameManager.prefabPools;
        transform = gameObject.transform;
        ship = gameManager.ship;
        particleScaler.scaleRatioX = gameManager.scaleRatioX;
    }

    public void SetStartIcon(int column, int row, int newType)
    {
        iconMobile = false;
        iconType = newType;
        currentEffectMultiplier = 1.0f;

        EnableCurrentFace();
    }

    public void DisableCurrentFace()
    {
        IconFace[iconType % board.numUniqueIcons].enabled = false;
    }

    public void EnableCurrentFace()
    {
        IconFace[iconType].enabled = true;
    }

    public void ActivateBomb(float damage, int row, int column)
    {
        iconBomb = prefabPools.PopIconBomb();
        if (iconBomb != null)
        {
            iconBomb.PopOffStack(damage, transform, iconRow, iconColumn);
        }
    }

    public void DeactivateBomb()
    {
        iconBomb.PushOnStack();
        iconBomb = null;
    }

    public void ActivateFire(float damagePerSecond)
    {
        if (!spriteFire.activeSelf)
        {
            spriteFire.SetActive(true);
            StartCoroutine(FireDamage(damagePerSecond));
        }
    }

    public void DeactivateFire()
    {
        spriteFire.SetActive(false);
    }

    IEnumerator FireDamage(float damagePerSecond)
    {
        int fireDuration = 8;
        while (fireDuration > 0 && spriteFire.activeSelf)
        {
            fireDuration--;
            ship.PlayerTakeDamage(damagePerSecond);
            yield return new WaitForSeconds(1.0f);
        }
        DeactivateFire();
    }

    public void ActivateAcid(float projectileDamage, int duration)
    {
        if (!spriteAcid.enabled)
        {
            spriteAcid.enabled = true;
            animatorAcid.enabled = true;
            StartCoroutine(AcidDamage(projectileDamage, duration));
        }
    }

    public void DeactivateAcid()
    {
        spriteAcid.enabled = false;
        animatorAcid.enabled = false;
        ship.DeductAcid();
    }

    IEnumerator AcidDamage(float damagePerSecond, int duration)
    {
        while (duration > 0 && spriteAcid.enabled)
        {
            yield return new WaitForSeconds(2.0f);
            duration--;
            if (spriteAcid.enabled)
            {
                ship.PlayerTakeDamage(damagePerSecond);
            }
        }
        DeactivateAcid();
    }

    public void ActivateHalfEffect()
    {
        if (currentEffectMultiplier < 2.0f)
        {
            spriteArrow.enabled = true;
            spriteArrow.transform.rotation = Quaternion.identity;
            animatorHalf.enabled = true;
            currentEffectMultiplier = 0.5f;
        }
        else
        {
            spriteArrow.enabled = false;
            animatorHalf.enabled = false;
            currentEffectMultiplier = 1.0f;
        }
    }
    
    public void ActivateEmpower()
    {
        spriteArrow.enabled = true;
        spriteArrow.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        particleScaler.gameObject.SetActive(true);
        animatorHalf.enabled = true;
        currentEffectMultiplier = gameManager.upgradeValues[45, PlayerStats.playerStats.upgradeStatus[45]];
    }

    public void ActivateLock(int duration, int row, int column)
    {
        spriteLock.enabled = true;
        animatorLock.enabled = true;
        animatorLock.Play(0);
        HighlightRemove();
        savedIconType = iconType;

        iconType += (7 + column + row * 7) * 7;

        //iconType = (int)Time.fixedTime;
        StartCoroutine(TrackLock());
    }

    private IEnumerator TrackLock()
    {
        yield return new WaitForSeconds(12.0f);
        if (spriteLock.enabled)
        {
            DeactivateLock();
            board.LockDisabled();
        }
    }

    public void DeactivateLock()
    {
        if (iconType > board.numUniqueIcons)
        {
            iconType = savedIconType;
        }
        spriteLock.enabled = false;
        animatorLock.enabled = false;
    }

    public float IconMatched(float effectMultiplier)
    {
        if (spriteArrow.enabled)
        {
            effectMultiplier *= currentEffectMultiplier;
        }
        currentEffectMultiplier = 1.0f;

        RemoveEffects();

        return effectMultiplier;
    }

    public void RemoveEffects()
    {
        if (iconBomb != null)
        {
            DeactivateBomb();
        }

        if (spriteArrow.enabled)
        {
            spriteArrow.enabled = false;
            animatorHalf.enabled = false;
            if (particleScaler.gameObject.activeSelf)
            {
                particleScaler.gameObject.SetActive(false);
            }
        }

        if (spriteFire.activeSelf)
        {
            DeactivateFire();
        }

        if (spriteAcid.enabled)
        {
            DeactivateAcid();
        }

        if (spriteLock.enabled)
        {
            DeactivateLock();
        }

        //HighlightRemove();
    }

    public void HighlightAdd()
    {
        if (iconType < board.numUniqueIcons)
        {
            if (!animator[iconType].enabled)
            {
                animator[iconType].enabled = true;
            }
        }
    }

    public void HighlightRemove()
    {
        if (iconType < board.numUniqueIcons)
        {
            if (animator[iconType].enabled)
            {
                SpriteRenderer spriteRenderer = IconFace[iconType];
                animator[iconType].enabled = false;
                spriteRenderer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                spriteRenderer.color = Color.white;
                //gameObject.SetActive(false); //thought I needed this to remove batch increasing, but I guess not?
                //gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator IconDrop(int row)
    {
        float deltaTime = 0.0f;
        iconMobile = true;
        //iconRow = row;
        while (transform.localPosition.y > iconRow * 0.71f + 0.05f)
        {
            deltaTime += Time.deltaTime;
            transform.localPosition -= new Vector3(0, 2.7f * Time.deltaTime + (deltaTime * 0.065f), 0);
            yield return null;
        }
        transform.localPosition = new Vector2(transform.localPosition.x, iconRow * 0.71f);
        iconMobile = false;
        board.iconsInMotion--;
    }

    public IEnumerator Shuffle()
    {
        for (int iteration = 0; iteration < 5; iteration++)
        {
            DisableCurrentFace();
            iconType = Random.Range(0, board.numUniqueIcons);
            EnableCurrentFace();
            yield return new WaitForSeconds(0.1f + 0.1f * iteration);
        }
    }

    public void SetType(int randomType)
    {
        DisableCurrentFace();
        iconType = randomType;
        EnableCurrentFace();
    }

    public void ResetPosition(int column, int row, int newType)
    {
        RemoveEffects();
        DisableCurrentFace();
        SetStartIcon(column, row, newType);
        transform.localPosition = new Vector2(column * 0.71f, row * 0.71f);
    }
}