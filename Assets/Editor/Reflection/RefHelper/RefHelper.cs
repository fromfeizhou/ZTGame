using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace ReflectionUnityEditr
{
	internal class RefHelper
	{
		private object _refObject;
		public object refObject{
			get{ 
				return _refObject;
			}
		}
		public RefHelper(Type assemblyType, string className, object[] ConstructorParams = null){
			Assembly asm = Assembly.GetAssembly(assemblyType);
			if (asm != null) {
				Type type = asm.GetType ("UnityEditor." + className);
				if (type != null) {
					Type[] pt = new Type[ConstructorParams.Length];
					for (int i = 0; i < ConstructorParams.Length; i++) {
						pt [i] = ConstructorParams [i].GetType ();
					}
					ConstructorInfo ci = type.GetConstructor (pt);
					if (ci != null) {
						_refObject = ci.Invoke (ConstructorParams); 
					} else {
						Debug.LogError ("found Constructor." + "Type:" + assemblyType + ", className:" + className);
					}
				} else {
					Debug.LogError ("found className. " + "className:" + className);
				}
			} else {
				Debug.LogError ("found Assembly." + "asm:" + assemblyType);
			}
		}
		   
		#region 反射字段 - Field
		private bool GetField(string fieldName, ref FieldInfo fieldInfo, BindingFlags bindings){
			Type t = refObject.GetType();
			if (t != null) {
				fieldInfo = t.GetField (fieldName, bindings);
				if (fieldInfo != null) {
					return true;
				}
			}
			Debug.LogError ("GetField fail." + " FieldName:" + fieldName + "   t:" + bindings);
			return false;
		}
		/// <summary>
		/// 设置字段值
		/// </summary>
		/// <returns><c>true</c>, if field value was set, <c>false</c> otherwise.</returns>
		/// <param name="fieldName">Field name.</param>
		/// <param name="fieldValue">Field value.</param>
		/// <param name="binding">Binding.</param>
		public bool SetFieldValue(string fieldName, object fieldValue, BindingFlags binding, bool igoneType = false){
			FieldInfo fi = null;
			if (GetField (fieldName, ref fi, binding)) {
				if (fi.FieldType.Equals (fieldValue.GetType ()) || igoneType) {
					fi.SetValue (refObject, fieldValue);
					return true;
				}
				Debug.LogError ("SetFieldValue fail, value isn't equals. scrType:" + fieldValue.GetType() + ", destType:" + fi.FieldType);
				return false;
			}
			return false;
		}
		/// <summary>
		/// 获取字段值
		/// </summary>
		/// <returns><c>true</c>, if field value was gotten, <c>false</c> otherwise.</returns>
		/// <param name="fieldName">Field name.</param>
		/// <param name="value">Value.</param>
		/// <param name="binding">Binding.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public bool GetFieldValue<T>(string fieldName, ref T value, BindingFlags binding, bool igoneType = false){
			FieldInfo fi = null;
			if (GetField (fieldName, ref fi, binding)) {
				object refValue = fi.GetValue(refObject);
				if (refValue.GetType ().Equals (typeof(T)) || igoneType) {
					value = (T)refValue;
					return true;
				}
				Debug.LogError ("GetFieldValue fail, value isn't equals. scrType:" + typeof(T) + ", destType:" + fi.FieldType);
				return false;
			}
			return false;
		}
#endregion				
				
		#region 反射属性 - Property
		private bool GetProperty(string propertyName, ref PropertyInfo propertyInfo, BindingFlags bindings){
			Type t = refObject.GetType();
			if (t != null) {
				propertyInfo = t.GetProperty (propertyName, bindings);
				if (propertyInfo != null) {
					return true;
				}
			}
			Debug.LogError ("GetProperty fail." + " PropertyName:" + propertyName + "   t:" + bindings);
			return false;
		}
		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <returns><c>true</c>, if property value was set, <c>false</c> otherwise.</returns>
		/// <param name="propertyName">Property name.</param>
		/// <param name="propertyValue">Property value.</param>
		/// <param name="index">Index.</param>
		/// <param name="binding">Binding.</param>
		public bool SetPropertyValue(string propertyName, object propertyValue, BindingFlags binding, bool igoneType = false, object[] index = null){
			PropertyInfo propertyInfo = null;
			if (GetProperty (propertyName, ref propertyInfo, binding)) {
				if (propertyInfo.PropertyType.Equals (propertyValue.GetType ()) || igoneType) {
					propertyInfo.SetValue (refObject, propertyValue, index);
					return true;
				}
				Debug.LogError ("SetFieldValue fail, value isn't equals. scrType:" + propertyValue.GetType() + ", destType:" + propertyInfo.PropertyType);
				return false;
			}
			return false;
		}
		/// <summary>
		/// 获取属性值
		/// </summary>
		/// <returns><c>true</c>, if property value was gotten, <c>false</c> otherwise.</returns>
		/// <param name="propertyName">Property name.</param>
		/// <param name="value">Value.</param>
		/// <param name="index">Index.</param>
		/// <param name="binding">Binding.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public bool GetPropertyValue<T>(string propertyName, ref T value, BindingFlags binding, bool igoneType = false, object[] index = null){
			PropertyInfo propertyInfo = null;
			if (GetProperty (propertyName, ref propertyInfo, binding)) {
				object refValue = propertyInfo.GetValue(refObject,index);
				if (refValue.GetType ().Equals (typeof(T)) || igoneType) {
					value = (T)refValue;
					return true;
				}
				Debug.LogError ("GetPropertyValue fail, value isn't equals. scrType:" + typeof(T) + ", destType:" + propertyInfo.PropertyType);
				return false;
			}
			return false;
		}
#endregion	


		public bool GetMethod(string methodName, ref MethodInfo methodInfo, Type[] types){
			Type t = refObject.GetType();
			if (t != null) {
				if (types == null)
					methodInfo = t.GetMethod (methodName,new Type[0]);
				else
					methodInfo = t.GetMethod (methodName,types);
				if (methodInfo != null) {
					return true;
				}
			}
			Debug.LogError ("GetMethod fail." + " MethodName:" + methodName);
			return false;
		}

		public bool GetMethod(string methodName, ref MethodInfo methodInfo, BindingFlags binding){
			Type t = refObject.GetType();
			if (t != null) {
				methodInfo = t.GetMethod (methodName,binding);
				if (methodInfo != null) {
					return true;
				}
			}
			Debug.LogError ("GetMethod fail." + " MethodName:" + methodName);
			return false;
		}



		public bool InvokeMethod(string methodName,object[] ConstructorParams){
			MethodInfo methodInfo = null;
			if (ConstructorParams != null && ConstructorParams.Length > 0) {
				Type[] cp = new Type[ConstructorParams.Length];
				for (int i = 0; i < ConstructorParams.Length; i++) {
					cp [i] = ConstructorParams [i].GetType ();
				}
				if (GetMethod (methodName, ref methodInfo, cp)) {
					methodInfo.Invoke (refObject, ConstructorParams);
					return true;
				}
			} else {
				if (GetMethod (methodName, ref methodInfo, null)) {
					methodInfo.Invoke (refObject,new Type[0]);
					return true;
				}
			}
			return false;
		}

		/*
		公开、私有
		有返回值，无返回值
		带参，不带参
		*/
	}
}