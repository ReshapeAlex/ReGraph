using System;
using Reshape.Unity;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace Reshape.Reframework
{
	[CreateAssetMenu(menuName="Reshape/String Variable", order = 12)]
	public class StringVariable : VariableScriptableObject
	{	
		[Multiline(5)]
		[OnValueChanged("OnUpdatePreview")]
		[OnInspectorInit("OnUpdatePreview")]
		public string value;
		[ListDrawerSettings(OnBeginListElementGUI = "OnUpdatePreview")]
		public VariableScriptableObject[] param;
		[ReadOnly]
		[Multiline(5)]
		[HideInEditorMode]
		public string runtimeValue;

		[HideInInspector]
		public bool inited;

#if UNITY_EDITOR
		[Multiline(5)]
		[HideInPlayMode]
		[LabelText("Preview")]
		public string previewValue;		
#endif

		public string GetValue ()
		{
			if (Init())
				SetValue(value);
			return runtimeValue;
		}

		public void SetValue (string value)
		{
			if (param != null)
			{
				string temp = value;
				if (param.Length > 0)
				{
					try
					{
						temp = String.Format(value, param);
					} catch { }
				}
				if (!IsEqual(temp))
				{
					runtimeValue = temp;
					OnChanged();
				}
			}
			else
			{
				if (!IsEqual(value))
				{
					runtimeValue = value;
					OnChanged();
				}
			}
			Init();
		}

		private bool Init()
		{
			if (inited == false)
			{
				inited = true;
				if (param != null)
				{
					for (int i = 0; i < param.Length; i++)
					{
						param[i].onEarlyChange -= OnParamEarlyChanged;
						param[i].onEarlyChange += OnParamEarlyChanged;
					}
				}
				return true;
			}
			return false;
		}

		private void OnParamEarlyChanged()
		{
			SetValue(value);
		}

		public bool IsEqual(string value)
		{
			return string.Equals(runtimeValue, value);
		}

		public static implicit operator string(StringVariable s)
	    {
	        return s.GetValue();
	    }
		
		public override string ToString()
		{
			return GetValue();
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
			inited = false;
		}
		
#if UNITY_EDITOR
		private void OnUpdatePreview()
		{
			if (!EditorApplication.isPlaying)
			{
				SetValue(value);
				if (param != null && param.Length > 0)
				{
					try
					{
						previewValue = String.Format(value, param);
					}
					catch
					{
						previewValue = value;
					}
				}
				else
				{
					previewValue = value;
				}
			}
		}
#endif
	}
	
#if UNITY_EDITOR
	[InitializeOnLoad]
	public static class StringVariableResetOnPlay
	{
	    static StringVariableResetOnPlay ()
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
			    string[] guids = AssetDatabase.FindAssets("t:StringVariable");
			    if ( guids.Length > 0 )
			    {
				    for ( int i=0; i<guids.Length; i++ )
				    {
					    StringVariable variable = (StringVariable)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(UnityEngine.Object));
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