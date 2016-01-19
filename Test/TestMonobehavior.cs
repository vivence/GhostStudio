using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Test
{
	public class TestMonobehavior : MonoBehaviour 
	{
		private static void LogTime(string str)
		{
			Debug.LogFormat("{0} at: {1}", str, System.DateTime.Now.ToLongTimeString());
		}
		
		private void CallScriptMethod(string name)
		{
		}
		
		void Awake()
		{
			LogTime("Awake");
			CallScriptMethod("Awake");
		}
		void Start() 
		{
			LogTime("Start");
			CallScriptMethod("Start");
		}
		void Update() 
		{
			LogTime("Update");
			CallScriptMethod("Update");
		}
		void FixedUpdate()
		{
			LogTime("FixedUpdate");
			CallScriptMethod("FixedUpdate");
		}
		void LateUpdate()
		{
			LogTime("LateUpdate");
			CallScriptMethod("LateUpdate");
		}
		void Reset()
		{
			LogTime("Reset");
			CallScriptMethod("Reset");
		}
		
		void OnMouseEnter()
		{
			LogTime("OnMouseEnter");
			CallScriptMethod("OnMouseEnter");
		}
		void OnMouseOver()
		{
			LogTime("OnMouseOver");
			CallScriptMethod("OnMouseOver");
		}
		void OnMouseExit()
		{
			LogTime("OnMouseExit");
			CallScriptMethod("OnMouseExit");
		}
		void OnMouseDown()
		{
			LogTime("OnMouseDown");
			CallScriptMethod("OnMouseDown");
		}
		void OnMouseUp()
		{
			LogTime("OnMouseUp");
			CallScriptMethod("OnMouseUp");
		}
		void OnMouseUpAsButton()
		{
			LogTime("OnMouseUpAsButton");
			CallScriptMethod("OnMouseUpAsButton");
		}
		void OnMouseDrag()
		{
			LogTime("OnMouseDrag");
			CallScriptMethod("OnMouseDrag");
		}
		
		void OnTriggerEnter()
		{
			LogTime("OnTriggerEnter");
			CallScriptMethod("OnTriggerEnter");
		}
		void OnTriggerExit()
		{
			LogTime("OnTriggerExit");
			CallScriptMethod("OnTriggerExit");
		}
		void OnTriggerStay()
		{
			LogTime("OnTriggerStay");
			CallScriptMethod("OnTriggerStay");
		}
		void OnCollisionEnter()
		{
			LogTime("OnCollisionEnter");
			CallScriptMethod("OnCollisionEnter");
		}		
		void OnCollisionExit()
		{
			LogTime("OnCollisionExit");
			CallScriptMethod("OnCollisionExit");
		}
		void OnCollisionStay()
		{
			LogTime("OnCollisionStay");
			CallScriptMethod("OnCollisionStay");
		}
		void OnControllerColliderHit()
		{
			LogTime("OnControllerColliderHit");
			CallScriptMethod("OnControllerColliderHit");
		}
		void OnJointBreak()
		{
			LogTime("OnJointBreak");
			CallScriptMethod("OnJointBreak");
		}
		void OnParticleCollision()
		{
			LogTime("OnParticleCollision");
			CallScriptMethod("OnParticleCollision");
		}
		
		void OnBecameVisible()
		{
			LogTime("OnBecameVisible");
			CallScriptMethod("OnBecameVisible");
		}
		void OnBecameInvisible()
		{
			LogTime("OnBecameInvisible");
			CallScriptMethod("OnBecameInvisible");
		}
		void OnEnable()
		{
			LogTime("OnEnable");
			CallScriptMethod("OnEnable");
		}
		void OnDisable()
		{
			LogTime("OnDisable");
			CallScriptMethod("OnDisable");
		}
		void OnDestroy()
		{
			LogTime("OnDestroy");
			CallScriptMethod("OnDestroy");
		}
		
		void OnLevelWasLoaded()
		{
			LogTime("OnLevelWasLoaded");
			CallScriptMethod("OnLevelWasLoaded");
		}
		void OnPreCull()
		{
			LogTime("OnPreCull");
			CallScriptMethod("OnPreCull");
		}
		void OnPreRender()
		{
			LogTime("OnPreRender");
			CallScriptMethod("OnPreRender");
		}
		void OnPostRender()
		{
			LogTime("OnPostRender");
			CallScriptMethod("OnPostRender");
		}
		void OnRenderObject()
		{
			LogTime("OnRenderObject");
			CallScriptMethod("OnRenderObject");
		}
		void OnWillRenderObject()
		{
			LogTime("OnWillRenderObject");
			CallScriptMethod("OnWillRenderObject");
		}
		void OnGUI()
		{
			LogTime("OnGUI");
			CallScriptMethod("OnGUI");
		}	
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			LogTime("OnRenderImage");
			CallScriptMethod("OnRenderImage");
		}
		void OnDrawGizmosSelected()
		{
			LogTime("OnDrawGizmosSelected");
			CallScriptMethod("OnDrawGizmosSelected");
		}
		void OnDrawGizmos()
		{
			LogTime("OnDrawGizmos");
			CallScriptMethod("OnDrawGizmos");
		}
		void OnApplicationPause()
		{
			LogTime("OnApplicationPause");
			CallScriptMethod("OnApplicationPause");
		}
		void OnApplicationFocus()
		{
			LogTime("OnApplicationFocus");
			CallScriptMethod("OnApplicationFocus");
		}
		void OnApplicationQuit()
		{
			LogTime("OnApplicationQuit");
			CallScriptMethod("OnApplicationQuit");
		}
		
		void OnPlayerConnected()
		{
			LogTime("OnPlayerConnected");
			CallScriptMethod("OnPlayerConnected");
		}
		void OnServerInitialized()
		{
			LogTime("OnServerInitialized");
			CallScriptMethod("OnServerInitialized");
		}
		void OnConnectedToServer()
		{
			LogTime("OnConnectedToServer");
			CallScriptMethod("OnConnectedToServer");
		}
		void OnPlayerDisconnected()
		{
			LogTime("OnPlayerDisconnected");
			CallScriptMethod("OnPlayerDisconnected");
		}
		void OnDisconnectedFromServer()
		{
			LogTime("OnDisconnectedFromServer");
			CallScriptMethod("OnDisconnectedFromServer");
		}
		void OnFailedToConnect()
		{
			LogTime("OnFailedToConnect");
			CallScriptMethod("OnFailedToConnect");
		}
		void OnFailedToConnectToMasterServer()
		{
			LogTime("OnFailedToConnectToMasterServer");
			CallScriptMethod("OnFailedToConnectToMasterServer");
		}
		void OnMasterServerEvent()
		{
			LogTime("OnMasterServerEvent");
			CallScriptMethod("OnMasterServerEvent");
		}
		void OnNetworkInstantiate(NetworkMessageInfo msg)
		{
			LogTime("OnNetworkInstantiate");
			CallScriptMethod("OnNetworkInstantiate");
		}
		void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
		{
			LogTime("OnSerializeNetworkView");
			CallScriptMethod("OnSerializeNetworkView");
		}
	
	}
} // namespace Ghost.Test
