using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDKInterfaceLibrary.Entities;
using Utils;
using System.Numerics;
using System.Data.Objects.DataClasses;

namespace VisualStudioProvider.PDB.Headers
{
    public static class HeaderEntityExtensions
    {
        public static SdkInterfaceLibraryEntities Entities { private get; set; }
        public static event EventHandlerT<string> WriteLog;
        public static event EventHandler IndentLog;
        public static event EventHandler OutdentLog;
        private static System.Type thisType = typeof(HeaderEntityExtensions);
    
        public static bool InheritsFrom(this System.Type type, string baseType)
        {
            while (type != null)
            {
                type = type.BaseType;

                if (type != null && type.Name == baseType)
                {
                    return true;
                }
            }

            return false;
        }


        public static EntityObject GetEntityParent(this object entityObject)
        {
            return null;
        }

        public static bool IsGetCompatibleEntityObject(this System.Type type, ref object entityObject)
        {
            while (entityObject != null)
            {
                if (type == entityObject.GetType())
                {
                    return true;
                }

                entityObject = entityObject.GetEntityParent();
            }

            return false;
        }

        public static Guid? DoSave(object obj, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            var method = typeof(HeaderEntityExtensions).GetExtensionMethods().Single(m => m.Name == "SaveOrGetId" && m.GetParameters().Length == 5 && m.GetParameters().First().ParameterType.FullName == obj.GetType().FullName);

            return (Guid?) method.Invoke(null, new object[] { obj, string.Empty, tblSdkHeaderFile, noRecurse, false });
        }

        public static Guid? DoSave(object obj, tblSDKHeaderFile tblSdkHeaderFile, object owningObject, bool noRecurse = false)
        {
            var method = typeof(HeaderEntityExtensions).GetExtensionMethods().Single(m => m.Name == "SaveOrGetId" && m.GetParameters().Length == 6 && m.GetParameters().First().ParameterType.FullName == obj.GetType().FullName && m.GetParameters().ElementAt(3).ParameterType.IsGetCompatibleEntityObject(ref owningObject));

            return (Guid?) method.Invoke(null, new object[] { obj, string.Empty, tblSdkHeaderFile, owningObject, noRecurse, false });
        }

        public static Guid? DoSave(object obj, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            var method = typeof(HeaderEntityExtensions).GetExtensionMethods().Single(m => m.Name == "SaveOrGetId" && m.GetParameters().Length == 5 && m.GetParameters().First().ParameterType.FullName == obj.GetType().FullName);

            return (Guid?) method.Invoke(null, new object[] { obj, parentPropertyName, tblSdkHeaderFile, noRecurse, false });
        }

        public static Guid? DoSave(object obj, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, object owningObject, bool noRecurse = false)
        {
            var method = typeof(HeaderEntityExtensions).GetExtensionMethods().Single(m => m.Name == "SaveOrGetId" && m.GetParameters().Length == 6 && m.GetParameters().First().ParameterType.FullName == obj.GetType().FullName  && m.GetParameters().ElementAt(3).ParameterType.IsGetCompatibleEntityObject(ref owningObject));

            return (Guid?) method.Invoke(null, new object[] { obj, parentPropertyName, tblSdkHeaderFile, owningObject, noRecurse, false });
        }

        public static Guid? Save(this IntegerValue integerValue, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(integerValue, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this IntegerValue integerValue, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(integerValue, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this IntegerValue integerValue, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (integerValue == null)
            {
                return null;
            }
            else if (checkInheritingSave && integerValue.GetType().InheritsFrom("IntegerValue"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(integerValue, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(integerValue, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = integerValue.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "IntegerValue", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderIntegerValue = Entities.SaveIfNotExists<tblSDKHeaderIntegerValue>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "IntegerValue")));

                    return new tblSDKHeaderIntegerValue
                    {
                        IntegerValueId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Value = (long) integerValue.Value,
                        StringValue = integerValue.StringValue,
                        ValueType = integerValue.ValueType,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "IntegerValue")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderIntegerValue.Value != (long) integerValue.Value)
                    {
                        tblSDKHeaderIntegerValue.Value = (long) integerValue.Value;
                        resave = true;
                    }

                    if (tblSDKHeaderIntegerValue.StringValue != integerValue.StringValue)
                    {
                        tblSDKHeaderIntegerValue.StringValue = integerValue.StringValue;
                        resave = true;
                    }

                    if (tblSDKHeaderIntegerValue.ValueType != integerValue.ValueType)
                    {
                        tblSDKHeaderIntegerValue.ValueType = integerValue.ValueType;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderIntegerValue.IntegerValueId;
            }
        }

        public static Guid? Save(this Type type, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(type, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Type type, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(type, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Type type, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (type == null)
            {
                return null;
            }
            else if (checkInheritingSave && type.GetType().InheritsFrom("Type"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(type, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(type, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = type.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Type", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderType = Entities.SaveIfNotExists<tblSDKHeaderType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Type")));

                    return new tblSDKHeaderType
                    {
                        TypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Kind = type.Kind,
                        IsDependent = type.IsDependent,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Type")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderType.Kind != type.Kind)
                    {
                        tblSDKHeaderType.Kind = type.Kind;
                        resave = true;
                    }

                    if (tblSDKHeaderType.IsDependent != type.IsDependent)
                    {
                        tblSDKHeaderType.IsDependent = type.IsDependent;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderType.TypeId;
            }
        }

        public static Guid? Save(this VTableLayout vTableLayout, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(vTableLayout, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VTableLayout vTableLayout, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(vTableLayout, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VTableLayout vTableLayout, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (vTableLayout == null)
            {
                return null;
            }
            else if (checkInheritingSave && vTableLayout.GetType().InheritsFrom("VTableLayout"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(vTableLayout, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(vTableLayout, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = vTableLayout.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VTableLayout", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVTableLayout = Entities.SaveIfNotExists<tblSDKHeaderVTableLayout>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VTableLayout")));

                    return new tblSDKHeaderVTableLayout
                    {
                        VTableLayoutId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VTableLayout")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    vTableLayout.Components.SaveAll("VTableLayout.Components", tblSdkHeaderFile, tblSDKHeaderVTableLayout);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVTableLayout.VTableLayoutId;
            }
        }

        public static void SaveAll(this IEnumerable<VTableComponent> components, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVTableLayout tblSDKHeaderVTableLayout)
        {
            var x = 0;

            foreach (var component in components)
            {
                component.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderVTableLayout);
                x++;
            }
        }

        public static Guid? Save(this VTableComponent component, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVTableLayout tblSDKHeaderVTableLayout, bool noRecurse = false)
        {
            return DoSave(component, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderVTableLayout, noRecurse);
        }

        public static Guid? Save(this VTableComponent component, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVTableLayout tblSDKHeaderVTableLayout, bool noRecurse = false)
        {
            return DoSave(component, tblSdkHeaderFile, tblSDKHeaderVTableLayout, noRecurse);
        }
        public static Guid? Save(this BlockCommandCommentArgument blockCommandCommentArgument, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(blockCommandCommentArgument, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BlockCommandCommentArgument blockCommandCommentArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(blockCommandCommentArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BlockCommandCommentArgument blockCommandCommentArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (blockCommandCommentArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && blockCommandCommentArgument.GetType().InheritsFrom("BlockCommandCommentArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(blockCommandCommentArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(blockCommandCommentArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = blockCommandCommentArgument.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BlockCommandCommentArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBlockCommandCommentArgument = Entities.SaveIfNotExists<tblSDKHeaderBlockCommandCommentArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BlockCommandCommentArgument")));

                    return new tblSDKHeaderBlockCommandCommentArgument
                    {
                        BlockCommandCommentArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningBlockCommandCommentId = blockCommandCommentArgument.OwningBlockCommandComment.SaveOrGetId("BlockCommandCommentArgument.OwningBlockCommandCommentId", tblSdkHeaderFile, noRecurse, true),
                        Text = blockCommandCommentArgument.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BlockCommandCommentArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderBlockCommandCommentArgument.Text != blockCommandCommentArgument.Text)
                    {
                        tblSDKHeaderBlockCommandCommentArgument.Text = blockCommandCommentArgument.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBlockCommandCommentArgument.BlockCommandCommentArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this BlockCommandCommentArgument blockCommandCommentArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderBlockCommandComment tblOwningSDKHeaderBlockCommandComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (blockCommandCommentArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && blockCommandCommentArgument.GetType().InheritsFrom("BlockCommandCommentArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(blockCommandCommentArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(blockCommandCommentArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = blockCommandCommentArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderBlockCommandComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BlockCommandCommentArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBlockCommandCommentArgument = Entities.SaveIfNotExists<tblSDKHeaderBlockCommandCommentArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BlockCommandCommentArgument")));

                    return new tblSDKHeaderBlockCommandCommentArgument
                    {
                        BlockCommandCommentArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningBlockCommandCommentId = tblOwningSDKHeaderBlockCommandComment.BlockCommandCommentId,
                        Text = blockCommandCommentArgument.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BlockCommandCommentArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderBlockCommandCommentArgument.Text != blockCommandCommentArgument.Text)
                    {
                        tblSDKHeaderBlockCommandCommentArgument.Text = blockCommandCommentArgument.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBlockCommandCommentArgument.BlockCommandCommentArgumentId;
            }
        }

        public static Guid? Save(this BuiltinType builtinType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(builtinType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BuiltinType builtinType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(builtinType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BuiltinType builtinType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (builtinType == null)
            {
                return null;
            }
            else if (checkInheritingSave && builtinType.GetType().InheritsFrom("BuiltinType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(builtinType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(builtinType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = builtinType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BuiltinType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBuiltinType = Entities.SaveIfNotExists<tblSDKHeaderBuiltinType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BuiltinType")));

                    return new tblSDKHeaderBuiltinType
                    {
                        BuiltinTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Type = builtinType.Type,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BuiltinType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderBuiltinType.Type != builtinType.Type)
                    {
                        tblSDKHeaderBuiltinType.Type = builtinType.Type;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBuiltinType.BuiltinTypeId;
            }
        }

        public static Guid? Save(this BuiltinTypeExpression builtinTypeExpression, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(builtinTypeExpression, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BuiltinTypeExpression builtinTypeExpression, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(builtinTypeExpression, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BuiltinTypeExpression builtinTypeExpression, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (builtinTypeExpression == null)
            {
                return null;
            }
            else if (checkInheritingSave && builtinTypeExpression.GetType().InheritsFrom("BuiltinTypeExpression"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(builtinTypeExpression, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(builtinTypeExpression, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = builtinTypeExpression.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BuiltinTypeExpression", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBuiltinTypeExpression = Entities.SaveIfNotExists<tblSDKHeaderBuiltinTypeExpression>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BuiltinTypeExpression")));

                    return new tblSDKHeaderBuiltinTypeExpression
                    {
                        BuiltinTypeExpressionId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BuiltinTypeExpression")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBuiltinTypeExpression.BuiltinTypeExpressionId;
            }
        }

        public static Guid? Save(this CallExpr callExpr, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(callExpr, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this CallExpr callExpr, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(callExpr, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this CallExpr callExpr, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (callExpr == null)
            {
                return null;
            }
            else if (checkInheritingSave && callExpr.GetType().InheritsFrom("CallExpr"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(callExpr, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(callExpr, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = callExpr.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "CallExpr", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderCallExpr = Entities.SaveIfNotExists<tblSDKHeaderCallExpr>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "CallExpr")));

                    return new tblSDKHeaderCallExpr
                    {
                        CallExprId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "CallExpr")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    callExpr.Arguments.SaveAll("CallExpr.Arguments", tblSdkHeaderFile, tblSDKHeaderCallExpr);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderCallExpr.CallExprId;
            }
        }

        public static void SaveAll(this IEnumerable<Expression> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderCallExpr tblSDKHeaderCallExpr)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderCallExpr);
                x++;
            }
        }

        public static Guid? Save(this Expression argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderCallExpr tblSDKHeaderCallExpr, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderCallExpr, noRecurse);
        }

        public static Guid? Save(this Expression argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderCallExpr tblSDKHeaderCallExpr, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderCallExpr, noRecurse);
        }
        public static Guid? Save(this ClassLayout classLayout, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classLayout, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ClassLayout classLayout, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classLayout, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ClassLayout classLayout, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (classLayout == null)
            {
                return null;
            }
            else if (checkInheritingSave && classLayout.GetType().InheritsFrom("ClassLayout"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(classLayout, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(classLayout, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = classLayout.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ClassLayout", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderClassLayout = Entities.SaveIfNotExists<tblSDKHeaderClassLayout>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ClassLayout")));

                    return new tblSDKHeaderClassLayout
                    {
                        ClassLayoutId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LayoutVTableLayoutId = classLayout.Layout.SaveOrGetId("ClassLayout.LayoutVTableLayoutId", tblSdkHeaderFile, noRecurse, true),
                        ABI = classLayout.ABI,
                        Alignment = classLayout.Alignment,
                        DataSize = classLayout.DataSize,
                        HasOwnVFPtr = classLayout.HasOwnVFPtr,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ClassLayout")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderClassLayout.ABI != classLayout.ABI)
                    {
                        tblSDKHeaderClassLayout.ABI = classLayout.ABI;
                        resave = true;
                    }

                    if (tblSDKHeaderClassLayout.Alignment != classLayout.Alignment)
                    {
                        tblSDKHeaderClassLayout.Alignment = classLayout.Alignment;
                        resave = true;
                    }

                    if (tblSDKHeaderClassLayout.DataSize != classLayout.DataSize)
                    {
                        tblSDKHeaderClassLayout.DataSize = classLayout.DataSize;
                        resave = true;
                    }

                    if (tblSDKHeaderClassLayout.HasOwnVFPtr != classLayout.HasOwnVFPtr)
                    {
                        tblSDKHeaderClassLayout.HasOwnVFPtr = classLayout.HasOwnVFPtr;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderClassLayout.ClassLayoutId;
            }
        }

        public static Guid? Save(this ClassTemplate classTemplate, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classTemplate, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ClassTemplate classTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ClassTemplate classTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (classTemplate == null)
            {
                return null;
            }
            else if (checkInheritingSave && classTemplate.GetType().InheritsFrom("ClassTemplate"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(classTemplate, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(classTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = classTemplate.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ClassTemplate", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderClassTemplate = Entities.SaveIfNotExists<tblSDKHeaderClassTemplate>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ClassTemplate")));

                    return new tblSDKHeaderClassTemplate
                    {
                        ClassTemplateId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) classTemplate.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ClassTemplate")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderClassTemplate.LocationIdentifier != (long) classTemplate.LocationIdentifier)
                    {
                        tblSDKHeaderClassTemplate.LocationIdentifier = (long) classTemplate.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    classTemplate.Specializations.SaveAll("ClassTemplate.Specializations", tblSdkHeaderFile, tblSDKHeaderClassTemplate);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderClassTemplate.ClassTemplateId;
            }
        }

        public static void SaveAll(this IEnumerable<ClassTemplateSpecialization> specializations, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplate tblSDKHeaderClassTemplate)
        {
            var x = 0;

            foreach (var specialization in specializations)
            {
                specialization.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderClassTemplate);
                x++;
            }
        }

        public static Guid? Save(this ClassTemplateSpecialization specialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplate tblSDKHeaderClassTemplate, bool noRecurse = false)
        {
            return DoSave(specialization, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderClassTemplate, noRecurse);
        }

        public static Guid? Save(this ClassTemplateSpecialization specialization, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplate tblSDKHeaderClassTemplate, bool noRecurse = false)
        {
            return DoSave(specialization, tblSdkHeaderFile, tblSDKHeaderClassTemplate, noRecurse);
        }
        public static Guid? Save(this ClassTemplatePartialSpecialization classTemplatePartialSpecialization, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classTemplatePartialSpecialization, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ClassTemplatePartialSpecialization classTemplatePartialSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classTemplatePartialSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ClassTemplatePartialSpecialization classTemplatePartialSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (classTemplatePartialSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && classTemplatePartialSpecialization.GetType().InheritsFrom("ClassTemplatePartialSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(classTemplatePartialSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(classTemplatePartialSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = classTemplatePartialSpecialization.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ClassTemplatePartialSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderClassTemplatePartialSpecialization = Entities.SaveIfNotExists<tblSDKHeaderClassTemplatePartialSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ClassTemplatePartialSpecialization")));

                    return new tblSDKHeaderClassTemplatePartialSpecialization
                    {
                        ClassTemplatePartialSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) classTemplatePartialSpecialization.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ClassTemplatePartialSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderClassTemplatePartialSpecialization.LocationIdentifier != (long) classTemplatePartialSpecialization.LocationIdentifier)
                    {
                        tblSDKHeaderClassTemplatePartialSpecialization.LocationIdentifier = (long) classTemplatePartialSpecialization.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderClassTemplatePartialSpecialization.ClassTemplatePartialSpecializationId;
            }
        }

        public static Guid? Save(this Comment comment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(comment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Comment comment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(comment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Comment comment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (comment == null)
            {
                return null;
            }
            else if (checkInheritingSave && comment.GetType().InheritsFrom("Comment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(comment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(comment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = comment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Comment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderComment = Entities.SaveIfNotExists<tblSDKHeaderComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Comment")));

                    return new tblSDKHeaderComment
                    {
                        CommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Kind = comment.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Comment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderComment.Kind != comment.Kind)
                    {
                        tblSDKHeaderComment.Kind = comment.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderComment.CommentId;
            }
        }

        public static Guid? SaveOrGetId(this Comment comment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblOwningSDKHeaderDeclaration, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (comment == null)
            {
                return null;
            }
            else if (checkInheritingSave && comment.GetType().InheritsFrom("Comment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(comment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(comment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = comment.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDeclaration);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Comment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderComment = Entities.SaveIfNotExists<tblSDKHeaderComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Comment")));

                    return new tblSDKHeaderComment
                    {
                        CommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Kind = comment.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Comment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderComment.Kind != comment.Kind)
                    {
                        tblSDKHeaderComment.Kind = comment.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderComment.CommentId;
            }
        }

        public static Guid? Save(this CXXConstructExpr cXXConstructExpr, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(cXXConstructExpr, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this CXXConstructExpr cXXConstructExpr, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(cXXConstructExpr, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this CXXConstructExpr cXXConstructExpr, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (cXXConstructExpr == null)
            {
                return null;
            }
            else if (checkInheritingSave && cXXConstructExpr.GetType().InheritsFrom("CXXConstructExpr"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(cXXConstructExpr, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(cXXConstructExpr, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = cXXConstructExpr.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "CXXConstructExpr", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderCXXConstructExpr = Entities.SaveIfNotExists<tblSDKHeaderCXXConstructExpr>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "CXXConstructExpr")));

                    return new tblSDKHeaderCXXConstructExpr
                    {
                        CXXConstructExprId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "CXXConstructExpr")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    cXXConstructExpr.Arguments.SaveAll("CXXConstructExpr.Arguments", tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderCXXConstructExpr.CXXConstructExprId;
            }
        }

        public static void SaveAll(this IEnumerable<Expression> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr tblSDKHeaderCXXConstructExpr)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr);
                x++;
            }
        }

        public static Guid? Save(this Expression argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr tblSDKHeaderCXXConstructExpr, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr, noRecurse);
        }

        public static Guid? Save(this Expression argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr tblSDKHeaderCXXConstructExpr, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderCXXConstructExpr, noRecurse);
        }
        public static Guid? Save(this Expression expression, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(expression, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Expression expression, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(expression, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Expression expression, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (expression == null)
            {
                return null;
            }
            else if (checkInheritingSave && expression.GetType().InheritsFrom("Expression"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(expression, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(expression, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = expression.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Expression", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderExpression = Entities.SaveIfNotExists<tblSDKHeaderExpression>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Expression")));

                    return new tblSDKHeaderExpression
                    {
                        ExpressionId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Expression")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderExpression.ExpressionId;
            }
        }

        public static Guid? Save(this FunctionTemplate functionTemplate, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(functionTemplate, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this FunctionTemplate functionTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(functionTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this FunctionTemplate functionTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (functionTemplate == null)
            {
                return null;
            }
            else if (checkInheritingSave && functionTemplate.GetType().InheritsFrom("FunctionTemplate"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(functionTemplate, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(functionTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = functionTemplate.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FunctionTemplate", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFunctionTemplate = Entities.SaveIfNotExists<tblSDKHeaderFunctionTemplate>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FunctionTemplate")));

                    return new tblSDKHeaderFunctionTemplate
                    {
                        FunctionTemplateId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) functionTemplate.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FunctionTemplate")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderFunctionTemplate.LocationIdentifier != (long) functionTemplate.LocationIdentifier)
                    {
                        tblSDKHeaderFunctionTemplate.LocationIdentifier = (long) functionTemplate.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    functionTemplate.Specializations.SaveAll("FunctionTemplate.Specializations", tblSdkHeaderFile, tblSDKHeaderFunctionTemplate);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFunctionTemplate.FunctionTemplateId;
            }
        }

        public static void SaveAll(this IEnumerable<FunctionTemplateSpecialization> specializations, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplate tblSDKHeaderFunctionTemplate)
        {
            var x = 0;

            foreach (var specialization in specializations)
            {
                specialization.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderFunctionTemplate);
                x++;
            }
        }

        public static Guid? Save(this FunctionTemplateSpecialization specialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplate tblSDKHeaderFunctionTemplate, bool noRecurse = false)
        {
            return DoSave(specialization, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderFunctionTemplate, noRecurse);
        }

        public static Guid? Save(this FunctionTemplateSpecialization specialization, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplate tblSDKHeaderFunctionTemplate, bool noRecurse = false)
        {
            return DoSave(specialization, tblSdkHeaderFile, tblSDKHeaderFunctionTemplate, noRecurse);
        }
        public static Guid? Save(this Header header, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(header, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Header header, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(header, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Header header, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (header == null)
            {
                return null;
            }
            else if (checkInheritingSave && header.GetType().InheritsFrom("Header"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(header, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(header, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = header.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Header", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderHeader = Entities.SaveIfNotExists<tblSDKHeaderHeader>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Header")));

                    return new tblSDKHeaderHeader
                    {
                        HeaderId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) header.LocationIdentifier,
                        Name = header.Name,
                        FileName = header.FileName,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Header")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderHeader.LocationIdentifier != (long) header.LocationIdentifier)
                    {
                        tblSDKHeaderHeader.LocationIdentifier = (long) header.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderHeader.Name != header.Name)
                    {
                        tblSDKHeaderHeader.Name = header.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderHeader.FileName != header.FileName)
                    {
                        tblSDKHeaderHeader.FileName = header.FileName;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    header.Macros.SaveAll("Header.Macros", tblSdkHeaderFile, tblSDKHeaderHeader);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderHeader.HeaderId;
            }
        }

        public static void SaveAll(this IEnumerable<MacroDefinition> macros, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHeader tblSDKHeaderHeader)
        {
            var x = 0;

            foreach (var macro in macros)
            {
                macro.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderHeader);
                x++;
            }
        }

        public static Guid? Save(this MacroDefinition macro, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHeader tblSDKHeaderHeader, bool noRecurse = false)
        {
            return DoSave(macro, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderHeader, noRecurse);
        }

        public static Guid? Save(this MacroDefinition macro, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHeader tblSDKHeaderHeader, bool noRecurse = false)
        {
            return DoSave(macro, tblSdkHeaderFile, tblSDKHeaderHeader, noRecurse);
        }
        public static Guid? Save(this HTMLEndTagComment hTMLEndTagComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLEndTagComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this HTMLEndTagComment hTMLEndTagComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLEndTagComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this HTMLEndTagComment hTMLEndTagComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (hTMLEndTagComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && hTMLEndTagComment.GetType().InheritsFrom("HTMLEndTagComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(hTMLEndTagComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(hTMLEndTagComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = hTMLEndTagComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "HTMLEndTagComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderHTMLEndTagComment = Entities.SaveIfNotExists<tblSDKHeaderHTMLEndTagComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "HTMLEndTagComment")));

                    return new tblSDKHeaderHTMLEndTagComment
                    {
                        HTMLEndTagCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        TagName = hTMLEndTagComment.TagName,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "HTMLEndTagComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderHTMLEndTagComment.TagName != hTMLEndTagComment.TagName)
                    {
                        tblSDKHeaderHTMLEndTagComment.TagName = hTMLEndTagComment.TagName;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderHTMLEndTagComment.HTMLEndTagCommentId;
            }
        }

        public static Guid? Save(this HTMLStartTagComment hTMLStartTagComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLStartTagComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this HTMLStartTagComment hTMLStartTagComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLStartTagComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this HTMLStartTagComment hTMLStartTagComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (hTMLStartTagComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && hTMLStartTagComment.GetType().InheritsFrom("HTMLStartTagComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(hTMLStartTagComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(hTMLStartTagComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = hTMLStartTagComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "HTMLStartTagComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderHTMLStartTagComment = Entities.SaveIfNotExists<tblSDKHeaderHTMLStartTagComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "HTMLStartTagComment")));

                    return new tblSDKHeaderHTMLStartTagComment
                    {
                        HTMLStartTagCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        TagName = hTMLStartTagComment.TagName,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "HTMLStartTagComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderHTMLStartTagComment.TagName != hTMLStartTagComment.TagName)
                    {
                        tblSDKHeaderHTMLStartTagComment.TagName = hTMLStartTagComment.TagName;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    hTMLStartTagComment.Attributes.SaveAll("HTMLStartTagComment.Attributes", tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderHTMLStartTagComment.HTMLStartTagCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<HTMLStartTagCommentAttribute> attributes, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment tblSDKHeaderHTMLStartTagComment)
        {
            var x = 0;

            foreach (var attribute in attributes)
            {
                attribute.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment);
                x++;
            }
        }

        public static Guid? Save(this HTMLStartTagCommentAttribute attribute, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment tblSDKHeaderHTMLStartTagComment, bool noRecurse = false)
        {
            return DoSave(attribute, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment, noRecurse);
        }

        public static Guid? Save(this HTMLStartTagCommentAttribute attribute, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment tblSDKHeaderHTMLStartTagComment, bool noRecurse = false)
        {
            return DoSave(attribute, tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment, noRecurse);
        }
        public static Guid? Save(this HTMLStartTagCommentAttribute hTMLStartTagCommentAttribute, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLStartTagCommentAttribute, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this HTMLStartTagCommentAttribute hTMLStartTagCommentAttribute, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLStartTagCommentAttribute, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this HTMLStartTagCommentAttribute hTMLStartTagCommentAttribute, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (hTMLStartTagCommentAttribute == null)
            {
                return null;
            }
            else if (checkInheritingSave && hTMLStartTagCommentAttribute.GetType().InheritsFrom("HTMLStartTagCommentAttribute"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(hTMLStartTagCommentAttribute, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(hTMLStartTagCommentAttribute, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = hTMLStartTagCommentAttribute.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "HTMLStartTagCommentAttribute", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderHTMLStartTagCommentAttribute = Entities.SaveIfNotExists<tblSDKHeaderHTMLStartTagCommentAttribute>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "HTMLStartTagCommentAttribute")));

                    return new tblSDKHeaderHTMLStartTagCommentAttribute
                    {
                        HTMLStartTagCommentAttributeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningHTMLStartTagCommentId = hTMLStartTagCommentAttribute.OwningHTMLStartTagComment.SaveOrGetId("HTMLStartTagCommentAttribute.OwningHTMLStartTagCommentId", tblSdkHeaderFile, noRecurse, true),
                        Name = hTMLStartTagCommentAttribute.Name,
                        Value = hTMLStartTagCommentAttribute.Value,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "HTMLStartTagCommentAttribute")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderHTMLStartTagCommentAttribute.Name != hTMLStartTagCommentAttribute.Name)
                    {
                        tblSDKHeaderHTMLStartTagCommentAttribute.Name = hTMLStartTagCommentAttribute.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderHTMLStartTagCommentAttribute.Value != hTMLStartTagCommentAttribute.Value)
                    {
                        tblSDKHeaderHTMLStartTagCommentAttribute.Value = hTMLStartTagCommentAttribute.Value;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderHTMLStartTagCommentAttribute.HTMLStartTagCommentAttributeId;
            }
        }

        public static Guid? SaveOrGetId(this HTMLStartTagCommentAttribute hTMLStartTagCommentAttribute, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment tblOwningSDKHeaderHTMLStartTagComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (hTMLStartTagCommentAttribute == null)
            {
                return null;
            }
            else if (checkInheritingSave && hTMLStartTagCommentAttribute.GetType().InheritsFrom("HTMLStartTagCommentAttribute"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(hTMLStartTagCommentAttribute, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(hTMLStartTagCommentAttribute, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = hTMLStartTagCommentAttribute.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderHTMLStartTagComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "HTMLStartTagCommentAttribute", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderHTMLStartTagCommentAttribute = Entities.SaveIfNotExists<tblSDKHeaderHTMLStartTagCommentAttribute>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "HTMLStartTagCommentAttribute")));

                    return new tblSDKHeaderHTMLStartTagCommentAttribute
                    {
                        HTMLStartTagCommentAttributeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningHTMLStartTagCommentId = tblOwningSDKHeaderHTMLStartTagComment.HTMLStartTagCommentId,
                        Name = hTMLStartTagCommentAttribute.Name,
                        Value = hTMLStartTagCommentAttribute.Value,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "HTMLStartTagCommentAttribute")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderHTMLStartTagCommentAttribute.Name != hTMLStartTagCommentAttribute.Name)
                    {
                        tblSDKHeaderHTMLStartTagCommentAttribute.Name = hTMLStartTagCommentAttribute.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderHTMLStartTagCommentAttribute.Value != hTMLStartTagCommentAttribute.Value)
                    {
                        tblSDKHeaderHTMLStartTagCommentAttribute.Value = hTMLStartTagCommentAttribute.Value;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderHTMLStartTagCommentAttribute.HTMLStartTagCommentAttributeId;
            }
        }

        public static Guid? Save(this HTMLTagComment hTMLTagComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLTagComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this HTMLTagComment hTMLTagComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(hTMLTagComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this HTMLTagComment hTMLTagComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (hTMLTagComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && hTMLTagComment.GetType().InheritsFrom("HTMLTagComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(hTMLTagComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(hTMLTagComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = hTMLTagComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "HTMLTagComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderHTMLTagComment = Entities.SaveIfNotExists<tblSDKHeaderHTMLTagComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "HTMLTagComment")));

                    return new tblSDKHeaderHTMLTagComment
                    {
                        HTMLTagCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "HTMLTagComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderHTMLTagComment.HTMLTagCommentId;
            }
        }

        public static Guid? Save(this InlineCommandCommentArgument inlineCommandCommentArgument, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(inlineCommandCommentArgument, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this InlineCommandCommentArgument inlineCommandCommentArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(inlineCommandCommentArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this InlineCommandCommentArgument inlineCommandCommentArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (inlineCommandCommentArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && inlineCommandCommentArgument.GetType().InheritsFrom("InlineCommandCommentArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(inlineCommandCommentArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(inlineCommandCommentArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = inlineCommandCommentArgument.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "InlineCommandCommentArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderInlineCommandCommentArgument = Entities.SaveIfNotExists<tblSDKHeaderInlineCommandCommentArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "InlineCommandCommentArgument")));

                    return new tblSDKHeaderInlineCommandCommentArgument
                    {
                        InlineCommandCommentArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningInlineCommandCommentId = inlineCommandCommentArgument.OwningInlineCommandComment.SaveOrGetId("InlineCommandCommentArgument.OwningInlineCommandCommentId", tblSdkHeaderFile, noRecurse, true),
                        Text = inlineCommandCommentArgument.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "InlineCommandCommentArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderInlineCommandCommentArgument.Text != inlineCommandCommentArgument.Text)
                    {
                        tblSDKHeaderInlineCommandCommentArgument.Text = inlineCommandCommentArgument.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderInlineCommandCommentArgument.InlineCommandCommentArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this InlineCommandCommentArgument inlineCommandCommentArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderInlineCommandComment tblOwningSDKHeaderInlineCommandComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (inlineCommandCommentArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && inlineCommandCommentArgument.GetType().InheritsFrom("InlineCommandCommentArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(inlineCommandCommentArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(inlineCommandCommentArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = inlineCommandCommentArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderInlineCommandComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "InlineCommandCommentArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderInlineCommandCommentArgument = Entities.SaveIfNotExists<tblSDKHeaderInlineCommandCommentArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "InlineCommandCommentArgument")));

                    return new tblSDKHeaderInlineCommandCommentArgument
                    {
                        InlineCommandCommentArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningInlineCommandCommentId = tblOwningSDKHeaderInlineCommandComment.InlineCommandCommentId,
                        Text = inlineCommandCommentArgument.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "InlineCommandCommentArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderInlineCommandCommentArgument.Text != inlineCommandCommentArgument.Text)
                    {
                        tblSDKHeaderInlineCommandCommentArgument.Text = inlineCommandCommentArgument.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderInlineCommandCommentArgument.InlineCommandCommentArgumentId;
            }
        }

        public static Guid? Save(this MacroDefinition macroDefinition, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(macroDefinition, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this MacroDefinition macroDefinition, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(macroDefinition, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this MacroDefinition macroDefinition, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (macroDefinition == null)
            {
                return null;
            }
            else if (checkInheritingSave && macroDefinition.GetType().InheritsFrom("MacroDefinition"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(macroDefinition, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(macroDefinition, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = macroDefinition.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "MacroDefinition", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderMacroDefinition = Entities.SaveIfNotExists<tblSDKHeaderMacroDefinition>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "MacroDefinition")));

                    return new tblSDKHeaderMacroDefinition
                    {
                        MacroDefinitionId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Expression = macroDefinition.Expression,
                        Name = macroDefinition.Name,
                        LineNumberStart = macroDefinition.LineNumberStart,
                        LineNumberEnd = macroDefinition.LineNumberEnd,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "MacroDefinition")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderMacroDefinition.Expression != macroDefinition.Expression)
                    {
                        tblSDKHeaderMacroDefinition.Expression = macroDefinition.Expression;
                        resave = true;
                    }

                    if (tblSDKHeaderMacroDefinition.Name != macroDefinition.Name)
                    {
                        tblSDKHeaderMacroDefinition.Name = macroDefinition.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderMacroDefinition.LineNumberStart != macroDefinition.LineNumberStart)
                    {
                        tblSDKHeaderMacroDefinition.LineNumberStart = macroDefinition.LineNumberStart;
                        resave = true;
                    }

                    if (tblSDKHeaderMacroDefinition.LineNumberEnd != macroDefinition.LineNumberEnd)
                    {
                        tblSDKHeaderMacroDefinition.LineNumberEnd = macroDefinition.LineNumberEnd;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderMacroDefinition.MacroDefinitionId;
            }
        }

        public static Guid? Save(this Namespace _namespace, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(_namespace, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Namespace _namespace, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(_namespace, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Namespace _namespace, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (_namespace == null)
            {
                return null;
            }
            else if (checkInheritingSave && _namespace.GetType().InheritsFrom("Namespace"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(_namespace, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(_namespace, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = _namespace.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Namespace", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderNamespace = Entities.SaveIfNotExists<tblSDKHeaderNamespace>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Namespace")));

                    return new tblSDKHeaderNamespace
                    {
                        NamespaceId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) _namespace.LocationIdentifier,
                        Name = _namespace.Name,
                        IsInline = _namespace.IsInline,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Namespace")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderNamespace.LocationIdentifier != (long) _namespace.LocationIdentifier)
                    {
                        tblSDKHeaderNamespace.LocationIdentifier = (long) _namespace.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderNamespace.Name != _namespace.Name)
                    {
                        tblSDKHeaderNamespace.Name = _namespace.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderNamespace.IsInline != _namespace.IsInline)
                    {
                        tblSDKHeaderNamespace.IsInline = _namespace.IsInline;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderNamespace.NamespaceId;
            }
        }

        public static Guid? Save(this PackExpansionType packExpansionType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(packExpansionType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this PackExpansionType packExpansionType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(packExpansionType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this PackExpansionType packExpansionType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (packExpansionType == null)
            {
                return null;
            }
            else if (checkInheritingSave && packExpansionType.GetType().InheritsFrom("PackExpansionType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(packExpansionType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(packExpansionType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = packExpansionType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "PackExpansionType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderPackExpansionType = Entities.SaveIfNotExists<tblSDKHeaderPackExpansionType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "PackExpansionType")));

                    return new tblSDKHeaderPackExpansionType
                    {
                        PackExpansionTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "PackExpansionType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderPackExpansionType.PackExpansionTypeId;
            }
        }

        public static Guid? Save(this ParagraphComment paragraphComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(paragraphComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ParagraphComment paragraphComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(paragraphComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ParagraphComment paragraphComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (paragraphComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && paragraphComment.GetType().InheritsFrom("ParagraphComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(paragraphComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(paragraphComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = paragraphComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ParagraphComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderParagraphComment = Entities.SaveIfNotExists<tblSDKHeaderParagraphComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ParagraphComment")));

                    return new tblSDKHeaderParagraphComment
                    {
                        ParagraphCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        IsWhitespace = paragraphComment.IsWhitespace,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ParagraphComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderParagraphComment.IsWhitespace != paragraphComment.IsWhitespace)
                    {
                        tblSDKHeaderParagraphComment.IsWhitespace = paragraphComment.IsWhitespace;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    paragraphComment.Content.SaveAll("ParagraphComment.Content", tblSdkHeaderFile, tblSDKHeaderParagraphComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderParagraphComment.ParagraphCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<InlineContentComment> content, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderParagraphComment tblSDKHeaderParagraphComment)
        {
            var x = 0;

            foreach (var _content in content)
            {
                _content.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderParagraphComment);
                x++;
            }
        }

        public static Guid? Save(this InlineContentComment _content, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderParagraphComment tblSDKHeaderParagraphComment, bool noRecurse = false)
        {
            return DoSave(_content, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderParagraphComment, noRecurse);
        }

        public static Guid? Save(this InlineContentComment _content, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderParagraphComment tblSDKHeaderParagraphComment, bool noRecurse = false)
        {
            return DoSave(_content, tblSdkHeaderFile, tblSDKHeaderParagraphComment, noRecurse);
        }
        public static Guid? Save(this PreprocessedEntity preprocessedEntity, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(preprocessedEntity, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this PreprocessedEntity preprocessedEntity, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(preprocessedEntity, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this PreprocessedEntity preprocessedEntity, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (preprocessedEntity == null)
            {
                return null;
            }
            else if (checkInheritingSave && preprocessedEntity.GetType().InheritsFrom("PreprocessedEntity"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(preprocessedEntity, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(preprocessedEntity, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = preprocessedEntity.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "PreprocessedEntity", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderPreprocessedEntity = Entities.SaveIfNotExists<tblSDKHeaderPreprocessedEntity>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "PreprocessedEntity")));

                    return new tblSDKHeaderPreprocessedEntity
                    {
                        PreprocessedEntityId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationId = preprocessedEntity.OwningDeclaration.SaveOrGetId("PreprocessedEntity.OwningDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        Kind = preprocessedEntity.Kind,
                        MacroLocation = preprocessedEntity.MacroLocation,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "PreprocessedEntity")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderPreprocessedEntity.Kind != preprocessedEntity.Kind)
                    {
                        tblSDKHeaderPreprocessedEntity.Kind = preprocessedEntity.Kind;
                        resave = true;
                    }

                    if (tblSDKHeaderPreprocessedEntity.MacroLocation != preprocessedEntity.MacroLocation)
                    {
                        tblSDKHeaderPreprocessedEntity.MacroLocation = preprocessedEntity.MacroLocation;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderPreprocessedEntity.PreprocessedEntityId;
            }
        }

        public static Guid? SaveOrGetId(this PreprocessedEntity preprocessedEntity, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblOwningSDKHeaderDeclaration, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (preprocessedEntity == null)
            {
                return null;
            }
            else if (checkInheritingSave && preprocessedEntity.GetType().InheritsFrom("PreprocessedEntity"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(preprocessedEntity, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(preprocessedEntity, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = preprocessedEntity.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDeclaration);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "PreprocessedEntity", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderPreprocessedEntity = Entities.SaveIfNotExists<tblSDKHeaderPreprocessedEntity>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "PreprocessedEntity")));

                    return new tblSDKHeaderPreprocessedEntity
                    {
                        PreprocessedEntityId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationId = tblOwningSDKHeaderDeclaration.DeclarationId,
                        Kind = preprocessedEntity.Kind,
                        MacroLocation = preprocessedEntity.MacroLocation,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "PreprocessedEntity")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderPreprocessedEntity.Kind != preprocessedEntity.Kind)
                    {
                        tblSDKHeaderPreprocessedEntity.Kind = preprocessedEntity.Kind;
                        resave = true;
                    }

                    if (tblSDKHeaderPreprocessedEntity.MacroLocation != preprocessedEntity.MacroLocation)
                    {
                        tblSDKHeaderPreprocessedEntity.MacroLocation = preprocessedEntity.MacroLocation;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderPreprocessedEntity.PreprocessedEntityId;
            }
        }

        public static Guid? Save(this QualifiedType qualifiedType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(qualifiedType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this QualifiedType qualifiedType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(qualifiedType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this QualifiedType qualifiedType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (qualifiedType == null)
            {
                return null;
            }
            else if (checkInheritingSave && qualifiedType.GetType().InheritsFrom("QualifiedType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(qualifiedType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(qualifiedType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = qualifiedType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "QualifiedType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderQualifiedType = Entities.SaveIfNotExists<tblSDKHeaderQualifiedType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "QualifiedType")));

                    return new tblSDKHeaderQualifiedType
                    {
                        QualifiedTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        TypeId = qualifiedType.Type.SaveOrGetId("QualifiedType.TypeId", tblSdkHeaderFile, noRecurse, true),
                        Qualifiers = qualifiedType.Qualifiers,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "QualifiedType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderQualifiedType.Qualifiers != qualifiedType.Qualifiers)
                    {
                        tblSDKHeaderQualifiedType.Qualifiers = qualifiedType.Qualifiers;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderQualifiedType.QualifiedTypeId;
            }
        }

        public static Guid? Save(this RawComment rawComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(rawComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this RawComment rawComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(rawComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this RawComment rawComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (rawComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && rawComment.GetType().InheritsFrom("RawComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(rawComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(rawComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = rawComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "RawComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderRawComment = Entities.SaveIfNotExists<tblSDKHeaderRawComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "RawComment")));

                    return new tblSDKHeaderRawComment
                    {
                        RawCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        CommentBlockFullCommentId = rawComment.CommentBlock.SaveOrGetId("RawComment.CommentBlockFullCommentId", tblSdkHeaderFile, noRecurse, true),
                        BriefText = rawComment.BriefText,
                        Text = rawComment.Text,
                        Kind = rawComment.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "RawComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderRawComment.BriefText != rawComment.BriefText)
                    {
                        tblSDKHeaderRawComment.BriefText = rawComment.BriefText;
                        resave = true;
                    }

                    if (tblSDKHeaderRawComment.Text != rawComment.Text)
                    {
                        tblSDKHeaderRawComment.Text = rawComment.Text;
                        resave = true;
                    }

                    if (tblSDKHeaderRawComment.Kind != rawComment.Kind)
                    {
                        tblSDKHeaderRawComment.Kind = rawComment.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderRawComment.RawCommentId;
            }
        }

        public static Guid? Save(this Statement statement, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(statement, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Statement statement, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(statement, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Statement statement, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (statement == null)
            {
                return null;
            }
            else if (checkInheritingSave && statement.GetType().InheritsFrom("Statement"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(statement, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(statement, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = statement.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Statement", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderStatement = Entities.SaveIfNotExists<tblSDKHeaderStatement>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Statement")));

                    return new tblSDKHeaderStatement
                    {
                        StatementId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DeclDeclarationId = statement.Decl.SaveOrGetId("Statement.DeclDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        Class = statement.Class,
                        String = statement.String,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Statement")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderStatement.Class != statement.Class)
                    {
                        tblSDKHeaderStatement.Class = statement.Class;
                        resave = true;
                    }

                    if (tblSDKHeaderStatement.String != statement.String)
                    {
                        tblSDKHeaderStatement.String = statement.String;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderStatement.StatementId;
            }
        }

        public static Guid? Save(this TemplateTemplateParameter templateTemplateParameter, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateTemplateParameter, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TemplateTemplateParameter templateTemplateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateTemplateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TemplateTemplateParameter templateTemplateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateTemplateParameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateTemplateParameter.GetType().InheritsFrom("TemplateTemplateParameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateTemplateParameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateTemplateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateTemplateParameter.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateTemplateParameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateTemplateParameter = Entities.SaveIfNotExists<tblSDKHeaderTemplateTemplateParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateTemplateParameter")));

                    return new tblSDKHeaderTemplateTemplateParameter
                    {
                        TemplateTemplateParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) templateTemplateParameter.LocationIdentifier,
                        IsExpandedParameterPack = templateTemplateParameter.IsExpandedParameterPack,
                        IsPackExpansion = templateTemplateParameter.IsPackExpansion,
                        IsParameterPack = templateTemplateParameter.IsParameterPack,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateTemplateParameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateTemplateParameter.LocationIdentifier != (long) templateTemplateParameter.LocationIdentifier)
                    {
                        tblSDKHeaderTemplateTemplateParameter.LocationIdentifier = (long) templateTemplateParameter.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateTemplateParameter.IsExpandedParameterPack != templateTemplateParameter.IsExpandedParameterPack)
                    {
                        tblSDKHeaderTemplateTemplateParameter.IsExpandedParameterPack = templateTemplateParameter.IsExpandedParameterPack;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateTemplateParameter.IsPackExpansion != templateTemplateParameter.IsPackExpansion)
                    {
                        tblSDKHeaderTemplateTemplateParameter.IsPackExpansion = templateTemplateParameter.IsPackExpansion;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateTemplateParameter.IsParameterPack != templateTemplateParameter.IsParameterPack)
                    {
                        tblSDKHeaderTemplateTemplateParameter.IsParameterPack = templateTemplateParameter.IsParameterPack;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateTemplateParameter.TemplateTemplateParameterId;
            }
        }

        public static Guid? Save(this TextComment textComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(textComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TextComment textComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(textComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TextComment textComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (textComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && textComment.GetType().InheritsFrom("TextComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(textComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(textComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = textComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TextComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTextComment = Entities.SaveIfNotExists<tblSDKHeaderTextComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TextComment")));

                    return new tblSDKHeaderTextComment
                    {
                        TextCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Text = textComment.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TextComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTextComment.Text != textComment.Text)
                    {
                        tblSDKHeaderTextComment.Text = textComment.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTextComment.TextCommentId;
            }
        }

        public static Guid? Save(this TParamCommandComment tParamCommandComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(tParamCommandComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TParamCommandComment tParamCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(tParamCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TParamCommandComment tParamCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (tParamCommandComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && tParamCommandComment.GetType().InheritsFrom("TParamCommandComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(tParamCommandComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(tParamCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = tParamCommandComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TParamCommandComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTParamCommandComment = Entities.SaveIfNotExists<tblSDKHeaderTParamCommandComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TParamCommandComment")));

                    return new tblSDKHeaderTParamCommandComment
                    {
                        TParamCommandCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TParamCommandComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    tParamCommandComment.Positions.SaveAll("TParamCommandComment.Positions", tblSdkHeaderFile, tblSDKHeaderTParamCommandComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTParamCommandComment.TParamCommandCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<IntegerValue> positions, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTParamCommandComment tblSDKHeaderTParamCommandComment)
        {
            var x = 0;

            foreach (var position in positions)
            {
                position.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderTParamCommandComment);
                x++;
            }
        }

        public static Guid? Save(this IntegerValue position, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTParamCommandComment tblSDKHeaderTParamCommandComment, bool noRecurse = false)
        {
            return DoSave(position, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderTParamCommandComment, noRecurse);
        }

        public static Guid? Save(this IntegerValue position, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTParamCommandComment tblSDKHeaderTParamCommandComment, bool noRecurse = false)
        {
            return DoSave(position, tblSdkHeaderFile, tblSDKHeaderTParamCommandComment, noRecurse);
        }
        public static Guid? Save(this TranslationUnit translationUnit, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(translationUnit, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TranslationUnit translationUnit, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(translationUnit, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TranslationUnit translationUnit, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (translationUnit == null)
            {
                return null;
            }
            else if (checkInheritingSave && translationUnit.GetType().InheritsFrom("TranslationUnit"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(translationUnit, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(translationUnit, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = translationUnit.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TranslationUnit", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTranslationUnit = Entities.SaveIfNotExists<tblSDKHeaderTranslationUnit>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TranslationUnit")));

                    return new tblSDKHeaderTranslationUnit
                    {
                        TranslationUnitId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) translationUnit.LocationIdentifier,
                        FileName = translationUnit.FileName,
                        IsSystemHeader = translationUnit.IsSystemHeader,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TranslationUnit")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTranslationUnit.LocationIdentifier != (long) translationUnit.LocationIdentifier)
                    {
                        tblSDKHeaderTranslationUnit.LocationIdentifier = (long) translationUnit.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderTranslationUnit.FileName != translationUnit.FileName)
                    {
                        tblSDKHeaderTranslationUnit.FileName = translationUnit.FileName;
                        resave = true;
                    }

                    if (tblSDKHeaderTranslationUnit.IsSystemHeader != translationUnit.IsSystemHeader)
                    {
                        tblSDKHeaderTranslationUnit.IsSystemHeader = translationUnit.IsSystemHeader;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    translationUnit.Macros.SaveAll("TranslationUnit.Macros", tblSdkHeaderFile, tblSDKHeaderTranslationUnit);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTranslationUnit.TranslationUnitId;
            }
        }

        public static void SaveAll(this IEnumerable<MacroDefinition> macros, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTranslationUnit tblSDKHeaderTranslationUnit)
        {
            var x = 0;

            foreach (var macro in macros)
            {
                macro.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderTranslationUnit);
                x++;
            }
        }

        public static Guid? Save(this MacroDefinition macro, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTranslationUnit tblSDKHeaderTranslationUnit, bool noRecurse = false)
        {
            return DoSave(macro, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderTranslationUnit, noRecurse);
        }

        public static Guid? Save(this MacroDefinition macro, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTranslationUnit tblSDKHeaderTranslationUnit, bool noRecurse = false)
        {
            return DoSave(macro, tblSdkHeaderFile, tblSDKHeaderTranslationUnit, noRecurse);
        }
        public static Guid? Save(this TypeAliasTemplate typeAliasTemplate, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeAliasTemplate, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypeAliasTemplate typeAliasTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeAliasTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypeAliasTemplate typeAliasTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeAliasTemplate == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeAliasTemplate.GetType().InheritsFrom("TypeAliasTemplate"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeAliasTemplate, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeAliasTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeAliasTemplate.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeAliasTemplate", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeAliasTemplate = Entities.SaveIfNotExists<tblSDKHeaderTypeAliasTemplate>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeAliasTemplate")));

                    return new tblSDKHeaderTypeAliasTemplate
                    {
                        TypeAliasTemplateId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) typeAliasTemplate.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeAliasTemplate")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeAliasTemplate.LocationIdentifier != (long) typeAliasTemplate.LocationIdentifier)
                    {
                        tblSDKHeaderTypeAliasTemplate.LocationIdentifier = (long) typeAliasTemplate.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeAliasTemplate.TypeAliasTemplateId;
            }
        }

        public static Guid? Save(this TypeDefDecl typeDefDecl, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeDefDecl, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypeDefDecl typeDefDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeDefDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypeDefDecl typeDefDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeDefDecl == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeDefDecl.GetType().InheritsFrom("TypeDefDecl"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeDefDecl, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeDefDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeDefDecl.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeDefDecl", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeDefDecl = Entities.SaveIfNotExists<tblSDKHeaderTypeDefDecl>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeDefDecl")));

                    return new tblSDKHeaderTypeDefDecl
                    {
                        TypeDefDeclId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) typeDefDecl.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeDefDecl")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeDefDecl.LocationIdentifier != (long) typeDefDecl.LocationIdentifier)
                    {
                        tblSDKHeaderTypeDefDecl.LocationIdentifier = (long) typeDefDecl.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeDefDecl.TypeDefDeclId;
            }
        }

        public static Guid? Save(this VarTemplate varTemplate, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(varTemplate, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VarTemplate varTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(varTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VarTemplate varTemplate, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (varTemplate == null)
            {
                return null;
            }
            else if (checkInheritingSave && varTemplate.GetType().InheritsFrom("VarTemplate"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(varTemplate, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(varTemplate, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = varTemplate.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VarTemplate", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVarTemplate = Entities.SaveIfNotExists<tblSDKHeaderVarTemplate>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VarTemplate")));

                    return new tblSDKHeaderVarTemplate
                    {
                        VarTemplateId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LocationIdentifier = (long) varTemplate.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VarTemplate")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVarTemplate.LocationIdentifier != (long) varTemplate.LocationIdentifier)
                    {
                        tblSDKHeaderVarTemplate.LocationIdentifier = (long) varTemplate.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    varTemplate.Specializations.SaveAll("VarTemplate.Specializations", tblSdkHeaderFile, tblSDKHeaderVarTemplate);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVarTemplate.VarTemplateId;
            }
        }

        public static void SaveAll(this IEnumerable<VarTemplateSpecialization> specializations, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplate tblSDKHeaderVarTemplate)
        {
            var x = 0;

            foreach (var specialization in specializations)
            {
                specialization.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderVarTemplate);
                x++;
            }
        }

        public static Guid? Save(this VarTemplateSpecialization specialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplate tblSDKHeaderVarTemplate, bool noRecurse = false)
        {
            return DoSave(specialization, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderVarTemplate, noRecurse);
        }

        public static Guid? Save(this VarTemplateSpecialization specialization, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplate tblSDKHeaderVarTemplate, bool noRecurse = false)
        {
            return DoSave(specialization, tblSdkHeaderFile, tblSDKHeaderVarTemplate, noRecurse);
        }
        public static Guid? Save(this VerbatimBlockComment verbatimBlockComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(verbatimBlockComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VerbatimBlockComment verbatimBlockComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(verbatimBlockComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VerbatimBlockComment verbatimBlockComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (verbatimBlockComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && verbatimBlockComment.GetType().InheritsFrom("VerbatimBlockComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(verbatimBlockComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(verbatimBlockComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = verbatimBlockComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VerbatimBlockComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVerbatimBlockComment = Entities.SaveIfNotExists<tblSDKHeaderVerbatimBlockComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VerbatimBlockComment")));

                    return new tblSDKHeaderVerbatimBlockComment
                    {
                        VerbatimBlockCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VerbatimBlockComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    verbatimBlockComment.Lines.SaveAll("VerbatimBlockComment.Lines", tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVerbatimBlockComment.VerbatimBlockCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<VerbatimBlockLineComment> lines, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment tblSDKHeaderVerbatimBlockComment)
        {
            var x = 0;

            foreach (var line in lines)
            {
                line.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment);
                x++;
            }
        }

        public static Guid? Save(this VerbatimBlockLineComment line, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment tblSDKHeaderVerbatimBlockComment, bool noRecurse = false)
        {
            return DoSave(line, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment, noRecurse);
        }

        public static Guid? Save(this VerbatimBlockLineComment line, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment tblSDKHeaderVerbatimBlockComment, bool noRecurse = false)
        {
            return DoSave(line, tblSdkHeaderFile, tblSDKHeaderVerbatimBlockComment, noRecurse);
        }
        public static Guid? Save(this VerbatimLineComment verbatimLineComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(verbatimLineComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VerbatimLineComment verbatimLineComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(verbatimLineComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VerbatimLineComment verbatimLineComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (verbatimLineComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && verbatimLineComment.GetType().InheritsFrom("VerbatimLineComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(verbatimLineComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(verbatimLineComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = verbatimLineComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VerbatimLineComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVerbatimLineComment = Entities.SaveIfNotExists<tblSDKHeaderVerbatimLineComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VerbatimLineComment")));

                    return new tblSDKHeaderVerbatimLineComment
                    {
                        VerbatimLineCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        Text = verbatimLineComment.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VerbatimLineComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVerbatimLineComment.Text != verbatimLineComment.Text)
                    {
                        tblSDKHeaderVerbatimLineComment.Text = verbatimLineComment.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVerbatimLineComment.VerbatimLineCommentId;
            }
        }

        public static Guid? Save(this AccessSpecifierDecl accessSpecifierDecl, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(accessSpecifierDecl, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this AccessSpecifierDecl accessSpecifierDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(accessSpecifierDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this AccessSpecifierDecl accessSpecifierDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (accessSpecifierDecl == null)
            {
                return null;
            }
            else if (checkInheritingSave && accessSpecifierDecl.GetType().InheritsFrom("AccessSpecifierDecl"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(accessSpecifierDecl, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(accessSpecifierDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = accessSpecifierDecl.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "AccessSpecifierDecl", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderAccessSpecifierDecl = Entities.SaveIfNotExists<tblSDKHeaderAccessSpecifierDecl>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "AccessSpecifierDecl")));

                    return new tblSDKHeaderAccessSpecifierDecl
                    {
                        AccessSpecifierDeclId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = accessSpecifierDecl.OwningClass.SaveOrGetId("AccessSpecifierDecl.OwningClassId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) accessSpecifierDecl.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "AccessSpecifierDecl")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderAccessSpecifierDecl.LocationIdentifier != (long) accessSpecifierDecl.LocationIdentifier)
                    {
                        tblSDKHeaderAccessSpecifierDecl.LocationIdentifier = (long) accessSpecifierDecl.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderAccessSpecifierDecl.AccessSpecifierDeclId;
            }
        }

        public static Guid? SaveOrGetId(this AccessSpecifierDecl accessSpecifierDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (accessSpecifierDecl == null)
            {
                return null;
            }
            else if (checkInheritingSave && accessSpecifierDecl.GetType().InheritsFrom("AccessSpecifierDecl"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(accessSpecifierDecl, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(accessSpecifierDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = accessSpecifierDecl.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderClass);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "AccessSpecifierDecl", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderAccessSpecifierDecl = Entities.SaveIfNotExists<tblSDKHeaderAccessSpecifierDecl>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "AccessSpecifierDecl")));

                    return new tblSDKHeaderAccessSpecifierDecl
                    {
                        AccessSpecifierDeclId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = tblOwningSDKHeaderClass.ClassId,
                        LocationIdentifier = (long) accessSpecifierDecl.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "AccessSpecifierDecl")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderAccessSpecifierDecl.LocationIdentifier != (long) accessSpecifierDecl.LocationIdentifier)
                    {
                        tblSDKHeaderAccessSpecifierDecl.LocationIdentifier = (long) accessSpecifierDecl.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderAccessSpecifierDecl.AccessSpecifierDeclId;
            }
        }

        public static Guid? Save(this ArrayType arrayType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(arrayType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ArrayType arrayType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(arrayType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ArrayType arrayType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (arrayType == null)
            {
                return null;
            }
            else if (checkInheritingSave && arrayType.GetType().InheritsFrom("ArrayType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(arrayType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(arrayType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = arrayType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ArrayType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderArrayType = Entities.SaveIfNotExists<tblSDKHeaderArrayType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ArrayType")));

                    return new tblSDKHeaderArrayType
                    {
                        ArrayTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        QualifiedTypeId = arrayType.QualifiedType.SaveOrGetId("ArrayType.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        ElementSize = arrayType.ElementSize,
                        Size = arrayType.Size,
                        SizeType = arrayType.SizeType,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ArrayType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderArrayType.ElementSize != arrayType.ElementSize)
                    {
                        tblSDKHeaderArrayType.ElementSize = arrayType.ElementSize;
                        resave = true;
                    }

                    if (tblSDKHeaderArrayType.Size != arrayType.Size)
                    {
                        tblSDKHeaderArrayType.Size = arrayType.Size;
                        resave = true;
                    }

                    if (tblSDKHeaderArrayType.SizeType != arrayType.SizeType)
                    {
                        tblSDKHeaderArrayType.SizeType = arrayType.SizeType;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderArrayType.ArrayTypeId;
            }
        }

        public static Guid? Save(this BaseClassSpecifier baseClassSpecifier, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(baseClassSpecifier, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BaseClassSpecifier baseClassSpecifier, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(baseClassSpecifier, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BaseClassSpecifier baseClassSpecifier, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (baseClassSpecifier == null)
            {
                return null;
            }
            else if (checkInheritingSave && baseClassSpecifier.GetType().InheritsFrom("BaseClassSpecifier"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(baseClassSpecifier, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(baseClassSpecifier, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = baseClassSpecifier.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BaseClassSpecifier", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBaseClassSpecifier = Entities.SaveIfNotExists<tblSDKHeaderBaseClassSpecifier>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BaseClassSpecifier")));

                    return new tblSDKHeaderBaseClassSpecifier
                    {
                        BaseClassSpecifierId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = baseClassSpecifier.OwningClass.SaveOrGetId("BaseClassSpecifier.OwningClassId", tblSdkHeaderFile, noRecurse, true),
                        TypeId = baseClassSpecifier.Type.SaveOrGetId("BaseClassSpecifier.TypeId", tblSdkHeaderFile, noRecurse, true),
                        Access = baseClassSpecifier.Access,
                        IsVirtual = baseClassSpecifier.IsVirtual,
                        Offset = baseClassSpecifier.Offset,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BaseClassSpecifier")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderBaseClassSpecifier.Access != baseClassSpecifier.Access)
                    {
                        tblSDKHeaderBaseClassSpecifier.Access = baseClassSpecifier.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderBaseClassSpecifier.IsVirtual != baseClassSpecifier.IsVirtual)
                    {
                        tblSDKHeaderBaseClassSpecifier.IsVirtual = baseClassSpecifier.IsVirtual;
                        resave = true;
                    }

                    if (tblSDKHeaderBaseClassSpecifier.Offset != baseClassSpecifier.Offset)
                    {
                        tblSDKHeaderBaseClassSpecifier.Offset = baseClassSpecifier.Offset;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBaseClassSpecifier.BaseClassSpecifierId;
            }
        }

        public static Guid? SaveOrGetId(this BaseClassSpecifier baseClassSpecifier, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (baseClassSpecifier == null)
            {
                return null;
            }
            else if (checkInheritingSave && baseClassSpecifier.GetType().InheritsFrom("BaseClassSpecifier"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(baseClassSpecifier, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(baseClassSpecifier, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = baseClassSpecifier.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderClass);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BaseClassSpecifier", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBaseClassSpecifier = Entities.SaveIfNotExists<tblSDKHeaderBaseClassSpecifier>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BaseClassSpecifier")));

                    return new tblSDKHeaderBaseClassSpecifier
                    {
                        BaseClassSpecifierId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = tblOwningSDKHeaderClass.ClassId,
                        TypeId = baseClassSpecifier.Type.SaveOrGetId("BaseClassSpecifier.TypeId", tblSdkHeaderFile, noRecurse, true),
                        Access = baseClassSpecifier.Access,
                        IsVirtual = baseClassSpecifier.IsVirtual,
                        Offset = baseClassSpecifier.Offset,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BaseClassSpecifier")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderBaseClassSpecifier.Access != baseClassSpecifier.Access)
                    {
                        tblSDKHeaderBaseClassSpecifier.Access = baseClassSpecifier.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderBaseClassSpecifier.IsVirtual != baseClassSpecifier.IsVirtual)
                    {
                        tblSDKHeaderBaseClassSpecifier.IsVirtual = baseClassSpecifier.IsVirtual;
                        resave = true;
                    }

                    if (tblSDKHeaderBaseClassSpecifier.Offset != baseClassSpecifier.Offset)
                    {
                        tblSDKHeaderBaseClassSpecifier.Offset = baseClassSpecifier.Offset;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBaseClassSpecifier.BaseClassSpecifierId;
            }
        }

        public static Guid? Save(this BlockCommandComment blockCommandComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(blockCommandComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BlockCommandComment blockCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(blockCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BlockCommandComment blockCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (blockCommandComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && blockCommandComment.GetType().InheritsFrom("BlockCommandComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(blockCommandComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(blockCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = blockCommandComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BlockCommandComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBlockCommandComment = Entities.SaveIfNotExists<tblSDKHeaderBlockCommandComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BlockCommandComment")));

                    return new tblSDKHeaderBlockCommandComment
                    {
                        BlockCommandCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        ParagraphCommentId = blockCommandComment.ParagraphComment.SaveOrGetId("BlockCommandComment.ParagraphCommentId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BlockCommandComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    blockCommandComment.Arguments.SaveAll("BlockCommandComment.Arguments", tblSdkHeaderFile, tblSDKHeaderBlockCommandComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBlockCommandComment.BlockCommandCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<BlockCommandCommentArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderBlockCommandComment tblSDKHeaderBlockCommandComment)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderBlockCommandComment);
                x++;
            }
        }

        public static Guid? Save(this BlockCommandCommentArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderBlockCommandComment tblSDKHeaderBlockCommandComment, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderBlockCommandComment, noRecurse);
        }

        public static Guid? Save(this BlockCommandCommentArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderBlockCommandComment tblSDKHeaderBlockCommandComment, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderBlockCommandComment, noRecurse);
        }
        public static Guid? Save(this BlockContentComment blockContentComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(blockContentComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BlockContentComment blockContentComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(blockContentComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BlockContentComment blockContentComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (blockContentComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && blockContentComment.GetType().InheritsFrom("BlockContentComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(blockContentComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(blockContentComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = blockContentComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BlockContentComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBlockContentComment = Entities.SaveIfNotExists<tblSDKHeaderBlockContentComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BlockContentComment")));

                    return new tblSDKHeaderBlockContentComment
                    {
                        BlockContentCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = blockContentComment.OwningComment.SaveOrGetId("BlockContentComment.OwningCommentId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BlockContentComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBlockContentComment.BlockContentCommentId;
            }
        }

        public static Guid? SaveOrGetId(this BlockContentComment blockContentComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (blockContentComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && blockContentComment.GetType().InheritsFrom("BlockContentComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(blockContentComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(blockContentComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = blockContentComment.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BlockContentComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBlockContentComment = Entities.SaveIfNotExists<tblSDKHeaderBlockContentComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BlockContentComment")));

                    return new tblSDKHeaderBlockContentComment
                    {
                        BlockContentCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = tblOwningSDKHeaderComment.CommentId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BlockContentComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBlockContentComment.BlockContentCommentId;
            }
        }

        public static Guid? Save(this Class _class, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(_class, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Class _class, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(_class, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Class _class, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (_class == null)
            {
                return null;
            }
            else if (checkInheritingSave && _class.GetType().InheritsFrom("Class"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(_class, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(_class, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = _class.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Class", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderClass = Entities.SaveIfNotExists<tblSDKHeaderClass>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Class")));

                    return new tblSDKHeaderClass
                    {
                        ClassId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LayoutClassLayoutId = _class.Layout.SaveOrGetId("Class.LayoutClassLayoutId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) _class.LocationIdentifier,
                        ClassName = _class.ClassName,
                        HasNonTrivialCopyConstructor = _class.HasNonTrivialCopyConstructor,
                        HasNonTrivialDefaultConstructor = _class.HasNonTrivialDefaultConstructor,
                        HasNonTrivialDestructor = _class.HasNonTrivialDestructor,
                        IsAbstract = _class.IsAbstract,
                        IsDynamic = _class.IsDynamic,
                        IsExternCContext = _class.IsExternCContext,
                        IsPOD = _class.IsPOD,
                        IsPolymorphic = _class.IsPolymorphic,
                        IsUnion = _class.IsUnion,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Class")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderClass.LocationIdentifier != (long) _class.LocationIdentifier)
                    {
                        tblSDKHeaderClass.LocationIdentifier = (long) _class.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.ClassName != _class.ClassName)
                    {
                        tblSDKHeaderClass.ClassName = _class.ClassName;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.HasNonTrivialCopyConstructor != _class.HasNonTrivialCopyConstructor)
                    {
                        tblSDKHeaderClass.HasNonTrivialCopyConstructor = _class.HasNonTrivialCopyConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.HasNonTrivialDefaultConstructor != _class.HasNonTrivialDefaultConstructor)
                    {
                        tblSDKHeaderClass.HasNonTrivialDefaultConstructor = _class.HasNonTrivialDefaultConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.HasNonTrivialDestructor != _class.HasNonTrivialDestructor)
                    {
                        tblSDKHeaderClass.HasNonTrivialDestructor = _class.HasNonTrivialDestructor;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.IsAbstract != _class.IsAbstract)
                    {
                        tblSDKHeaderClass.IsAbstract = _class.IsAbstract;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.IsDynamic != _class.IsDynamic)
                    {
                        tblSDKHeaderClass.IsDynamic = _class.IsDynamic;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.IsExternCContext != _class.IsExternCContext)
                    {
                        tblSDKHeaderClass.IsExternCContext = _class.IsExternCContext;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.IsPOD != _class.IsPOD)
                    {
                        tblSDKHeaderClass.IsPOD = _class.IsPOD;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.IsPolymorphic != _class.IsPolymorphic)
                    {
                        tblSDKHeaderClass.IsPolymorphic = _class.IsPolymorphic;
                        resave = true;
                    }

                    if (tblSDKHeaderClass.IsUnion != _class.IsUnion)
                    {
                        tblSDKHeaderClass.IsUnion = _class.IsUnion;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    _class.BaseClassSpecifiers.SaveAll("Class.BaseClassSpecifiers", tblSdkHeaderFile, tblSDKHeaderClass);
                    _class.Fields.SaveAll("Class.Fields", tblSdkHeaderFile, tblSDKHeaderClass);
                    _class.Methods.SaveAll("Class.Methods", tblSdkHeaderFile, tblSDKHeaderClass);
                    _class.Specifiers.SaveAll("Class.Specifiers", tblSdkHeaderFile, tblSDKHeaderClass);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderClass.ClassId;
            }
        }

        public static void SaveAll(this IEnumerable<BaseClassSpecifier> baseClassSpecifiers, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass)
        {
            var x = 0;

            foreach (var baseClassSpecifier in baseClassSpecifiers)
            {
                baseClassSpecifier.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderClass);
                x++;
            }
        }

        public static Guid? Save(this BaseClassSpecifier baseClassSpecifier, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(baseClassSpecifier, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }

        public static Guid? Save(this BaseClassSpecifier baseClassSpecifier, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(baseClassSpecifier, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Field> fields, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass)
        {
            var x = 0;

            foreach (var field in fields)
            {
                field.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderClass);
                x++;
            }
        }

        public static Guid? Save(this Field field, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(field, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }

        public static Guid? Save(this Field field, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(field, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Method> methods, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass)
        {
            var x = 0;

            foreach (var method in methods)
            {
                method.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderClass);
                x++;
            }
        }

        public static Guid? Save(this Method method, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(method, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }

        public static Guid? Save(this Method method, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(method, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }
        public static void SaveAll(this IEnumerable<AccessSpecifierDecl> specifiers, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass)
        {
            var x = 0;

            foreach (var specifier in specifiers)
            {
                specifier.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderClass);
                x++;
            }
        }

        public static Guid? Save(this AccessSpecifierDecl specifier, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(specifier, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }

        public static Guid? Save(this AccessSpecifierDecl specifier, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblSDKHeaderClass, bool noRecurse = false)
        {
            return DoSave(specifier, tblSdkHeaderFile, tblSDKHeaderClass, noRecurse);
        }
        public static Guid? Save(this Declaration declaration, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(declaration, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Declaration declaration, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(declaration, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Declaration declaration, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (declaration == null)
            {
                return null;
            }
            else if (checkInheritingSave && declaration.GetType().InheritsFrom("Declaration"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(declaration, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(declaration, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = declaration.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Declaration", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderDeclaration = Entities.SaveIfNotExists<tblSDKHeaderDeclaration>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Declaration")));

                    return new tblSDKHeaderDeclaration
                    {
                        DeclarationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationId = declaration.OwningDeclaration.SaveOrGetId("Declaration.OwningDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        CompleteDeclarationId = declaration.CompleteDeclaration.SaveOrGetId("Declaration.CompleteDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) declaration.LocationIdentifier,
                        DefinitionOrder = declaration.DefinitionOrder,
                        LineNumberStart = declaration.LineNumberStart,
                        LineNumberEnd = declaration.LineNumberEnd,
                        IsDependent = declaration.IsDependent,
                        IsImplicit = declaration.IsImplicit,
                        IsIncomplete = declaration.IsIncomplete,
                        Kind = declaration.Kind,
                        USR = declaration.USR,
                        Name = declaration.Name,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Declaration")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderDeclaration.LocationIdentifier != (long) declaration.LocationIdentifier)
                    {
                        tblSDKHeaderDeclaration.LocationIdentifier = (long) declaration.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.DefinitionOrder != declaration.DefinitionOrder)
                    {
                        tblSDKHeaderDeclaration.DefinitionOrder = declaration.DefinitionOrder;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.LineNumberStart != declaration.LineNumberStart)
                    {
                        tblSDKHeaderDeclaration.LineNumberStart = declaration.LineNumberStart;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.LineNumberEnd != declaration.LineNumberEnd)
                    {
                        tblSDKHeaderDeclaration.LineNumberEnd = declaration.LineNumberEnd;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.IsDependent != declaration.IsDependent)
                    {
                        tblSDKHeaderDeclaration.IsDependent = declaration.IsDependent;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.IsImplicit != declaration.IsImplicit)
                    {
                        tblSDKHeaderDeclaration.IsImplicit = declaration.IsImplicit;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.IsIncomplete != declaration.IsIncomplete)
                    {
                        tblSDKHeaderDeclaration.IsIncomplete = declaration.IsIncomplete;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.Kind != declaration.Kind)
                    {
                        tblSDKHeaderDeclaration.Kind = declaration.Kind;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.USR != declaration.USR)
                    {
                        tblSDKHeaderDeclaration.USR = declaration.USR;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.Name != declaration.Name)
                    {
                        tblSDKHeaderDeclaration.Name = declaration.Name;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    declaration.PreprocessedEntities.SaveAll("Declaration.PreprocessedEntities", tblSdkHeaderFile, tblSDKHeaderDeclaration);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderDeclaration.DeclarationId;
            }
        }

        public static void SaveAll(this IEnumerable<PreprocessedEntity> preprocessedEntities, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblSDKHeaderDeclaration)
        {
            var x = 0;

            foreach (var preprocessedEntity in preprocessedEntities)
            {
                preprocessedEntity.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclaration);
                x++;
            }
        }

        public static Guid? Save(this PreprocessedEntity preprocessedEntity, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblSDKHeaderDeclaration, bool noRecurse = false)
        {
            return DoSave(preprocessedEntity, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclaration, noRecurse);
        }

        public static Guid? Save(this PreprocessedEntity preprocessedEntity, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblSDKHeaderDeclaration, bool noRecurse = false)
        {
            return DoSave(preprocessedEntity, tblSdkHeaderFile, tblSDKHeaderDeclaration, noRecurse);
        }
        public static Guid? SaveOrGetId(this Declaration declaration, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblOwningSDKHeaderDeclaration, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (declaration == null)
            {
                return null;
            }
            else if (checkInheritingSave && declaration.GetType().InheritsFrom("Declaration"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(declaration, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(declaration, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = declaration.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDeclaration);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Declaration", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderDeclaration = Entities.SaveIfNotExists<tblSDKHeaderDeclaration>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Declaration")));

                    return new tblSDKHeaderDeclaration
                    {
                        DeclarationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationId = tblOwningSDKHeaderDeclaration.DeclarationId,
                        CompleteDeclarationId = declaration.CompleteDeclaration.SaveOrGetId("Declaration.CompleteDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) declaration.LocationIdentifier,
                        DefinitionOrder = declaration.DefinitionOrder,
                        LineNumberStart = declaration.LineNumberStart,
                        LineNumberEnd = declaration.LineNumberEnd,
                        IsDependent = declaration.IsDependent,
                        IsImplicit = declaration.IsImplicit,
                        IsIncomplete = declaration.IsIncomplete,
                        Kind = declaration.Kind,
                        USR = declaration.USR,
                        Name = declaration.Name,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Declaration")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderDeclaration.LocationIdentifier != (long) declaration.LocationIdentifier)
                    {
                        tblSDKHeaderDeclaration.LocationIdentifier = (long) declaration.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.DefinitionOrder != declaration.DefinitionOrder)
                    {
                        tblSDKHeaderDeclaration.DefinitionOrder = declaration.DefinitionOrder;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.LineNumberStart != declaration.LineNumberStart)
                    {
                        tblSDKHeaderDeclaration.LineNumberStart = declaration.LineNumberStart;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.LineNumberEnd != declaration.LineNumberEnd)
                    {
                        tblSDKHeaderDeclaration.LineNumberEnd = declaration.LineNumberEnd;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.IsDependent != declaration.IsDependent)
                    {
                        tblSDKHeaderDeclaration.IsDependent = declaration.IsDependent;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.IsImplicit != declaration.IsImplicit)
                    {
                        tblSDKHeaderDeclaration.IsImplicit = declaration.IsImplicit;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.IsIncomplete != declaration.IsIncomplete)
                    {
                        tblSDKHeaderDeclaration.IsIncomplete = declaration.IsIncomplete;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.Kind != declaration.Kind)
                    {
                        tblSDKHeaderDeclaration.Kind = declaration.Kind;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.USR != declaration.USR)
                    {
                        tblSDKHeaderDeclaration.USR = declaration.USR;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclaration.Name != declaration.Name)
                    {
                        tblSDKHeaderDeclaration.Name = declaration.Name;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    declaration.PreprocessedEntities.SaveAll("Declaration.PreprocessedEntities", tblSdkHeaderFile, tblSDKHeaderDeclaration);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderDeclaration.DeclarationId;
            }
        }

        public static Guid? Save(this DeclarationContext declarationContext, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(declarationContext, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this DeclarationContext declarationContext, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(declarationContext, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this DeclarationContext declarationContext, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (declarationContext == null)
            {
                return null;
            }
            else if (checkInheritingSave && declarationContext.GetType().InheritsFrom("DeclarationContext"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(declarationContext, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(declarationContext, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = declarationContext.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "DeclarationContext", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderDeclarationContext = Entities.SaveIfNotExists<tblSDKHeaderDeclarationContext>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "DeclarationContext")));

                    return new tblSDKHeaderDeclarationContext
                    {
                        DeclarationContextId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        CommentId = declarationContext.Comment.SaveOrGetId("DeclarationContext.CommentId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) declarationContext.LocationIdentifier,
                        Name = declarationContext.Name,
                        IsAnonymous = declarationContext.IsAnonymous,
                        Access = declarationContext.Access,
                        DebugText = declarationContext.DebugText,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "DeclarationContext")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderDeclarationContext.LocationIdentifier != (long) declarationContext.LocationIdentifier)
                    {
                        tblSDKHeaderDeclarationContext.LocationIdentifier = (long) declarationContext.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclarationContext.Name != declarationContext.Name)
                    {
                        tblSDKHeaderDeclarationContext.Name = declarationContext.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclarationContext.IsAnonymous != declarationContext.IsAnonymous)
                    {
                        tblSDKHeaderDeclarationContext.IsAnonymous = declarationContext.IsAnonymous;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclarationContext.Access != declarationContext.Access)
                    {
                        tblSDKHeaderDeclarationContext.Access = declarationContext.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderDeclarationContext.DebugText != declarationContext.DebugText)
                    {
                        tblSDKHeaderDeclarationContext.DebugText = declarationContext.DebugText;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    declarationContext.Functions.SaveAll("DeclarationContext.Functions", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.Classes.SaveAll("DeclarationContext.Classes", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.Namespaces.SaveAll("DeclarationContext.Namespaces", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.Enums.SaveAll("DeclarationContext.Enums", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.Templates.SaveAll("DeclarationContext.Templates", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.TypeDefs.SaveAll("DeclarationContext.TypeDefs", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.TypeAliases.SaveAll("DeclarationContext.TypeAliases", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.Variables.SaveAll("DeclarationContext.Variables", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                    declarationContext.Friends.SaveAll("DeclarationContext.Friends", tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderDeclarationContext.DeclarationContextId;
            }
        }

        public static void SaveAll(this IEnumerable<Function> functions, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var function in functions)
            {
                function.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Function function, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(function, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Function function, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(function, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Class> classes, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var _class in classes)
            {
                _class.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Class _class, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(_class, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Class _class, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(_class, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Namespace> namespaces, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var _namespace in namespaces)
            {
                _namespace.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Namespace _namespace, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(_namespace, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Namespace _namespace, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(_namespace, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Enumeration> enums, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var _enum in enums)
            {
                _enum.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Enumeration _enum, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(_enum, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Enumeration _enum, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(_enum, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Template> templates, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var template in templates)
            {
                template.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Template template, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(template, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Template template, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(template, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<TypeDef> typeDefs, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var typeDef in typeDefs)
            {
                typeDef.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this TypeDef typeDef, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(typeDef, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this TypeDef typeDef, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(typeDef, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<TypeAlias> typeAliases, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var typeAlias in typeAliases)
            {
                typeAlias.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this TypeAlias typeAlias, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(typeAlias, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this TypeAlias typeAlias, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(typeAlias, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Variable> variables, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var variable in variables)
            {
                variable.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Variable variable, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(variable, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Variable variable, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(variable, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static void SaveAll(this IEnumerable<Friend> friends, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext)
        {
            var x = 0;

            foreach (var friend in friends)
            {
                friend.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDeclarationContext);
                x++;
            }
        }

        public static Guid? Save(this Friend friend, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(friend, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }

        public static Guid? Save(this Friend friend, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblSDKHeaderDeclarationContext, bool noRecurse = false)
        {
            return DoSave(friend, tblSdkHeaderFile, tblSDKHeaderDeclarationContext, noRecurse);
        }
        public static Guid? Save(this DependentNameType dependentNameType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(dependentNameType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this DependentNameType dependentNameType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(dependentNameType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this DependentNameType dependentNameType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (dependentNameType == null)
            {
                return null;
            }
            else if (checkInheritingSave && dependentNameType.GetType().InheritsFrom("DependentNameType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(dependentNameType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(dependentNameType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = dependentNameType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "DependentNameType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderDependentNameType = Entities.SaveIfNotExists<tblSDKHeaderDependentNameType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "DependentNameType")));

                    return new tblSDKHeaderDependentNameType
                    {
                        DependentNameTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DesugaredQualifiedTypeId = dependentNameType.Desugared.SaveOrGetId("DependentNameType.DesugaredQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "DependentNameType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderDependentNameType.DependentNameTypeId;
            }
        }

        public static Guid? Save(this DependentTemplateSpecializationType dependentTemplateSpecializationType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(dependentTemplateSpecializationType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this DependentTemplateSpecializationType dependentTemplateSpecializationType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(dependentTemplateSpecializationType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this DependentTemplateSpecializationType dependentTemplateSpecializationType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (dependentTemplateSpecializationType == null)
            {
                return null;
            }
            else if (checkInheritingSave && dependentTemplateSpecializationType.GetType().InheritsFrom("DependentTemplateSpecializationType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(dependentTemplateSpecializationType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(dependentTemplateSpecializationType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = dependentTemplateSpecializationType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "DependentTemplateSpecializationType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderDependentTemplateSpecializationType = Entities.SaveIfNotExists<tblSDKHeaderDependentTemplateSpecializationType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "DependentTemplateSpecializationType")));

                    return new tblSDKHeaderDependentTemplateSpecializationType
                    {
                        DependentTemplateSpecializationTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DesugaredQualifiedTypeId = dependentTemplateSpecializationType.Desugared.SaveOrGetId("DependentTemplateSpecializationType.DesugaredQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "DependentTemplateSpecializationType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    dependentTemplateSpecializationType.Arguments.SaveAll("DependentTemplateSpecializationType.Arguments", tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderDependentTemplateSpecializationType.DependentTemplateSpecializationTypeId;
            }
        }

        public static void SaveAll(this IEnumerable<TemplateArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType tblSDKHeaderDependentTemplateSpecializationType)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType);
                x++;
            }
        }

        public static Guid? Save(this TemplateArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType tblSDKHeaderDependentTemplateSpecializationType, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType, noRecurse);
        }

        public static Guid? Save(this TemplateArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType tblSDKHeaderDependentTemplateSpecializationType, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType, noRecurse);
        }
        public static Guid? Save(this Enumeration enumeration, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(enumeration, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Enumeration enumeration, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(enumeration, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Enumeration enumeration, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (enumeration == null)
            {
                return null;
            }
            else if (checkInheritingSave && enumeration.GetType().InheritsFrom("Enumeration"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(enumeration, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(enumeration, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = enumeration.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Enumeration", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderEnumeration = Entities.SaveIfNotExists<tblSDKHeaderEnumeration>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Enumeration")));

                    return new tblSDKHeaderEnumeration
                    {
                        EnumerationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        BuiltInTypeId = enumeration.BuiltInType.SaveOrGetId("Enumeration.BuiltInTypeId", tblSdkHeaderFile, noRecurse, true),
                        EnumIndex = enumeration.EnumIndex,
                        LocationIdentifier = (long) enumeration.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Enumeration")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderEnumeration.EnumIndex != enumeration.EnumIndex)
                    {
                        tblSDKHeaderEnumeration.EnumIndex = enumeration.EnumIndex;
                        resave = true;
                    }

                    if (tblSDKHeaderEnumeration.LocationIdentifier != (long) enumeration.LocationIdentifier)
                    {
                        tblSDKHeaderEnumeration.LocationIdentifier = (long) enumeration.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    enumeration.Items.SaveAll("Enumeration.Items", tblSdkHeaderFile, tblSDKHeaderEnumeration);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderEnumeration.EnumerationId;
            }
        }

        public static void SaveAll(this IEnumerable<EnumerationItem> items, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderEnumeration tblSDKHeaderEnumeration)
        {
            var x = 0;

            foreach (var item in items)
            {
                item.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderEnumeration);
                x++;
            }
        }

        public static Guid? Save(this EnumerationItem item, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderEnumeration tblSDKHeaderEnumeration, bool noRecurse = false)
        {
            return DoSave(item, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderEnumeration, noRecurse);
        }

        public static Guid? Save(this EnumerationItem item, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderEnumeration tblSDKHeaderEnumeration, bool noRecurse = false)
        {
            return DoSave(item, tblSdkHeaderFile, tblSDKHeaderEnumeration, noRecurse);
        }
        public static Guid? Save(this Friend friend, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(friend, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Friend friend, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(friend, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Friend friend, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (friend == null)
            {
                return null;
            }
            else if (checkInheritingSave && friend.GetType().InheritsFrom("Friend"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(friend, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(friend, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = friend.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Friend", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFriend = Entities.SaveIfNotExists<tblSDKHeaderFriend>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Friend")));

                    return new tblSDKHeaderFriend
                    {
                        FriendId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        FriendDeclarationId = friend.FriendDeclaration.SaveOrGetId("Friend.FriendDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) friend.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Friend")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderFriend.LocationIdentifier != (long) friend.LocationIdentifier)
                    {
                        tblSDKHeaderFriend.LocationIdentifier = (long) friend.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFriend.FriendId;
            }
        }

        public static Guid? Save(this FunctionTemplateSpecialization functionTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(functionTemplateSpecialization, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this FunctionTemplateSpecialization functionTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(functionTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this FunctionTemplateSpecialization functionTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (functionTemplateSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && functionTemplateSpecialization.GetType().InheritsFrom("FunctionTemplateSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(functionTemplateSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(functionTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = functionTemplateSpecialization.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FunctionTemplateSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFunctionTemplateSpecialization = Entities.SaveIfNotExists<tblSDKHeaderFunctionTemplateSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FunctionTemplateSpecialization")));

                    return new tblSDKHeaderFunctionTemplateSpecialization
                    {
                        FunctionTemplateSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningFunctionTemplateId = functionTemplateSpecialization.OwningFunctionTemplate.SaveOrGetId("FunctionTemplateSpecialization.OwningFunctionTemplateId", tblSdkHeaderFile, noRecurse, true),
                        SpecializedFunctionId = functionTemplateSpecialization.SpecializedFunction.SaveOrGetId("FunctionTemplateSpecialization.SpecializedFunctionId", tblSdkHeaderFile, noRecurse, true),
                        SpecializationKind = functionTemplateSpecialization.SpecializationKind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FunctionTemplateSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderFunctionTemplateSpecialization.SpecializationKind != functionTemplateSpecialization.SpecializationKind)
                    {
                        tblSDKHeaderFunctionTemplateSpecialization.SpecializationKind = functionTemplateSpecialization.SpecializationKind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    functionTemplateSpecialization.Arguments.SaveAll("FunctionTemplateSpecialization.Arguments", tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFunctionTemplateSpecialization.FunctionTemplateSpecializationId;
            }
        }

        public static void SaveAll(this IEnumerable<TemplateArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization tblSDKHeaderFunctionTemplateSpecialization)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization);
                x++;
            }
        }

        public static Guid? Save(this TemplateArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization tblSDKHeaderFunctionTemplateSpecialization, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization, noRecurse);
        }

        public static Guid? Save(this TemplateArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization tblSDKHeaderFunctionTemplateSpecialization, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization, noRecurse);
        }
        public static Guid? SaveOrGetId(this FunctionTemplateSpecialization functionTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplate tblOwningSDKHeaderFunctionTemplate, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (functionTemplateSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && functionTemplateSpecialization.GetType().InheritsFrom("FunctionTemplateSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(functionTemplateSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(functionTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = functionTemplateSpecialization.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderFunctionTemplate);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FunctionTemplateSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFunctionTemplateSpecialization = Entities.SaveIfNotExists<tblSDKHeaderFunctionTemplateSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FunctionTemplateSpecialization")));

                    return new tblSDKHeaderFunctionTemplateSpecialization
                    {
                        FunctionTemplateSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningFunctionTemplateId = tblOwningSDKHeaderFunctionTemplate.FunctionTemplateId,
                        SpecializedFunctionId = functionTemplateSpecialization.SpecializedFunction.SaveOrGetId("FunctionTemplateSpecialization.SpecializedFunctionId", tblSdkHeaderFile, noRecurse, true),
                        SpecializationKind = functionTemplateSpecialization.SpecializationKind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FunctionTemplateSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderFunctionTemplateSpecialization.SpecializationKind != functionTemplateSpecialization.SpecializationKind)
                    {
                        tblSDKHeaderFunctionTemplateSpecialization.SpecializationKind = functionTemplateSpecialization.SpecializationKind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    functionTemplateSpecialization.Arguments.SaveAll("FunctionTemplateSpecialization.Arguments", tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFunctionTemplateSpecialization.FunctionTemplateSpecializationId;
            }
        }

        public static Guid? Save(this FunctionType functionType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(functionType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this FunctionType functionType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(functionType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this FunctionType functionType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (functionType == null)
            {
                return null;
            }
            else if (checkInheritingSave && functionType.GetType().InheritsFrom("FunctionType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(functionType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(functionType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = functionType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FunctionType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFunctionType = Entities.SaveIfNotExists<tblSDKHeaderFunctionType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FunctionType")));

                    return new tblSDKHeaderFunctionType
                    {
                        FunctionTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        ReturnTypeQualifiedTypeId = functionType.ReturnType.SaveOrGetId("FunctionType.ReturnTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        CallingConvention = functionType.CallingConvention,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FunctionType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderFunctionType.CallingConvention != functionType.CallingConvention)
                    {
                        tblSDKHeaderFunctionType.CallingConvention = functionType.CallingConvention;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    functionType.Parameters.SaveAll("FunctionType.Parameters", tblSdkHeaderFile, tblSDKHeaderFunctionType);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFunctionType.FunctionTypeId;
            }
        }

        public static void SaveAll(this IEnumerable<Parameter> parameters, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionType tblSDKHeaderFunctionType)
        {
            var x = 0;

            foreach (var parameter in parameters)
            {
                parameter.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderFunctionType);
                x++;
            }
        }

        public static Guid? Save(this Parameter parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionType tblSDKHeaderFunctionType, bool noRecurse = false)
        {
            return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderFunctionType, noRecurse);
        }

        public static Guid? Save(this Parameter parameter, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionType tblSDKHeaderFunctionType, bool noRecurse = false)
        {
            return DoSave(parameter, tblSdkHeaderFile, tblSDKHeaderFunctionType, noRecurse);
        }
        public static Guid? Save(this InlineCommandComment inlineCommandComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(inlineCommandComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this InlineCommandComment inlineCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(inlineCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this InlineCommandComment inlineCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (inlineCommandComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && inlineCommandComment.GetType().InheritsFrom("InlineCommandComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(inlineCommandComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(inlineCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = inlineCommandComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "InlineCommandComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderInlineCommandComment = Entities.SaveIfNotExists<tblSDKHeaderInlineCommandComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "InlineCommandComment")));

                    return new tblSDKHeaderInlineCommandComment
                    {
                        InlineCommandCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        CommandIdIntegerValueId = inlineCommandComment.CommandId.SaveOrGetId("InlineCommandComment.CommandIdIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        CommentRenderKind = inlineCommandComment.CommentRenderKind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "InlineCommandComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderInlineCommandComment.CommentRenderKind != inlineCommandComment.CommentRenderKind)
                    {
                        tblSDKHeaderInlineCommandComment.CommentRenderKind = inlineCommandComment.CommentRenderKind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    inlineCommandComment.Arguments.SaveAll("InlineCommandComment.Arguments", tblSdkHeaderFile, tblSDKHeaderInlineCommandComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderInlineCommandComment.InlineCommandCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<InlineCommandCommentArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderInlineCommandComment tblSDKHeaderInlineCommandComment)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderInlineCommandComment);
                x++;
            }
        }

        public static Guid? Save(this InlineCommandCommentArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderInlineCommandComment tblSDKHeaderInlineCommandComment, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderInlineCommandComment, noRecurse);
        }

        public static Guid? Save(this InlineCommandCommentArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderInlineCommandComment tblSDKHeaderInlineCommandComment, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderInlineCommandComment, noRecurse);
        }
        public static Guid? Save(this InlineContentComment inlineContentComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(inlineContentComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this InlineContentComment inlineContentComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(inlineContentComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this InlineContentComment inlineContentComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (inlineContentComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && inlineContentComment.GetType().InheritsFrom("InlineContentComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(inlineContentComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(inlineContentComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = inlineContentComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "InlineContentComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderInlineContentComment = Entities.SaveIfNotExists<tblSDKHeaderInlineContentComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "InlineContentComment")));

                    return new tblSDKHeaderInlineContentComment
                    {
                        InlineContentCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = inlineContentComment.OwningComment.SaveOrGetId("InlineContentComment.OwningCommentId", tblSdkHeaderFile, noRecurse, true),
                        HasTrailingNewline = inlineContentComment.HasTrailingNewline,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "InlineContentComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderInlineContentComment.HasTrailingNewline != inlineContentComment.HasTrailingNewline)
                    {
                        tblSDKHeaderInlineContentComment.HasTrailingNewline = inlineContentComment.HasTrailingNewline;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderInlineContentComment.InlineContentCommentId;
            }
        }

        public static Guid? SaveOrGetId(this InlineContentComment inlineContentComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (inlineContentComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && inlineContentComment.GetType().InheritsFrom("InlineContentComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(inlineContentComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(inlineContentComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = inlineContentComment.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "InlineContentComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderInlineContentComment = Entities.SaveIfNotExists<tblSDKHeaderInlineContentComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "InlineContentComment")));

                    return new tblSDKHeaderInlineContentComment
                    {
                        InlineContentCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = tblOwningSDKHeaderComment.CommentId,
                        HasTrailingNewline = inlineContentComment.HasTrailingNewline,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "InlineContentComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderInlineContentComment.HasTrailingNewline != inlineContentComment.HasTrailingNewline)
                    {
                        tblSDKHeaderInlineContentComment.HasTrailingNewline = inlineContentComment.HasTrailingNewline;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderInlineContentComment.InlineContentCommentId;
            }
        }

        public static Guid? Save(this MacroExpansion macroExpansion, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(macroExpansion, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this MacroExpansion macroExpansion, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(macroExpansion, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this MacroExpansion macroExpansion, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (macroExpansion == null)
            {
                return null;
            }
            else if (checkInheritingSave && macroExpansion.GetType().InheritsFrom("MacroExpansion"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(macroExpansion, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(macroExpansion, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = macroExpansion.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "MacroExpansion", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderMacroExpansion = Entities.SaveIfNotExists<tblSDKHeaderMacroExpansion>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "MacroExpansion")));

                    return new tblSDKHeaderMacroExpansion
                    {
                        MacroExpansionId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DefinitionMacroDefinitionId = macroExpansion.Definition.SaveOrGetId("MacroExpansion.DefinitionMacroDefinitionId", tblSdkHeaderFile, noRecurse, true),
                        Name = macroExpansion.Name,
                        Text = macroExpansion.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "MacroExpansion")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderMacroExpansion.Name != macroExpansion.Name)
                    {
                        tblSDKHeaderMacroExpansion.Name = macroExpansion.Name;
                        resave = true;
                    }

                    if (tblSDKHeaderMacroExpansion.Text != macroExpansion.Text)
                    {
                        tblSDKHeaderMacroExpansion.Text = macroExpansion.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderMacroExpansion.MacroExpansionId;
            }
        }

        public static Guid? Save(this MemberPointerType memberPointerType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(memberPointerType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this MemberPointerType memberPointerType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(memberPointerType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this MemberPointerType memberPointerType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (memberPointerType == null)
            {
                return null;
            }
            else if (checkInheritingSave && memberPointerType.GetType().InheritsFrom("MemberPointerType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(memberPointerType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(memberPointerType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = memberPointerType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "MemberPointerType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderMemberPointerType = Entities.SaveIfNotExists<tblSDKHeaderMemberPointerType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "MemberPointerType")));

                    return new tblSDKHeaderMemberPointerType
                    {
                        MemberPointerTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        PointeeQualifiedTypeId = memberPointerType.Pointee.SaveOrGetId("MemberPointerType.PointeeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "MemberPointerType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderMemberPointerType.MemberPointerTypeId;
            }
        }

        public static Guid? Save(this ParamCommandComment paramCommandComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(paramCommandComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ParamCommandComment paramCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(paramCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ParamCommandComment paramCommandComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (paramCommandComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && paramCommandComment.GetType().InheritsFrom("ParamCommandComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(paramCommandComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(paramCommandComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = paramCommandComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ParamCommandComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderParamCommandComment = Entities.SaveIfNotExists<tblSDKHeaderParamCommandComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ParamCommandComment")));

                    return new tblSDKHeaderParamCommandComment
                    {
                        ParamCommandCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        ParamIndexIntegerValueId = paramCommandComment.ParamIndex.SaveOrGetId("ParamCommandComment.ParamIndexIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        Direction = paramCommandComment.Direction,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ParamCommandComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderParamCommandComment.Direction != paramCommandComment.Direction)
                    {
                        tblSDKHeaderParamCommandComment.Direction = paramCommandComment.Direction;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderParamCommandComment.ParamCommandCommentId;
            }
        }

        public static Guid? Save(this PointerType pointerType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(pointerType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this PointerType pointerType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(pointerType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this PointerType pointerType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (pointerType == null)
            {
                return null;
            }
            else if (checkInheritingSave && pointerType.GetType().InheritsFrom("PointerType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(pointerType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(pointerType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = pointerType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "PointerType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderPointerType = Entities.SaveIfNotExists<tblSDKHeaderPointerType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "PointerType")));

                    return new tblSDKHeaderPointerType
                    {
                        PointerTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        QualifiedPointeeQualifiedTypeId = pointerType.QualifiedPointee.SaveOrGetId("PointerType.QualifiedPointeeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Modifier = pointerType.Modifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "PointerType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderPointerType.Modifier != pointerType.Modifier)
                    {
                        tblSDKHeaderPointerType.Modifier = pointerType.Modifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderPointerType.PointerTypeId;
            }
        }

        public static Guid? Save(this TagType tagType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(tagType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TagType tagType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(tagType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TagType tagType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (tagType == null)
            {
                return null;
            }
            else if (checkInheritingSave && tagType.GetType().InheritsFrom("TagType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(tagType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(tagType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = tagType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TagType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTagType = Entities.SaveIfNotExists<tblSDKHeaderTagType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TagType")));

                    return new tblSDKHeaderTagType
                    {
                        TagTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DeclarationId = tagType.Declaration.SaveOrGetId("TagType.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TagType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTagType.TagTypeId;
            }
        }

        public static Guid? Save(this Template template, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(template, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Template template, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(template, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Template template, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (template == null)
            {
                return null;
            }
            else if (checkInheritingSave && template.GetType().InheritsFrom("Template"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(template, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(template, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = template.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Template", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplate = Entities.SaveIfNotExists<tblSDKHeaderTemplate>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Template")));

                    return new tblSDKHeaderTemplate
                    {
                        TemplateId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        TemplateDeclarationId = template.TemplateDeclaration.SaveOrGetId("Template.TemplateDeclarationId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) template.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Template")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplate.LocationIdentifier != (long) template.LocationIdentifier)
                    {
                        tblSDKHeaderTemplate.LocationIdentifier = (long) template.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    template.Parameters.SaveAll("Template.Parameters", tblSdkHeaderFile, tblSDKHeaderTemplate);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplate.TemplateId;
            }
        }

        public static void SaveAll(this IEnumerable<Declaration> parameters, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplate tblSDKHeaderTemplate)
        {
            var x = 0;

            foreach (var parameter in parameters)
            {
                parameter.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderTemplate);
                x++;
            }
        }

        public static Guid? Save(this Declaration parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplate tblSDKHeaderTemplate, bool noRecurse = false)
        {
            return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderTemplate, noRecurse);
        }

        public static Guid? Save(this Declaration parameter, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplate tblSDKHeaderTemplate, bool noRecurse = false)
        {
            return DoSave(parameter, tblSdkHeaderFile, tblSDKHeaderTemplate, noRecurse);
        }
        public static Guid? Save(this TemplateParameterSubstitutionType templateParameterSubstitutionType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateParameterSubstitutionType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TemplateParameterSubstitutionType templateParameterSubstitutionType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateParameterSubstitutionType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TemplateParameterSubstitutionType templateParameterSubstitutionType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateParameterSubstitutionType == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateParameterSubstitutionType.GetType().InheritsFrom("TemplateParameterSubstitutionType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateParameterSubstitutionType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateParameterSubstitutionType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateParameterSubstitutionType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateParameterSubstitutionType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateParameterSubstitutionType = Entities.SaveIfNotExists<tblSDKHeaderTemplateParameterSubstitutionType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateParameterSubstitutionType")));

                    return new tblSDKHeaderTemplateParameterSubstitutionType
                    {
                        TemplateParameterSubstitutionTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        ReplacementQualifiedTypeId = templateParameterSubstitutionType.Replacement.SaveOrGetId("TemplateParameterSubstitutionType.ReplacementQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateParameterSubstitutionType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateParameterSubstitutionType.TemplateParameterSubstitutionTypeId;
            }
        }

        public static Guid? Save(this TypeDefNameDecl typeDefNameDecl, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeDefNameDecl, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypeDefNameDecl typeDefNameDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeDefNameDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypeDefNameDecl typeDefNameDecl, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeDefNameDecl == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeDefNameDecl.GetType().InheritsFrom("TypeDefNameDecl"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeDefNameDecl, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeDefNameDecl, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeDefNameDecl.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeDefNameDecl", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeDefNameDecl = Entities.SaveIfNotExists<tblSDKHeaderTypeDefNameDecl>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeDefNameDecl")));

                    return new tblSDKHeaderTypeDefNameDecl
                    {
                        TypeDefNameDeclId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        QualifiedTypeId = typeDefNameDecl.QualifiedType.SaveOrGetId("TypeDefNameDecl.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) typeDefNameDecl.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeDefNameDecl")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeDefNameDecl.LocationIdentifier != (long) typeDefNameDecl.LocationIdentifier)
                    {
                        tblSDKHeaderTypeDefNameDecl.LocationIdentifier = (long) typeDefNameDecl.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeDefNameDecl.TypeDefNameDeclId;
            }
        }

        public static Guid? Save(this TypedefType typedefType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typedefType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypedefType typedefType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typedefType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypedefType typedefType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typedefType == null)
            {
                return null;
            }
            else if (checkInheritingSave && typedefType.GetType().InheritsFrom("TypedefType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typedefType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typedefType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typedefType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypedefType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypedefType = Entities.SaveIfNotExists<tblSDKHeaderTypedefType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypedefType")));

                    return new tblSDKHeaderTypedefType
                    {
                        TypedefTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DeclarationId = typedefType.Declaration.SaveOrGetId("TypedefType.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypedefType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypedefType.TypedefTypeId;
            }
        }

        public static Guid? Save(this TypeTemplateParameter typeTemplateParameter, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeTemplateParameter, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypeTemplateParameter typeTemplateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeTemplateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypeTemplateParameter typeTemplateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeTemplateParameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeTemplateParameter.GetType().InheritsFrom("TypeTemplateParameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeTemplateParameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeTemplateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeTemplateParameter.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeTemplateParameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeTemplateParameter = Entities.SaveIfNotExists<tblSDKHeaderTypeTemplateParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeTemplateParameter")));

                    return new tblSDKHeaderTypeTemplateParameter
                    {
                        TypeTemplateParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DefaultArgumentQualifiedTypeId = typeTemplateParameter.DefaultArgument.SaveOrGetId("TypeTemplateParameter.DefaultArgumentQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) typeTemplateParameter.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeTemplateParameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeTemplateParameter.LocationIdentifier != (long) typeTemplateParameter.LocationIdentifier)
                    {
                        tblSDKHeaderTypeTemplateParameter.LocationIdentifier = (long) typeTemplateParameter.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeTemplateParameter.TypeTemplateParameterId;
            }
        }

        public static Guid? Save(this VarTemplatePartialSpecialization varTemplatePartialSpecialization, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(varTemplatePartialSpecialization, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VarTemplatePartialSpecialization varTemplatePartialSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(varTemplatePartialSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VarTemplatePartialSpecialization varTemplatePartialSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (varTemplatePartialSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && varTemplatePartialSpecialization.GetType().InheritsFrom("VarTemplatePartialSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(varTemplatePartialSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(varTemplatePartialSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = varTemplatePartialSpecialization.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VarTemplatePartialSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVarTemplatePartialSpecialization = Entities.SaveIfNotExists<tblSDKHeaderVarTemplatePartialSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VarTemplatePartialSpecialization")));

                    return new tblSDKHeaderVarTemplatePartialSpecialization
                    {
                        VarTemplatePartialSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningVarTemplateId = varTemplatePartialSpecialization.OwningVarTemplate.SaveOrGetId("VarTemplatePartialSpecialization.OwningVarTemplateId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) varTemplatePartialSpecialization.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VarTemplatePartialSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVarTemplatePartialSpecialization.LocationIdentifier != (long) varTemplatePartialSpecialization.LocationIdentifier)
                    {
                        tblSDKHeaderVarTemplatePartialSpecialization.LocationIdentifier = (long) varTemplatePartialSpecialization.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVarTemplatePartialSpecialization.VarTemplatePartialSpecializationId;
            }
        }

        public static Guid? SaveOrGetId(this VarTemplatePartialSpecialization varTemplatePartialSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplate tblOwningSDKHeaderVarTemplate, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (varTemplatePartialSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && varTemplatePartialSpecialization.GetType().InheritsFrom("VarTemplatePartialSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(varTemplatePartialSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(varTemplatePartialSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = varTemplatePartialSpecialization.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderVarTemplate);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VarTemplatePartialSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVarTemplatePartialSpecialization = Entities.SaveIfNotExists<tblSDKHeaderVarTemplatePartialSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VarTemplatePartialSpecialization")));

                    return new tblSDKHeaderVarTemplatePartialSpecialization
                    {
                        VarTemplatePartialSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningVarTemplateId = tblOwningSDKHeaderVarTemplate.VarTemplateId,
                        LocationIdentifier = (long) varTemplatePartialSpecialization.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VarTemplatePartialSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVarTemplatePartialSpecialization.LocationIdentifier != (long) varTemplatePartialSpecialization.LocationIdentifier)
                    {
                        tblSDKHeaderVarTemplatePartialSpecialization.LocationIdentifier = (long) varTemplatePartialSpecialization.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVarTemplatePartialSpecialization.VarTemplatePartialSpecializationId;
            }
        }

        public static Guid? Save(this VarTemplateSpecialization varTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(varTemplateSpecialization, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VarTemplateSpecialization varTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(varTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VarTemplateSpecialization varTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (varTemplateSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && varTemplateSpecialization.GetType().InheritsFrom("VarTemplateSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(varTemplateSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(varTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = varTemplateSpecialization.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VarTemplateSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVarTemplateSpecialization = Entities.SaveIfNotExists<tblSDKHeaderVarTemplateSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VarTemplateSpecialization")));

                    return new tblSDKHeaderVarTemplateSpecialization
                    {
                        VarTemplateSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        TemplateDeclVarTemplateId = varTemplateSpecialization.TemplateDecl.SaveOrGetId("VarTemplateSpecialization.TemplateDeclVarTemplateId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) varTemplateSpecialization.LocationIdentifier,
                        SpecializationKind = varTemplateSpecialization.SpecializationKind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VarTemplateSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVarTemplateSpecialization.LocationIdentifier != (long) varTemplateSpecialization.LocationIdentifier)
                    {
                        tblSDKHeaderVarTemplateSpecialization.LocationIdentifier = (long) varTemplateSpecialization.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderVarTemplateSpecialization.SpecializationKind != varTemplateSpecialization.SpecializationKind)
                    {
                        tblSDKHeaderVarTemplateSpecialization.SpecializationKind = varTemplateSpecialization.SpecializationKind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    varTemplateSpecialization.Arguments.SaveAll("VarTemplateSpecialization.Arguments", tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVarTemplateSpecialization.VarTemplateSpecializationId;
            }
        }

        public static void SaveAll(this IEnumerable<TemplateArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization tblSDKHeaderVarTemplateSpecialization)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization);
                x++;
            }
        }

        public static Guid? Save(this TemplateArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization tblSDKHeaderVarTemplateSpecialization, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization, noRecurse);
        }

        public static Guid? Save(this TemplateArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization tblSDKHeaderVarTemplateSpecialization, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization, noRecurse);
        }
        public static Guid? Save(this VerbatimBlockLineComment verbatimBlockLineComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(verbatimBlockLineComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VerbatimBlockLineComment verbatimBlockLineComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(verbatimBlockLineComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VerbatimBlockLineComment verbatimBlockLineComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (verbatimBlockLineComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && verbatimBlockLineComment.GetType().InheritsFrom("VerbatimBlockLineComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(verbatimBlockLineComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(verbatimBlockLineComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = verbatimBlockLineComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VerbatimBlockLineComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVerbatimBlockLineComment = Entities.SaveIfNotExists<tblSDKHeaderVerbatimBlockLineComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VerbatimBlockLineComment")));

                    return new tblSDKHeaderVerbatimBlockLineComment
                    {
                        VerbatimBlockLineCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = verbatimBlockLineComment.OwningComment.SaveOrGetId("VerbatimBlockLineComment.OwningCommentId", tblSdkHeaderFile, noRecurse, true),
                        Text = verbatimBlockLineComment.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VerbatimBlockLineComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVerbatimBlockLineComment.Text != verbatimBlockLineComment.Text)
                    {
                        tblSDKHeaderVerbatimBlockLineComment.Text = verbatimBlockLineComment.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVerbatimBlockLineComment.VerbatimBlockLineCommentId;
            }
        }

        public static Guid? SaveOrGetId(this VerbatimBlockLineComment verbatimBlockLineComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (verbatimBlockLineComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && verbatimBlockLineComment.GetType().InheritsFrom("VerbatimBlockLineComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(verbatimBlockLineComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(verbatimBlockLineComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = verbatimBlockLineComment.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VerbatimBlockLineComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVerbatimBlockLineComment = Entities.SaveIfNotExists<tblSDKHeaderVerbatimBlockLineComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VerbatimBlockLineComment")));

                    return new tblSDKHeaderVerbatimBlockLineComment
                    {
                        VerbatimBlockLineCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = tblOwningSDKHeaderComment.CommentId,
                        Text = verbatimBlockLineComment.Text,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VerbatimBlockLineComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVerbatimBlockLineComment.Text != verbatimBlockLineComment.Text)
                    {
                        tblSDKHeaderVerbatimBlockLineComment.Text = verbatimBlockLineComment.Text;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVerbatimBlockLineComment.VerbatimBlockLineCommentId;
            }
        }

        public static Guid? Save(this AttributedType attributedType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(attributedType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this AttributedType attributedType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(attributedType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this AttributedType attributedType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (attributedType == null)
            {
                return null;
            }
            else if (checkInheritingSave && attributedType.GetType().InheritsFrom("AttributedType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(attributedType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(attributedType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = attributedType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "AttributedType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderAttributedType = Entities.SaveIfNotExists<tblSDKHeaderAttributedType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "AttributedType")));

                    return new tblSDKHeaderAttributedType
                    {
                        AttributedTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        EquivalentQualifiedTypeId = attributedType.Equivalent.SaveOrGetId("AttributedType.EquivalentQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        ModifiedQualifiedTypeId = attributedType.Modified.SaveOrGetId("AttributedType.ModifiedQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "AttributedType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderAttributedType.AttributedTypeId;
            }
        }

        public static Guid? Save(this BinaryOperator binaryOperator, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(binaryOperator, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this BinaryOperator binaryOperator, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(binaryOperator, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this BinaryOperator binaryOperator, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (binaryOperator == null)
            {
                return null;
            }
            else if (checkInheritingSave && binaryOperator.GetType().InheritsFrom("BinaryOperator"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(binaryOperator, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(binaryOperator, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = binaryOperator.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "BinaryOperator", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderBinaryOperator = Entities.SaveIfNotExists<tblSDKHeaderBinaryOperator>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "BinaryOperator")));

                    return new tblSDKHeaderBinaryOperator
                    {
                        BinaryOperatorId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        LHSExpressionId = binaryOperator.LHS.SaveOrGetId("BinaryOperator.LHSExpressionId", tblSdkHeaderFile, noRecurse, true),
                        RHSExpressionId = binaryOperator.RHS.SaveOrGetId("BinaryOperator.RHSExpressionId", tblSdkHeaderFile, noRecurse, true),
                        OpCodeString = binaryOperator.OpCodeString,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "BinaryOperator")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderBinaryOperator.OpCodeString != binaryOperator.OpCodeString)
                    {
                        tblSDKHeaderBinaryOperator.OpCodeString = binaryOperator.OpCodeString;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderBinaryOperator.BinaryOperatorId;
            }
        }

        public static Guid? Save(this ClassTemplateSpecialization classTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classTemplateSpecialization, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this ClassTemplateSpecialization classTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(classTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this ClassTemplateSpecialization classTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (classTemplateSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && classTemplateSpecialization.GetType().InheritsFrom("ClassTemplateSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(classTemplateSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(classTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = classTemplateSpecialization.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ClassTemplateSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderClassTemplateSpecialization = Entities.SaveIfNotExists<tblSDKHeaderClassTemplateSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ClassTemplateSpecialization")));

                    return new tblSDKHeaderClassTemplateSpecialization
                    {
                        ClassTemplateSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateId = classTemplateSpecialization.OwningClassTemplate.SaveOrGetId("ClassTemplateSpecialization.OwningClassTemplateId", tblSdkHeaderFile, noRecurse, true),
                        TemplatedDeclClassTemplateId = classTemplateSpecialization.TemplatedDecl.SaveOrGetId("ClassTemplateSpecialization.TemplatedDeclClassTemplateId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) classTemplateSpecialization.LocationIdentifier,
                        SpecializationKind = classTemplateSpecialization.SpecializationKind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ClassTemplateSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderClassTemplateSpecialization.LocationIdentifier != (long) classTemplateSpecialization.LocationIdentifier)
                    {
                        tblSDKHeaderClassTemplateSpecialization.LocationIdentifier = (long) classTemplateSpecialization.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderClassTemplateSpecialization.SpecializationKind != classTemplateSpecialization.SpecializationKind)
                    {
                        tblSDKHeaderClassTemplateSpecialization.SpecializationKind = classTemplateSpecialization.SpecializationKind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    classTemplateSpecialization.Arguments.SaveAll("ClassTemplateSpecialization.Arguments", tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderClassTemplateSpecialization.ClassTemplateSpecializationId;
            }
        }

        public static void SaveAll(this IEnumerable<TemplateArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization tblSDKHeaderClassTemplateSpecialization)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization);
                x++;
            }
        }

        public static Guid? Save(this TemplateArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization tblSDKHeaderClassTemplateSpecialization, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization, noRecurse);
        }

        public static Guid? Save(this TemplateArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization tblSDKHeaderClassTemplateSpecialization, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization, noRecurse);
        }
        public static Guid? SaveOrGetId(this ClassTemplateSpecialization classTemplateSpecialization, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplate tblOwningSDKHeaderClassTemplate, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (classTemplateSpecialization == null)
            {
                return null;
            }
            else if (checkInheritingSave && classTemplateSpecialization.GetType().InheritsFrom("ClassTemplateSpecialization"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(classTemplateSpecialization, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(classTemplateSpecialization, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = classTemplateSpecialization.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderClassTemplate);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "ClassTemplateSpecialization", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderClassTemplateSpecialization = Entities.SaveIfNotExists<tblSDKHeaderClassTemplateSpecialization>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "ClassTemplateSpecialization")));

                    return new tblSDKHeaderClassTemplateSpecialization
                    {
                        ClassTemplateSpecializationId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateId = tblOwningSDKHeaderClassTemplate.ClassTemplateId,
                        TemplatedDeclClassTemplateId = classTemplateSpecialization.TemplatedDecl.SaveOrGetId("ClassTemplateSpecialization.TemplatedDeclClassTemplateId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) classTemplateSpecialization.LocationIdentifier,
                        SpecializationKind = classTemplateSpecialization.SpecializationKind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "ClassTemplateSpecialization")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderClassTemplateSpecialization.LocationIdentifier != (long) classTemplateSpecialization.LocationIdentifier)
                    {
                        tblSDKHeaderClassTemplateSpecialization.LocationIdentifier = (long) classTemplateSpecialization.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderClassTemplateSpecialization.SpecializationKind != classTemplateSpecialization.SpecializationKind)
                    {
                        tblSDKHeaderClassTemplateSpecialization.SpecializationKind = classTemplateSpecialization.SpecializationKind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    classTemplateSpecialization.Arguments.SaveAll("ClassTemplateSpecialization.Arguments", tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderClassTemplateSpecialization.ClassTemplateSpecializationId;
            }
        }

        public static Guid? Save(this EnumerationItem enumerationItem, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(enumerationItem, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this EnumerationItem enumerationItem, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(enumerationItem, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this EnumerationItem enumerationItem, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (enumerationItem == null)
            {
                return null;
            }
            else if (checkInheritingSave && enumerationItem.GetType().InheritsFrom("EnumerationItem"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(enumerationItem, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(enumerationItem, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = enumerationItem.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "EnumerationItem", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderEnumerationItem = Entities.SaveIfNotExists<tblSDKHeaderEnumerationItem>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "EnumerationItem")));

                    return new tblSDKHeaderEnumerationItem
                    {
                        EnumerationItemId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningEnumerationId = enumerationItem.OwningEnumeration.SaveOrGetId("EnumerationItem.OwningEnumerationId", tblSdkHeaderFile, noRecurse, true),
                        ValueIntegerValueId = enumerationItem.Value.SaveOrGetId("EnumerationItem.ValueIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) enumerationItem.LocationIdentifier,
                        Expression = enumerationItem.Expression,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "EnumerationItem")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderEnumerationItem.LocationIdentifier != (long) enumerationItem.LocationIdentifier)
                    {
                        tblSDKHeaderEnumerationItem.LocationIdentifier = (long) enumerationItem.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderEnumerationItem.Expression != enumerationItem.Expression)
                    {
                        tblSDKHeaderEnumerationItem.Expression = enumerationItem.Expression;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderEnumerationItem.EnumerationItemId;
            }
        }

        public static Guid? SaveOrGetId(this EnumerationItem enumerationItem, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderEnumeration tblOwningSDKHeaderEnumeration, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (enumerationItem == null)
            {
                return null;
            }
            else if (checkInheritingSave && enumerationItem.GetType().InheritsFrom("EnumerationItem"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(enumerationItem, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(enumerationItem, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = enumerationItem.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderEnumeration);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "EnumerationItem", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderEnumerationItem = Entities.SaveIfNotExists<tblSDKHeaderEnumerationItem>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "EnumerationItem")));

                    return new tblSDKHeaderEnumerationItem
                    {
                        EnumerationItemId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningEnumerationId = tblOwningSDKHeaderEnumeration.EnumerationId,
                        ValueIntegerValueId = enumerationItem.Value.SaveOrGetId("EnumerationItem.ValueIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) enumerationItem.LocationIdentifier,
                        Expression = enumerationItem.Expression,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "EnumerationItem")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderEnumerationItem.LocationIdentifier != (long) enumerationItem.LocationIdentifier)
                    {
                        tblSDKHeaderEnumerationItem.LocationIdentifier = (long) enumerationItem.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderEnumerationItem.Expression != enumerationItem.Expression)
                    {
                        tblSDKHeaderEnumerationItem.Expression = enumerationItem.Expression;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderEnumerationItem.EnumerationItemId;
            }
        }

        public static Guid? Save(this FullComment fullComment, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(fullComment, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this FullComment fullComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(fullComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this FullComment fullComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (fullComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && fullComment.GetType().InheritsFrom("FullComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(fullComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(fullComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = fullComment.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FullComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFullComment = Entities.SaveIfNotExists<tblSDKHeaderFullComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FullComment")));

                    return new tblSDKHeaderFullComment
                    {
                        FullCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = fullComment.OwningComment.SaveOrGetId("FullComment.OwningCommentId", tblSdkHeaderFile, noRecurse, true),
                        OwningRawCommentId = fullComment.OwningRawComment.SaveOrGetId("FullComment.OwningRawCommentId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FullComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    fullComment.Blocks.SaveAll("FullComment.Blocks", tblSdkHeaderFile, tblSDKHeaderFullComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFullComment.FullCommentId;
            }
        }

        public static void SaveAll(this IEnumerable<BlockContentComment> blocks, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFullComment tblSDKHeaderFullComment)
        {
            var x = 0;

            foreach (var block in blocks)
            {
                block.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderFullComment);
                x++;
            }
        }

        public static Guid? Save(this BlockContentComment block, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFullComment tblSDKHeaderFullComment, bool noRecurse = false)
        {
            return DoSave(block, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderFullComment, noRecurse);
        }

        public static Guid? Save(this BlockContentComment block, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFullComment tblSDKHeaderFullComment, bool noRecurse = false)
        {
            return DoSave(block, tblSdkHeaderFile, tblSDKHeaderFullComment, noRecurse);
        }
        public static Guid? SaveOrGetId(this FullComment fullComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (fullComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && fullComment.GetType().InheritsFrom("FullComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(fullComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(fullComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = fullComment.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FullComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFullComment = Entities.SaveIfNotExists<tblSDKHeaderFullComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FullComment")));

                    return new tblSDKHeaderFullComment
                    {
                        FullCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = tblOwningSDKHeaderComment.CommentId,
                        OwningRawCommentId = fullComment.OwningRawComment.SaveOrGetId("FullComment.OwningRawCommentId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FullComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    fullComment.Blocks.SaveAll("FullComment.Blocks", tblSdkHeaderFile, tblSDKHeaderFullComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFullComment.FullCommentId;
            }
        }

        public static Guid? SaveOrGetId(this FullComment fullComment, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderRawComment tblOwningSDKHeaderRawComment, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (fullComment == null)
            {
                return null;
            }
            else if (checkInheritingSave && fullComment.GetType().InheritsFrom("FullComment"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(fullComment, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(fullComment, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = fullComment.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderRawComment);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "FullComment", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFullComment = Entities.SaveIfNotExists<tblSDKHeaderFullComment>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "FullComment")));

                    return new tblSDKHeaderFullComment
                    {
                        FullCommentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningCommentId = fullComment.OwningComment.SaveOrGetId("FullComment.OwningCommentId", tblSdkHeaderFile, noRecurse, true),
                        OwningRawCommentId = tblOwningSDKHeaderRawComment.RawCommentId,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "FullComment")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    fullComment.Blocks.SaveAll("FullComment.Blocks", tblSdkHeaderFile, tblSDKHeaderFullComment);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFullComment.FullCommentId;
            }
        }

        public static Guid? Save(this InjectedClassNameType injectedClassNameType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(injectedClassNameType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this InjectedClassNameType injectedClassNameType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(injectedClassNameType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this InjectedClassNameType injectedClassNameType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (injectedClassNameType == null)
            {
                return null;
            }
            else if (checkInheritingSave && injectedClassNameType.GetType().InheritsFrom("InjectedClassNameType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(injectedClassNameType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(injectedClassNameType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = injectedClassNameType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "InjectedClassNameType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderInjectedClassNameType = Entities.SaveIfNotExists<tblSDKHeaderInjectedClassNameType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "InjectedClassNameType")));

                    return new tblSDKHeaderInjectedClassNameType
                    {
                        InjectedClassNameTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        ClassId = injectedClassNameType.Class.SaveOrGetId("InjectedClassNameType.ClassId", tblSdkHeaderFile, noRecurse, true),
                        InjectedSpecializationTypeQualifiedTypeId = injectedClassNameType.InjectedSpecializationType.SaveOrGetId("InjectedClassNameType.InjectedSpecializationTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "InjectedClassNameType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderInjectedClassNameType.InjectedClassNameTypeId;
            }
        }

        public static Guid? Save(this Method method, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(method, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Method method, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(method, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Method method, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (method == null)
            {
                return null;
            }
            else if (checkInheritingSave && method.GetType().InheritsFrom("Method"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(method, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(method, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = method.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Method", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderMethod = Entities.SaveIfNotExists<tblSDKHeaderMethod>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Method")));

                    return new tblSDKHeaderMethod
                    {
                        MethodId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = method.OwningClass.SaveOrGetId("Method.OwningClassId", tblSdkHeaderFile, noRecurse, true),
                        ConversionTypeQualifiedTypeId = method.ConversionType.SaveOrGetId("Method.ConversionTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) method.LocationIdentifier,
                        IsConst = method.IsConst,
                        IsCopyConstructor = method.IsCopyConstructor,
                        IsDefaultConstructor = method.IsDefaultConstructor,
                        IsExplicit = method.IsExplicit,
                        IsImplicit = method.IsImplicit,
                        IsMoveConstructor = method.IsMoveConstructor,
                        IsOverride = method.IsOverride,
                        IsPure = method.IsPure,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Method")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderMethod.LocationIdentifier != (long) method.LocationIdentifier)
                    {
                        tblSDKHeaderMethod.LocationIdentifier = (long) method.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsConst != method.IsConst)
                    {
                        tblSDKHeaderMethod.IsConst = method.IsConst;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsCopyConstructor != method.IsCopyConstructor)
                    {
                        tblSDKHeaderMethod.IsCopyConstructor = method.IsCopyConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsDefaultConstructor != method.IsDefaultConstructor)
                    {
                        tblSDKHeaderMethod.IsDefaultConstructor = method.IsDefaultConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsExplicit != method.IsExplicit)
                    {
                        tblSDKHeaderMethod.IsExplicit = method.IsExplicit;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsImplicit != method.IsImplicit)
                    {
                        tblSDKHeaderMethod.IsImplicit = method.IsImplicit;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsMoveConstructor != method.IsMoveConstructor)
                    {
                        tblSDKHeaderMethod.IsMoveConstructor = method.IsMoveConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsOverride != method.IsOverride)
                    {
                        tblSDKHeaderMethod.IsOverride = method.IsOverride;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsPure != method.IsPure)
                    {
                        tblSDKHeaderMethod.IsPure = method.IsPure;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderMethod.MethodId;
            }
        }

        public static Guid? SaveOrGetId(this Method method, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (method == null)
            {
                return null;
            }
            else if (checkInheritingSave && method.GetType().InheritsFrom("Method"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(method, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(method, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = method.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderClass);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Method", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderMethod = Entities.SaveIfNotExists<tblSDKHeaderMethod>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Method")));

                    return new tblSDKHeaderMethod
                    {
                        MethodId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = tblOwningSDKHeaderClass.ClassId,
                        ConversionTypeQualifiedTypeId = method.ConversionType.SaveOrGetId("Method.ConversionTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) method.LocationIdentifier,
                        IsConst = method.IsConst,
                        IsCopyConstructor = method.IsCopyConstructor,
                        IsDefaultConstructor = method.IsDefaultConstructor,
                        IsExplicit = method.IsExplicit,
                        IsImplicit = method.IsImplicit,
                        IsMoveConstructor = method.IsMoveConstructor,
                        IsOverride = method.IsOverride,
                        IsPure = method.IsPure,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Method")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderMethod.LocationIdentifier != (long) method.LocationIdentifier)
                    {
                        tblSDKHeaderMethod.LocationIdentifier = (long) method.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsConst != method.IsConst)
                    {
                        tblSDKHeaderMethod.IsConst = method.IsConst;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsCopyConstructor != method.IsCopyConstructor)
                    {
                        tblSDKHeaderMethod.IsCopyConstructor = method.IsCopyConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsDefaultConstructor != method.IsDefaultConstructor)
                    {
                        tblSDKHeaderMethod.IsDefaultConstructor = method.IsDefaultConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsExplicit != method.IsExplicit)
                    {
                        tblSDKHeaderMethod.IsExplicit = method.IsExplicit;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsImplicit != method.IsImplicit)
                    {
                        tblSDKHeaderMethod.IsImplicit = method.IsImplicit;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsMoveConstructor != method.IsMoveConstructor)
                    {
                        tblSDKHeaderMethod.IsMoveConstructor = method.IsMoveConstructor;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsOverride != method.IsOverride)
                    {
                        tblSDKHeaderMethod.IsOverride = method.IsOverride;
                        resave = true;
                    }

                    if (tblSDKHeaderMethod.IsPure != method.IsPure)
                    {
                        tblSDKHeaderMethod.IsPure = method.IsPure;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderMethod.MethodId;
            }
        }

        public static Guid? Save(this NonTypeTemplateParameter nonTypeTemplateParameter, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(nonTypeTemplateParameter, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this NonTypeTemplateParameter nonTypeTemplateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(nonTypeTemplateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this NonTypeTemplateParameter nonTypeTemplateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (nonTypeTemplateParameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && nonTypeTemplateParameter.GetType().InheritsFrom("NonTypeTemplateParameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(nonTypeTemplateParameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(nonTypeTemplateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = nonTypeTemplateParameter.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "NonTypeTemplateParameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderNonTypeTemplateParameter = Entities.SaveIfNotExists<tblSDKHeaderNonTypeTemplateParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "NonTypeTemplateParameter")));

                    return new tblSDKHeaderNonTypeTemplateParameter
                    {
                        NonTypeTemplateParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DefaultArgumentExpressionId = nonTypeTemplateParameter.DefaultArgument.SaveOrGetId("NonTypeTemplateParameter.DefaultArgumentExpressionId", tblSdkHeaderFile, noRecurse, true),
                        PositionIntegerValueId = nonTypeTemplateParameter.Position.SaveOrGetId("NonTypeTemplateParameter.PositionIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) nonTypeTemplateParameter.LocationIdentifier,
                        IsExpandedParameterPack = nonTypeTemplateParameter.IsExpandedParameterPack,
                        IsPackExpansion = nonTypeTemplateParameter.IsPackExpansion,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "NonTypeTemplateParameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderNonTypeTemplateParameter.LocationIdentifier != (long) nonTypeTemplateParameter.LocationIdentifier)
                    {
                        tblSDKHeaderNonTypeTemplateParameter.LocationIdentifier = (long) nonTypeTemplateParameter.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderNonTypeTemplateParameter.IsExpandedParameterPack != nonTypeTemplateParameter.IsExpandedParameterPack)
                    {
                        tblSDKHeaderNonTypeTemplateParameter.IsExpandedParameterPack = nonTypeTemplateParameter.IsExpandedParameterPack;
                        resave = true;
                    }

                    if (tblSDKHeaderNonTypeTemplateParameter.IsPackExpansion != nonTypeTemplateParameter.IsPackExpansion)
                    {
                        tblSDKHeaderNonTypeTemplateParameter.IsPackExpansion = nonTypeTemplateParameter.IsPackExpansion;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderNonTypeTemplateParameter.NonTypeTemplateParameterId;
            }
        }

        public static Guid? Save(this TemplateParameter templateParameter, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateParameter, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TemplateParameter templateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TemplateParameter templateParameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateParameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateParameter.GetType().InheritsFrom("TemplateParameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateParameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateParameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateParameter.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateParameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateParameter = Entities.SaveIfNotExists<tblSDKHeaderTemplateParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateParameter")));

                    return new tblSDKHeaderTemplateParameter
                    {
                        TemplateParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DepthIntegerValueId = templateParameter.Depth.SaveOrGetId("TemplateParameter.DepthIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        IndexIntegerValueId = templateParameter.Index.SaveOrGetId("TemplateParameter.IndexIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) templateParameter.LocationIdentifier,
                        IsParameterPack = templateParameter.IsParameterPack,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateParameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateParameter.LocationIdentifier != (long) templateParameter.LocationIdentifier)
                    {
                        tblSDKHeaderTemplateParameter.LocationIdentifier = (long) templateParameter.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateParameter.IsParameterPack != templateParameter.IsParameterPack)
                    {
                        tblSDKHeaderTemplateParameter.IsParameterPack = templateParameter.IsParameterPack;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateParameter.TemplateParameterId;
            }
        }

        public static Guid? Save(this TemplateSpecializationType templateSpecializationType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateSpecializationType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TemplateSpecializationType templateSpecializationType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateSpecializationType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TemplateSpecializationType templateSpecializationType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateSpecializationType == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateSpecializationType.GetType().InheritsFrom("TemplateSpecializationType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateSpecializationType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateSpecializationType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateSpecializationType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateSpecializationType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateSpecializationType = Entities.SaveIfNotExists<tblSDKHeaderTemplateSpecializationType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateSpecializationType")));

                    return new tblSDKHeaderTemplateSpecializationType
                    {
                        TemplateSpecializationTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        TemplateId = templateSpecializationType.Template.SaveOrGetId("TemplateSpecializationType.TemplateId", tblSdkHeaderFile, noRecurse, true),
                        DesugardQualifiedTypeId = templateSpecializationType.Desugard.SaveOrGetId("TemplateSpecializationType.DesugardQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateSpecializationType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    templateSpecializationType.Arguments.SaveAll("TemplateSpecializationType.Arguments", tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateSpecializationType.TemplateSpecializationTypeId;
            }
        }

        public static void SaveAll(this IEnumerable<TemplateArgument> arguments, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType tblSDKHeaderTemplateSpecializationType)
        {
            var x = 0;

            foreach (var argument in arguments)
            {
                argument.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType);
                x++;
            }
        }

        public static Guid? Save(this TemplateArgument argument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType tblSDKHeaderTemplateSpecializationType, bool noRecurse = false)
        {
            return DoSave(argument, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType, noRecurse);
        }

        public static Guid? Save(this TemplateArgument argument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType tblSDKHeaderTemplateSpecializationType, bool noRecurse = false)
        {
            return DoSave(argument, tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType, noRecurse);
        }
        public static Guid? Save(this TypeDef typeDef, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeDef, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypeDef typeDef, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeDef, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypeDef typeDef, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeDef == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeDef.GetType().InheritsFrom("TypeDef"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeDef, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeDef, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeDef.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeDef", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeDef = Entities.SaveIfNotExists<tblSDKHeaderTypeDef>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeDef")));

                    return new tblSDKHeaderTypeDef
                    {
                        TypeDefId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationContextId = typeDef.OwningDeclarationContext.SaveOrGetId("TypeDef.OwningDeclarationContextId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = typeDef.QualifiedType.SaveOrGetId("TypeDef.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) typeDef.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeDef")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeDef.LocationIdentifier != (long) typeDef.LocationIdentifier)
                    {
                        tblSDKHeaderTypeDef.LocationIdentifier = (long) typeDef.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeDef.TypeDefId;
            }
        }

        public static Guid? SaveOrGetId(this TypeDef typeDef, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblOwningSDKHeaderDeclarationContext, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeDef == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeDef.GetType().InheritsFrom("TypeDef"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeDef, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeDef, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeDef.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDeclarationContext);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeDef", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeDef = Entities.SaveIfNotExists<tblSDKHeaderTypeDef>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeDef")));

                    return new tblSDKHeaderTypeDef
                    {
                        TypeDefId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationContextId = tblOwningSDKHeaderDeclarationContext.DeclarationContextId,
                        QualifiedTypeId = typeDef.QualifiedType.SaveOrGetId("TypeDef.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) typeDef.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeDef")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeDef.LocationIdentifier != (long) typeDef.LocationIdentifier)
                    {
                        tblSDKHeaderTypeDef.LocationIdentifier = (long) typeDef.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeDef.TypeDefId;
            }
        }

        public static Guid? Save(this UnaryTransformType unaryTransformType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(unaryTransformType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this UnaryTransformType unaryTransformType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(unaryTransformType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this UnaryTransformType unaryTransformType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (unaryTransformType == null)
            {
                return null;
            }
            else if (checkInheritingSave && unaryTransformType.GetType().InheritsFrom("UnaryTransformType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(unaryTransformType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(unaryTransformType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = unaryTransformType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "UnaryTransformType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderUnaryTransformType = Entities.SaveIfNotExists<tblSDKHeaderUnaryTransformType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "UnaryTransformType")));

                    return new tblSDKHeaderUnaryTransformType
                    {
                        UnaryTransformTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DesugardedQualifiedTypeId = unaryTransformType.Desugarded.SaveOrGetId("UnaryTransformType.DesugardedQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        BaseTypeQualifiedTypeId = unaryTransformType.BaseType.SaveOrGetId("UnaryTransformType.BaseTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "UnaryTransformType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderUnaryTransformType.UnaryTransformTypeId;
            }
        }

        public static Guid? Save(this Variable variable, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(variable, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Variable variable, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(variable, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Variable variable, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (variable == null)
            {
                return null;
            }
            else if (checkInheritingSave && variable.GetType().InheritsFrom("Variable"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(variable, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(variable, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = variable.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Variable", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVariable = Entities.SaveIfNotExists<tblSDKHeaderVariable>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Variable")));

                    return new tblSDKHeaderVariable
                    {
                        VariableId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationContextId = variable.OwningDeclarationContext.SaveOrGetId("Variable.OwningDeclarationContextId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = variable.QualifiedType.SaveOrGetId("Variable.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) variable.LocationIdentifier,
                        Mangled = variable.Mangled,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Variable")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVariable.LocationIdentifier != (long) variable.LocationIdentifier)
                    {
                        tblSDKHeaderVariable.LocationIdentifier = (long) variable.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderVariable.Mangled != variable.Mangled)
                    {
                        tblSDKHeaderVariable.Mangled = variable.Mangled;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVariable.VariableId;
            }
        }

        public static Guid? SaveOrGetId(this Variable variable, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblOwningSDKHeaderDeclarationContext, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (variable == null)
            {
                return null;
            }
            else if (checkInheritingSave && variable.GetType().InheritsFrom("Variable"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(variable, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(variable, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = variable.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDeclarationContext);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Variable", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVariable = Entities.SaveIfNotExists<tblSDKHeaderVariable>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Variable")));

                    return new tblSDKHeaderVariable
                    {
                        VariableId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationContextId = tblOwningSDKHeaderDeclarationContext.DeclarationContextId,
                        QualifiedTypeId = variable.QualifiedType.SaveOrGetId("Variable.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) variable.LocationIdentifier,
                        Mangled = variable.Mangled,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Variable")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVariable.LocationIdentifier != (long) variable.LocationIdentifier)
                    {
                        tblSDKHeaderVariable.LocationIdentifier = (long) variable.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderVariable.Mangled != variable.Mangled)
                    {
                        tblSDKHeaderVariable.Mangled = variable.Mangled;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVariable.VariableId;
            }
        }

        public static Guid? Save(this VectorType vectorType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(vectorType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VectorType vectorType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(vectorType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VectorType vectorType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (vectorType == null)
            {
                return null;
            }
            else if (checkInheritingSave && vectorType.GetType().InheritsFrom("VectorType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(vectorType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(vectorType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = vectorType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VectorType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVectorType = Entities.SaveIfNotExists<tblSDKHeaderVectorType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VectorType")));

                    return new tblSDKHeaderVectorType
                    {
                        VectorTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        ElementTypeQualifiedTypeId = vectorType.ElementType.SaveOrGetId("VectorType.ElementTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        NumElementsIntegerValueId = vectorType.NumElements.SaveOrGetId("VectorType.NumElementsIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VectorType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVectorType.VectorTypeId;
            }
        }

        public static Guid? Save(this VTableComponent vTableComponent, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(vTableComponent, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this VTableComponent vTableComponent, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(vTableComponent, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this VTableComponent vTableComponent, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (vTableComponent == null)
            {
                return null;
            }
            else if (checkInheritingSave && vTableComponent.GetType().InheritsFrom("VTableComponent"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(vTableComponent, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(vTableComponent, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = vTableComponent.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VTableComponent", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVTableComponent = Entities.SaveIfNotExists<tblSDKHeaderVTableComponent>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VTableComponent")));

                    return new tblSDKHeaderVTableComponent
                    {
                        VTableComponentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningVTableLayoutId = vTableComponent.OwningVTableLayout.SaveOrGetId("VTableComponent.OwningVTableLayoutId", tblSdkHeaderFile, noRecurse, true),
                        DeclarationId = vTableComponent.Declaration.SaveOrGetId("VTableComponent.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        OffsetIntegerValueId = vTableComponent.Offset.SaveOrGetId("VTableComponent.OffsetIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        Kind = vTableComponent.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VTableComponent")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVTableComponent.Kind != vTableComponent.Kind)
                    {
                        tblSDKHeaderVTableComponent.Kind = vTableComponent.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVTableComponent.VTableComponentId;
            }
        }

        public static Guid? SaveOrGetId(this VTableComponent vTableComponent, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVTableLayout tblOwningSDKHeaderVTableLayout, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (vTableComponent == null)
            {
                return null;
            }
            else if (checkInheritingSave && vTableComponent.GetType().InheritsFrom("VTableComponent"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(vTableComponent, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(vTableComponent, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = vTableComponent.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderVTableLayout);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "VTableComponent", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderVTableComponent = Entities.SaveIfNotExists<tblSDKHeaderVTableComponent>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "VTableComponent")));

                    return new tblSDKHeaderVTableComponent
                    {
                        VTableComponentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningVTableLayoutId = tblOwningSDKHeaderVTableLayout.VTableLayoutId,
                        DeclarationId = vTableComponent.Declaration.SaveOrGetId("VTableComponent.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        OffsetIntegerValueId = vTableComponent.Offset.SaveOrGetId("VTableComponent.OffsetIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        Kind = vTableComponent.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "VTableComponent")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderVTableComponent.Kind != vTableComponent.Kind)
                    {
                        tblSDKHeaderVTableComponent.Kind = vTableComponent.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderVTableComponent.VTableComponentId;
            }
        }

        public static Guid? Save(this DecayedType decayedType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(decayedType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this DecayedType decayedType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(decayedType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this DecayedType decayedType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (decayedType == null)
            {
                return null;
            }
            else if (checkInheritingSave && decayedType.GetType().InheritsFrom("DecayedType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(decayedType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(decayedType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = decayedType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "DecayedType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderDecayedType = Entities.SaveIfNotExists<tblSDKHeaderDecayedType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "DecayedType")));

                    return new tblSDKHeaderDecayedType
                    {
                        DecayedTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DecayedQualifiedTypeId = decayedType.Decayed.SaveOrGetId("DecayedType.DecayedQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        OriginalQualifiedTypeId = decayedType.Original.SaveOrGetId("DecayedType.OriginalQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        PointeeQualifiedTypeId = decayedType.Pointee.SaveOrGetId("DecayedType.PointeeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "DecayedType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderDecayedType.DecayedTypeId;
            }
        }

        public static Guid? Save(this Field field, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(field, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Field field, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(field, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Field field, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (field == null)
            {
                return null;
            }
            else if (checkInheritingSave && field.GetType().InheritsFrom("Field"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(field, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(field, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = field.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Field", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderField = Entities.SaveIfNotExists<tblSDKHeaderField>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Field")));

                    return new tblSDKHeaderField
                    {
                        FieldId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = field.OwningClass.SaveOrGetId("Field.OwningClassId", tblSdkHeaderFile, noRecurse, true),
                        BitWidthIntegerValueId = field.BitWidth.SaveOrGetId("Field.BitWidthIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = field.QualifiedType.SaveOrGetId("Field.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        FieldIndex = field.FieldIndex,
                        LocationIdentifier = (long) field.LocationIdentifier,
                        IsBitField = field.IsBitField,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Field")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderField.FieldIndex != field.FieldIndex)
                    {
                        tblSDKHeaderField.FieldIndex = field.FieldIndex;
                        resave = true;
                    }

                    if (tblSDKHeaderField.LocationIdentifier != (long) field.LocationIdentifier)
                    {
                        tblSDKHeaderField.LocationIdentifier = (long) field.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderField.IsBitField != field.IsBitField)
                    {
                        tblSDKHeaderField.IsBitField = field.IsBitField;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderField.FieldId;
            }
        }

        public static Guid? SaveOrGetId(this Field field, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (field == null)
            {
                return null;
            }
            else if (checkInheritingSave && field.GetType().InheritsFrom("Field"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(field, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(field, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = field.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderClass);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Field", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderField = Entities.SaveIfNotExists<tblSDKHeaderField>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Field")));

                    return new tblSDKHeaderField
                    {
                        FieldId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassId = tblOwningSDKHeaderClass.ClassId,
                        BitWidthIntegerValueId = field.BitWidth.SaveOrGetId("Field.BitWidthIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = field.QualifiedType.SaveOrGetId("Field.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        FieldIndex = field.FieldIndex,
                        LocationIdentifier = (long) field.LocationIdentifier,
                        IsBitField = field.IsBitField,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Field")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderField.FieldIndex != field.FieldIndex)
                    {
                        tblSDKHeaderField.FieldIndex = field.FieldIndex;
                        resave = true;
                    }

                    if (tblSDKHeaderField.LocationIdentifier != (long) field.LocationIdentifier)
                    {
                        tblSDKHeaderField.LocationIdentifier = (long) field.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderField.IsBitField != field.IsBitField)
                    {
                        tblSDKHeaderField.IsBitField = field.IsBitField;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderField.FieldId;
            }
        }

        public static Guid? Save(this Function function, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(function, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Function function, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(function, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Function function, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (function == null)
            {
                return null;
            }
            else if (checkInheritingSave && function.GetType().InheritsFrom("Function"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(function, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(function, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = function.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Function", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderFunction = Entities.SaveIfNotExists<tblSDKHeaderFunction>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Function")));

                    return new tblSDKHeaderFunction
                    {
                        FunctionId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        CommentId = function.Comment.SaveOrGetId("Function.CommentId", tblSdkHeaderFile, noRecurse, true),
                        InstantiatedFromFunctionId = function.InstantiatedFrom.SaveOrGetId("Function.InstantiatedFromFunctionId", tblSdkHeaderFile, noRecurse, true),
                        ReturnTypeQualifiedTypeId = function.ReturnType.SaveOrGetId("Function.ReturnTypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        FunctionName = function.FunctionName,
                        Access = function.Access,
                        CallingConvention = function.CallingConvention,
                        HasThisReturn = function.HasThisReturn,
                        IsDeleted = function.IsDeleted,
                        IsInline = function.IsInline,
                        IsPure = function.IsPure,
                        IsReturnIndirect = function.IsReturnIndirect,
                        IsVariadic = function.IsVariadic,
                        OperatorKind = function.OperatorKind,
                        Mangled = function.Mangled,
                        Signature = function.Signature,
                        DebugText = function.DebugText,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Function")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderFunction.FunctionName != function.FunctionName)
                    {
                        tblSDKHeaderFunction.FunctionName = function.FunctionName;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.Access != function.Access)
                    {
                        tblSDKHeaderFunction.Access = function.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.CallingConvention != function.CallingConvention)
                    {
                        tblSDKHeaderFunction.CallingConvention = function.CallingConvention;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.HasThisReturn != function.HasThisReturn)
                    {
                        tblSDKHeaderFunction.HasThisReturn = function.HasThisReturn;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.IsDeleted != function.IsDeleted)
                    {
                        tblSDKHeaderFunction.IsDeleted = function.IsDeleted;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.IsInline != function.IsInline)
                    {
                        tblSDKHeaderFunction.IsInline = function.IsInline;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.IsPure != function.IsPure)
                    {
                        tblSDKHeaderFunction.IsPure = function.IsPure;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.IsReturnIndirect != function.IsReturnIndirect)
                    {
                        tblSDKHeaderFunction.IsReturnIndirect = function.IsReturnIndirect;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.IsVariadic != function.IsVariadic)
                    {
                        tblSDKHeaderFunction.IsVariadic = function.IsVariadic;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.OperatorKind != function.OperatorKind)
                    {
                        tblSDKHeaderFunction.OperatorKind = function.OperatorKind;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.Mangled != function.Mangled)
                    {
                        tblSDKHeaderFunction.Mangled = function.Mangled;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.Signature != function.Signature)
                    {
                        tblSDKHeaderFunction.Signature = function.Signature;
                        resave = true;
                    }

                    if (tblSDKHeaderFunction.DebugText != function.DebugText)
                    {
                        tblSDKHeaderFunction.DebugText = function.DebugText;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                    function.Parameters.SaveAll("Function.Parameters", tblSdkHeaderFile, tblSDKHeaderFunction);
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderFunction.FunctionId;
            }
        }

        public static void SaveAll(this IEnumerable<Parameter> parameters, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunction tblSDKHeaderFunction)
        {
            var x = 0;

            foreach (var parameter in parameters)
            {
                parameter.Save(string.Format("{0}[{1}]", parentPropertyName, x), tblSdkHeaderFile, tblSDKHeaderFunction);
                x++;
            }
        }

        public static Guid? Save(this Parameter parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunction tblSDKHeaderFunction, bool noRecurse = false)
        {
            return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, tblSDKHeaderFunction, noRecurse);
        }

        public static Guid? Save(this Parameter parameter, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunction tblSDKHeaderFunction, bool noRecurse = false)
        {
            return DoSave(parameter, tblSdkHeaderFile, tblSDKHeaderFunction, noRecurse);
        }
        public static Guid? Save(this TemplateParameterType templateParameterType, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateParameterType, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TemplateParameterType templateParameterType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateParameterType, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TemplateParameterType templateParameterType, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateParameterType == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateParameterType.GetType().InheritsFrom("TemplateParameterType"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateParameterType, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateParameterType, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateParameterType.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateParameterType", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateParameterType = Entities.SaveIfNotExists<tblSDKHeaderTemplateParameterType>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateParameterType")));

                    return new tblSDKHeaderTemplateParameterType
                    {
                        TemplateParameterTypeId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        DepthIntegerValueId = templateParameterType.Depth.SaveOrGetId("TemplateParameterType.DepthIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        IndexIntegerValueId = templateParameterType.Index.SaveOrGetId("TemplateParameterType.IndexIntegerValueId", tblSdkHeaderFile, noRecurse, true),
                        ParameterTypeTemplateParameterId = templateParameterType.Parameter.SaveOrGetId("TemplateParameterType.ParameterTypeTemplateParameterId", tblSdkHeaderFile, noRecurse, true),
                        IsParameterPack = templateParameterType.IsParameterPack,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateParameterType")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateParameterType.IsParameterPack != templateParameterType.IsParameterPack)
                    {
                        tblSDKHeaderTemplateParameterType.IsParameterPack = templateParameterType.IsParameterPack;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateParameterType.TemplateParameterTypeId;
            }
        }

        public static Guid? Save(this TypeAlias typeAlias, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeAlias, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TypeAlias typeAlias, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(typeAlias, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TypeAlias typeAlias, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeAlias == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeAlias.GetType().InheritsFrom("TypeAlias"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeAlias, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeAlias, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeAlias.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeAlias", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeAlias = Entities.SaveIfNotExists<tblSDKHeaderTypeAlia>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeAlias")));

                    return new tblSDKHeaderTypeAlia
                    {
                        TypeAliasId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationContextId = typeAlias.OwningDeclarationContext.SaveOrGetId("TypeAlias.OwningDeclarationContextId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = typeAlias.QualifiedType.SaveOrGetId("TypeAlias.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        DescribedAliasTemplateId = typeAlias.DescribedAliasTemplate.SaveOrGetId("TypeAlias.DescribedAliasTemplateId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) typeAlias.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeAlias")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeAlias.LocationIdentifier != (long) typeAlias.LocationIdentifier)
                    {
                        tblSDKHeaderTypeAlias.LocationIdentifier = (long) typeAlias.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeAlias.TypeAliasId;
            }
        }

        public static Guid? SaveOrGetId(this TypeAlias typeAlias, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblOwningSDKHeaderDeclarationContext, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (typeAlias == null)
            {
                return null;
            }
            else if (checkInheritingSave && typeAlias.GetType().InheritsFrom("TypeAlias"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(typeAlias, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(typeAlias, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = typeAlias.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDeclarationContext);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TypeAlias", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTypeAlias = Entities.SaveIfNotExists<tblSDKHeaderTypeAlia>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TypeAlias")));

                    return new tblSDKHeaderTypeAlia
                    {
                        TypeAliasId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningDeclarationContextId = tblOwningSDKHeaderDeclarationContext.DeclarationContextId,
                        QualifiedTypeId = typeAlias.QualifiedType.SaveOrGetId("TypeAlias.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        DescribedAliasTemplateId = typeAlias.DescribedAliasTemplate.SaveOrGetId("TypeAlias.DescribedAliasTemplateId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) typeAlias.LocationIdentifier,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TypeAlias")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTypeAlias.LocationIdentifier != (long) typeAlias.LocationIdentifier)
                    {
                        tblSDKHeaderTypeAlias.LocationIdentifier = (long) typeAlias.LocationIdentifier;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTypeAlias.TypeAliasId;
            }
        }

        public static Guid? Save(this Parameter parameter, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(parameter, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this Parameter parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this Parameter parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (parameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && parameter.GetType().InheritsFrom("Parameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(parameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = parameter.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Parameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderParameter = Entities.SaveIfNotExists<tblSDKHeaderParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Parameter")));

                    return new tblSDKHeaderParameter
                    {
                        ParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningFunctionTypeId = parameter.OwningFunctionType.SaveOrGetId("Parameter.OwningFunctionTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionId = parameter.OwningFunction.SaveOrGetId("Parameter.OwningFunctionId", tblSdkHeaderFile, noRecurse, true),
                        CommentId = parameter.Comment.SaveOrGetId("Parameter.CommentId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = parameter.QualifiedType.SaveOrGetId("Parameter.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) parameter.LocationIdentifier,
                        DebugText = parameter.DebugText,
                        Access = parameter.Access,
                        DefaultArgument = parameter.DefaultArgument,
                        IsIndirect = parameter.IsIndirect,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Parameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderParameter.LocationIdentifier != (long) parameter.LocationIdentifier)
                    {
                        tblSDKHeaderParameter.LocationIdentifier = (long) parameter.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.DebugText != parameter.DebugText)
                    {
                        tblSDKHeaderParameter.DebugText = parameter.DebugText;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.Access != parameter.Access)
                    {
                        tblSDKHeaderParameter.Access = parameter.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.DefaultArgument != parameter.DefaultArgument)
                    {
                        tblSDKHeaderParameter.DefaultArgument = parameter.DefaultArgument;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.IsIndirect != parameter.IsIndirect)
                    {
                        tblSDKHeaderParameter.IsIndirect = parameter.IsIndirect;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderParameter.ParameterId;
            }
        }

        public static Guid? SaveOrGetId(this Parameter parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionType tblOwningSDKHeaderFunctionType, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (parameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && parameter.GetType().InheritsFrom("Parameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(parameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = parameter.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderFunctionType);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Parameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderParameter = Entities.SaveIfNotExists<tblSDKHeaderParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Parameter")));

                    return new tblSDKHeaderParameter
                    {
                        ParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningFunctionTypeId = tblOwningSDKHeaderFunctionType.FunctionTypeId,
                        OwningFunctionId = parameter.OwningFunction.SaveOrGetId("Parameter.OwningFunctionId", tblSdkHeaderFile, noRecurse, true),
                        CommentId = parameter.Comment.SaveOrGetId("Parameter.CommentId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = parameter.QualifiedType.SaveOrGetId("Parameter.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) parameter.LocationIdentifier,
                        DebugText = parameter.DebugText,
                        Access = parameter.Access,
                        DefaultArgument = parameter.DefaultArgument,
                        IsIndirect = parameter.IsIndirect,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Parameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderParameter.LocationIdentifier != (long) parameter.LocationIdentifier)
                    {
                        tblSDKHeaderParameter.LocationIdentifier = (long) parameter.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.DebugText != parameter.DebugText)
                    {
                        tblSDKHeaderParameter.DebugText = parameter.DebugText;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.Access != parameter.Access)
                    {
                        tblSDKHeaderParameter.Access = parameter.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.DefaultArgument != parameter.DefaultArgument)
                    {
                        tblSDKHeaderParameter.DefaultArgument = parameter.DefaultArgument;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.IsIndirect != parameter.IsIndirect)
                    {
                        tblSDKHeaderParameter.IsIndirect = parameter.IsIndirect;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderParameter.ParameterId;
            }
        }

        public static Guid? SaveOrGetId(this Parameter parameter, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunction tblOwningSDKHeaderFunction, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (parameter == null)
            {
                return null;
            }
            else if (checkInheritingSave && parameter.GetType().InheritsFrom("Parameter"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(parameter, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(parameter, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = parameter.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderFunction);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "Parameter", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderParameter = Entities.SaveIfNotExists<tblSDKHeaderParameter>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "Parameter")));

                    return new tblSDKHeaderParameter
                    {
                        ParameterId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningFunctionTypeId = parameter.OwningFunctionType.SaveOrGetId("Parameter.OwningFunctionTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionId = tblOwningSDKHeaderFunction.FunctionId,
                        CommentId = parameter.Comment.SaveOrGetId("Parameter.CommentId", tblSdkHeaderFile, noRecurse, true),
                        QualifiedTypeId = parameter.QualifiedType.SaveOrGetId("Parameter.QualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        LocationIdentifier = (long) parameter.LocationIdentifier,
                        DebugText = parameter.DebugText,
                        Access = parameter.Access,
                        DefaultArgument = parameter.DefaultArgument,
                        IsIndirect = parameter.IsIndirect,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "Parameter")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderParameter.LocationIdentifier != (long) parameter.LocationIdentifier)
                    {
                        tblSDKHeaderParameter.LocationIdentifier = (long) parameter.LocationIdentifier;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.DebugText != parameter.DebugText)
                    {
                        tblSDKHeaderParameter.DebugText = parameter.DebugText;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.Access != parameter.Access)
                    {
                        tblSDKHeaderParameter.Access = parameter.Access;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.DefaultArgument != parameter.DefaultArgument)
                    {
                        tblSDKHeaderParameter.DefaultArgument = parameter.DefaultArgument;
                        resave = true;
                    }

                    if (tblSDKHeaderParameter.IsIndirect != parameter.IsIndirect)
                    {
                        tblSDKHeaderParameter.IsIndirect = parameter.IsIndirect;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderParameter.ParameterId;
            }
        }

        public static Guid? Save(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? Save(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false)
        {
            return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
        }

        public static Guid? SaveOrGetId(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateArgument.GetType().InheritsFrom("TemplateArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateArgument.GetWhere(tblSdkHeaderFile);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateArgument = Entities.SaveIfNotExists<tblSDKHeaderTemplateArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateArgument")));

                    return new tblSDKHeaderTemplateArgument
                    {
                        TemplateArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateSpecializationId = templateArgument.OwningClassTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningClassTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningDependentTemplateSpecializationTypeId = templateArgument.OwningDependentTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningDependentTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningVarTemplateSpecializationId = templateArgument.OwningVarTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningVarTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningTemplateSpecializationTypeId = templateArgument.OwningTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionTemplateSpecializationId = templateArgument.OwningFunctionTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningFunctionTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        DeclarationId = templateArgument.Declaration.SaveOrGetId("TemplateArgument.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        TypeQualifiedTypeId = templateArgument.Type.SaveOrGetId("TemplateArgument.TypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Integral = templateArgument.Integral,
                        Kind = templateArgument.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateArgument.Integral != templateArgument.Integral)
                    {
                        tblSDKHeaderTemplateArgument.Integral = templateArgument.Integral;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateArgument.Kind != templateArgument.Kind)
                    {
                        tblSDKHeaderTemplateArgument.Kind = templateArgument.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateArgument.TemplateArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization tblOwningSDKHeaderClassTemplateSpecialization, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateArgument.GetType().InheritsFrom("TemplateArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderClassTemplateSpecialization);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateArgument = Entities.SaveIfNotExists<tblSDKHeaderTemplateArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateArgument")));

                    return new tblSDKHeaderTemplateArgument
                    {
                        TemplateArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateSpecializationId = tblOwningSDKHeaderClassTemplateSpecialization.ClassTemplateSpecializationId,
                        OwningDependentTemplateSpecializationTypeId = templateArgument.OwningDependentTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningDependentTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningVarTemplateSpecializationId = templateArgument.OwningVarTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningVarTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningTemplateSpecializationTypeId = templateArgument.OwningTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionTemplateSpecializationId = templateArgument.OwningFunctionTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningFunctionTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        DeclarationId = templateArgument.Declaration.SaveOrGetId("TemplateArgument.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        TypeQualifiedTypeId = templateArgument.Type.SaveOrGetId("TemplateArgument.TypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Integral = templateArgument.Integral,
                        Kind = templateArgument.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateArgument.Integral != templateArgument.Integral)
                    {
                        tblSDKHeaderTemplateArgument.Integral = templateArgument.Integral;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateArgument.Kind != templateArgument.Kind)
                    {
                        tblSDKHeaderTemplateArgument.Kind = templateArgument.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateArgument.TemplateArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType tblOwningSDKHeaderDependentTemplateSpecializationType, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateArgument.GetType().InheritsFrom("TemplateArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderDependentTemplateSpecializationType);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateArgument = Entities.SaveIfNotExists<tblSDKHeaderTemplateArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateArgument")));

                    return new tblSDKHeaderTemplateArgument
                    {
                        TemplateArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateSpecializationId = templateArgument.OwningClassTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningClassTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningDependentTemplateSpecializationTypeId = tblOwningSDKHeaderDependentTemplateSpecializationType.DependentTemplateSpecializationTypeId,
                        OwningVarTemplateSpecializationId = templateArgument.OwningVarTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningVarTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningTemplateSpecializationTypeId = templateArgument.OwningTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionTemplateSpecializationId = templateArgument.OwningFunctionTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningFunctionTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        DeclarationId = templateArgument.Declaration.SaveOrGetId("TemplateArgument.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        TypeQualifiedTypeId = templateArgument.Type.SaveOrGetId("TemplateArgument.TypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Integral = templateArgument.Integral,
                        Kind = templateArgument.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateArgument.Integral != templateArgument.Integral)
                    {
                        tblSDKHeaderTemplateArgument.Integral = templateArgument.Integral;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateArgument.Kind != templateArgument.Kind)
                    {
                        tblSDKHeaderTemplateArgument.Kind = templateArgument.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateArgument.TemplateArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization tblOwningSDKHeaderVarTemplateSpecialization, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateArgument.GetType().InheritsFrom("TemplateArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderVarTemplateSpecialization);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateArgument = Entities.SaveIfNotExists<tblSDKHeaderTemplateArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateArgument")));

                    return new tblSDKHeaderTemplateArgument
                    {
                        TemplateArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateSpecializationId = templateArgument.OwningClassTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningClassTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningDependentTemplateSpecializationTypeId = templateArgument.OwningDependentTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningDependentTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningVarTemplateSpecializationId = tblOwningSDKHeaderVarTemplateSpecialization.VarTemplateSpecializationId,
                        OwningTemplateSpecializationTypeId = templateArgument.OwningTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionTemplateSpecializationId = templateArgument.OwningFunctionTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningFunctionTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        DeclarationId = templateArgument.Declaration.SaveOrGetId("TemplateArgument.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        TypeQualifiedTypeId = templateArgument.Type.SaveOrGetId("TemplateArgument.TypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Integral = templateArgument.Integral,
                        Kind = templateArgument.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateArgument.Integral != templateArgument.Integral)
                    {
                        tblSDKHeaderTemplateArgument.Integral = templateArgument.Integral;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateArgument.Kind != templateArgument.Kind)
                    {
                        tblSDKHeaderTemplateArgument.Kind = templateArgument.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateArgument.TemplateArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType tblOwningSDKHeaderTemplateSpecializationType, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateArgument.GetType().InheritsFrom("TemplateArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderTemplateSpecializationType);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateArgument = Entities.SaveIfNotExists<tblSDKHeaderTemplateArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateArgument")));

                    return new tblSDKHeaderTemplateArgument
                    {
                        TemplateArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateSpecializationId = templateArgument.OwningClassTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningClassTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningDependentTemplateSpecializationTypeId = templateArgument.OwningDependentTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningDependentTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningVarTemplateSpecializationId = templateArgument.OwningVarTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningVarTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningTemplateSpecializationTypeId = tblOwningSDKHeaderTemplateSpecializationType.TemplateSpecializationTypeId,
                        OwningFunctionTemplateSpecializationId = templateArgument.OwningFunctionTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningFunctionTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        DeclarationId = templateArgument.Declaration.SaveOrGetId("TemplateArgument.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        TypeQualifiedTypeId = templateArgument.Type.SaveOrGetId("TemplateArgument.TypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Integral = templateArgument.Integral,
                        Kind = templateArgument.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateArgument.Integral != templateArgument.Integral)
                    {
                        tblSDKHeaderTemplateArgument.Integral = templateArgument.Integral;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateArgument.Kind != templateArgument.Kind)
                    {
                        tblSDKHeaderTemplateArgument.Kind = templateArgument.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateArgument.TemplateArgumentId;
            }
        }

        public static Guid? SaveOrGetId(this TemplateArgument templateArgument, string parentPropertyName, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization tblOwningSDKHeaderFunctionTemplateSpecialization, bool noRecurse = false, bool checkInheritingSave = false)
        {
            if (templateArgument == null)
            {
                return null;
            }
            else if (checkInheritingSave && templateArgument.GetType().InheritsFrom("TemplateArgument"))
            {
                if (parentPropertyName.IsNullOrEmpty())
                {
                    return DoSave(templateArgument, tblSdkHeaderFile, noRecurse);
                }
                else
                {
                    return DoSave(templateArgument, parentPropertyName, tblSdkHeaderFile, noRecurse);
                }
            }
            else
            {
                var notExists = false;
                var where = templateArgument.GetWhere(tblSdkHeaderFile, tblOwningSDKHeaderFunctionTemplateSpecialization);

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("Calling SaveOrGetId for {0}", parentPropertyName)));
                    IndentLog(thisType, EventArgs.Empty);
                }

                WriteLog(thisType, new EventArgs<string>(string.Format("Call to SaveOrGetId for {0} WHERE NOT {1}", "TemplateArgument", HeaderWhereClauseExtensions.CurrentWhere)));
                IndentLog(thisType, EventArgs.Empty);

                var tblSDKHeaderTemplateArgument = Entities.SaveIfNotExists<tblSDKHeaderTemplateArgument>(where, () =>
                {
                    notExists = true;

                    WriteLog(thisType, new EventArgs<string>(string.Format("Does not exist. Saving {0} ...", "TemplateArgument")));

                    return new tblSDKHeaderTemplateArgument
                    {
                        TemplateArgumentId = Guid.NewGuid(),
                        HeaderFileId = tblSdkHeaderFile.HeaderFileId,
                        OwningClassTemplateSpecializationId = templateArgument.OwningClassTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningClassTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningDependentTemplateSpecializationTypeId = templateArgument.OwningDependentTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningDependentTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningVarTemplateSpecializationId = templateArgument.OwningVarTemplateSpecialization.SaveOrGetId("TemplateArgument.OwningVarTemplateSpecializationId", tblSdkHeaderFile, noRecurse, true),
                        OwningTemplateSpecializationTypeId = templateArgument.OwningTemplateSpecializationType.SaveOrGetId("TemplateArgument.OwningTemplateSpecializationTypeId", tblSdkHeaderFile, noRecurse, true),
                        OwningFunctionTemplateSpecializationId = tblOwningSDKHeaderFunctionTemplateSpecialization.FunctionTemplateSpecializationId,
                        DeclarationId = templateArgument.Declaration.SaveOrGetId("TemplateArgument.DeclarationId", tblSdkHeaderFile, noRecurse, true),
                        TypeQualifiedTypeId = templateArgument.Type.SaveOrGetId("TemplateArgument.TypeQualifiedTypeId", tblSdkHeaderFile, noRecurse, true),
                        Integral = templateArgument.Integral,
                        Kind = templateArgument.Kind,
                    };
                });

                if (!notExists)
                {
                    WriteLog(thisType, new EventArgs<string>(string.Format("{0} already exists", "TemplateArgument")));
                }

                if (!noRecurse && notExists)
                {
                    var resave = false;

                    if (tblSDKHeaderTemplateArgument.Integral != templateArgument.Integral)
                    {
                        tblSDKHeaderTemplateArgument.Integral = templateArgument.Integral;
                        resave = true;
                    }

                    if (tblSDKHeaderTemplateArgument.Kind != templateArgument.Kind)
                    {
                        tblSDKHeaderTemplateArgument.Kind = templateArgument.Kind;
                        resave = true;
                    }

                    if (resave)
                    {
                        Entities.SaveChanges();
                    }
                }

                if (!noRecurse)
                {    
                }

                if (!parentPropertyName.IsNullOrEmpty())
                {
                    OutdentLog(thisType, EventArgs.Empty);
                }

                OutdentLog(thisType, EventArgs.Empty);

                return tblSDKHeaderTemplateArgument.TemplateArgumentId;
            }
        }

    }
}
