using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptDisplay : MonoBehaviour
{

    public GameObject promptHolder;
    public Text promptText;
    public RawImage buttonImg;

    public float btnTextSpace;
    public float promptPosY;

    // Start is called before the first frame update
    void Start()
    {
        promptHolder.SetActive(false);
        updatePromptPos();
    }

    public void showPrompt(string t)
    {
        promptText.text = t;
        updatePromptPos();
        promptHolder.SetActive(true);
    }

    public void hidePrompt()
    {
        promptHolder.SetActive(false);
    }

    private void updatePromptPos()
    {
        promptText.transform.localPosition = new Vector3((buttonImg.rectTransform.rect.width + btnTextSpace) / 2, promptPosY, 0);
        buttonImg.transform.localPosition = new Vector3(-(promptText.preferredWidth + btnTextSpace) / 2, promptPosY, 0);
    }

}
