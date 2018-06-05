namespace xys
{
    using UnityEngine;
    using System.Collections.Generic;

    public interface ILSerialized
    {
        List<Object> Objs { get; }
    }
}