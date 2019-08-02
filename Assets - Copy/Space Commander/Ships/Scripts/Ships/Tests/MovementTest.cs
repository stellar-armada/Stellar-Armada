using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Game;
using SpaceCommander.Ships;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementTest : MonoBehaviour
{
    [HideInInspector] public Movement movement;
    private Ray cameraRay;
    private RaycastHit hit;

    bool CheckPlayer()
    {
        if (ShipManager.GetShips().Count < 1) return false;
        if (movement == null) movement = ShipManager.GetShips()[0].movement;
        return true;
    }

    public void MoveToPointInSpace()
    {
        movement.CmdMoveToPointInSpace(new Vector3(Random.Range(-300f, 300f), Random.Range(0f, 0f), Random.Range(-300f, 300f)));
    }

    public void StopMovement()
    {
        movement.CmdStopMovement();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if(!CheckPlayer()) return;
            MoveToPointInSpace();
            
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if(!CheckPlayer()) return;
            StopMovement();
        }
        
        else if (Input.GetMouseButtonDown(0))
        {
            if(!CheckPlayer()) return;
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log(cameraRay);
            if (Physics.Raycast(cameraRay, out hit))
            {

                if (hit.transform.tag == "Ship" && hit.transform != transform)
                {
                    movement.CmdMoveToShip(hit.transform.GetComponent<Ship>().netId);
                    Debug.Log("Raycast a ship, moving toward it");
                }
                else
                {
                    movement.CmdMoveToPointInSpace(hit.point);
                    Debug.Log("Raycast ground, moving toward it");
                }
            }
        }
    }
}