///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Kinemic
{

namespace Gesture
{

public enum AirmousePalmDirection
{

	/** Users palm is pointing downwards. */
	DOWNWARDS,

	/** Users palm is pointing sideways. */
	SIDEWAYS,

	/** Users palm is pointing upwards. */
	UPWARDS,

	/** The system could not conclude the users palm orientation. */
	INCONCLUSIVE,
}

public enum ConnectionState
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

public enum ConnectionReason
{
	NONE,
	PENDING,
	SUCCESS,
	CONNECTION_FAILED,
	BOARD_SETUP_FAILED,
	LOW_BATTERY,
	INCOMPATIBLE_DEVICE,
}

public enum ActivationState
{

	/**
	 * The button was pressed to temporarily disable gesture recognition.
	 */
	INACTIVE,

	/**
	 * The Kinemic Band is active, gestures are recognized.
	 */
	ACTIVE,
}

public enum Gesture
{
	/**
	 * The Rotate RL gesture.
	 */
	ROTATE_RL,

	/**
	 * The Rotate LR gesture.
	 */
	ROTATE_LR,

	/** The Circle R gesture. */
	CIRCLE_R,

	/** The Circle L gesture. */
	CIRCLE_L,

	/** The Swipe R gesture. */
	SWIPE_R,

	/** The Swipe L gesture. */
	SWIPE_L,

	/** The Swipe Up gesture. */
	SWIPE_UP,

	/** The Swipe Down gesture. */
	SWIPE_DOWN,

	/** The Check Mark gesture. */
	CHECK_MARK,

	/** The X Mark gesture. */
	CROSS_MARK,

	/** The Eartouch R gesture. */
	EARTOUCH_R,

	/** The Eartouch L gesture. */
	EARTOUCH_L,
}


public class OnGestureListener : AndroidJavaProxy
{
	public delegate void Callback(Gesture gesture);
	private Callback m_callback;

	public OnGestureListener(Callback callback) : base("de.kinemic.gesture.OnGestureListener")
	{
		m_callback = callback;
	}

	public void onGesture(AndroidJavaObject gesture)
	{
		m_callback((Gesture)gesture.Call<int>("ordinal"));
	}
}

public class OnConnectionStateChangeListener : AndroidJavaProxy
{
	public delegate void Callback(ConnectionState state, ConnectionReason reason);
	private Callback m_callback;

	public OnConnectionStateChangeListener(Callback callback) : base("de.kinemic.gesture.OnConnectionStateChangeListener")
	{
		m_callback = callback;
	}

	public void onConnectionStateChanged(AndroidJavaObject state, AndroidJavaObject reason)
	{
		m_callback((ConnectionState)state.Call<int>("ordinal"), (ConnectionReason)reason.Call<int>("ordinal"));
	}
}

public class OnAirmouseEventListener : AndroidJavaProxy
{
	public delegate void Callback(float x, float y, AirmousePalmDirection facing);
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
		m_callback(x, y, (AirmousePalmDirection)facing.Call<int>("ordinal"));
	}
}


public class Engine
{
	private AndroidJavaObject m_obj;

	public Engine()
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

		m_obj = new AndroidJavaObject("de.kinemic.gesture.Engine", context);

		// ask for permissions
		Input.location.Start(1000);
	}

	public void RegisterOnGestureListener(OnGestureListener listener)
	{
		m_obj.Call("registerOnGestureListener", listener);
	}

	public void UnegisterOnGestureListener(OnGestureListener listener)
	{
		m_obj.Call("unregisterOnGestureListener", listener);
	}

	public void RegisterOnAirmouseEventListener(OnAirmouseEventListener listener)
	{
		m_obj.Call("registerOnAirmouseEventListener", listener);
	}

	public void UnregisterOnAirmouseEventListener(OnAirmouseEventListener listener)
	{
		m_obj.Call("unregisterOnAirmouseEventListener", listener);
	}

	public void RegisterOnConnectionStateChangeListener(OnConnectionStateChangeListener listener)
	{
		m_obj.Call("registerOnConnectionStateChangeListener", listener);
	}

	public void UnregisterOnConnectionStateChangeListener(OnConnectionStateChangeListener listener)
	{
		m_obj.Call("unregisterOnConnectionStateChangeListener", listener);
	}

	public void Connect(string band)
	{
		m_obj.Call("connect", band);
	}

	public void Disconnect(string band)
	{
		m_obj.Call("disconnect", band);
	}

	public void Vibrate(int millis)
	{
		m_obj.Call("vibrate", millis);
	}

	public void StartAirmouse()
	{
		m_obj.Call("startAirmouse");
	}

	public void StopAirmouse()
	{
		m_obj.Call("stopAirmouse");
	}

	public void ConnectStrongest()
	{
		m_obj.Call("connectStrongest", null);
	}
}


	}
}
