namespace CMS.DataEngine
{
    public static class BaseInfoExtensions
    {
        public static string GetTableName(this ObjectTypeInfo baseInfo)
        {
            return baseInfo.ObjectClassName.Replace(".", "_");
        }
    }
}
