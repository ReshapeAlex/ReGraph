using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Reshape.Unity;
using Reshape.Controller;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using Reshape.Unity.Editor;
#endif

namespace Reshape.ReGraph
{
	[Serializable]
	public struct FlowAction
	{
		[HorizontalGroup]
		[HideLabel]
		public GraphRunner runner;
		[HorizontalGroup]
		[HideLabel]
		[ValueDropdown("DrawActionNameListDropdown", ExpandAllMenuItems = true)]
		public ActionNameChoice actionName;
			
		private static IEnumerable DrawActionNameListDropdown()
		{
			return ActionNameChoice.GetActionNameListDropdown();
		}

		public static void ExecuteList (FlowAction[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				if (actions[i].runner != null && actions[i].actionName != null)
				{
					actions[i].runner.TriggerAction(actions[i].actionName);
				}
			}
		}
	}
	
	[HideMonoScript]
	public class GraphManager : ReSinglonBehaviour <GraphManager>
	{
	    [LabelText("Init Actions")]
	    public FlowAction[] initActions;
        [LabelText("Begin Actions")]
        public FlowAction[] beginActions;
        [LabelText("Tick Actions")]
        public FlowAction[] tickActions;
        [LabelText("Uninit Actions")]
        public FlowAction[] uninitActions;

		private delegate void UpdateDelegate ();
		private UpdateDelegate updateDelegate;

		private string exitSceneName;

		public void Exit(string sceneName)
		{
			exitSceneName = sceneName;
			
			StartSystemUninitFlow();
			updateDelegate = UpdateUninitFlow;
			UpdateUninitFlow();
		}
		
		protected override void Awake ()
	    {
	    	base.Awake();
	        InitSystemFlow();
	    }

	    protected void Start ()
	    {
	    	updateDelegate = UpdateSystemInit;
	    }

	    protected void Update ()
	    {
	    	if ( updateDelegate != null )
	    		updateDelegate();
	    }

	    private void UpdateSystemInit ()
	    {
	    	if ( IsSystemFlowInited() )
	    	{
	    		StartSystemInitFlow();
	        	updateDelegate = UpdateInitFlow;
	        	UpdateInitFlow();
	    	}
	    }

	    private void UpdateInitFlow ()
	    {
	    	UpdateSystemFlow();
	        if (IsSystemInitFlowCompleted())
	        {
		        FlowAction.ExecuteList(initActions);
	        	ReDebug.Log("ReSystemFlowController.UpdateInitFlow", "System Init Flow Completed");
	            StartSystemBeginFlow();
	            updateDelegate = UpdateBeginFlow;
	            UpdateBeginFlow();
	        }
	    }

	    private void UpdateBeginFlow ()
	    {
	        UpdateSystemFlow();
	        if (IsSystemBeginFlowCompleted())
	        {
		        FlowAction.ExecuteList(beginActions);
	        	ReDebug.Log("ReSystemFlowController.UpdateBeginFlow", "System Begin Flow Completed");
	            updateDelegate = UpdateTickFlow;
	        }
	    }

	    private void UpdateTickFlow ()
	    {
	        StartSystemTickFlow();
	        if (!IsSystemTickFlowCompleted())
	        {
	            ReDebug.LogError("ReSystemFlowController.UpdateTickFlow", "System Update Flow Not Completed Within A Frame");
	        }
	        FlowAction.ExecuteList(tickActions);
	    }

	    private void UpdateUninitFlow()
	    {
		    UpdateSystemFlow();
		    if (IsSystemUninitFlowCompleted())
		    {
			    FlowAction.ExecuteList(uninitActions);
			    ReDebug.Log("ReSystemFlowController.OnDestroy", "System Uninit Flow Completed");
			    updateDelegate = null;
			    ClearInstance();
			    ClearSystemFlow();
			    
			    SceneManager.LoadScene(exitSceneName);
		    }
	    }

#if UNITY_EDITOR
		[MenuItem("GameObject/Reshape/Graph Manager", false, 100)]
		public static void AddGraphManager()
	    {
	    	if ( ReEditorHelper.IsInPrefabStage() )
	    	{
	    		ReDebug.LogWarning("Not able to do this action when you are editing a prefab!");
	    		return;
	    	}
	        GameObject[] selected = Selection.gameObjects;
	        if ( selected.Length > 1 )
	        {
		        ReDebug.LogWarning("Not able to do this action when you are selected multiple gameObjects!");
		        return;
	        }

	        GameObject go;
	        if (selected.Length == 0)
	        {
		        go = new GameObject("GraphManager");
	        }
	        else
	        {
		        go = selected[0];
		        if ( go.GetComponent<GraphManager>() != null )
		        {
			        ReDebug.LogWarning("Not able to do this action when selected gameObject already contain the component!");
			        return;
		        }
	        }
	        go.AddComponent<GraphManager>();
			ReDebug.Log("Created GraphManager GameObject!");
			EditorSceneManager.MarkAllScenesDirty();
	    }
#endif
	}
}