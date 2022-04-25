using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    private Vector2 firstClickPos;
    private Vector2 secondClickPos;

    private GameObject mPlayer;

    [SerializeField] private float runSpeed;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float minDistanceSwap = 20f;

    [SerializeField] private bool detectSwipeAfterRelease;

    public static event Action<SwipeData> OnSwipe = delegate { };

    public struct SwipeData
    {
        public Vector2 startPos;
        public Vector2 endPos;
        public SwipeDirection direction;
    }

    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private bool isVerticalSwipe()
    {
        return verticalMovementDistance() > horizontalMovementDistace();
    }

    private bool swipeDistanceCheck()
    {
        return verticalMovementDistance() > minDistanceSwap || horizontalMovementDistace() > minDistanceSwap;
    }

    private float verticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    private float horizontalMovementDistace()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    private void Start()
    {
        mPlayer = this.gameObject;
    }

    void Update()
    {
        continuousMove();
#if UNITY_ANDROID
        detectTouch();
#endif
#if UNITY_EDITOR
        detectClick();
#endif
        
    }

    private void continuousMove()
    {
        float horizontalAxis = Input.GetAxis("Horizontal") * horizontalSpeed * Time.deltaTime;
        mPlayer.GetComponent<Transform>().Translate(horizontalAxis, 0, runSpeed * Time.deltaTime);
    }

    private void detectTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
            }

            if (detectSwipeAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPos = touch.position;
                detectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                detectSwipe();
            }
        }
    }

    private void detectSwipe()
    {
        if (swipeDistanceCheck())
        {
            if (!isVerticalSwipe())
            {
                var direction = fingerDownPos.x - fingerUpPos.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                playerMovement(direction, fingerDownPos);
                sendSwipe(direction);
            }

            fingerUpPos = fingerDownPos;
        }
    }

    private void sendSwipe(SwipeDirection direction)
    {
        SwipeData swipeData = new SwipeData()
        {
            direction = direction,
            startPos = fingerDownPos,
            endPos = fingerUpPos
        };

        Debug.Log("Swipe direction: " + swipeData.direction);
        OnSwipe(swipeData);
    }

    private void playerMovement(Enum direction, Vector2 fingerPos)
    {
        if (direction.Equals(SwipeDirection.Left))
        {
            mPlayer.GetComponent<Transform>().Translate(fingerPos.x * horizontalSpeed * Time.deltaTime, 0, 0);
        }
        else if (direction.Equals(SwipeDirection.Right))
        {
            mPlayer.GetComponent<Transform>().Translate(fingerPos.x * horizontalSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void detectClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (detectSwipeAfterRelease && Input.GetMouseButton(0))
        {
            firstClickPos = Input.mousePosition;
            var direction = firstClickPos.x - secondClickPos.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            playerMovement(direction, firstClickPos);
            sendSwipe(direction);
        }

        if (Input.GetMouseButtonUp(0))
        {
            secondClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            var direction = firstClickPos.x - secondClickPos.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            playerMovement(direction, firstClickPos);
            sendSwipe(direction);
        }
    }

}
