namespace WebApi.Permissions;

public static class PermissionConstants 
{
    public static List<string> GeneratePermissionsForModule(string module)
    {
        return new List<string>()
        {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete",
        };
    }
    
    
    public static class Quotes
    {
        public const string View = "Permissions.Quotes.View";
        public const string Create = "Permissions.Quotes.Create";
        public const string Edit = "Permissions.Quotes.Edit";
        public const string Delete = "Permissions.Quotes.Delete";
    }
    
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
    }
}