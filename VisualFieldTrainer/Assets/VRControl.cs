//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VRControl : MonoBehaviour
{
	GraphicRaycaster m_Raycaster;
	PointerEventData m_PointerEventData;
	EventSystem m_EventSystem;

	void Start()
	{
		//Fetch the Raycaster from the GameObject (the Canvas)
		m_Raycaster = GetComponent<GraphicRaycaster>();
		//Fetch the Event System from the Scene
		m_EventSystem = GetComponent<EventSystem>();
	}

	void Update()
	{
		//Check if the left Mouse button is clicked
		if (Input.GetKey(KeyCode.Escape))
		{
			//Set up the new Pointer Event
			m_PointerEventData = new PointerEventData(m_EventSystem);
			//Set the Pointer Event Position to that of the mouse position
			Debug.Log("Hit " + Input.mousePosition);
			m_PointerEventData.position = new Vector2(Screen.width/4, Screen.height/2);

			//Create a list of Raycast Results
			List<RaycastResult> results = new List<RaycastResult>();

			//Raycast using the Graphics Raycaster and mouse click position
			m_Raycaster.Raycast(m_PointerEventData, results);

			//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
			foreach (RaycastResult result in results)
			{
				Debug.Log("Hit " + result.gameObject.name);
				//result.gameObject.onClick.Invoke();

				ExecuteEvents.Execute<IPointerClickHandler>(result.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);

				//MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);

				// maybe link this to the SettingManager OnEnable part for clearer work
				//if ((result.gameObject.GetComponent<Button>() != null) | (result.gameObject.GetComponent<Slider>() != null)) {
				//	Debug.Log ("Hit this Button");
				//	result.gameObject.GetComponent<Button>().onClick.Invoke();
				////	result.gameObject.GetComponent<Button> ().enabled = true;
				//}

				// maybe we can do something similar with sliders! But first check whether you're even going to use sliders
			}
		}
	}
}