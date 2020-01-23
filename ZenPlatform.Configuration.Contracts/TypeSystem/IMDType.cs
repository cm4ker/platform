namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IMDType
    {
        string RelTableName { get; set; }
        
        string Name { get; set; }
    }
}