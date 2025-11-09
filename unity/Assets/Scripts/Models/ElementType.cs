using System;

namespace FiveElements.Shared
{
    public enum ElementType
    {
        None = -1,
        Metal = 0,
        Wood = 1,
        Water = 2,
        Fire = 3,
        Earth = 4
    }

    public static class ElementTypeExtensions
    {
        private static readonly ElementType[,] GenerateRelations = new ElementType[5, 5]
        {
            // Metal generates Water
            { ElementType.None, ElementType.None, ElementType.Water, ElementType.None, ElementType.None },
            // Wood generates Fire
            { ElementType.None, ElementType.None, ElementType.None, ElementType.Fire, ElementType.None },
            // Water generates Wood
            { ElementType.None, ElementType.Wood, ElementType.None, ElementType.None, ElementType.None },
            // Fire generates Earth
            { ElementType.None, ElementType.None, ElementType.None, ElementType.None, ElementType.Earth },
            // Earth generates Metal
            { ElementType.Metal, ElementType.None, ElementType.None, ElementType.None, ElementType.None }
        };

        private static readonly ElementType[,] OvercomeRelations = new ElementType[5, 5]
        {
            // Metal overcomes Wood
            { ElementType.None, ElementType.Wood, ElementType.None, ElementType.None, ElementType.None },
            // Wood overcomes Earth
            { ElementType.None, ElementType.None, ElementType.None, ElementType.None, ElementType.Earth },
            // Water overcomes Fire
            { ElementType.None, ElementType.None, ElementType.None, ElementType.Fire, ElementType.None },
            // Fire overcomes Metal
            { ElementType.Metal, ElementType.None, ElementType.None, ElementType.None, ElementType.None },
            // Earth overcomes Water
            { ElementType.None, ElementType.None, ElementType.Water, ElementType.None, ElementType.None }
        };

        public static bool Generates(this ElementType source, ElementType target)
        {
            if (source == ElementType.None || target == ElementType.None) return false;
            return GenerateRelations[(int)source, (int)target] == target;
        }

        public static bool Overcomes(this ElementType source, ElementType target)
        {
            if (source == ElementType.None || target == ElementType.None) return false;
            return OvercomeRelations[(int)source, (int)target] == target;
        }

        public static bool IsGeneratedBy(this ElementType target, ElementType source)
        {
            return source.Generates(target);
        }

        public static bool IsOvercomeBy(this ElementType target, ElementType source)
        {
            return source.Overcomes(target);
        }
    }
}
