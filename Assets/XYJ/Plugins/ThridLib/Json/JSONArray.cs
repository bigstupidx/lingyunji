using UnityEngine;
using System.Collections.Generic;
using System.Text;

// Based on http://www.json.org/javadoc/org/json/JSONArray.html
/// A JSONArray is an ordered sequence of values. Its external text form is a string wrapped in square brackets with commas separating the values. The internal form is an object having get and opt methods for accessing the values by index, and put methods for adding or replacing values. The values can be any of these types: Boolean, JSONArray, JSONObject, Number, String, or the JSONObject.NULL object.
public class JSONArray {
	
	private List<object> arrayList;

	/// <summary>
	/// Initializes a new instance of the <see cref="JSONArray"/> class.
	/// </summary>
	public JSONArray()
	{
		arrayList = new List<object>();
	}

    JSONArray(List<object> obj)
	{
		arrayList = obj;
	}

    public static JSONArray Create<T>(List<T> objs)
    {
        if (objs == null)
            return new JSONArray();

        List<object> arrayList = new List<object>(objs.Count);
        for (int i = 0; i < objs.Count; ++i)
            arrayList.Add(objs[i]);

        return new JSONArray(arrayList);
    }

    public static JSONArray Create<T>(List<T> objs, System.Func<T, object> fun)
    {
        if (objs == null)
            return new JSONArray();

        List<object> arrayList = new List<object>(objs.Count);
        for (int i = 0; i < objs.Count; ++i)
            arrayList.Add(fun(objs[i]));

        return new JSONArray(arrayList);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="JSONArray"/> class
    /// </summary>
    /// <param name='json'>
    /// string constructor
    /// </param>
    public JSONArray(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			arrayList = new List<object>();
			return;
		}

		int idx = 0;
		bool success = true;
        JSONArray value = JSONObject.ParseArray(ref json, ref idx, ref success);
		if (value != null)
		{
			arrayList = value.arrayList;
		}
		else
		{
            Log.Error("JSONArray.cs constructor ParseArray failed");
			arrayList = new List<object>();
		}
	}
	
	/// <summary>
	/// Get the object in the array specified index.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	public System.Object get(int index)
	{
		if (index < arrayList.Count)
		{
			System.Object value;
			value = arrayList[index];
			if (value is string)
				return JSONObject.unquote((string) value);
			return value;
		}
        Log.Error("JSONArray.cs get(): index " + index + " out of range " + arrayList.Count);
		return null;
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public bool getBoolean(int index)
	{
		bool defaultValue = false;
		
		if (index < arrayList.Count)
		{
			System.Object value;
			value = arrayList[index];
			if (value is bool)
				return (bool) value;
            Log.Error("JSONArray.cs getBoolean() failed cast with index " + index);	
			return defaultValue;
		}

        Log.Error("JSONArray.cs getBoolean(): index " + index + " out of range " + arrayList.Count);
		return defaultValue;
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public double getDouble(int index)
	{
		double defaultValue = 0.0;
		
		if (index < arrayList.Count)
		{
			System.Object obj = arrayList[index];
			if (obj is int || obj is long || obj is double)
			{
				return System.Convert.ToDouble(obj);
			}
            Log.Error("JSONArray.cs getDouble() failed cast with index " + index);	
			return defaultValue;
		}

        Log.Error("JSONArray.cs getDouble(): index " + index + " out of range " + arrayList.Count);
		return defaultValue;
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public int getInt(int index)
	{
		int defaultValue = 0;
		
		if (index < arrayList.Count)
		{
			System.Object obj = arrayList[index];
			if (obj is int || obj is long || obj is double)
			{
				return System.Convert.ToInt32(obj);
			}
            Log.Error("JSONArray.cs getInt() failed cast with index " + index);	
			return defaultValue;
		}

        Log.Error("JSONArray.cs getInt(): index " + index + " out of range " + arrayList.Count);
		return defaultValue;
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public long getLong(int index)
	{
		long defaultValue = 0;
		
		if (index < arrayList.Count)
		{
			System.Object obj = arrayList[index];
			if (obj is int || obj is long || obj is double)
			{
				return System.Convert.ToInt64(obj);
			}
            Log.Error("JSONArray.cs getLong() failed cast with index " + index);	
			return defaultValue;
		}

        Log.Error("JSONArray.cs getLong(): index " + index + " out of range " + arrayList.Count);
		return defaultValue;
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public JSONArray getJSONArray(int index)
	{
		if (index < arrayList.Count)
		{
			System.Object obj = arrayList[index];
			if (obj is JSONArray)
			{
				return (JSONArray) obj;
			}
			else if (obj is string)
				return new JSONArray(JSONObject.unquote((string) obj));

            Log.Error("JSONArray.cs getJSONArray() failed cast with index " + index);	
			return new JSONArray();
		}

        Log.Error("JSONArray.cs getJSONArray(): index " + index + " out of range " + arrayList.Count);
		return new JSONArray();
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public JSONObject getJSONObject(int index)
	{
		if (index < arrayList.Count)
		{
			System.Object obj = arrayList[index];
			if (obj is JSONObject)
			{
				return (JSONObject) obj;
			}
			else if (obj is string)
			{
				return new JSONObject(JSONObject.unquote((string) obj));
			}
            Log.Error("JSONArray.cs getJSONObject() failed cast with index " + index);	
			return new JSONObject();
		}

        Log.Error("JSONArray.cs getJSONObject(): index " + index + " out of range " + arrayList.Count);
		return new JSONObject();
	}
	
	/// <summary>
	/// Get the casted object in the array at the specified index.
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value and logs the error if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public string getString(int index)
	{
		System.Object obj = get (index);
		if (obj == null)
			return string.Empty;
		
		if (obj is string)
			return (string) obj;
		return obj.ToString();
	}
	
	/// <summary>
	/// Returns if a null object is held at the specified index
	/// </summary>
	/// <returns>
	/// If a null object is held at the specified index
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param> 
	public bool isNull(int index)
	{
		System.Object obj = get (index);
		if (obj == null)
			return true;
		return obj == null;
	}
	
	/// <summary>
	/// Length of the array
	/// </summary>
	public int length()
	{
		return arrayList.Count;
	}
	
	/// <summary>
	/// Length of the array
	/// </summary>
	public int Count()
	{
		return length();
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	public System.Object opt(int index)
	{
		if (index < arrayList.Count)
		{
			System.Object value;
			value = arrayList[index];
			if (value is string)
				return JSONObject.unquote((string) value);
			return value;
		}
		return new System.Object();
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns the default value you specify if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='defaultValue'>
	/// The default value to return on failure.
	/// </param>
	public bool optBoolean(int index, bool defaultValue)
	{
		if (index < arrayList.Count)
		{
			object value = arrayList[index];
			if (value is bool)
				return (bool) value;
		}
		return defaultValue;
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public double optDouble(int index)
	{
		return optDouble(index, double.NaN);
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns the default value you specify if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='defaultValue'>
	/// The default value to return on failure.
	/// </param>
	public double optDouble(int index, double defaultValue)
	{
		if (index < arrayList.Count)
		{
			object obj = arrayList[index];
			if (obj is int || obj is long || obj is double)
			{
				return System.Convert.ToDouble(obj);
			}
		}
		return defaultValue;
	}
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public int optInt(int index)
	{
		return optInt(index, 0);
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns the default value you specify if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='defaultValue'>
	/// The default value to return on failure.
	/// </param>
	public int optInt(int index, int defaultValue)
	{
		if (index < arrayList.Count)
		{
			object obj = arrayList[index];
			if (obj is int || obj is long || obj is double)
			{
				return System.Convert.ToInt32(obj);
			}
		}
		return defaultValue;
	}
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public long optLong(int index)
	{
		return optLong(index,0);
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns the default value you specify if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='defaultValue'>
	/// The default value to return on failure.
	/// </param>
	public long optLong(int index, long defaultValue)
	{
		if (index < arrayList.Count)
		{
			object obj = arrayList[index];
			if (obj is int || obj is long || obj is double)
			{
				return System.Convert.ToInt64(obj);
			}
		}
		return defaultValue;
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public string optString(int index)
	{
		return optString(index, string.Empty);
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns the default value you specify if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='defaultValue'>
	/// The default value to return on failure.
	/// </param>
	public string optString(int index, string defaultValue)
	{
		if (index < arrayList.Count)
		{
			object obj = arrayList[index];
			if (obj is string)
				return JSONObject.unquote((string) obj);
			return obj.ToString();
		}
		return defaultValue;
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public JSONArray optJSONArray(int index)
	{
		if (index < arrayList.Count)
		{
			object obj = arrayList[index];
			if (obj is JSONArray)
				return (JSONArray) obj;
		}
		return new JSONArray();
	}
	
	/// <summary>
	/// Same as the get version of this same function, but does not log errors on failure
	/// </summary>
	/// <returns>
	/// The casted object. Returns a default value if the conversion is not possible, or the index is out of bounds.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	public JSONObject optJSONObject(int index)
	{
		if (index < arrayList.Count)
		{
			object obj = arrayList[index];
			if (obj is JSONObject)
				return (JSONObject) obj;
		}
		return new JSONObject();
	}
	
	/// <summary>
	/// Put the specified value at index.
	/// </summary>
	/// <param name='index'>
	/// Which index to put
	/// </param>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(int index, bool value)
	{
		if (index == arrayList.Count)
			put (value);
		arrayList[index]=value;
		return this;
	}
	
	/// <summary>
	/// Put the specified value at index.
	/// </summary>
	/// <param name='index'>
	/// Which index to put
	/// </param>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(int index, double value)
	{
		if (index == arrayList.Count)
			put (value);
		arrayList[index]=value;
		return this;
	}
	
	/// <summary>
	/// Put the specified value at index.
	/// </summary>
	/// <param name='index'>
	/// Which index to put
	/// </param>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(int index, int value)
	{
		if (index == arrayList.Count)
			put (value);
		arrayList[index]=value;
		return this;
	}
	
	/// <summary>
	/// Put the specified value at index.
	/// </summary>
	/// <param name='index'>
	/// Which index to put
	/// </param>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(int index, long value)
	{
		if (index == arrayList.Count)
			put (value);
		arrayList[index]=value;
		return this;
	}
	
	/// <summary>
	/// Put the specified value at index.
	/// </summary>
	/// <param name='index'>
	/// Which index to put
	/// </param>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(int index, System.Object value)
	{
		if (index == arrayList.Count)
			put (value);
		if (value is string)
			arrayList[index]=JSONObject.quote((string) value);
		else
			arrayList[index]=value;
		return this;
	}
	
	/// <summary>
	/// Append the specified value
	/// </summary>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(bool value)
	{
		arrayList.Add(value);
		return this;
	}
	
	/// <summary>
	/// Append the specified value
	/// </summary>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(double value)
	{
		arrayList.Add(value);
		return this;
	}
	
	/// <summary>
	/// Append the specified value
	/// </summary>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(int value)
	{
		arrayList.Add(value);
		return this;
	}
	
	/// <summary>
	/// Append the specified value
	/// </summary>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(long value)
	{
		arrayList.Add(value);
		return this;
	}
	
	/// <summary>
	/// Append the specified value
	/// </summary>
	/// <param name='value'>
	/// The value to put
	/// </param>
	public JSONArray put(System.Object value)
	{
		if (value is string)
			arrayList.Add(JSONObject.quote((string) value));
		else
			arrayList.Add(value);
		return this;
	}
	
	/// <summary>
	/// Remove the item at the specified index
	/// </summary>
	/// <param name='index'>
	/// Index to remove from
	/// </param>
	System.Object remove(int index)
	{
		arrayList.Remove(index);
		return this;
	}
	
	/// <summary>
	/// Returns string representation of the JSONARray
	/// </summary>
	/// <returns>
	/// The string.
	/// </returns>
	public string toString()
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append('[');

		foreach (var v in arrayList) {
			if (v is string)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(v);
				stringBuilder.Append("\"");
			}
			else if (v is bool)
			{
				if ((bool) v)
					stringBuilder.Append("true");
				else
					stringBuilder.Append("false");
			}
            else if (v == null)
            {
                stringBuilder.Append("null");
            }
			else
			{
				stringBuilder.Append(v.ToString());
			}
			stringBuilder.Append(',');
		}
		if (arrayList.Count > 0) {
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
	
	/// <summary>
	/// Same as toString()
	/// </summary>
	/// <returns>
	/// The string.
	/// </returns>
	public override string ToString()
	{
		return toString();
	}
	public bool RemoveAt(int index)
	{
		if(arrayList.Count>index){
			arrayList.RemoveAt (index);
			return true;
		}else{
			return false;
		}
	}

    public void RemoveRange(int index,int count)
    {
        arrayList.RemoveRange(index,count);
    }

    public void Clear()
    {
        arrayList.Clear();
    }
}
