using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDataContainer
{
    public int index;

    public PlayerInput playerInput;

    public Color playerColor;

    public int score = 0;
    public int numKnockouts = 0;
    public int numFalls = 0;



    public PlayerDataContainer(PlayerInput _playerInput){  
        playerInput = _playerInput;
        index = playerInput.playerIndex;
    }

    
    public void SetColor(){
        
    }
}
