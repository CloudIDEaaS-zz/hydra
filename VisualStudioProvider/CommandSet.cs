using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider
{
    public class CommandSet
    {
        public string Name { get; private set; }
        public Guid Guid { get; private set; }
        public Dictionary<uint, string> CmdIds { get; set; }

        public CommandSet(Guid guid, string name, Type enumType)
        {
            this.Guid = guid;
            this.Name = name;
            this.CmdIds = new Dictionary<uint, string>();

            foreach (object value in Enum.GetValues(enumType))
            {
                var valueName = Enum.GetName(enumType, value);
                var enumValue = (uint)(int) value;

                if (!CmdIds.ContainsKey(enumValue))
                {
                    CmdIds.Add(enumValue, valueName);
                }
                else
                {
                    CmdIds[enumValue] += ", " + valueName;
                }
            }
        }
    }
}
