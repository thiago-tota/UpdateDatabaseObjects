using System;

namespace Swift.Database.DTO.Enum
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumValue : Attribute
    {
        public string Value { get; private set; }

        public EnumValue(string value)
        {
            Value = value;
        }
    }
}
