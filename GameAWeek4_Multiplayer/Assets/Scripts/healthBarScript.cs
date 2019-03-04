using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarScript : MonoBehaviour
{
    public PlayerMovement player;
    public Image healthbarImage;
    public InputField nicknameInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scaleHealthUI();
    }

    private void scaleHealthUI()
    {
        healthbarImage.rectTransform.sizeDelta = new Vector2(player.health * 10, healthbarImage.rectTransform.sizeDelta.y);
    }

    public void onNickNameEntered()
    {
        player.playerNickname = nicknameInput.text;
        nicknameInput.gameObject.SetActive(false);
    }
}

