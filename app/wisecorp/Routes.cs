using System.Windows;
using MaterialDesignThemes.Wpf;




public class DynamicViewInfoDictionary : Dictionary<string, object>
{
    public new object this[string key]
    {
        get
        {
            if (base.TryGetValue(key, out var value))
            {
                // Check if the value is a Func<string> and invoke it if so
                if (value is Func<string> func)
                {
                    return func();
                }
                return value;
            }
            return null;
        }
    }
}


public static class Routes
{

    public static readonly Dictionary<string, int> PermissionsLevels = new()
    {
        { "Admin", 3 },
        { "Manager", 2 },
        { "User", 1 },
        { "Guest", 0 }
    };

    public static readonly Dictionary<string, string> HomeViews = new()
    {
        { "Admin", "Views/Admin/ViewAdmin.xaml" },
        { "Manager", "Views/Manager/ViewManageProjects.xaml" },
        { "User", "Views/ViewTimeSheet.xaml" },
        { "Guest", "Views/ViewLogin.xaml" }
    };

    public static readonly Dictionary<string, DynamicViewInfoDictionary> ViewInfos = new()
    {
        {
            "Views/ViewLogin.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("login") },
                { "Icon", PackIconKind.Login},
                { "Permission", 0 },
                { "Hidden", true }
            }
        },
        {
            "Views/Admin/ViewAdmin.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("admin") },
                { "Icon", PackIconKind.ShieldAccountOutline },
                { "Permission", 3 }
            }
        },
        {
            "Views/Admin/ViewSecurityLogs.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("securityLogs") },
                { "Icon", PackIconKind.Security },
                { "Permission", 3 }
            }
        },
        {
            "Views/Manager/ViewManageProjects.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("manageProjects") },
                { "Icon", PackIconKind.BriefcaseSearchOutline },
                { "Permission", 2 }
            }
        },
        {
            "Views/Admin/ViewAjoutAcc.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("addAccount") },
                { "Icon", PackIconKind.AccountPlusOutline },
                { "Permission", 3 }
            }
        },
        {
            "Views/Manager/ViewApproveTS.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("approveTimeSheet") },
                { "Icon", PackIconKind.CalendarCheck },
                { "Permission", 2 }
            }
        },
        {
            "Views/ViewTimeSheet.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("timeSheet") },
                { "Icon", PackIconKind.CalendarClock },
                { "Permission", 1 }
            }
        },
        {
            "Views/ViewForgotPassword.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("iForgotMyPassword") },
                { "Icon", PackIconKind.AccountKeyOutline },
                { "Permission", 0 },
                { "Hidden", true }
            }
        },
        {
            "Views/Manager/ViewAssignProjects.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("assignProjects") },
                { "Icon", PackIconKind.AccountHardHatOutline },
                { "Permission", 2 }
                // { "Permission", 0 }
            }
        },
        // {
        //    "Views/Manager/ViewAjouterProjects.xaml",
        //    new()
        //    {
        //        { "Title", () => (string)Application.Current.FindResource("ajouterProjets") },
        //        { "Icon", PackIconKind.BriefcaseAddOutline },
        //        { "Permission", 2 }
        //        // { "Permission", 0 }
        //    }
        //},
        {
            "Views/ViewProfile.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("profile") },
                { "Icon", PackIconKind.AccountDetailsOutline },
                { "Permission", 1 }
            }
        },
        {
            "Views/ViewSettings.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("settings") },
                { "Icon", PackIconKind.CogOutline },
                { "Permission", 0 }
            }
        },
            {
            "Views/Logout.xaml",
            new()
            {
                { "Title", () => (string)Application.Current.FindResource("logout") },
                { "Icon", PackIconKind.Logout },
                { "Permission", 1 } // permission 1 because guest doesn't need to logout
            }
        },
    };
}