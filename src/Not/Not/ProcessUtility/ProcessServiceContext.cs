namespace Not.ProcessUtility;

public class ProcessServiceContext
{
    public ProcessServiceContext(string parentPID)
    {
       ParentProcessID = int.Parse(parentPID);
    }

    public int ParentProcessID { get; set; }
}
