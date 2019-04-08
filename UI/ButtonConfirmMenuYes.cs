using UnityEngine;

public class ButtonConfirmMenuYes : MonoBehaviour
{
    public ConfirmMenu confirmMenu;
	
    void OnMouseDown()
    {
        AudioManager.audioManager.PlaySound(9);
        confirmMenu.ConfirmClicked();
    }
}
