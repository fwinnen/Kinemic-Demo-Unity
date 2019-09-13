///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using Kinemic.Gesture;
	

/// <summary>
/// Class for manang the pause and resume camera buttons
/// </summary>
public class GestureController : MonoBehaviour
{

    private Engine m_engine;

    private float m_x;

    /// <summary>
    /// See Unity docs
    /// </summary>
    private void Start()
    {
        m_engine = new Engine();

        m_engine.RegisterOnGestureListener(new OnGestureListener((gesture) =>
        {
            Debug.Log("ButtonController OnGesture(" + gesture + ")");
        }));

        m_engine.RegisterOnAirmouseEventListener(new OnAirmouseEventListener((x, y, facing) =>
        {
            m_x = x;
            //Debug.Log("ButtonController OnMove(" + x + ", " + y + ", " + facing + ")");
        }));

        m_engine.RegisterOnConnectionStateChangeListener(new OnConnectionStateChangeListener((state, reason) =>
        {
            Debug.Log("ButtonController OnConnectionStateChanged(" + state + ", " + reason + ")");

            if (state == ConnectionState.CONNECTED)
            {
                m_engine.StartAirmouse();
            }
        }));


        m_engine.ConnectStrongest();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * m_x);
    }
}
