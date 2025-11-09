using System;

namespace FiveElements.Shared.Models
{
    public class PlayerElementStats
    {
        public ElementType MainElement { get; set; }
        
        // Current values and max limits for each element
        public int MetalValue { get; set; } = 0;
        public int MetalMax { get; set; } = 100;
        
        public int WoodValue { get; set; } = 0;
        public int WoodMax { get; set; } = 100;
        
        public int WaterValue { get; set; } = 0;
        public int WaterMax { get; set; } = 100;
        
        public int FireValue { get; set; } = 0;
        public int FireMax { get; set; } = 100;
        
        public int EarthValue { get; set; } = 0;
        public int EarthMax { get; set; } = 100;

        public int GetValue(ElementType element)
        {
            return element switch
            {
                ElementType.Metal => MetalValue,
                ElementType.Wood => WoodValue,
                ElementType.Water => WaterValue,
                ElementType.Fire => FireValue,
                ElementType.Earth => EarthValue,
                _ => 0
            };
        }

        public void SetValue(ElementType element, int value)
        {
            switch (element)
            {
                case ElementType.Metal:
                    MetalValue = Math.Max(0, Math.Min(value, MetalMax));
                    break;
                case ElementType.Wood:
                    WoodValue = Math.Max(0, Math.Min(value, WoodMax));
                    break;
                case ElementType.Water:
                    WaterValue = Math.Max(0, Math.Min(value, WaterMax));
                    break;
                case ElementType.Fire:
                    FireValue = Math.Max(0, Math.Min(value, FireMax));
                    break;
                case ElementType.Earth:
                    EarthValue = Math.Max(0, Math.Min(value, EarthMax));
                    break;
            }
        }

        public int GetMax(ElementType element)
        {
            return element switch
            {
                ElementType.Metal => MetalMax,
                ElementType.Wood => WoodMax,
                ElementType.Water => WaterMax,
                ElementType.Fire => FireMax,
                ElementType.Earth => EarthMax,
                _ => 100
            };
        }

        public void SetMax(ElementType element, int max)
        {
            switch (element)
            {
                case ElementType.Metal:
                    MetalMax = Math.Max(1, max);
                    MetalValue = Math.Min(MetalValue, MetalMax);
                    break;
                case ElementType.Wood:
                    WoodMax = Math.Max(1, max);
                    WoodValue = Math.Min(WoodValue, WoodMax);
                    break;
                case ElementType.Water:
                    WaterMax = Math.Max(1, max);
                    WaterValue = Math.Min(WaterValue, WaterMax);
                    break;
                case ElementType.Fire:
                    FireMax = Math.Max(1, max);
                    FireValue = Math.Min(FireValue, FireMax);
                    break;
                case ElementType.Earth:
                    EarthMax = Math.Max(1, max);
                    EarthValue = Math.Min(EarthValue, EarthMax);
                    break;
            }
        }

        public bool CanConsume(ElementType element, int amount)
        {
            return GetValue(element) >= amount;
        }

        public bool Consume(ElementType element, int amount)
        {
            if (!CanConsume(element, amount)) return false;
            
            SetValue(element, GetValue(element) - amount);
            return true;
        }

        public void AddValue(ElementType element, int amount)
        {
            SetValue(element, GetValue(element) + amount);
        }
    }
}