using System;
using System.Reflection;
using UnityEngine;

namespace ReflectionUnityEditr
{
	public class RefUnityEditr
	{
		private RefHelper _ref;
		public object refObject{
			get{ 
				return _ref.refObject;
			}
		}

		public const BindingFlags DefBindingFlags = BindingFlags.Public | BindingFlags.Instance;
		public RefUnityEditr (string className, object[] constructorParams = null)
		{
			_ref = new ReflectionUnityEditr.RefHelper (typeof(UnityEditor.AssetImporter), className, constructorParams);
		}

		public bool SetFieldValue (string fieldName, object fieldValue, bool igoneType = false, BindingFlags binding = DefBindingFlags)
		{
			return _ref.SetFieldValue (fieldName, fieldValue, binding, igoneType);
		}

		public bool GetFieldValue<T> (string fieldName, ref T value, bool igoneType = false, BindingFlags binding = DefBindingFlags)
		{
			return _ref.GetFieldValue<T> (fieldName, ref value, binding, igoneType);
		}

		public bool SetPropertyValue (string propertyName, object propertyValue, bool igoneType = false, object[] index = null, BindingFlags binding = DefBindingFlags)
		{
			return _ref.SetPropertyValue (propertyName, propertyValue, binding, igoneType, index);
		}

		public bool GetPropertyValue<T> (string propertyName, ref T value, bool igoneType = false, object[] index = null, BindingFlags binding = DefBindingFlags)
		{
			return _ref.GetPropertyValue<T> (propertyName, ref value, binding, igoneType, index);
		}

		public bool GetMethod(string methodName, ref MethodInfo methodInfo, Type[] types = null)
		{
			return _ref.GetMethod (methodName, ref methodInfo, types);
		}

		public bool GetMethods(string methodName, ref MethodInfo methodInfo, BindingFlags binding = DefBindingFlags)
		{
			return _ref.GetMethod (methodName, ref methodInfo, binding | BindingFlags.Instance );
		}
	}   
}
	
