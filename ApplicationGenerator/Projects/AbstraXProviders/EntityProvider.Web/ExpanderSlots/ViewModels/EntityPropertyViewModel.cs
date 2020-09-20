using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityProvider.Web.ExpanderSlots.ViewModels
{
    class EntityPropertyViewModel
    {
        public string KeyField { get; set; }
        public string DataType { get; set; }
        public int DataSize { get; set; }
        public string DefaultValue { get; set; }
        public bool AllowNull { get; set; }

#if SILVERLIGHT
            // TextBlock Labels
            TextBlock lblDataType = new TextBlock()
            {
                Height = 25,
                Width = 150,
                Text = "Data Type",
            };

            TextBlock lblAllowNull = new TextBlock()
            {
                Height = 25,
                Width = 150,
                Text = "Allow Null",
            };

            TextBlock lblDefaultValue = new TextBlock()
            {
                Height = 25,
                Width = 150,
                Text = "Default Value",
            };

            TextBlock lblDataSize = new TextBlock()
            {
                Height = 25,
                Width = 150,
                Text = "Data Size",
            };

            TextBlock lblKey= new TextBlock()
            {
                Height = 25,
                Width = 150,
                Text = "Key",
            };


            // ComboBox Data Type
            ComboBox cmbDataType = new ComboBox()
            {
                Height = 25,
                Width = 150,
            };

            ComboBoxItem cmbTypeBoolean = new ComboBoxItem()
            {
                Name = "Boolean",
                Content = "Boolean",
            };

            ComboBoxItem cmbTypeSByte = new ComboBoxItem()
            {
                Name = "SByte",
                Content = "SByte",
            };

            ComboBoxItem cmbTypeByte = new ComboBoxItem()
            {
                Name = "Byte",
                Content = "Byte",
            };

            ComboBoxItem cmbTypeInt16 = new ComboBoxItem()
            {
                Name = "Int16",
                Content = "Int16",
            };

            ComboBoxItem cmbTypeInt32 = new ComboBoxItem()
            {
                Name = "Int32",
                Content = "Int32",
            };

            ComboBoxItem cmbTypeInt64 = new ComboBoxItem()
            {
                Name = "Int64",
                Content = "Int64",
            };

            ComboBoxItem cmbTypeSingle = new ComboBoxItem()
            {
                Name = "Single",
                Content = "Single",
            };

            ComboBoxItem cmbTypeDouble = new ComboBoxItem()
            {
                Name = "Double",
                Content = "Double",
            };

            ComboBoxItem cmbTypeDecimal = new ComboBoxItem()
            {
                Name = "Decimal",
                Content = "Decimal",
            };

            ComboBoxItem cmbTypeDateTime = new ComboBoxItem()
            {
                Name = "DateTime",
                Content = "DateTime",
            };

            ComboBoxItem cmbTypeTime = new ComboBoxItem()
            {
                Name = "Time",
                Content = "Time",
            };

            ComboBoxItem cmbTypeString = new ComboBoxItem()
            {
                Name = "String",
                Content = "String",
            };

            ComboBoxItem cmbTypeBinary = new ComboBoxItem()
            {
                Name = "Binary",
                Content = "Binary",

            };

            cmbDataType.Items.Add(cmbTypeBinary);
            cmbDataType.Items.Add(cmbTypeBoolean);
            cmbDataType.Items.Add(cmbTypeByte);
            cmbDataType.Items.Add(cmbTypeDateTime);
            cmbDataType.Items.Add(cmbTypeDecimal);
            cmbDataType.Items.Add(cmbTypeDouble);
            cmbDataType.Items.Add(cmbTypeInt16);
            cmbDataType.Items.Add(cmbTypeInt32);
            cmbDataType.Items.Add(cmbTypeInt64);
            cmbDataType.Items.Add(cmbTypeSByte);
            cmbDataType.Items.Add(cmbTypeSingle);
            cmbDataType.Items.Add(cmbTypeString);
            cmbDataType.Items.Add(cmbTypeTime);

            // TextBox Default Value
            TextBox txtDefaultValue= new TextBox()
            {
                Height = 25,
                Width = 150,
            };

            // TextBox Data Size
            TextBox txtDataSize = new TextBox()
            {
                Height = 25,
                Width = 150,
            };

            // ComboBox Allow Null
            ComboBox cmbAllowNull = new ComboBox()
            {
                Height = 25,
                Width = 150,
            };

            ComboBoxItem cmbNullTrue = new ComboBoxItem()
            {
                Name = "True",
                Content = "True",
            };

            ComboBoxItem cmbNullFalse = new ComboBoxItem()
            {
                Name = "False",
                Content = "False ",
            };

            cmbAllowNull.Items.Add(cmbNullTrue);
            cmbAllowNull.Items.Add(cmbNullFalse);


            // ComboBox Key Status
            ComboBox cmbKey = new ComboBox()
            {
                Height = 25,
                Width = 150,
            };

            ComboBoxItem cmbKeyPrimary = new ComboBoxItem()
            {
                Name = "PrimaryKey",
                Content = "Primary Key",
            };

            ComboBoxItem cmbKeyForeign = new ComboBoxItem()
            {
                Name = "ForeignKey",
                Content = "Foreign Key",
            };

            ComboBoxItem cmbKeyNone = new ComboBoxItem()
            {
                Name = "None",
                Content = "None",
            };

            cmbKey.Items.Add(cmbKeyPrimary);
            cmbKey.Items.Add(cmbKeyForeign);
            cmbKey.Items.Add(cmbKeyNone);
#endif
    }
}
