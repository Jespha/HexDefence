using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : Singleton<MouseController>
{

    public Action OnLeftMouseClick;
    public Action OnRightMouseClick;
    public Action OnMiddleMouseClick;
    public EventSystem eventSystem;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            CheckMouseClick(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CheckMouseClick(1);
        }
        if (Input.GetMouseButtonDown(2))
        {
            CheckMouseClick(2);
        }
    }

    void CheckMouseClick(int button)
    {

            switch (button)
            {
                case 0:
                    OnLeftMouseClick?.Invoke();
                    break;
                case 1:
                    OnRightMouseClick?.Invoke();
                    break;
                case 2:
                    OnMiddleMouseClick?.Invoke();
                    break;
            }
        
    }

}