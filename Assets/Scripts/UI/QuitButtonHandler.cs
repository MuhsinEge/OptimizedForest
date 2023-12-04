using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButtonHandler : MonoBehaviour
{
    public Button quitButton;
    void Awake()
    {
        quitButton.onClick.AddListener(QuitApplication);
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}
