using StellarArmada;
using StellarArmada.Ships;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    [HideInInspector] public EntityMovement shipMovement;
    private Ray cameraRay;
    private RaycastHit hit;

    bool CheckPlayer()
    {
        if (EntityManager.GetEntities().Count < 1) return false;
        if (shipMovement == null) shipMovement = ((Ship)EntityManager.GetEntities()[0]).movement;
        return true;
    }

    public void MoveToPointInSpace()
    {
        shipMovement.CmdMoveToPoint(new Vector3(Random.Range(-300f, 300f), Random.Range(0f, 0f), Random.Range(-300f, 300f)), Quaternion.identity);
    }

    public void StopMovement()
    {
        shipMovement.StopMovement();
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
                    shipMovement.MoveToEntity(hit.transform.GetComponent<Ship>().netId);
                    Debug.Log("Raycast a ship, moving toward it");
                }
                else
                {
                    shipMovement.CmdMoveToPoint(hit.point, Quaternion.identity);
                    Debug.Log("Raycast ground, moving toward it");
                }
            }
        }
    }
}