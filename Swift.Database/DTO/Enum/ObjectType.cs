﻿namespace Swift.Database.DTO.Enum
{

    public enum ObjectType : byte
    {
        [EnumValue("*")]
        All = 0,

        [EnumValue("P")]
        Procedures = 1,

        [EnumValue("FN")]
        [EnumValue("FS")]
        [EnumValue("FT")]
        [EnumValue("IF")]
        [EnumValue("TF")]
        Functions = 2,

        [EnumValue("V")]
        Views = 3,

        [EnumValue("TR")]
        Triggers = 4,

        [EnumValue("AF")] //Not possible to update
        AggregateFunctions = 5
    }
}