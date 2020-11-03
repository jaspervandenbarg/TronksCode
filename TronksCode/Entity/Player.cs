using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set;}

    [HideInInspector] public Entity entity;
    Plane groundPlane;

    private Joystick joystick;
    Vector2 joystickPoint; // positie voor touch inputs
    public float joystickDistance, playerDistance, pauseButtonDistance; // afstand voor touch inputs
    Vector2 pauseButtonPoint;
    private int touches = 0;

    //slomotion ability
    private bool canSlomo;
    //[HideInInspector] public Slider slider;

    [SerializeField] private bool mobile;


    private void Awake()
    {
        Instance = this;
        entity = gameObject.GetComponentInChildren<Entity>();
        entity.isPlayerEntity = true;
        
        //slider = GameObject.FindGameObjectWithTag("Slomobar").GetComponent<Slider>();
        canSlomo = true;
        //slider.value = 100;

        mobile = NeverUnload.Instance.mobile;
    }

    private void Start()
    {
        // maak een plane op de hoogte van de turrent voor LookAtCursor
        groundPlane = new Plane(Vector3.up, entity.barrelEnd.transform.position);
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        joystickPoint = joystick.transform.position;
        pauseButtonPoint = GameObject.Find("Pause").transform.position;
    }

    private void Update()
    {
        if (entity == null)
            return;
        //Debug.Log("Entity found");

        if (NeverUnload.Instance.timeStopped)
            return;

        if (mobile)
            MobileControls();
        else
            PCControls();        
    }

    private void MobileControls()
    {
        //doe pas iets wanneer aantal touches niet gelijk is aan vorige aantal touches. zodat er maar 1x geschoten wordt
        if(Input.touchCount != touches)
        {
            //shooting als er op het scherm gedrukt wordt
            if (Input.touchCount >= 1)
            {
                // voor alle touches
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Vector2 playerPoint = Camera.main.WorldToScreenPoint(entity.transform.position);
                    Vector2 touchPoint = Input.touches[i].position;
                    //Debug.DrawRay(entity.transform.position, (Camera.main.ScreenToWorldPoint(touchPoint) - entity.transform.position) , Color.red);

                    // als er niet op een ui item geclickt wordt
                    if(!touchPoint.Equals(joystick.joystickTouchPosition) && Vector2.Distance(pauseButtonPoint, touchPoint) > (Screen.height / pauseButtonDistance))
                    {
                        //schieten
                        if (Vector2.Distance(playerPoint, touchPoint) > (Screen.height / playerDistance))
                        {
                            LookAtCursor(entity.turrent, touchPoint);
                            entity.FireMechanic.Fire();
                        }
                        //mijn neerleggen
                        else if (Vector2.Distance(playerPoint, touchPoint) < (Screen.height / playerDistance))
                        {
                            entity.FireMechanic.LayMine();
                        }
                    }


                }
            }
            touches = Input.touchCount;
        }

        //movement  breekt wanneer er tegelijk geschoten wordt.
        float horizontalMovement = 0;
        float verticalMovement = 0;

        horizontalMovement = joystick.Horizontal;
        verticalMovement = joystick.Vertical;

        //movement
        Vector3 direction = new Vector3(horizontalMovement, 0, verticalMovement);
        entity.Move(direction);
    }

    private void PCControls()
    {
        float horizontalMovement = 0;
        float verticalMovement = 0;

        //movement controls
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontalMovement++;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontalMovement--;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            verticalMovement++;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            verticalMovement--;

        //attack controls
        if (Input.GetMouseButtonDown(0))
            entity.FireMechanic.Fire();
        if (Input.GetMouseButtonDown(1))
            entity.FireMechanic.LayMine();

        //slowmotion controls
        //if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.RightControl)) && !entity.UIManagerScript.loading)
        //{
        //    slider.value -= 150 * Time.deltaTime;
        //    if (canSlomo)
        //    {
        //        Time.timeScale = 0.2f;
        //        NeverUnload.Instance.slomoSeconds += Time.unscaledDeltaTime;
        //        NeverUnload.Instance.realSeconds += Time.deltaTime;
        //    }
        //    else Time.timeScale = 1;
        //}
        //else if (!entity.UIManagerScript.loading)
        //{
        //    Time.timeScale = 1;
        //    slider.value += 3 * Time.deltaTime;
        //}

        //canSlomo = (slider.value <= 0.5f) ? false : true;

        //movement
        Vector3 direction = new Vector3(horizontalMovement, 0, verticalMovement);
        entity.Move(direction.normalized);

        //extra lives
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))
            NeverUnload.Instance.playerLives++;

        LookAtCursor(entity.turrent, Input.mousePosition);
    }

    //cast een ray naar een plane op de hoogte van de turrent zodat de turrent perfect schiet waar de muis is of gedrukt wordt
    private void LookAtCursor(GameObject aimer, Vector3 _position)
    {
        Ray ray = Camera.main.ScreenPointToRay(_position);
        float distance;
        Vector3 axis = Vector3.zero;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            axis = (point - aimer.transform.position).normalized;
            axis = new Vector3(axis.x, 0f, axis.z);
        }

        aimer.transform.rotation = Quaternion.LookRotation(axis);
    }
}
