using EasyBuildSystem.Features.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingSurfaceCollider : MonoBehaviour
{
    [AddComponentMenu("Easy Build System/Buildable Surfaces/Surface Collider")]
        private Bounds SurfaceBounds;

    private void OnDrawGizmosSelected()
    {
        if (SurfaceBounds.size == Vector3.zero)
        {
            SurfaceBounds = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            return;
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.cyan / 2f;
        Gizmos.DrawCube(SurfaceBounds.center, SurfaceBounds.size * 1.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(SurfaceBounds.center, SurfaceBounds.size * 1.1f);
    }
}


