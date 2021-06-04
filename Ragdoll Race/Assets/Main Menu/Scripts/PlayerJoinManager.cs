using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
    public AddPlayersScreenDisplay addPlayersScreenDisplay;

    public List<PlayerDataContainer> playerDataContainers;


    public void OnPlayerJoined(PlayerInput playerInput){
        // Adds the most recently joined player to the list

        PlayerDataContainer newPlayer = new PlayerDataContainer(playerInput);

        playerDataContainers.Add(newPlayer);

        playerInput.transform.SetParent(this.transform);
    }


    public void OnPlayerLeft(){

    }
    
    public void ClearPlayersList(){

    }




}
