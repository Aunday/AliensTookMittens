using UnityEngine;
using System.Collections;

public class MessageScreen : MonoBehaviour
{
    public TextMesh textMessage;
    public TextMesh textMessage2;
    public SpriteRenderer spriteGif;
    public SpriteRenderer spriteGif2;
    public SpriteRenderer spriteGif3;
    public GameObject spriteMittens;
    public SpriteRenderer spriteMessageBox;
    public SpriteRenderer spriteLayer;
    public BoxCollider boxCollider;
    public GameObject tapToContinue;
    public Animator animator;
    private bool waitForTap;
    private int charPerIteration = 1;
    private string[] storyMessages = new string[]
    {
        "Hey there. Name's Gif, but\nyou can call me Jif. Space\ntravel isn't normally my\nthing. But they took my\ncat, they took Mittens!\nWe are getting him back.", //0 //Starting tutorial
        "We can't leave Mittens\nto this fate.",
        "This search is going to\ntake longer than I\nexpected.", //1
        "Times used to be that a\nman-dog could live his\nlife, enjoying a nice can of\nchow, without having his\ncat-friend nabbed.",
        "It's funny. To an alien, we\nwould be considered an\nalien.",
        "The scent has led us to\nEunomia. Bombs are the\nfavored arsenal here. If\none hits us, a match will\nstop it from exploding.", //2
        "Four out of five doctors\nagree. Space battles\nare bad for your health.",
        "If we don't make it out of\nthis alive, I want you to\nknow. Chuck Norris is\nmy father.",
        "Rumor places a peculiar\nentity in this system.\nCrossing my paw-fingers\nthat Mittens is seen as\npeculiar by others as well.", //3
        "In case you didn't notice, I\nam not a human. Just\nthought I should put that\nout there.",
        "Is that a floating furball?\nNope,  just an angry\nalien coming at us.",
        "Mittens and I didn't always\nget along. Hard to wear\nmittens if you have paws\namirite? ...no? Not a fan\nof puns?", //4
        "Be honest. Does this\nspace suit make me look\nfat?",
        "If Aquaman is only able to\ncontrol fish, why is he able\nto control whales?",
        "This system is a\nsafeharbor for criminals\nand outlaws. There is a\ngood chance the captors\ncame here.", //5
        "Okay, new plan. Let's try\ndestroying them before\nthey destroy us.\nAlright,\nBREAK.",
        "Oh, you are wondering\nwhat happened to my\neyepatch? I just wear\nthat to look cool. Isn't that\nwhat they are for?",
        "That last ship had\nscratch marks all over it!\nClear sign of a cat attack.\nWe must be getting close.", //6
        "Second time's the charm.",
        "Third time's the charm?",
        "The Synid race native\nto this system commonly\nkidnaps pets. That is\ngenerally frowned upon. It\nalso makes this a good\nplace to search for\nmittens.", //7
        "Synids also prefer to\nshoot on sight. Forgot to\nmention that part.",
        "Synids commonly wear\nscarves, but I don't\nbelieve that factoid is\nrelevant to our current\nsituation.",
        "The droid's map passes\nthrough this system.\nHope you R not 2 busy to\nD2our through here.\nReferences!", //8
        "We should have stopped\nat that last gas station\nand asked for directions.",
        "I lost my eyepatch. This\nwas the only thing I\ncould find. Never speak\nof this.",
        "No space trip/kidnapping\nsearch is complete\nwithout a trip to Karu's\nFunWorld. It's the happiest\nplace in the galaxy.", //9
        "For how big space is, I\nhave a real knack for\nrunning into angry living\nspecies.",
        "I figure that Mario would\nhave just given Peach\na GPS locator by now.",
        "Have to put myself in\nMitten's shoes. If I were\ngetting kidnapped, where\nwould I want to go?", //10
        "Logic dictates that we\nmust now have a staring\ncontest. First one to blink\nloses. Ready? Go!",
        "What? Oh, we are flying?\nSorry, I was distracted\nplaying this great game. It\nis called \"You Must Build a\nBoat\". Long name, I know.",
        "I just received word that\none of my informants saw\nMittens being taken to\nan unknown system. We\nneed to find my informant.", //11
        "My grandma built this ship\nout of tin cans, bubble\ngum, and hope. I think\nit turned out rather well.",
        "Alright target audience,\nlet's have a little chat.\nYour age range is too\nwide. I can't even make my\ncustomary raunchy\ncomments right now.",
        "The informant's napkin\nsketch has led us here.\nWait, is it upside-down?\nSpace-North is a very\nconfusing concept.", //12
        "Do the musings of an\nanthropomorphic space\ndog have any less merit\nthan those of a Greek\nphilosopher?",
        "Isn't it strange how\nhumans call each other\nracist when they are all\none race?",
        "They took Mittens... I have\nto get her back.", //13
        "I'm coming for you buddy!",
        "It's the final countdown!\nThat song gets me every\ntime."
    };
    private string[] humorMessages = new string[]
    {
        "Times used to be that a\nman-dog could live his\nlife, enjoying a nice can of\nchow, without having his\ncat-friend nabbed.", //0
        "It's funny. To an alien, we\nwould be considered an\nalien.",
        "Four out of five doctors\nagree. Space battles\nare bad for your health.",
        "If we don't make it out of\nthis alive, I want you to\nknow. Chuck Norris is\nmy father.",
        "In case you didn't notice, I\nam not a human. Just\nthought I should put that\nout there.",
        "Is that a floating furball?\nNope,  just an angry\nalien coming at us.", //5
        "Be honest. Does this\nspace suit make me look\nfat?",
        "If Aquaman is only able to\ncontrol fish, why is he able\nto control whales?",
        "Okay, new plan. Let's try\ndestroying them before\nthey destroy us.\nAlright,\nBREAK.",
        "Oh, you are wondering\nwhat happened to my\neyepatch? I just wear\nthat to look cool. Isn't that\nwhat they are for?",
        "We should have stopped\nat that last gas station\nand asked for directions.", //10
        "I lost my eyepatch. This\nwas the only thing I\ncould find. Never speak\nof this.",
        "For how big space is, I\nhave a real knack for\nrunning into angry living\nspecies.",
        "I figure that Mario would\nhave just given Peach\na GPS locator by now.",
        "Logic dictates that we\nmust now have a staring\ncontest. First one to blink\nloses. Ready? Go!",
        "What? Oh, we are flying?\nSorry, I was distracted\nplaying this great game. It\nis called \"You Must Build a\nBoat\". Long name, I know.", //15
        "My grandma built this ship\nout of tin cans, bubble\ngum, and hope. I think\nit turned out rather well.",
        "Alright target audience,\nlet's have a little chat.\nYour age range is too\nwide. I can't even make my\ncustomary raunchy\ncomments right now.",
        "Do the musings of an\nanthropomorphic space\ndog have any less merit\nthan those of a Greek\nphilosopher?",
        "Isn't it strange how\nhumans call each other\nracist when they are all\none race?",
        "Ah, the fringe systems.\nEqual odds here of being\nshot, or having your\nsister kidnapped by a\nhorse.", //20
        "Woof ... no? I am sticking\nwith woof.",
        "What is the sneakiest\narmor?...\nLeather!\nBecause it is made of\nhide!\n...Is that a cricket?",
        "Better watch out, I only\nhave so much to say. I\nmight just start repeating\nmyself.",
        "Hey there, name’s gif...\nnah just kidding.",
        "What's that? You want\nto know whether Mittens\nis male or female? Well\nclearly Mittens is\nkrzzckk..........\nHmm, looks like the\ntransceiver is acting up.", //25
        "", //When I add more, I need to change the number that GameManager references as the limit
        "",
        "",
        "",
        "", //30
        "",
        "",
        "",
        "",
        "", //35
        "",
        "",
        "",
        "",
        "" //40
    };
    private string[] bossMessages = new string[]
    {
        "Well that definitely is\nnot Mittens.", //0
        "That ship is bigger than\nours.", //1
        "Get ready!", //2
        "I... think we found our\npeculiar entity.", //3
        "That thing looks angry.", //4
        "Is that all you got?                \nNope, I guess they have more.", //5 (not currently using)
        "", //6 (not currently using)
        "There is such thing as\ntoo much bling...", //7
        "", //8 (not currently using)
        "Well that just doesn't\nseem possible...", //9
        "", //10 (not currently using)
        "Why do they always send\nthe big ships at us?", //11
        "I have always wanted one\nof these ships.", //12 (not currently using)
        "I have a good feeling\nabout this." //13
    };
    private string[] levelThreatMessages = new string[]
    {
        "", //0
        "", //1
        "", //2
        "Be on your toes. These\nships throw flame bursts.\nIf they hit us, extinguish\nthe fires by moving their\nicons.", //3
        "These ships charge up\nmassive energy bursts,\nbut only when stationary.\nKnocking them back\ndelays their shots.", //4
        "There is one of the\nbombers. Be ready to\nmatch away the bombs.", //5
        "Energy drainers! They\nwill steal our energy and\nshoot it back at us.", //6
        "Synids prefer to\noverwhelm with numbers.\nOur blast is the best way\nto deal with them.", //7
        "Watch out for acid\nattacks. If any acid hits\nus, it can be removed by\nmatching.", //8
        "Any control icons\ndrained by these ships will\nhave weakened matches.", //9
        "", //10
        "Destroy that explosive\nship before it gets to us!", //11
        "These ships will lock\nour control icons. We\nwon't be able to create\nmatches with locked icons.", //12
        "They have shields too?\nIt may stop the laser,\nbut our blast will go\nright through it.", //13
        "That is a phase ship. Their\ndesign allows them to\nphase out, avoiding most\nof our projectiles. Shoot\nwhen they are solid." //14
    };

    public IEnumerator PlayMessageNoPause(int whichMessage, string message)
    {
        textMessage.color = Color.white;
        textMessage2.color = new Color(0.91f, 0.6f, 0.0f, 1.0f);
        textMessage.text = "";
        textMessage2.text = "";
        if (message == null)
        {
            message = bossMessages[whichMessage];
            PlayerStats.playerStats.bossMessageStatus[whichMessage] = false;
        }
        else if (message == "")
        {
            message = levelThreatMessages[whichMessage];
            PlayerStats.playerStats.threatMessageStatus[whichMessage] = false;
        }
        int numChars = message.Length;
        spriteGif.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        spriteMessageBox.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        spriteLayer.enabled = true;
        spriteMessageBox.gameObject.SetActive(true);
        textMessage2.gameObject.SetActive(true);

        while (spriteGif.color.a < 1.0f)
        {
            spriteGif.color += new Color(0.0f, 0.0f, 0.0f, 0.2f);
            spriteMessageBox.color += new Color(0.0f, 0.0f, 0.0f, 0.2f);
            yield return new WaitForSeconds(0.04f);
        }
        for (int curChar = 0; curChar < numChars; curChar++)
        {
            char currentChar = message[curChar];
            textMessage.text += currentChar;
            yield return new WaitForSeconds(0.015f);
            textMessage2.text += currentChar;
            if (currentChar == '.' || currentChar == '?' || currentChar == '!')
            {
                yield return new WaitForSeconds(0.2f);
            }
            else if (currentChar == ',')
            {
                yield return new WaitForSeconds(0.14f);
            }
            else
            {
                yield return new WaitForSeconds(0.009f);
            }
        }
        yield return new WaitForSeconds(0.026f * numChars);
        while (textMessage.color.a > 0)
        {
            spriteGif.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            spriteMessageBox.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            textMessage.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            textMessage2.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }
        spriteMessageBox.gameObject.SetActive(false);
        textMessage2.gameObject.SetActive(false);
        spriteLayer.enabled = false;
        yield return new WaitForSeconds(0.6f);
    }

    public IEnumerator PlayMessagePause(int whichMessage, string message)
    {
        int lineNumber = 0;
        animator.enabled = true;
        textMessage.color = Color.white;
        textMessage2.color = new Color(0.91f, 0.6f, 0.0f, 1.0f);
        textMessage.text = "";
        textMessage2.text = "";
        SpriteRenderer spriteRendererMittens = null;
        SpriteRenderer spriteRendererMittensMouth = null;
        if (message == null)
        {
            message = storyMessages[whichMessage];
        }
        else if (message == "")
        {
            message = humorMessages[whichMessage];
        }
        else if (whichMessage == 99)
        {
            spriteGif.enabled = false;
            spriteMittens.SetActive(true);
            spriteRendererMittens = spriteMittens.GetComponent<SpriteRenderer>();
            spriteRendererMittensMouth = spriteMittens.transform.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRendererMittens.color = Color.white;
            spriteRendererMittensMouth.color = Color.white;
        }
        else if (whichMessage == 100)
        {
            //spriteGif.enabled = false;
            spriteGif3.enabled = true;
            spriteGif3.color = Color.white;
        }
        int numChars = message.Length;

        spriteGif.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        spriteMessageBox.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        spriteLayer.enabled = true;
        spriteMessageBox.gameObject.SetActive(true);
        textMessage2.gameObject.SetActive(true);
        boxCollider.enabled = true;

        int curChar = 0;
        while (curChar < numChars)
        {
            int charThisIteration = charPerIteration;
            for (int charactersToDisplay = charThisIteration; charactersToDisplay > 0; charactersToDisplay--)
            {
                if (curChar < numChars)
                {
                    char currentChar = message[curChar];
                    textMessage.text += currentChar;
                    if (charactersToDisplay == 1)
                    {
                        yield return new WaitForSeconds(0.015f);
                    }
                    textMessage2.text += currentChar;
                    if (charactersToDisplay == 1)
                    {
                        if (currentChar == '.' || currentChar == '?' || currentChar == '!')
                        {
                            yield return new WaitForSeconds(0.18f);
                        }
                        else if (currentChar == ',')
                        {
                            yield return new WaitForSeconds(0.08f);
                        }
                        else
                        {
                            yield return new WaitForSeconds(0.005f);
                        }
                    }
                    if (currentChar == '\n')
                    {
                        lineNumber++;
                        if (lineNumber == 5)
                        {
                            waitForTap = true;
                            tapToContinue.SetActive(true);
                            while (waitForTap)
                            {
                                yield return new WaitForSeconds(0.1f);
                            }
                            boxCollider.enabled = true;
                            textMessage.text = "";
                            textMessage2.text = "";
                            lineNumber = 0;
                        }
                    }
                    curChar++;
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        waitForTap = true;
        tapToContinue.SetActive(true);
        while (waitForTap)
        {
            yield return new WaitForSeconds(0.1f);
        }
        while (textMessage.color.a > 0)
        {
            textMessage.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            textMessage2.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            spriteGif.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            spriteMessageBox.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            if (whichMessage == 99)
            {
                spriteRendererMittens.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
                spriteRendererMittensMouth.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            }
            else if (whichMessage == 100)
            {
                spriteGif3.color -= new Color(0.0f, 0.0f, 0.0f, 0.1f);
            }
            yield return new WaitForSeconds(0.04f);
        }
        if (whichMessage == 99)
        {
            spriteGif.enabled = true;
            spriteMittens.SetActive(false);
        }
        else if (whichMessage == 100)
        {
            //spriteGif.enabled = true;
            spriteGif3.enabled = false;
        }
        spriteMessageBox.gameObject.SetActive(false);
        textMessage2.gameObject.SetActive(false);
        spriteLayer.enabled = false;
        animator.enabled = false;
    }

    void OnMouseDown()
    {
        if (waitForTap == true)
        {
            waitForTap = false;
            tapToContinue.SetActive(false);
            boxCollider.enabled = false;
        }
        charPerIteration = 5;
    }

    void OnMouseUp()
    {
        charPerIteration = 1;
    }
}