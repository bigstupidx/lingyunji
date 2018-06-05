#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;



namespace NAudio
{    
    /// <summary>
    /// these will become extension methods once we move to .NET 3.5
    /// </summary>
    public class InvalidDataException : Exception
    {
        public InvalidDataException(string message) : base(message)
        {

        }
    }
}

#endif