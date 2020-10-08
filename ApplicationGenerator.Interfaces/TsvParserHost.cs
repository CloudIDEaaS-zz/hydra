// file:	TsvParserHost.cs
//
// summary:	Implements the tsv parser host class

using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utils;

namespace AbstraX
{
    /// <summary>   A Template Tsv parser host. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>
    ///
    /// <typeparam name="TRecord">  Type of the record. </typeparam>

    public class TsvParserHost<TRecord> : ITsvParserHost<TRecord>
    {
        /// <summary>   The CSV parser. </summary>
        private TextFieldParser csvParser;
        private Stack<object> parentItemRecords;
        private bool hasItemConverter;
        private int currentLevel;
        private object lastRecord;
        private List<TRecord> topLevelRecords;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>

        public TsvParserHost()
        {
            parentItemRecords = new Stack<object>();
        }

        /// <summary>   Parse file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="path">             Full pathname of the file. </param>
        /// <param name="textReplacements"> The text replacements. </param>
        ///
        /// <returns>   A List&lt;TRecord&gt; </returns>

        public virtual List<TRecord> ParseFile(string path, Dictionary<string, string> textReplacements)
        {
            var type = typeof(TRecord);

            topLevelRecords = new List<TRecord>();

            hasItemConverter = type.GetProperties().Any(p => p.HasCustomAttribute<TsvParserChildRecursionAttribute>());

            using (var csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "\t" });
                csvParser.HasFieldsEnclosedInQuotes = true;
                csvParser.TrimWhiteSpace = true;

                this.csvParser = csvParser;

                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    var fields = csvParser.ReadFields();
                    var record = Activator.CreateInstance<TRecord>();

                    AddRecord(fields, record, topLevelRecords, textReplacements);
                }
            }

            return topLevelRecords;
        }

        /// <summary>   Adds a record. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <exception cref="NotSupportedException">    Thrown when the requested operation is not
        ///                                             supported. </exception>
        ///
        /// <param name="fields">           The fields. </param>
        /// <param name="record">           The record. </param>
        /// <param name="records">          The records. </param>
        /// <param name="textReplacements"> The text replacements. </param>
        /// <param name="startColumnIndex"> (Optional) The start column index. </param>

        private void AddRecord(string[] fields, object record, IList records, Dictionary<string, string> textReplacements, int startColumnIndex = 0)
        {
            var type = record.GetType();
            var properties = type.GetProperties().OrderBy(p => p.HasCustomAttribute<JsonPropertyAttribute>() ? p.GetCustomAttribute<JsonPropertyAttribute>().Order : 0);
            var firstFieldValue = fields.SkipWhile(f => f.IsNullOrEmpty()).First();
            var index = 0;

            foreach (var property in properties)
            {
                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                var childRecursionAttribute = property.GetCustomAttribute<TsvParserChildRecursionAttribute>();

                if (fields.Length > jsonPropertyAttribute.Order + startColumnIndex)
                {
                    var fieldValue = fields[jsonPropertyAttribute.Order + startColumnIndex];

                    fieldValue = textReplacements.HandleTextReplacements(fieldValue);

                    if (childRecursionAttribute != null)
                    {
                        var parms = childRecursionAttribute.ChildRecursionParameters.Cast<string>().ToList();
                        var itemType = childRecursionAttribute.ItemType;
                        var recursionFieldCount = childRecursionAttribute.ItemType.GetProperties().Where(p => !p.HasCustomAttribute<TsvParserChildRecursionAttribute>()).Count();

                        if (parms.Contains("--IndentRecurseColumns") && parms.Contains("--RecursionChildProperty"))
                        {
                            var childPropertyIndex = parms.IndexOf("--RecursionChildProperty") + 1;
                            var recursionChildProperty = parms[childPropertyIndex];
                            var thisLevel = fields.TakeWhile(f => f.IsNullOrEmpty()).Count();
                            object parentRecord = null;
                            IList list;

                            if (thisLevel > currentLevel)
                            {
                                parentItemRecords.Push(lastRecord); 
                                parentRecord = lastRecord;
                            }
                            else if (thisLevel < currentLevel)
                            {
                                for (var x = 0; x < currentLevel - thisLevel; x+= recursionFieldCount)
                                {
                                    if (parentItemRecords.Count > 0)
                                    {
                                        parentItemRecords.Pop();
                                    }
                                }

                                if (parentItemRecords.Count > 0)
                                {
                                    parentRecord = parentItemRecords.Peek();
                                }
                            }
                            else if (thisLevel == currentLevel && parentItemRecords.Count > 0)
                            {
                                parentRecord = parentItemRecords.Peek();
                            }

                            if (parentRecord != null)
                            {
                                var itemRecord = Activator.CreateInstance(childRecursionAttribute.ItemType);

                                if (parentRecord.GetType() == typeof(TRecord))
                                {
                                    list = (IList)property.GetValue(parentRecord);

                                    if (list == null)
                                    {
                                        list = (IList)Activator.CreateInstance(property.PropertyType);

                                        property.SetValue(parentRecord, list);
                                    }
                                }
                                else
                                {
                                    var listProperty = parentRecord.GetProperty(recursionChildProperty);

                                    list = (IList)listProperty.GetValue(parentRecord);

                                    if (list == null)
                                    {
                                        list = (IList)Activator.CreateInstance(listProperty.PropertyType);

                                        listProperty.SetValue(parentRecord, list);
                                    }
                                }

                                AddRecord(fields, itemRecord, list, textReplacements, thisLevel);

                                currentLevel = thisLevel;

                                return;
                            }

                            currentLevel = thisLevel;
                        }
                    }

                    if (!fieldValue.IsNullOrEmpty())
                    {
                        fieldValue = textReplacements.HandleTextReplacements(fieldValue);

                        property.SetValue(record, Convert.ChangeType(fieldValue, property.PropertyType));
                    }

                    index++;
                }
            }

            records.Add(record);
            lastRecord = record;
        }
    }
}