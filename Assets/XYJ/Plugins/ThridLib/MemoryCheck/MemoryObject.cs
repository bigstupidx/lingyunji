#if MEMORY_CHECK
public class MemoryObject
{
    public MemoryObject()
    {
        MemoryObjectMgr.Add(this);
    }

    ~MemoryObject()
    {
        MemoryObjectMgr.Sub(this);
    }
}
#endif