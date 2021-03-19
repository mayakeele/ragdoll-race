using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [Header("Component References")]
    public CameraController cameraController;


    [Header("Players")]
    [SerializeField] private List<Player> allPlayers;
    private List<Player> onscreenPlayers;


    [Header("Shared Player Properties")]
    public float characterHeight;
    public float characterRadius;



    // Main Functions

    void Start()
    {
        
    }


    void Update()
    {
        
    }



    // Public Functions

    public List<Player> GetAllPlayers(){
        return allPlayers;
    }

    public List<Player> GetOnscreenPlayers(){
        return onscreenPlayers;
    }


    public List<Vector3> PlayerPositions(List<Player> playersList){
        // Returns a list of the positions of the requested players

        List<Vector3> positions = new List<Vector3>();
        foreach(Player player in playersList){
            positions.Add(player.transform.position);
        }

        return positions;
    }

    public Vector3 AveragePlayersPosition(List<Player> playersList){
        // Calculates the average position of all of the players (skews towards groups of players)

        List<Vector3> playerPositions = PlayerPositions(playersList);
        Vector3 averagePos = playerPositions.Average();

        return averagePos;
    }

    public BoundingSphere GetPlayersBoundingSphere(List<Player> playersList){
        // Calculates the radius and position of a sphere enclosing all given players; returns a BoundingSphere struct with pos and rad properties

        List<Vector3> allPositions = PlayerPositions(playersList);
        List<Vector3> farthestPoints = BoundingVolume.GetFarthestPointPair(allPositions);

        Vector3 center = farthestPoints.Average();
        float radius = Vector3.Distance(farthestPoints[0], farthestPoints[1]) / 2;

        return new BoundingSphere(center, radius);
    }
    public Vector3 CenterPlayersPosition(List<Player> playersList){
        // Calculates the center position of an axis-aligned bounding box containing the players

        List<Vector3> playerPositions = PlayerPositions(playersList);
        
        Vector3 maxBounds = playerPositions.MaxComponents();
        Vector3 minBounds = playerPositions.MinComponents();

        Vector3 centerPos = (maxBounds + minBounds) / 2;

        return centerPos;
    }


    public Vector3 ScreenSpaceForward(){
        return cameraController.GetCameraForwardDirection();
    }



    // Private Functions

    private List<Player> FindStragglers(float separationDist){
        return new List<Player>();
    }

}
