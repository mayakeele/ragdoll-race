using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FrustumMeshGenerator : MonoBehaviour
{
    [Header("Frustum Properties")]
    public float focusPlaneWidth;
    public float focusPlaneHeight;
    [Space]
    public float depthBeyondFocusPlane;
    public float depthBeforeFocusPlane;
    [Space]
    public Camera camera;


    [Header("Generate Frustum Mesh")]
    public bool generateFrustumMesh = false;



    void OnValidate()
    {
        if(generateFrustumMesh){
            generateFrustumMesh = false;

            GenerateModifiedFrustumMesh();
        }
    }



    private void GenerateModifiedFrustumMesh(){
        // Generates a mesh in the shape of a modified frustum, which passes through and is centered on a clipped plane with given parameters.
        // Creates a new GameObject with the mesh attached, which widens in the +z direction.

        float vFOV = camera.fieldOfView;
        float hFOV = Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect);

        // Determine the rate of change of width and height with respect to depth using h and v FOV
        float vRateOfChange = Mathf.Tan(Mathf.Deg2Rad * vFOV / 2);
        float hRateOfChange = Mathf.Tan(Mathf.Deg2Rad * hFOV / 2);

        // Find the tip of the pyramid/tent the frustum derives from
        float vDepthConstraint = focusPlaneHeight / vRateOfChange;
        float hDepthConstraint = focusPlaneWidth / hRateOfChange;
        float minDepthConstraint = Mathf.Min(vDepthConstraint, hDepthConstraint);


        // Create the center points of the two planes bounding the frustum
        Vector3 tipCenter = new Vector3(0,0, -minDepthConstraint);
        Vector3 nearCenter = new Vector3(0,0, Mathf.Max(-depthBeforeFocusPlane, tipCenter.z));
        Vector3 farCenter = new Vector3(0,0, depthBeyondFocusPlane);

        // Calculate the distance of each plane from the tip
        float nearDistFromTip = nearCenter.z - tipCenter.z;
        float farDistFromTip = farCenter.z - tipCenter.z;

        // Calculate the width and height of the pyramid/tent hybrid's tip. One dimension will always be zero, while the other is >= 0
        float tipHeight = focusPlaneHeight - (vRateOfChange * minDepthConstraint);
        float tipWidth = focusPlaneWidth - (hRateOfChange * minDepthConstraint);

        // Calculate the width and height of the near and far bounding planes
        float nearWidthHalf = tipWidth + (hRateOfChange * nearDistFromTip);
        float nearHeightHalf = tipHeight + (vRateOfChange * nearDistFromTip);
        float farWidthHalf = tipWidth + (hRateOfChange * farDistFromTip);
        float farHeightHalf = tipHeight + (vRateOfChange * farDistFromTip);


        // Create vertices at the four corners of the near and far planes
        List<Vector3> vertices = new List<Vector3>{
            farCenter + new Vector3(-farWidthHalf, farHeightHalf, 0),
            farCenter + new Vector3(farWidthHalf, farHeightHalf, 0),
            farCenter + new Vector3(farWidthHalf, -farHeightHalf, 0),
            farCenter + new Vector3(-farWidthHalf, -farHeightHalf, 0),

            nearCenter + new Vector3(-nearWidthHalf, nearHeightHalf, 0),
            nearCenter + new Vector3(nearWidthHalf, nearHeightHalf, 0),
            nearCenter + new Vector3(nearWidthHalf, -nearHeightHalf, 0),
            nearCenter + new Vector3(-nearWidthHalf, -nearHeightHalf, 0)
        };

        // Create faces for the far and near planes. Other faces are unnecessary since convex mesh is generated automatically
        int[] triangles = new int[]{
            0,1,2,
            2,3,0,
            4,5,6,
            6,7,4
        };
        
        // Finally, generate the mesh and gameobject holding it
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.triangles = triangles;

        GameObject frustumContainer = new GameObject("Frustum Mesh");
        frustumContainer.transform.SetParent(transform.parent);
        frustumContainer.transform.localPosition = Vector3.zero;
        frustumContainer.transform.localRotation = Quaternion.identity;
        frustumContainer.transform.localScale = Vector3.one;

        MeshCollider frustumCollider = frustumContainer.AddComponent<MeshCollider>();
        frustumCollider.sharedMesh = mesh;
        frustumCollider.convex = true;

    }
}
