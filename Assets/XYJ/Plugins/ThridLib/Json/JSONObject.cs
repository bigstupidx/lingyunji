using System.Collections.Generic;
using System.Text;
using System;
using System.Globalization;

// Based on http://www.json.org/javadoc/org/json/JSONObject.html
public class JSONObject
{
    private Dictionary<string, object> map;

    public Dictionary<string, object>.Enumerator GetEnumerator()
    {
        return map.GetEnumerator();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JSONObject"/> class.
    /// </summary>
    public JSONObject()
    {
        map = new Dictionary<string, object>();
    }

    public JSONObject(string key, object value, params object[] objs)
    {
        map = new Dictionary<string, object>();
        map.Add(key, value);
        if (objs == null || objs.Length == 0)
            return;

        for (int i = 1; i < objs.Length; i += 2)
        {
            map.Add((string)objs[i - 1], objs[i]);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JSONObject"/> class from a dictionary
    /// </summary>
    /// <param name='dictionary'>
    /// Dictionary.
    /// </param>
    public JSONObject(Dictionary<string, object> dictionary)
    {
        map = dictionary;
    }

    public static JSONObject Create<T>(Dictionary<string, T> objs)
    {
        Dictionary<string, object> map = new Dictionary<string, object>(objs.Count);
        foreach (KeyValuePair<string, T> itor in objs)
            map.Add(itor.Key, itor.Value);

        return new JSONObject(map);
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name='other'>
    /// The JSONObject to copy from
    /// </param>
    public JSONObject(JSONObject other)
    {
        map = new Dictionary<string, object>();

        if (other != null)
        {
            foreach (var keyValuePair in other.map)
            {
                map[keyValuePair.Key] = keyValuePair.Value;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JSONObject"/> class from a string
    /// </summary>
    /// <param name='source'>
    /// The string to initialize from. 
    /// </param>
    public JSONObject(string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            map = new Dictionary<string, object>();
            return;
        }

        //char[] charArray = source.ToCharArray();
        int index = 0;
        bool success = true;

        object obj = ParseValue(ref source, ref index, ref success);
        JSONObject value = (JSONObject)obj;

        if (value == null)
        {
            Log.Error("JSONObject.cs JSONObject() failed at index " + index.ToString());
            map = new Dictionary<string, object>();
            return;
        }
        map = value.map;
    }

    /// <summary>
    /// Accumulate values under a key. It is similar to the put method except that if there is already an object stored under the key then a JSONArray is stored under the key to hold all of the accumulated values. If there is already a JSONArray, then the new value is appended to it. In contrast, the put method replaces the previous value. If only one value is accumulated that is not a JSONArray, then the result will be the same as using put. But if multiple values are accumulated, then the result will be like append.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject accumulate(string key, System.Object value)
    {
        object existingValue;
        if (map.TryGetValue(key, out existingValue))
        {
            if (value is JSONArray)
            {
                JSONArray ja = (JSONArray)existingValue;
                ja.put(value);
                map[key] = ja;
            }
            else
            {
                JSONArray ja = new JSONArray();
                ja.put(existingValue);
                ja.put(value);
                map[key] = ja;
            }
        }
        else
        {
            put(key, value);
        }

        return this;
    }

    /// <summary>
    /// Append values to the array under a key. If the key does not exist in the JSONObject, then the key is put in the JSONObject with its value being a JSONArray containing the value parameter. If the key was already associated with a JSONArray, then the value parameter is appended to it.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject append(string key, System.Object value)
    {
        object existingValue;
        if (map.TryGetValue(key, out existingValue))
        {
            if (value is JSONArray)
            {
                JSONArray ja = (JSONArray)existingValue;
                ja.put(value);
                map[key] = ja;
            }
            else
            {
                JSONArray ja = new JSONArray();
                ja.put(existingValue);
                ja.put(value);
                map[key] = ja;
            }
        }
        else
        {
            JSONArray ja = new JSONArray();
            ja.put(value);
            map[key] = ja;
        }

        return this;
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public System.Object get(string key)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is string)
                return unquote((string)value);
            return value;
        }

        Log.Error("JSONObject.cs: TryGetValue() failed with key " + key);
        return null;
    }

    public bool TryGetValueJSONArray(string key, out JSONArray array)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is JSONArray)
            {
                array = (JSONArray)value;
                return true;
            }
        }

        array = null;
        return false;
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public bool getBoolean(string key)
    {
        bool defaultValue = false;
        System.Object obj = get(key);
        if (obj == null)
            return defaultValue;
        if (obj is bool)
            return (bool)obj;
        if (obj is int)
            return (int)(obj) == 1 ? true : false;
        if (obj is long)
            return (long)(obj) == 1 ? true : false;

        Log.Error("JSONObject.cs: getBoolean() failed cast with key " + key);
        return defaultValue;
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public double getDouble(string key)
    {
        double defaultValue = 0.0;
        System.Object obj = get(key);
        if (obj == null)
            return defaultValue;
        if (obj is int || obj is long || obj is double)
        {
            return System.Convert.ToDouble(obj);
        }
        Log.Error("JSONObject.cs: getDouble() failed cast with key " + key);
        return defaultValue;
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public int getInt(string key)
    {
        int defaultValue = 0;
        System.Object obj = get(key);
        if (obj == null)
            return defaultValue;
        if (obj is int || obj is long || obj is double)
        {
            return System.Convert.ToInt32(obj);
        }
        Log.Error("JSONObject.cs: getInt() failed cast with key " + key);
        return defaultValue;
    }

    public bool Get(string key, out int value)
    {
        value = 0;
        System.Object obj = null;
        if (!map.TryGetValue(key, out obj))
            return false;

        if (obj == null)
            return false;

        if (obj is int || obj is long || obj is double)
        {
            value = System.Convert.ToInt32(obj);
            return true;
        }

        Log.Error("JSONObject.cs: getInt() failed cast with key " + key);
        return false;
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public long getLong(string key)
    {
        long defaultValue = 0;
        System.Object obj = get(key);
        if (obj == null)
            return defaultValue;
        if (obj is int || obj is long || obj is double)
        {
            return System.Convert.ToInt64(obj);
        }
        Log.Error("JSONObject.cs: getLong() failed cast with key " + key);
        return defaultValue;
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public string getString(string key)
    {
        System.Object obj = get(key);
        if (obj == null)
            return string.Empty;
        return obj.ToString();
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public JSONArray getJSONArray(string key)
    {
        System.Object obj = get(key);
        if (obj == null)
            return new JSONArray();
        if (obj is string)
            return new JSONArray((string)obj);
        if (obj is JSONArray)
            return (JSONArray)obj;
        Log.Error("JSONObject.cs: getJSONArray() failed cast with key " + key);
        return new JSONArray();
    }

    /// <summary>
    /// Get the value associated with the specified key, casting if necessary. A default value is returned, and an error logged, if the key does not exist or cannot be casted
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public JSONObject getJSONObject(string key)
    {
        System.Object obj = get(key);
        if (obj == null)
            return new JSONObject();
        if (obj is string)
            return new JSONObject((string)obj);
        if (obj is JSONObject)
            return (JSONObject)obj;
        Log.Error("JSONObject.cs: getJSONObject() failed cast with key " + key);
        return new JSONObject();
    }

    /// <summary>
    /// Returns an array of keynames for the specified JSONObject
    /// </summary>
    /// <returns>
    /// The names.
    /// </returns>
    /// <param name='jo'>
    /// The JSONObject to query
    /// </param>
    static public string[] getNames(JSONObject jo)
    {
        string[] array = new string[jo.map.Count];
        jo.map.Keys.CopyTo(array, 0);
        return array;
    }

    /// <summary>
    /// Returns if the JSONObject contains the specified key
    /// </summary>
    /// <param name='key'>
    /// The key
    /// </param>
    public bool has(string key)
    {
        return map.ContainsKey(key);
    }

    /// <summary>
    /// Increment a property of a JSONObject. If there is no such property, create one with a value of 1. If there is such a property, and if it is an Integer, Long, Double, or Float, then add one to it.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public JSONObject increment(string key)
    {
        System.Object obj = get(key);
        if (obj == null)
        {
            put(key, (int)1);
            return this;
        }

        if (obj is int)
        {
            int x = (int)obj;
            x += 1;
            put(key, x);
        }
        else if (obj is long)
        {
            long x = (long)obj;
            x += 1;
            put(key, x);
        }
        else if (obj is double)
        {
            double x = (double)obj;
            x += 1.0;
            put(key, x);
        }
        else
        {
            Log.Error("JSONObject.cs: increment failed cast with key " + key);
        }

        return this;
    }

    /// <summary>
    /// Returns if the JSONObject contains a null value associated with the specified key
    /// </summary>
    /// <returns>
    /// If the JSONObject contains a null value associated with the specified key
    /// </returns>
    /// <param name='key'>
    /// If set to <c>true</c> key.
    /// </param>
    public bool isNull(string key)
    {
        return map.ContainsKey(key) == false;
    }

    /// <summary>
    /// Number of key/value pairs at the root
    /// </summary>
    public int length()
    {
        return map.Count;
    }

    /// <summary>
    /// Number of key/value pairs at the root
    /// </summary>
    public int Count()
    {
        return length();
    }

    /// <summary>
    /// Produce a JSONArray containing the names of the elements of this JSONObject.
    /// </summary>
    public JSONArray names()
    {
        JSONArray jsonArray = new JSONArray();
        foreach (string s in map.Keys)
        {
            jsonArray.put(s);
        }
        return jsonArray;
    }

    /// <summary>
    /// Same as get(), however returns a predefined defaultValue if no such key exists, or a cast is not possible
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public System.Object opt(string key)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is string)
                return unquote((string)value);
        }
        return new System.Object();
    }

    /// <summary>
    /// Same as get(), however returns defaultValue if no such key exists, or a cast is not possible
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public bool optBoolean(string key, bool defaultValue)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is bool)
                return (bool)value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Get an optional double associated with a key, or NaN if there is no such key or if its value is not a number.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public double optDouble(string key)
    {
        return optDouble(key, double.NaN);
    }

    // <summary>
    /// Get an optional double associated with a key, or defaultValue if there is no such key or if its value is not a number.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public double optDouble(string key, double defaultValue)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is int || value is long || value is double || value is float)
            {
                return System.Convert.ToDouble(value);
            }
        }
        return defaultValue;
    }
    /// <summary>
    /// Get an optional int associated with a key, or 0 if there is no such key or if its value is not a number.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public int optInt(string key)
    {
        return optInt(key, 0);
    }

    /// <summary>
    /// Get an optional int associated with a key, or defaultValue if there is no such key or if its value is not a number.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public int optInt(string key, int defaultValue)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is int || value is long || value is double)
            {
                return System.Convert.ToInt32(value);
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// Get an optional long associated with a key, or 0 if there is no such key or if its value is not a number.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public long optLong(string key)
    {
        return optLong(key, 0);
    }

    /// <summary>
    /// Get an optional long associated with a key, or defaultValue if there is no such key or if its value is not a number.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public long optLong(string key, long defaultValue)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is int || value is long || value is double)
            {
                return System.Convert.ToInt64(value);
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// Get an optional string associated with a key, or an empty string if there is no such key or if its value is not a string.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public string optString(string key)
    {
        return optString(key, string.Empty);
    }

    /// <summary>
    /// Get an optional string associated with a key, or defaultValue if there is no such key or if its value is not a string.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public string optString(string key, string defaultValue)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value == null)
                return defaultValue;
            else if (value is string)
                return unquote((string)value);
            else
                return value.ToString();
        }
        return defaultValue;
    }

    /// <summary>
    /// Get an optional JSONArray associated with a key, or an empty JSONArray if there is no such key or if its value is not a JSONArray.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public JSONArray optJSONArray(string key)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is JSONArray)
            {
                return (JSONArray)value;
            }
        }
        return new JSONArray();
    }

    /// <summary>
    /// Get an optional JSONObject associated with a key, or an empty JSONObject if there is no such key or if its value is not a JSONObject.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public JSONObject optJSONObject(string key)
    {
        object value;
        if (map.TryGetValue(key, out value))
        {
            if (value is JSONObject)
            {
                return (JSONObject)value;
            }
        }
        return new JSONObject();
    }

    /// <summary>
    /// Put the specified value associated with key
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject put(string key, bool value)
    {
        map[key] = value;
        return this;
    }

    /// <summary>
    /// Put the specified value associated with key
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject put(string key, double value)
    {
        map[key] = value;
        return this;
    }

    /// <summary>
    /// Put the specified value associated with key
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject put(string key, int value)
    {
        map[key] = value;
        return this;
    }

    /// <summary>
    /// Put the specified value associated with key
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject put(string key, long value)
    {
        map[key] = value;
        return this;
    }

    /// <summary>
    /// Put the specified JSONObject denoted by a Dictionary
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject put(string key, Dictionary<string, object> value)
    {
        map[key] = new JSONObject(value);
        return this;
    }

    /// <summary>
    /// Put the specified value associated with key
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject put(string key, System.Object value)
    {
        if (value is string)
            map[key] = quote((string)value);
        else
            map[key] = value;
        return this;
    }

    /// <summary>
    /// Put a key/value pair in the JSONObject, but only if the key and the value are both non-null, and only if there is not already a member with that name.
    /// </summary>
    /// <returns>
    /// this
    /// </returns>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject putOnce(string key, System.Object value)
    {
        if (string.IsNullOrEmpty(key) || value == null || has(key))
            return this;
        if (value is string)
            map[key] = quote((string)value);
        else
            map[key] = value;
        return this;
    }

    /// <summary>
    /// Put a key/value pair in the JSONObject, but only if the key and the value are both non-null.
    /// </summary>
    /// <returns>
    /// this
    /// </returns>
    /// <param name='key'>
    /// Key.
    /// </param>
    /// <param name='value'>
    /// Value.
    /// </param>
    public JSONObject putOpt(string key, System.Object value)
    {
        if (string.IsNullOrEmpty(key) || value == null)
            return this;
        if (value is string)
            map[key] = quote((string)value);
        else
            map[key] = value;
        return this;
    }

    /// <summary>
    /// Produce a string in double quotes with backslash sequences in all the right places.
    /// </summary>
    /// <param name='value'>
    /// Value.
    /// </param>
    public static string quote(string value)
    {
        string quoted = value.Replace("\\", "\\\\");
        quoted = quoted.Replace("\"", "\\\"").Replace("\n", "\\n");
        return quoted;
    }

    /// <summary>
    /// Does the opposite of quote()
    /// </summary>
    /// <param name='value'>
    /// Value.
    /// </param>
    public static string unquote(string value)
    {
        if (value.Contains("\\\"") || value.Contains("\\n") || value.Contains("\\\\"))
        {
            string unquoted = string.Copy((string)value);
            unquoted = unquoted.Replace("\\\"", "\"");
            unquoted = unquoted.Replace("\\n", "\n");
            unquoted = unquoted.Replace("\\\\", "\\");
            return unquoted;
        }

        return value;
    }

    /// <summary>
    /// Removes the value at the specified key.
    /// </summary>
    /// <param name='key'>
    /// Key.
    /// </param>
    public System.Object remove(string key)
    {
        System.Object obj = get(key);
        map.Remove(key);
        return obj;
    }

    public void GetText(StringBuilder stringBuilder)
    {
        stringBuilder.Append('{');

        foreach (KeyValuePair<string, object> pair in map)
        {
            stringBuilder.Append('\"' + pair.Key + '\"');
            stringBuilder.Append(": ");
            object v = pair.Value;
            if (v is string)
            {
                stringBuilder.Append('\"');
                stringBuilder.Append(v);
                stringBuilder.Append('\"');
            }
            else if (v is bool)
            {
                if ((bool)v)
                    stringBuilder.Append("true");
                else
                    stringBuilder.Append("false");
            }
            else
            {
                if (v == null)
                    stringBuilder.Append("null");
                else
                    stringBuilder.Append(v.ToString());
            }
            stringBuilder.Append(',');
        }
        if (map.Count > 0)
        {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }

        stringBuilder.Append('}');
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="JSONObject"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents the current <see cref="JSONObject"/>.
    /// </returns>
    public string toString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        GetText(stringBuilder);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="JSONObject"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents the current <see cref="JSONObject"/>.
    /// </returns>
    public override string ToString()
    {
        return toString();
    }

    // Following code is from 
    // http://techblog.procurios.nl/k/618/news/view/14605/14863/how-do-i-write-my-own-parser-(for-json).html
    // From the website: "The software is subject to the MIT license: you are free to use it in any way you like, but it must keep its license."

    protected const int TOKEN_NONE = 0;
    protected const int TOKEN_CURLY_OPEN = 1;
    protected const int TOKEN_CURLY_CLOSE = 2;
    protected const int TOKEN_SQUARED_OPEN = 3;
    protected const int TOKEN_SQUARED_CLOSE = 4;
    protected const int TOKEN_COLON = 5;
    protected const int TOKEN_COMMA = 6;
    protected const int TOKEN_STRING = 7;
    protected const int TOKEN_NUMBER = 8;
    protected const int TOKEN_TRUE = 9;
    protected const int TOKEN_FALSE = 10;
    protected const int TOKEN_NULL = 11;

    protected static object ParseValue(ref string json, ref int index, ref bool success)
    {
        switch (LookAhead(ref json, index))
        {
        case TOKEN_STRING:
            return ParseString(ref json, ref index, ref success);
        case TOKEN_NUMBER:
            return ParseNumber(ref json, ref index, ref success);
        case TOKEN_CURLY_OPEN:
            return ParseObject(ref json, ref index, ref success);
        case TOKEN_SQUARED_OPEN:
            return ParseArray(ref json, ref index, ref success);
        case TOKEN_TRUE:
            NextToken(ref json, ref index);
            return true;
        case TOKEN_FALSE:
            NextToken(ref json, ref index);
            return false;
        case TOKEN_NULL:
            NextToken(ref json, ref index);
            return null;
        case TOKEN_NONE:
            break;
        }

        success = false;
        return null;
    }

    protected static JSONObject ParseObject(ref string json, ref int index, ref bool success)
    {
        JSONObject table = new JSONObject();
        int token;

        // {
        NextToken(ref json, ref index);

        bool done = false;
        while (!done)
        {
            token = LookAhead(ref json, index);
            if (token == TOKEN_NONE)
            {
                success = false;
                return null;
            }
            else if (token == TOKEN_COMMA)
            {
                NextToken(ref json, ref index);
            }
            else if (token == TOKEN_CURLY_CLOSE)
            {
                NextToken(ref json, ref index);
                return table;
            }
            else
            {

                // name
                string name = ParseString(ref json, ref index, ref success);
                if (!success)
                {
                    success = false;
                    return null;
                }

                // :
                token = NextToken(ref json, ref index);
                if (token != TOKEN_COLON)
                {
                    success = false;
                    return null;
                }

                // value
                object value = ParseValue(ref json, ref index, ref success);
                if (!success)
                {
                    success = false;
                    return null;
                }

                if (value is bool)
                    table.put(name, (bool)value);
                else if (value is double)
                    table.put(name, (double)value);
                else if (value is int)
                    table.put(name, (int)value);
                else if (value is long)
                    table.put(name, (long)value);
                else if (value is string)
                    table.put(name, (string)value);
                else if (value is JSONObject)
                    table.put(name, (JSONObject)value);
                else if (value is JSONArray)
                    table.put(name, (JSONArray)value);
                else
                    table.put(name, value);

                //Debug.LogError("JSONObject ParseObject unknown type for array.put()");
            }
        }

        return table;
    }

    public static JSONArray ParseArray(ref string json, ref int index, ref bool success)
    {
        JSONArray array = new JSONArray();

        // [
        NextToken(ref json, ref index);

        bool done = false;
        while (!done)
        {
            int token = LookAhead(ref json, index);
            if (token == TOKEN_NONE)
            {
                success = false;
                return null;
            }
            else if (token == TOKEN_COMMA)
            {
                NextToken(ref json, ref index);
            }
            else if (token == TOKEN_SQUARED_CLOSE)
            {
                NextToken(ref json, ref index);
                break;
            }
            else
            {
                object value = ParseValue(ref json, ref index, ref success);
                if (!success)
                {
                    return null;
                }

                if (value is bool)
                    array.put((bool)value);
                else if (value is double)
                    array.put((double)value);
                else if (value is int)
                    array.put((int)value);
                else if (value is long)
                    array.put((long)value);
                else if (value is string)
                    array.put((string)value);
                else if (value is JSONObject)
                    array.put(((JSONObject)value));
                else if (value is JSONArray)
                    array.put(((JSONArray)value));
                // 				else
                // 					Debug.LogError("JSONObject ParseArray unknown type for array.put()");

            }
        }

        return array;
    }


    protected static string ParseString(ref string json, ref int index, ref bool success)
    {
        StringBuilder s = new StringBuilder();
        char c;

        EatWhitespace(ref json, ref index);

        // "
        c = json[index++];

        bool complete = false;
        while (!complete)
        {

            if (index == json.Length)
            {
                break;
            }

            c = json[index++];
            if (c == '"')
            {
                complete = true;
                break;
            }
            else if (c == '\\')
            {

                if (index == json.Length)
                {
                    break;
                }
                c = json[index++];
                if (c == '"')
                {
                    s.Append('"');
                }
                else if (c == '\\')
                {
                    s.Append('\\');
                }
                else if (c == '/')
                {
                    s.Append('/');
                }
                else if (c == 'b')
                {
                    s.Append('\b');
                }
                else if (c == 'f')
                {
                    s.Append('\f');
                }
                else if (c == 'n')
                {
                    s.Append('\n');
                }
                else if (c == 'r')
                {
                    s.Append('\r');
                }
                else if (c == 't')
                {
                    s.Append('\t');
                }
                else if (c == 'u')
                {
                    int remainingLength = json.Length - index;
                    if (remainingLength >= 4)
                    {
                        // parse the 32 bit hex into an integer codepoint
                        uint codePoint;
                        if (!(success = UInt32.TryParse(json.Substring(index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                        {
                            return "";
                        }
                        // convert the integer codepoint to a unicode char and add to string
                        try
                        {
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                        }
                        catch (Exception /*ex*/)
                        {
                            s.Append("?");
                        }

                        // skip 4 chars
                        index += 4;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                s.Append(c);
            }

        }

        if (!complete)
        {
            success = false;
            return null;
        }

        return s.ToString();
    }

    protected static object ParseNumber(ref string json, ref int index, ref bool success)
    {
        EatWhitespace(ref json, ref index);

        int lastIndex = GetLastIndexOfNumber(ref json, index);
        int charLength = (lastIndex - index) + 1;

        double numberd;
        string numberAsString = json.Substring(index, charLength);
        success = Double.TryParse(numberAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out numberd);

        // KevinJ: Modded to return a long, if possible, rather than always a double
        if (success)
        {
            if (Math.Floor(numberd) == numberd)
            {
                long numberl;
                if (long.TryParse(numberAsString, out numberl))
                {
                    index = lastIndex + 1;
                    return numberl;
                }
            }
        }

        index = lastIndex + 1;
        return numberd;
    }

    protected static int GetLastIndexOfNumber(ref string json, int index)
    {
        int lastIndex;

        for (lastIndex = index; lastIndex < json.Length; lastIndex++)
        {
            if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
            {
                break;
            }
        }
        return lastIndex - 1;
    }

    protected static void EatWhitespace(ref string json, ref int index)
    {
        for (; index < json.Length; index++)
        {
            if (" \t\n\r".IndexOf(json[index]) == -1)
            {
                break;
            }
        }
    }

    protected static int LookAhead(ref string json, int index)
    {
        int saveIndex = index;
        return NextToken(ref json, ref saveIndex);
    }

    protected static int NextToken(ref string json, ref int index)
    {
        EatWhitespace(ref json, ref index);

        if (index == json.Length)
        {
            return TOKEN_NONE;
        }

        char c = json[index];
        index++;
        switch (c)
        {
        case '{':
            return TOKEN_CURLY_OPEN;
        case '}':
            return TOKEN_CURLY_CLOSE;
        case '[':
            return TOKEN_SQUARED_OPEN;
        case ']':
            return TOKEN_SQUARED_CLOSE;
        case ',':
            return TOKEN_COMMA;
        case '"':
            return TOKEN_STRING;
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
        case '-':
            return TOKEN_NUMBER;
        case ':':
            return TOKEN_COLON;
        }
        index--;

        int remainingLength = json.Length - index;

        // false
        if (remainingLength >= 5)
        {
            if (json[index] == 'f' &&
                json[index + 1] == 'a' &&
                json[index + 2] == 'l' &&
                json[index + 3] == 's' &&
                json[index + 4] == 'e')
            {
                index += 5;
                return TOKEN_FALSE;
            }
        }

        // true
        if (remainingLength >= 4)
        {
            if (json[index] == 't' &&
                json[index + 1] == 'r' &&
                json[index + 2] == 'u' &&
                json[index + 3] == 'e')
            {
                index += 4;
                return TOKEN_TRUE;
            }
        }

        // null
        if (remainingLength >= 4)
        {
            if (json[index] == 'n' &&
                json[index + 1] == 'u' &&
                json[index + 2] == 'l' &&
                json[index + 3] == 'l')
            {
                index += 4;
                return TOKEN_NULL;
            }
        }

        return TOKEN_NONE;
    }
}
