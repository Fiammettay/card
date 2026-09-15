using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private Transform interactableTrans;

    [SerializeField]
    private float interactableRadius = 1.2f;

    [SerializeField]
    private LayerMask interactableLayer;

    [Tooltip("允许交互的最大水平角度")]
    [Range(0f, 180f)]
    [SerializeField]
    private float maxInteractableAngle = 90f;

    private readonly Collider[] interactableObjects = new Collider[20];

    private bool canDetect = true;

    public InteractableBase CurrentInteractable
    {
        get
        {
            return GameApp.InteractManager.CurrentInteractable;
        }
    }

    private void Awake()
    {
        interactableTrans = transform.Find("InteractableTrans");
    }

    private void Update()
    {
        if (!canDetect)
        {
            return;
        }

        DetectInteractable();
    }

    private void DetectInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(
            interactableTrans.position,
            interactableRadius,
            interactableObjects,
            interactableLayer,
            QueryTriggerInteraction.Collide
        );

        InteractableBase bestInteractable = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider targetCollider = interactableObjects[i];

            if (targetCollider == null)
            {
                continue;
            }

            InteractableBase interactable =
                targetCollider.GetComponent<InteractableBase>();

            if (interactable == null)
            {
                continue;
            }

            if (!interactable.CanInteract)
            {
                continue;
            }

            Vector3 direction =
                interactable.transform.position - transform.position;

            direction.y = 0f;

            float distance = direction.magnitude;

            Vector3 playerForward = transform.forward;
            playerForward.y = 0f;

            float angle = Vector3.Angle(
                playerForward,
                direction
            );

            if (angle > maxInteractableAngle)
            {
                continue;
            }

            float score = distance + angle * 0.02f;

            if (score < bestScore)
            {
                bestScore = score;
                bestInteractable = interactable;
            }
        }

        GameApp.InteractManager.SetCurrent(bestInteractable);
    }

}
