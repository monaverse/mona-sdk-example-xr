using Oculus.Interaction.Locomotion;
using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableLocomotion : MonoBehaviour
{
    public void Initialize()
    {
        var colliders = GetComponentsInChildren<MeshCollider>();
        for (var i = 0; i < colliders.Length; i++)
            InitializeCollider(colliders[i]);
    }

    private void InitializeCollider(Collider collider)
    {
        var teleport = collider.gameObject.GetComponent<TeleportInteractable>();
        if (teleport == null) teleport = collider.gameObject.AddComponent<TeleportInteractable>();

        var surface = collider.gameObject.GetComponent<ColliderSurface>();
        if (surface == null) surface = collider.gameObject.AddComponent<ColliderSurface>();

        surface.InjectCollider(collider);
        teleport.InjectSurface(surface);
    }
}
