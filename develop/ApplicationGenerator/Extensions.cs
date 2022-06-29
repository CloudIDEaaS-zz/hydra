// file:	Extensions.cs
//
// summary:	Implements the extensions class

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using System.IO;
using AbstraX.Models.Interfaces;
using System.Reflection;
using System.Configuration;
using System.Drawing;
using AbstraX.Resources;
using AbstraX.MarketingControls.SocialMedia;
using AbstraX.MarketingControls;
using AbstraX;

namespace AbstraX
{
    /// <summary>   An extensions. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public static partial class Extensions 
    {
        /// <summary>   A string extension method that executes the replacements operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <param name="text">         The text to act on. </param>
        /// <param name="replacements"> The replacements. </param>
        ///
        /// <returns>   A string. </returns>

        public static string DoReplacements(this string text, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                text = text.Replace(replacement.Key, replacement.Value);
            }

            return text;
        }

        /// <summary>   The UrlParts extension method that gets the replacements. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <param name="parts">    The parts to act on. </param>
        ///
        /// <returns>   The replacements. </returns>

        public static Dictionary<string, string> GetReplacements(this UrlParts parts)
        {
            var dictionary = new Dictionary<string, string>();

            if (!parts.ReplacementsBuilt)
            {
                parts.BuildReplacements();
            }

            foreach (var part in parts.Values)
            {
                if (part.CalculatedValue != null)
                {
                    dictionary.Add(part.Name.SurroundWith("[", "]"), part.CalculatedValue);
                }
            }

            return dictionary;
        }

        /// <summary>   A ColorPicker extension method that sets a color. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/2/2020. </remarks>
        ///
        /// <param name="colorPicker">  The colorPicker to act on. </param>
        /// <param name="color">        The color. </param>
        /// <param name="colorName">    (Optional) Name of the color. </param>

        public static void SetColor(this ColorPicker colorPicker, Color color, string colorName = null)
        {
            KeyValuePair<string, Color> pair;

            pair = colorPicker.Colors.Where(c => !c.Key.IsNullOrEmpty()).Select(c => new KeyValuePair<string, Color>(c.Key, c.Value.Value)).FirstOrDefault(k => k.Value.A == color.A && k.Value.R == color.R && k.Value.G == color.G && k.Value.B == color.B);

            if (pair.Key != null && colorName == null)
            {
                int index;

                colorName = pair.Key;
                index = colorPicker.Colors.IndexOf(k => k.Key == colorName);

                colorPicker.SelectedIndex = index;
            }
            else
            {
                colorPicker.AddCustomColor(colorName, color, true);
            }
        }

        /// <summary>   A SocialMediaEntry extension method that gets URL part property type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="socialMediaEntry"> The socialMediaList to act on. </param>
        /// <param name="part">             The part. </param>
        ///
        /// <returns>   The URL part property type. </returns>

        public static Type GetUrlPartPropertyType(this SocialMediaEntry socialMediaEntry, string part)
        {
            return typeof(string);
        }

        /// <summary>   A SocialMediaList extension method that gets URL part attributes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="socialMediaEntry"> The socialMediaList to act on. </param>
        /// <param name="part">             The part. </param>
        ///
        /// <returns>   An array of attribute. </returns>

        public static Attribute[] GetUrlPartAttributes(this SocialMediaEntry socialMediaEntry, string part)
        {
            var provider = socialMediaEntry.Provider;
            var attributes = provider.GetAttributes(part);

            return attributes.ToArray();
        }

        /// <summary>   A SocialMediaList extension method that gets URL part attributes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="tellOthers">   The tellOthers to act on. </param>
        /// <param name="part">         The part. </param>
        ///
        /// <returns>   An array of attribute. </returns>

        public static Attribute[] GetUrlPartAttributes(this TellOthers tellOthers, string part)
        {
            var provider = tellOthers.Provider;
            var attributes = provider.GetAttributes(part);

            return attributes.ToArray();
        }


        /// <summary>   An IEntityProperty extension method that query if 'property' is key. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="property"> The property to act on. </param>
        ///
        /// <returns>   True if key, false if not. </returns>

        public static bool IsKey(this IEntityProperty property)
        {
            return property.HasFacetAttribute<KeyAttribute>();
        }


        /// <summary>
        /// An IEnumerable&lt;AbstraX.QueryInfo&gt; extension method that gets query for kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="queries">  The queries. </param>
        /// <param name="kind">     The kind. </param>
        ///
        /// <returns>   The query for kind. </returns>

        public static QueryInfo GetQueryForKind(this IEnumerable<AbstraX.QueryInfo> queries, QueryKind kind)
        {
            return queries.Single(q => q.QueryKind == kind);
        }

        /// <summary>
        /// An IEnumerable&lt;AbstraX.QueryInfo&gt; extension method that query if 'queries' has query
        /// for kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="queries">  The queries. </param>
        /// <param name="kind">     The kind. </param>
        ///
        /// <returns>   True if query for kind, false if not. </returns>

        public static bool HasQueryForKind(this IEnumerable<AbstraX.QueryInfo> queries, QueryKind kind)
        {
            if (queries == null)
            {
                return false;
            }

            return queries.Any(q => q.QueryKind == kind);
        }



        /// <summary>   A List&lt;Module&gt; extension method that adds to the file. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="modules">  The modules to act on. </param>
        /// <param name="file">     The file. </param>

        public static void AddToFile(this List<AbstraX.Module> modules, AbstraX.FolderStructure.File file)
        {
            foreach (var export in modules)
            {
                export.File = file;
            }
        }

        /// <summary>   An IBase extension method that query if 'baseObject' is client identity. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if client identity, false if not. </returns>

        public static bool IsClientIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Client);
        }

        /// <summary>
        /// A HandlerObjectBase extension method that query if 'field' is login identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if login identity, false if not. </returns>

        public static bool IsLoginIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Login);
        }

        /// <summary>
        /// A HandlerObjectBase extension method that query if 'field' is register identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if register identity, false if not. </returns>

        public static bool IsRegisterIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Register);
        }

        /// <summary>
        /// A HandlerObjectBase extension method that query if 'field' is server identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if server identity, false if not. </returns>

        public static bool IsServerIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Server);
        }

        /// <summary>
        /// An IBase extension method that query if 'baseObject' is client identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   True if client identity, false if not. </returns>

        public static bool IsClientIdentity(this IBase baseObject)
        {
            return baseObject.HasIdentityCategoryFlag(IdentityFieldCategory.Client);
        }


        /// <summary>
        /// An IBase extension method that query if 'baseObject' has identity category flag.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">                        The field to act on. </param>
        /// <param name="identityFieldCategoryFlag">    The identity field category flag. </param>
        ///
        /// <returns>   True if identity category flag, false if not. </returns>

        public static bool HasIdentityCategoryFlag(this HandlerObjectBase field, IdentityFieldCategory identityFieldCategoryFlag)
        {
            return field.BaseObject.HasIdentityCategoryFlag(identityFieldCategoryFlag);
        }

        /// <summary>
        /// An IBase extension method that query if 'baseObject' has identity category flag.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="baseObject">                   The baseObject to act on. </param>
        /// <param name="identityFieldCategoryFlag">    The identity field category flag. </param>
        ///
        /// <returns>   True if identity category flag, false if not. </returns>

        public static bool HasIdentityCategoryFlag(this IBase baseObject, IdentityFieldCategory identityFieldCategoryFlag)
        {
            var identityFieldAttribute = baseObject.GetFacetAttribute<IdentityFieldAttribute>();
            var identityFieldKind = identityFieldAttribute.IdentityFieldKind;
            var identityFieldCategoryAttribute = identityFieldKind.GetIdentityFieldCategoryAttribute();
            var identityFieldCategory = identityFieldCategoryAttribute.IdentityFieldCategoryFlags;

            return identityFieldCategory.HasAnyFlag(identityFieldCategoryFlag);
        }

        /// <summary>   An int extension method that creates test string of length. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="length">   The length to act on. </param>
        ///
        /// <returns>   The new test string of length. </returns>

        public static string CreateTestStringOfLength(this int length)
        {
            var builder = new StringBuilder();

            1.Loop(length, (n) =>
            {
                var value = n % 10;

                if (n == 1)
                {
                    builder.Append("\"");
                }

                if (value == 0)
                {
                    builder.AppendFormat("0\" /* {0} */ + \"", n);
                }
                else
                {
                    builder.Append(value);
                }
            });

            builder.Append("\"");

            return builder.ToString();
        }

        /// <summary>
        /// An IGeneratorConfiguration extension method that generates an information.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generatorConfiguration to act on. </param>
        /// <param name="sessionVariables">         The session variables. </param>
        /// <param name="fileType">                 (Optional) Type of the file. </param>
        ///
        /// <returns>   The information. </returns>

        public static StringBuilder GenerateInfo(this IGeneratorConfiguration generatorConfiguration, Dictionary<string, object> sessionVariables, string fileType = null)
        {
            var builder = new StringBuilder();

            builder.AppendLineFormat("FileType: \"{0}\"", fileType);

            if (sessionVariables.ContainsKey("Output"))
            {
                var output = (string)sessionVariables["Output"];

                builder.Append(output);
            }
            else
            {
                if (sessionVariables.ContainsKey("PageName"))
                {
                    var pageName = (string)sessionVariables["PageName"];

                    builder.AppendLineFormat("PageName: \"{0}\"", pageName);
                }

                if (sessionVariables.ContainsKey("Imports"))
                {
                    var imports = (IEnumerable<ModuleImportDeclaration>)sessionVariables["Imports"];

                    builder.AppendLine("-".Repeat(255));

                    foreach (var import in imports)
                    {
                        builder.AppendLineSpaceIndent(2, import.DeclarationCode);
                    }

                    if (sessionVariables.ContainsKey("Exports"))
                    {
                        var exports = (IEnumerable<AbstraX.Module>)sessionVariables["Exports"];

                        builder.AppendLine();

                        foreach (var export in exports)
                        {
                            builder.AppendLine(export.GetDummyCode(2));
                        }
                    }

                    builder.AppendLine("-".Repeat(255));
                }
            }

            return builder;
        }
    }
}
