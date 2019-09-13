///////////////////////////////////////////////////////////////////////////////
//
// RealWear Development Software, Source Code and Object Code
// (c) RealWear, Inc. All rights reserved.
//
// Contact info@realwear.com for further information about the use of this
// code.
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using WearHFPlugin;



/// <summary>
/// Class for manang the pause and resume camera buttons
/// </summary>
public class ButtonsController : MonoBehaviour
{


    /// <summary>
    /// The on-scren pause button
    /// </summary>
    public Button PauseButton;

    /// <summary>
    /// The on-screen resume button
    /// </summary>
    public Button ResumeButton;

    private WebCamTexture m_webcam = null;
    private WearHF m_wearHf;

    private Color m_colorEnabled = Color.white;
    private Color m_colorDisabled = Color.grey;

    private AndroidJavaObject m_engine;

    /// <summary>
    /// Possible states for a button
    /// </summary>
    private enum ButtonState
    {
        Enabled,
        Disabled
    }

    class OnGestureListener : AndroidJavaProxy
    {
        public delegate void Callback(int gesture);
        private Callback m_callback;

        public OnGestureListener(Callback callback) : base("de.kinemic.gesture.OnGestureListener") {
            m_callback = callback;
        }

        public void onGesture(AndroidJavaObject gesture)
        {
            m_callback(gesture.Call<int>("ordinal"));
        }
    }

    enum ConnectionState
    {

        /** The Kinemic Band is in connecting state. */
        CONNECTING,

        /** The Kinemic Band is in connected state. */
        CONNECTED,

        /** The Kinemic Band is in disconnecting state. */
        DISCONNECTING,

        /** The Kinemic Band is in disconnected state. */
        DISCONNECTED,

        /** The Kinemic Band is in reconnecting state. */
        RECONNECTING,
    }

    class OnConnectionStateListener : AndroidJavaProxy
    {
        public delegate void Callback(ConnectionState state, int reason);
        private Callback m_callback;

        public OnConnectionStateListener(Callback callback) : base("de.kinemic.gesture.OnConnectionStateChangeListener")
        {
            m_callback = callback;
        }

        public void onConnectionStateChanged(AndroidJavaObject state, AndroidJavaObject reason)
        {
            m_callback((ButtonsController.ConnectionState) state.Call<int>("ordinal"), reason.Call<int>("ordinal"));
        }
    }

    class OnAirmouseEventListener : AndroidJavaProxy
    {
        public delegate void Callback(float x, float y, int facing);
        private Callback m_callback;

        public OnAirmouseEventListener(Callback callback) : base("de.kinemic.gesture.OnAirmouseEventListener")
        {
            m_callback = callback;
        }

        public void onClick()
        {

        }

        public void onMove(float x, float y, AndroidJavaObject facing)
        {
            m_callback(x, y, facing.Call<int>("ordinal"));
        }
    }

    /// <summary>
    /// See Unity docs
    /// </summary>
    private void Start()
    {
        if (Input.location.isEnabledByUser)
        {
            Debug.Log("Location enabled");
        } else
        { 
            Debug.Log("Location disabled");
            Input.location.Start(500);
        }
        Debug.Log("ButtonController Start()");
        //m_webcam =
        //    GameObject.Find("RawImage").
        //    GetComponent<CameraController>().
        //    Webcam;

        //m_wearHf = GameObject.Find("WearHF Manager").GetComponent<WearHF>();
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        m_engine = new AndroidJavaObject("de.kinemic.gesture.Engine", context);
        m_engine.Call("registerOnGestureListener", new OnGestureListener((gesture) =>
        {
            Debug.Log("ButtonController OnGesture(" + gesture +")");
        }));

        m_engine.Call("registerOnAirmouseEventListener", new OnAirmouseEventListener((x, y, facing) =>
        {
            //Debug.Log("ButtonController OnMove(" + x + ", " + y + ", " + facing + ")");
        }));

        m_engine.Call("registerOnConnectionStateChangeListener", new OnConnectionStateListener((state, reason) =>
        {
            Debug.Log("ButtonController OnConnectionStateChanged(" + state + ", " + reason + ")");

            if (state == ConnectionState.CONNECTED)
            {
                m_engine.Call("startAirmouse");
            }
        }));


        m_engine.Call("connectStrongest", null);


        ToggleButtonState(
            PauseButton,
            PauseButton.interactable ?
                ButtonState.Enabled : ButtonState.Disabled);

        ToggleButtonState(
            ResumeButton,
            ResumeButton.interactable ?
                ButtonState.Enabled : ButtonState.Disabled);
    }

    /// <summary>
    /// Pause the camera
    /// </summary>
    public void PauseCamera()
    {
        m_webcam.Stop();

        ToggleButtonState(PauseButton, ButtonState.Disabled);
        ToggleButtonState(ResumeButton, ButtonState.Enabled);
    }

    /// <summary>
    /// Resume the camera
    /// </summary>
    public void ResumeCamera()
    {
        m_webcam.Play();

        ToggleButtonState(PauseButton, ButtonState.Enabled);
        ToggleButtonState(ResumeButton, ButtonState.Disabled);
    }

    /// <summary>
    /// Toggle the state of a button
    /// </summary>
    /// <param name="button">The button to toggle</param>
    /// <param name="state">The new state for the button</param>
    private void ToggleButtonState(Button button, ButtonState state)
    {
        Text buttonText = button.GetComponentInChildren<Text>();

        switch (state)
        {
            case ButtonState.Enabled:
                buttonText.color = m_colorEnabled;
                button.interactable = true;

                //m_wearHf.AddVoiceCommand(
                //    buttonText.text, VoiceCommandCallback);
                break;
            case ButtonState.Disabled:
                buttonText.color = m_colorDisabled;
                button.interactable = false;

                //m_wearHf.RemoveVoiceCommand(buttonText.text);
                break;
        }
    }

    /// <summary>
    /// Called when a voice command is triggered by the user
    /// </summary>
    /// <param name="voiceCommand">The voice command that was triggered</param>
    private void VoiceCommandCallback(string voiceCommand)
    {
        if (PauseButton.GetComponentInChildren<Text>().text == voiceCommand)
        {
            PauseCamera();
        }
        else if (
            ResumeButton.GetComponentInChildren<Text>().text == voiceCommand)
        {
            ResumeCamera();
        }
    }
}
