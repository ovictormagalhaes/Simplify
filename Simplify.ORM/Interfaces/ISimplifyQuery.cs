namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyQuery
    {
        ISimplifyQueryBuilder GetBuilder();
        string TableName<T>() where T : SimplifyEntity;
        string ColumnName<T>(string property) where T : SimplifyEntity;
    }
}
