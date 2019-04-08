using UnityEngine;
using System.Collections;

public class TextDamage : MonoBehaviour
{
    public TextMesh damageTextMesh;
    public PrefabPools prefabPools;
    public MeshRenderer meshRenderer;
    public Animator animator;
    private Transform transformSpace;
    private Transform transformPool;

    public void StartSetup()
    {
        meshRenderer.sortingOrder = 86;
        transformSpace = prefabPools.gameManager.space.transform;
        transformPool = prefabPools.transform;
        //PushOnStack();
    }

    public void PushOnStack()
    {
        meshRenderer.enabled = false;
        animator.enabled = false;
        transform.parent = transformPool;
        prefabPools.stackTextDamage.Push(this);
        transform.localPosition = new Vector2(0.0f, 30.0f);
        enabled = false;
    }

    public void PopOffStack(float damageAmount, float locationX, float locationY, bool takingDamage)
    {
        damageTextMesh.text = Mathf.RoundToInt(damageAmount).ToString();
        if (takingDamage)
        {
            damageTextMesh.color = Color.red;
        }
        else
        {
            damageTextMesh.color = Color.green;
        }
        transform.parent = transformSpace;
        transform.localPosition = new Vector2(locationX, locationY);
        meshRenderer.enabled = true;
        animator.enabled = true;
        //StartCoroutine(fade());
    }

    private IEnumerator fade()
    {
        float targetHeight = transform.localPosition.y + 0.3f;
        while (transform.localPosition.y < targetHeight)
        {
            transform.localPosition += new Vector3(0, 0.02f, 0);
            yield return new WaitForSeconds(0.03f);
        }
        while (damageTextMesh.color.a > 0.0f)
        {
            transform.localPosition += new Vector3(0, 0.02f, 0);
            damageTextMesh.color -= new Color(0, 0, 0, 0.064f);
            yield return new WaitForSeconds(0.03f);
        }
        PushOnStack();
    }
}