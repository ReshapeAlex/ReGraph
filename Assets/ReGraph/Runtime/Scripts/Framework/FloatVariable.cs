using System;
using Reshape.Unity;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace Reshape.Reframework
{
	[CreateAssetMenu(menuName="Reshape/Float Variable", order = 11)]
	public class FloatVariable : VariableScriptableObject
	{	
		[OnValueChanged("OnChangeValue")]
		public float value;
		[ReadOnly]
		[HideInEditorMode]
		public float runtimeValue;
		
		public float GetValue ()
		{
			return runtimeValue;
		}

		public void SetValue (float value)
		{
			if (!IsEqual(value))
			{
				runtimeValue = value;
				OnChanged();
			}
		}

		public void AddValue (float value)
		{
			runtimeValue += value;
			OnChanged();
		}

		public void MinusValue (float value)
		{
			runtimeValue -= value;
			OnChanged();
		}

		public void MultiplyValue (float value)
		{
			runtimeValue *= value;
			OnChanged();
		}

		public void DivideValue (float value)
		{
			runtimeValue /= value;
			OnChanged();
		}
		
		public void RandomValue ()
		{
			runtimeValue = ReRandom.Range(1,101);
			OnChanged();
		}

		public bool IsEqual(float value)
		{
			return value.Equals(runtimeValue);
		}

		public static implicit operator float(FloatVariable f)
	    {
	        return f.runtimeValue;
	    }

	    public static implicit operator string(FloatVariable f)
	    {
	        return f.runtimeValue.ToString();
	    }

	    public override string ToString()
	    {
		    return runtimeValue.ToString();
	    }

	    public override void OnChanged()
	    {
		    onReset -= OnReset;
		    onReset += OnReset;
		    base.OnEarlyChanged();
		    base.OnChanged();
	    }

	    public void OnReset()
	    {
		    SetValue(value);
	    }
	    
#if UNITY_EDITOR
		private void OnChangeValue()
		{
			SetValue(value);
		}
#endif
	}
	
#if UNITY_EDITOR
	[InitializeOnLoad]
	public static class FloatVariableResetOnPlay
	{
	    static FloatVariableResetOnPlay ()
	    {
	        EditorApplication.playModeStateChanged += OnPlayModeChanged;
	    }

	    private static void OnPlayModeChanged (PlayModeStateChange state)
	    {
		    bool update = false;
	        if ( state == PlayModeStateChange.ExitingEditMode )
	        {
		        if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
		            update = true;
	        }
	        else if ( state == PlayModeStateChange.EnteredEditMode )
	        {
		        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			        update = true;
	        }

	        if (update)
	        {
		        string[] guids = AssetDatabase.FindAssets("t:FloatVariable");
		        if ( guids.Length > 0 )
		        {
			        for ( int i=0; i<guids.Length; i++ )
			        {
				        FloatVariable variable = (FloatVariable)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(UnityEngine.Object));
				        if ( variable != null )
				        {
					        variable.OnReset();
				        }
			        }
			        AssetDatabase.SaveAssets();
		        }
	        }
	    }
	}
#endif
}