using UnityEngine;
using System.Collections;

public class GameMessage : MonoBehaviour
{
    public TextMesh textMessage;
    public TextMesh textMessage2;

    public IEnumerator PlayMessage(string message)
    {

        for (int curChar = 0; curChar < message.Length; curChar++)
        {
            textMessage.text += message[curChar];
            yield return new WaitForSeconds(0.02f);
            textMessage2.text += message[curChar];
            yield return new WaitForSeconds(0.05f);
        }
    }
}
