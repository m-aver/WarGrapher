using System.Collections.Generic;
using System.Windows.Media.Imaging;
using WarGrapher.Common;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents a weapon
    /// </summary>
    public class Weapon : EquipItem
    {
        /// <summary>
        /// Gets the starting damage of a weapon
        /// </summary>
        public double MaxDamage { get; }
        /// <summary>
        /// Gets the final damage of a weapon  
        /// </summary>
        public double MinDamage { get; }
        /// <summary>
        /// Gets the initial distance of damage fall of a weapon
        /// </summary>
        public double Distance { get; }
        /// <summary>
        /// Gets the damage reduction rate represented as the amount of damage per meter
        /// </summary>
        public double DropDamagePerMeter { get; }
        /// <summary>
        /// Get the rate of fire represented as the number of shots per minute
        /// </summary>
        public double RPM { get; }

        /// <summary>
        /// Gets the head damage factor of a weapon
        /// </summary>
        public double HeadDamageFactor { get; }
        /// <summary>
        /// Gets the body damage factor of a weapon
        /// </summary>
        public double BodyDamageFactor { get; }
        /// <summary>
        /// Gets the arms damage factor of a weapon
        /// </summary>
        public double ArmsDamageFactor { get; }
        /// <summary>
        /// Gets the legs damage factor of a weapon
        /// </summary>
        public double LegsDamageFactor { get; }

        public Weapon(string name, BitmapImage icon, Dictionary<string, double> paramBase)
            : base(name, icon, paramBase)
        {
            MaxDamage = GetParam("damage") ?? 0;
            MinDamage = GetParam("mindamage") ?? 0;
            Distance = GetParam("dist") ?? 0;
            DropDamagePerMeter = GetParam("drop") ?? 0;
            RPM = GetParam("rpm") ?? 0;

            HeadDamageFactor = GetParam("headX") ?? 1;
            BodyDamageFactor = GetParam("bodyX") ?? 1;
            ArmsDamageFactor = GetParam("handX") ?? 1;
            LegsDamageFactor = GetParam("legX") ?? 1;
        }

        /// <summary>
        /// Gets the damage factor for a hitbox part represented by the passed constant of the <see cref="BodyPart"/> enumeration  
        /// </summary>
        public double GetBodyPartFactor(BodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case BodyPart.Arms:
                    return ArmsDamageFactor;
                case BodyPart.Body:
                    return BodyDamageFactor;
                case BodyPart.Head:
                    return HeadDamageFactor;
                case BodyPart.Legs:
                    return LegsDamageFactor;
                default:
                    return 1;
            }
        }
    }
}
