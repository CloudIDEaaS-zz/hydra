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
    public class TsvParserHost<TRecord> : ITsvParserHost<TRecord>
    {
        private TextFieldParser csvParser;

        public virtual List<TRecord> ParseFile(string path)
        {
            var records = new List<TRecord>();

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

                    AddRecord(fields, record, records);
                }
            }

            return records;
        }

        private static void AddRecord(string[] fields, object record, IList records, int startColumnIndex = 0)
        {
            var type = record.GetType();
            var properties = type.GetProperties().OrderBy(p => p.HasCustomAttribute<JsonPropertyAttribute>() ? p.GetCustomAttribute<JsonPropertyAttribute>().Order : 0);
            var index = 0;

            foreach (var property in properties)
            {
                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                if (jsonPropertyAttribute.ItemConverterType != null)
                {
                    var parms = jsonPropertyAttribute.ItemConverterParameters.Cast<string>().ToList();
                    var itemType = jsonPropertyAttribute.ItemConverterType;

                    if (parms.Contains("--IndentRecurseColumns") && parms.Contains("--RecursionChildProperty"))
                    {
                        if (startColumnIndex != 0)
                        {
                            var childPropertyIndex = parms.IndexOf("--RecursionChildProperty") + 1;
                            var recursionProperty = parms[childPropertyIndex];
                            var list = (IList)property.GetValue(record);

                            for (var itemIndex = 0; itemIndex < fields.Length - startColumnIndex; itemIndex++)
                            {
                                var itemRecord = Activator.CreateInstance(itemType);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("TsvParser can only suppport one recursion column property");
                        }
                    }
                }
                else
                {
                    var fieldValue = fields[jsonPropertyAttribute.Order + startColumnIndex];

                    if (!fieldValue.IsNullOrEmpty())
                    {
                        property.SetValue(record, Convert.ChangeType(fieldValue, property.PropertyType));
                    }
                }

                index++;
            }

            records.Add(record);
        }
    }
}