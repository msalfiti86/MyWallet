namespace OpenWallet.Services;

public static class SharedText
{
    private static readonly Dictionary<string, string> En = new()
    {
        ["Lookups"] = "Lookups", ["ManageLookupText"] = "Manage configurable dropdown values used across wallet screens.",
        ["NewLookup"] = "New lookup", ["LookupName"] = "Lookup name", ["Details"] = "Details", ["Status"] = "Status",
        ["Actions"] = "Actions", ["Active"] = "Active", ["Inactive"] = "Inactive", ["Edit"] = "Edit", ["Delete"] = "Delete",
        ["Lookup"] = "Lookup", ["LookupEditText"] = "The header stores the lookup name. Details store code, English value, and Arabic value.",
        ["Back"] = "Back", ["LookupDetails"] = "Lookup details", ["AddRow"] = "Add row", ["Code"] = "Code",
        ["ValueEn"] = "Value EN", ["ValueAr"] = "Value AR", ["Save"] = "Save"
    };

    private static readonly Dictionary<string, string> Ar = new()
    {
        ["Lookups"] = "القوائم المرجعية", ["ManageLookupText"] = "إدارة قيم القوائم المنسدلة المستخدمة في شاشات المحفظة.",
        ["NewLookup"] = "قائمة جديدة", ["LookupName"] = "اسم القائمة", ["Details"] = "التفاصيل", ["Status"] = "الحالة",
        ["Actions"] = "الإجراءات", ["Active"] = "نشط", ["Inactive"] = "غير نشط", ["Edit"] = "تعديل", ["Delete"] = "حذف",
        ["Lookup"] = "قائمة مرجعية", ["LookupEditText"] = "يحتوي الرأس على اسم القائمة وتحتوي التفاصيل على الكود والقيمة بالإنجليزية والعربية.",
        ["Back"] = "رجوع", ["LookupDetails"] = "تفاصيل القائمة", ["AddRow"] = "إضافة سطر", ["Code"] = "الكود",
        ["ValueEn"] = "القيمة بالإنجليزية", ["ValueAr"] = "القيمة بالعربية", ["Save"] = "حفظ"
    };

    public static string Get(string key, HttpContext httpContext)
    {
        var lang = httpContext.Request.Cookies["openwallet-lang"] ?? httpContext.Request.Query["culture"].ToString();
        var source = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? Ar : En;
        return source.TryGetValue(key, out var value) ? value : key;
    }
}
