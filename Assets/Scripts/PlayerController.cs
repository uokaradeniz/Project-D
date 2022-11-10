using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private PlayerInputActions playerInputActions;
    private Vector2 smoothVel;
    private Vector2 currentInputVec;
    private float smoothInputSpeed = 0.2f;
    private Transform playerBody;
    // Start is called before the first frame update
    void Start()
    {
        playerBody = transform.Find("PlayerBody");
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PlayerCombatController>().playerDied)
        {
            Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
            currentInputVec = Vector2.SmoothDamp(currentInputVec, inputVector, ref smoothVel, smoothInputSpeed);
            Vector3 translation =  new Vector3(currentInputVec.x, 0, currentInputVec.y) * Time.deltaTime;
            transform.Translate(translation * moveSpeed);
            Vector3 objPosOnScreen = Camera.main.WorldToScreenPoint(playerBody.transform.position);
            Vector3 lookDir = Input.mousePosition - objPosOnScreen;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            playerBody.rotation = Quaternion.AngleAxis(-angle + 90, Vector3.up);
        }
    }
}
