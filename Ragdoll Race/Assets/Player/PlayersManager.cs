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


    [Header("Materials")]
    [SerializeField] private List<Material> playerMaterials;


    [Header("Knockout Effects")]
    [SerializeField] private GameObject defaultKnockoutEffectPrefab;
    [SerializeField] private float KOCameraShakeAmount;
    [SerializeField] private float postKOCameraLingerDuration;


    [Header("Player Body Dimensions")]
    public float characterHeadHeight;
    public float characterPelvisHeight;
    public float characterRadius;


    [Header("Spawning")]
    public Transform spawnTransform;

    private int[] playerLifeCounts;
    private int[] playerKnockoutCounts;



    // Main Functions

    void Awake()
    {
        playerMaterials.Shuffle();
    }



    // Public Functions

    public void AddPlayer(Player player){
        allPlayers.Add(player);
        player.transform.SetParent(this.transform);
    }


    public List<Player> GetAllPlayers(){
        return allPlayers;
    }

    public List<Player> GetOnscreenPlayers(){
        return onscreenPlayers;
    }


    public List<Vector3> GetPositions(List<Player> playersList){
        // Returns a list of the positions of the requested players

        List<Vector3> positions = new List<Vector3>();
        foreach(Player player in playersList){
            positions.Add(player.rootRigidbody.worldCenterOfMass);
        }

        return positions;
    }

    public List<Vector3> GetVelocities(List<Player> playersList){
        // Returns a list of the positions of the requested players

        List<Vector3> velocities = new List<Vector3>();
        foreach(Player player in playersList){
            velocities.Add(player.rootRigidbody.velocity);
        }

        return velocities;
    }


    public Vector3 AveragePosition(List<Player> playersList){
        // Calculates the average position of all of the players (skews towards groups of players)

        List<Vector3> playerPositions = GetPositions(playersList);
        Vector3 averagePos = playerPositions.Average();

        return averagePos;
    }

    public BoundingSphere GetBoundingSphere(List<Player> playersList){
        // Calculates the radius and position of a sphere enclosing all given players; returns a BoundingSphere struct with pos and rad properties

        List<Vector3> allPositions = GetPositions(playersList);
        List<Vector3> farthestPoints = BoundingVolume.GetFarthestPointPair(allPositions);

        Vector3 center = farthestPoints.Average();
        float radius = Vector3.Distance(farthestPoints[0], farthestPoints[1]) / 2;

        return new BoundingSphere(center, radius);
    }

    public Vector3 CenterPosition(List<Player> playersList){
        // Calculates the center position of an axis-aligned bounding box containing the players

        List<Vector3> playerPositions = GetPositions(playersList);
        
        Vector3 maxBounds = playerPositions.MaxComponents();
        Vector3 minBounds = playerPositions.MinComponents();

        Vector3 centerPos = (maxBounds + minBounds) / 2;

        return centerPos;
    }


    public Vector3 AverageVelocity(List<Player> playersList){
        // Returns the average velocity of the selected player group
        List<Vector3> velocityList = GetVelocities(playersList);
        Vector3 averageVelocity = velocityList.Average();
        return averageVelocity;
    }


    public Vector3 ScreenSpaceForward(){
        return cameraController.GetCameraForwardDirection();
    }


    public Material GetPlayerMaterial(int playerIndex){
        if(playerIndex < playerMaterials.Count)  return playerMaterials[playerIndex];
        else return playerMaterials[playerMaterials.Count - 1];
    }


    public void KnockoutPlayer(Player player, Vector3 knockoutPosition, GameObject prefabToSpawn = null){
        // Updates player lives and knockouts given. If a prefab is specified, instantiates it
        // at the player's last position. Otherwise, instantiate the default KO effect

        // *** Update lives ***

        // *** Determine who last touched the player, award KO to them ***

        player.RespawnAtPosition(spawnTransform.position);

        // Spawn a KO effect prefab, which includes VFX, SFX, and anything else to spawn
        if(prefabToSpawn) Instantiate(prefabToSpawn, knockoutPosition, Quaternion.identity);
        else Instantiate(defaultKnockoutEffectPrefab, knockoutPosition, Quaternion.identity);

        // Add some camera shake
        cameraController.AddCameraShake(KOCameraShakeAmount);

        // Make the camera keep the knockout location in view for a little bit after the KO
        cameraController.CreateTemporaryAdditionalTarget(knockoutPosition, postKOCameraLingerDuration);
    }


    // Private Functions

    private List<Player> FindStragglers(float separationDist){
        return new List<Player>();
    }

}
