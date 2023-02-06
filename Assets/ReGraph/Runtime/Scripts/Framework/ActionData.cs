using System;
using System.Collections;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Reshape.ReGraph
{
	[Serializable]
	public struct BehaviourEvent
	{
		public BaseBehaviour behaviour;
		[ValueDropdown("DrawActionNameListDropdown", ExpandAllMenuItems = true)]
		public ActionNameChoice action;

		public void Invoke()
		{
			behaviour.Activate(action);
		}

#if UNITY_EDITOR
	    private static IEnumerable DrawActionNameListDropdown()
		{
			return ActionNameChoice.GetActionNameListDropdown();
		}
#endif
	}
	
	[Serializable]
	public struct ActionData
	{
		[ValueDropdown("DrawActionNameListDropdown", ExpandAllMenuItems = true)]
		[LabelText("Name")]
	    public ActionNameChoice actionChoice;
	    [TabGroup("$BehaviourTabName")] 
	    public BehaviourEvent[] behaviours;
	    [TabGroup("$UnityEventTabName")]
	    public UnityEvent activate;
		
	    public void Invoke ()
	    {
		    for (int i = 0; i < behaviours.Length; i++)
			    behaviours[i].Invoke();
		    activate?.Invoke();
	    }
	    
#if UNITY_EDITOR
	    private static IEnumerable DrawActionNameListDropdown()
		{
			return ActionNameChoice.GetActionNameListDropdown();
		}

	    private string BehaviourTabName()
	    {
		    return "Behaviour ("+behaviours.Length+")";
	    }
	    
	    private string UnityEventTabName()
	    {
		    return "Unity Event ("+activate.GetPersistentEventCount()+")";
	    }
#endif
	}
}