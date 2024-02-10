using System;
using UnityEngine;

public class MouseController : Singleton<MouseController>
{

    public Action<RaycastHit> OnLeftMouseClick;
    public Action<RaycastHit> OnRightMouseClick;
    public Action<RaycastHit> OnMiddleMouseClick;

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.Log("Mouse Clicked" + ray);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            switch (button)
            {
                case 0:
                    OnLeftMouseClick?.Invoke(hit);
                    break;
                case 1:
                    OnRightMouseClick?.Invoke(hit);
                    break;
                case 2:
                    OnMiddleMouseClick?.Invoke(hit);
                    break;
            }
        }
    }

}