using UnityEngine;
using System.Collections;

public class MatchText : MonoBehaviour
{
    public string matchTextString1;
    public string matchTextString2;
    public TextMesh matchTextMesh1;
    public TextMesh matchTextMesh2;
    public MatchText matchText;
    public MeshRenderer matchMeshRenderer1;
    public MeshRenderer matchMeshRenderer2;
    public TextActive matchScript1;
    public TextActive matchScript2;
    public MeshRenderer shadowRenderer1;
    public MeshRenderer shadowRenderer2;
    public MeshRenderer shadowRenderer3;
    public MeshRenderer shadowRenderer4;
    public TextMesh shadowText1;
    public TextMesh shadowText2;
    public TextMesh shadowText3;
    public TextMesh shadowText4;
    public SpriteRenderer[] spriteIconFace;
    public SpriteRenderer spriteIconFace0;
    public SpriteRenderer spriteIconFace1;
    public SpriteRenderer spriteIconFace2;
    public SpriteRenderer spriteIconFace3;
    public SpriteRenderer spriteIconFace4;
    public SpriteRenderer spriteIconFace5;
    public SpriteRenderer spriteIconFace6;
    public Transform mesh1Transform;
    public Transform mesh2Transform;
    public Color matchTextColor;
    public Animator animator;
    public PrefabPools prefabPools;
    //private Transform transformPrefabPools;

    public void SetSprites()
    {
        spriteIconFace = new SpriteRenderer[] { spriteIconFace0, spriteIconFace1, spriteIconFace2, spriteIconFace3, spriteIconFace4, spriteIconFace5, spriteIconFace6 };
    }

    public void PopOffStack(string displayText1, string displayText2, float locationX, float locationY, int matchType, int iconType)
    {
        matchTextString1 = displayText1;
        matchTextString2 = displayText2;
        animator.enabled = true;
        matchMeshRenderer1.enabled = true;
        matchMeshRenderer2.enabled = true;
        matchScript1.enabled = true;
        matchScript2.enabled = true;
        shadowRenderer1.enabled = true;
        shadowRenderer2.enabled = true;
        shadowRenderer3.enabled = true;
        shadowRenderer4.enabled = true;
        transform.localPosition = new Vector2(locationX, locationY);

        shadowText1.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        shadowText2.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        if (matchType == 0)
        {
            StartCoroutine(NormalMatch(iconType));
        }
        else if (matchType == 1)
        {
            LongMatch();
        }
        else if(matchType == 2)
        {
            ChainMatch();
        }
        else
        {
            MultiMatch();
        }
    }

    public void PushOnStack()
    {
        animator.enabled = false;
        matchMeshRenderer1.enabled = false;
        matchMeshRenderer2.enabled = false;
        matchScript1.enabled = false;
        matchScript2.enabled = false;
        shadowRenderer1.enabled = false;
        shadowRenderer2.enabled = false;
        shadowRenderer3.enabled = false;
        shadowRenderer4.enabled = false;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        prefabPools.stackMatchText.Push(this);
        enabled = false;
    }

    IEnumerator NormalMatch(int iconType)
    {
        Color[] textColor = { new Color(1.0f, 0.72f, 0), Color.red, Color.green, new Color(0.91f, 0, 1.0f), new Color(1.0f, 0.56f, 1.0f), new Color(0.0f, 0.75f, 1.0f), Color.yellow };
        matchTextMesh1.color = textColor[iconType];
        spriteIconFace[iconType].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        spriteIconFace[iconType].enabled = true;
        matchTextMesh1.anchor = TextAnchor.MiddleRight;
        shadowText1.anchor = TextAnchor.MiddleRight;
        shadowText2.anchor = TextAnchor.MiddleRight;
        matchTextMesh1.text = matchTextString1;
        matchTextMesh2.text = "";
        shadowText1.text = matchTextString1;
        shadowText2.text = matchTextString1;
        shadowText3.text = "";
        shadowText4.text = "";

        animator.SetInteger("matchType", 0);
        animator.speed = 0.5f;
        animator.Play(0);

        while (spriteIconFace[iconType].color.a > 0.9f)
        {
            spriteIconFace[iconType].color -= new Color(0.0f, 0.0f, 0.0f, 0.01f);
            yield return new WaitForSeconds(0.085f);
        }
        while (spriteIconFace[iconType].color.a > 0.0f)
        {
            spriteIconFace[iconType].color -= new Color(0.0f, 0.0f, 0.0f, 0.013f);
            yield return new WaitForSeconds(0.013f);
        }
        PushOnStack();
        spriteIconFace[iconType].enabled = false;
    }

    void MultiMatch()
    {
        matchTextMesh1.anchor = TextAnchor.MiddleCenter;
        matchTextMesh2.anchor = TextAnchor.MiddleCenter;
        shadowText1.anchor = TextAnchor.MiddleCenter;
        shadowText2.anchor = TextAnchor.MiddleCenter;
        shadowText3.anchor = TextAnchor.MiddleCenter;
        shadowText4.anchor = TextAnchor.MiddleCenter;
        matchTextMesh1.text = "MULTI";
        matchTextMesh2.text = "MATCH";
        shadowText1.text = "MULTI";
        shadowText2.text = "MULTI";
        shadowText3.text = "MATCH";
        shadowText4.text = "MATCH";

        matchTextMesh1.color = new Color(0.97f, 0.69f, 0.0f, 1.0f);
        matchTextMesh2.color = new Color(0.97f, 0.69f, 0.0f, 1.0f);

        animator.SetInteger("matchType", 2);
        animator.speed = 1.0f;
        animator.Play(0);
    }

    void ChainMatch()
    {
        matchTextMesh1.anchor = TextAnchor.MiddleCenter;
        matchTextMesh2.anchor = TextAnchor.MiddleCenter;
        shadowText1.anchor = TextAnchor.MiddleCenter;
        shadowText2.anchor = TextAnchor.MiddleCenter;
        shadowText3.anchor = TextAnchor.MiddleCenter;
        shadowText4.anchor = TextAnchor.MiddleCenter;
        matchTextMesh1.text = matchTextString1;
        matchTextMesh2.text = "CHAIN MATCH";
        shadowText1.text = matchTextString1;
        shadowText2.text = matchTextString1;
        shadowText3.text = "CHAIN MATCH";
        shadowText4.text = "CHAIN MATCH";

        matchTextMesh1.color = new Color(0.97f, 0.69f, 0.0f, 1.0f);
        matchTextMesh2.color = new Color(0.97f, 0.69f, 0.0f, 1.0f);

        animator.SetInteger("matchType", 1);
        animator.speed = 1.0f;
        animator.Play(0);
    }

    void LongMatch()
    {
        matchTextMesh1.anchor = TextAnchor.MiddleCenter;
        matchTextMesh2.anchor = TextAnchor.MiddleCenter;
        shadowText1.anchor = TextAnchor.MiddleCenter;
        shadowText2.anchor = TextAnchor.MiddleCenter;
        shadowText3.anchor = TextAnchor.MiddleCenter;
        shadowText4.anchor = TextAnchor.MiddleCenter;
        matchTextMesh1.text = matchTextString1;
        matchTextMesh2.text = matchTextString2;
        shadowText1.text = matchTextString1;
        shadowText2.text = matchTextString1;
        shadowText3.text = matchTextString2;
        shadowText4.text = matchTextString2;

        matchTextMesh1.color = new Color(0.97f, 0.69f, 0.0f, 1.0f);
        matchTextMesh2.color = new Color(0.97f, 0.69f, 0.0f, 1.0f);

        animator.SetInteger("matchType", 3);
        animator.speed = 1.0f;
        animator.Play(0);
    }
}