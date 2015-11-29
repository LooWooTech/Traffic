﻿using LoowooTech.Traffic.Models;
using System;
using System.ComponentModel;

namespace LoowooTech.Traffic.Common
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return null;
            var attribut = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribut == null ? value.ToString() : attribut.Description;
        }

        public static StatisticMode GetEnum(this string Description)
        {
            foreach (StatisticMode mode in Enum.GetValues(typeof(StatisticMode)))
            {
                if (mode.GetDescription() == Description)
                {
                    return mode;
                }
            }
            return StatisticMode.All;
            
        }
    }
}
