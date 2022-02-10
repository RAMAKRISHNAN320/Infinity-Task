using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PathCreation;
using PathCreation.Examples;

public class PlayerController : MonoBehaviour, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler
{
    public PathFollowerTest follower;
    public float sensitivity = 2f;

    Vector2 pressPosition;
    float xOffset;


    //CALLED IMMEDIATELY AFTER INPUT IS DETECTED
    public void OnInitializePotentialDrag(PointerEventData _data)
    {
        UIManager.instance.StartGame();
        follower.enabled = true;
    }

    //CALLED IMMEDIATELY AFTER DRAG INPUT IS DETECTED
    public void OnBeginDrag(PointerEventData _data)
    {
        pressPosition = _data.position;
        
    }

    //CALLED EVERY TIME DRAG IS DETECTED
    public void OnDrag(PointerEventData _data)
    {
        if (_data.dragging)
        {
            xOffset = (_data.position - pressPosition).x / Screen.width;
            xOffset *= sensitivity;
            follower.PathSideOffset(xOffset);
            pressPosition = _data.position;

        }
    }


}
