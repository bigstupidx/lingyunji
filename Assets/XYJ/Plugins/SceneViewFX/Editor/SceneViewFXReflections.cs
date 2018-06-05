using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public static class SceneViewFXReflections {
	public static Assembly asm = Assembly.GetAssembly(typeof(Editor));

	public class ReflectionField {
		public FieldInfo fieldInfo;

		public ReflectionField(string baseType, string fieldName, BindingFlags bindingFlags) {
			Type t = asm.GetType(baseType);
			if (t != null) {
				fieldInfo = t.GetField(fieldName, bindingFlags);
			}
		}
	}

	public class ReflectionMethod {
		public MethodInfo methodInfo;

		public ReflectionMethod(string baseType, string method, BindingFlags bindingFlags) {
			Type t = asm.GetType(baseType);
			if (t != null) {
				methodInfo = t.GetMethod(method, bindingFlags);
			}
		}

		public object Invoke(object scope, object[] parameters = null) {
			return methodInfo.Invoke(scope, parameters);
		}
	}

	public class ReflectionProperty {
		public PropertyInfo propertyInfo;

		public ReflectionProperty(string baseType, string property, BindingFlags bindingFlags) {
			Type t = asm.GetType(baseType);
			if (t != null) {
				propertyInfo = t.GetProperty(property, bindingFlags);
			}
		}
	}


	public static object RunMethod(string baseType, string method, BindingFlags bindingFlags, object scope, object[] parameters = null) {
		if (methods == null || !methods.ContainsKey(method)) {
			AddMethod(baseType, method, bindingFlags);
		}
		return methods[method].Invoke(scope, parameters);
	}
	
	public static object GetField(string baseType, string field, BindingFlags bindingFlags, object scope) {
		if (fields == null || !fields.ContainsKey(field)) {
			AddField(baseType, field, bindingFlags);
		}
		return fields[field].fieldInfo.GetValue(scope);
	}

	public static object GetProperty(string baseType, string property, BindingFlags bindingFlags, object scope) {
		if (properties == null || !properties.ContainsKey(property)) {
			AddProperty(baseType, property, bindingFlags);
		}
		return properties[property].propertyInfo.GetGetMethod(true).Invoke(scope, null);
	}

	public static void SetField(string baseType, string field, BindingFlags bindingFlags, object scope, object value) {
		if (fields == null || !fields.ContainsKey(field)) {
			AddField(baseType, field, bindingFlags);
		}
		fields[field].fieldInfo.SetValue(scope, value);
	}

	private static Dictionary<string, ReflectionMethod> methods;
	private static Dictionary<string, ReflectionProperty> properties;
	private static Dictionary<string, ReflectionField> fields;

	public static void AddMethod(string baseType, string method, BindingFlags bindingFlags) {
		if (methods == null) methods = new Dictionary<string, ReflectionMethod>();
		methods.Add(method, new ReflectionMethod(baseType, method, bindingFlags));
	}

	public static void AddField(string baseType, string fieldName, BindingFlags bindingFlags) {
		if (fields == null) fields = new Dictionary<string, ReflectionField>();
		fields.Add(fieldName, new ReflectionField(baseType, fieldName, bindingFlags));
	}

	public static void AddProperty(string baseType, string propertyName, BindingFlags bindingFlags) {
		if (properties == null) properties = new Dictionary<string, ReflectionProperty>();
		properties.Add(propertyName, new ReflectionProperty(baseType, propertyName, bindingFlags));
	}
}

