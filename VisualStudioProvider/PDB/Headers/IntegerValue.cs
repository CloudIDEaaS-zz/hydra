using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class IntegerValue
    {
        public BigInteger Value { get; set; }
        public string StringValue { get; set; }
        public string ValueType { get; set; }

        public IntegerValue(ulong ulongValue)
        {
            Value = ulongValue;
            ValueType = ulongValue.GetType().Name;
            StringValue = string.Format("0x{0:X8}", ulongValue);
        }

        public IntegerValue(uint uIntValue)
        {
            Value = uIntValue;
            ValueType = uIntValue.GetType().Name;
            StringValue = string.Format("0x{0:X4}", uIntValue);
        }
    }
}
