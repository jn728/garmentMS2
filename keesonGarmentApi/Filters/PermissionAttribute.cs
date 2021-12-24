namespace keesonGarmentApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionAttribute : Attribute
    {
        public string Code { get; }

        public PermissionAttribute(string code)
        {
            Code = code;
        }
    }
}
