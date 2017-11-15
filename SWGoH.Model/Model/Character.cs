using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Character
    {
        public string Name { get; set; }
        public string SWGoHUrl { get; set; }
        public int Stars { get; set; }
        public int Gear { get; set; }
        public int Level { get; set; }
        public int Power { get; set; }
        public int StatPower { get; set; }
        public List<Ability> Abilities { get; set; }
        public GeneralStats GeneralStats { get; set; }
        public OffenseStats OffenseStats { get; set; }    
        public Survivability Survivability { get; set; }        
        public List<Mod> Mods { get; set; }
    }
    public class Ability
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
    }
    public class GeneralStats
    {
        public int Health { get; set; }
        public int Protection { get; set; }
        public int Speed { get; set; }
        public int CriticalDamage { get; set; }
        public double Potency { get; set; }
        public double Tenacity { get; set; }
        public int HealthSteal { get; set; }
    }
    public class OffenseStats
    {
        public PhysicalOffense PhysicalOffense { get; set; }
        public SpecialOffense SpecialOffense { get; set; }
    }
    public class PhysicalOffense
    {        
        public int PhysicalDamage { get; set; }
        public double PhysicalCriticalChance { get; set; }
        public int ArmorPenetration { get; set; }
        public int PhysicalAccuracy { get; set; }     
    }
    public class SpecialOffense
    {        
        public int SpecialDamage { get; set; }
        public double SpecialCriticalChance { get; set; }
        public int ResistancePenetration { get; set; }
        public double SpecialAccuracy { get; set; }     
    }
    public class Survivability
    {
        public PhysicalSurvivability PhysicalSurvivability { get; set; }
        public SpecialSurvivability SpecialSurvivability { get; set; }
    }
    public class PhysicalSurvivability
    {    
        public double Armor { get; set; }
        public double DodgeChance { get; set; }
        public double PhysicalCriticalAvoidance { get; set; }     
    }
    public class SpecialSurvivability
    {     
        public double Resistance { get; set; }
        public double DeflectionChance { get; set; }
        public double SpecialCriticalAvoidance { get; set; }     
    }
}

