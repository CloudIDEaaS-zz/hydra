using System.Collections.Generic;

namespace AbstraX.Validation
{
    public interface IValidationSet
    {
        IEnumerable<ValidationEntry> ValidationEntries { get; }
        IEnumerable<ValidationMask> ValidationMasks { get; }
        ValidationElement PreferredValidationElement { get; }
        ValidationMask PreferredValidationMask { get; }
        string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame);
        void AddValidationEntry(string name, string functionExpression, string errorMessageKey, object validTestValue = null, object invalidTestValue = null, bool isForm = false, string functionCode = null);
        void AddValidationMask(string name, string maskExpression, string unmaskExpression, float priority = 0f);
        void AddValidationElement(string tagName, string inputType = "text", float priority = 0f);
        void AddValidationElement(ValidationElement validationElement);
        void AddValidationElement(string tagName, string inputType, float priority, params KeyValuePair<string, string>[] attributes);
        void AddValidationElement(string tagName, string inputType, params KeyValuePair<string, string>[] attributes);
    }
}