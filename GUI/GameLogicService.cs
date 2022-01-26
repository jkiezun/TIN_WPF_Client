using System;
using System.Collections.Generic;
using System.Text;

namespace GUI
{
    public class GameLogicService
    {

        public class Unit
        {
            public int Id { get; set; }

            public double CurrentHealth { get; set; }
            public int MaxHealth { get; set; }
            public int Damage { get; set; }
            public int Speed { get; set; }
            public int AttackSpeed { get; set; }
            public UnitType UnitType { get; set; }
        }

        public enum UnitType
        {
            Warrior = 2,
            Tank = 3,
            Assasin = 1
        }

        public static class UnitFactory
        {
            public static Unit GetUnit(UnitType unitType, int Id)
            {
                switch (unitType)
                {
                    case UnitType.Assasin:
                        {
                            return new Unit()
                            {
                                Id = Id,
                                Damage = 4,
                                CurrentHealth = 150,
                                MaxHealth = 150,
                                Speed = 5,
                                AttackSpeed = 1,
                                UnitType = UnitType.Assasin
                            };

                        }
                    case UnitType.Warrior:
                        {
                            return new Unit()
                            {
                                Id = Id,
                                Damage = 2,
                                CurrentHealth = 300,
                                MaxHealth = 300,
                                Speed = 3,
                                AttackSpeed = 1,
                                UnitType = UnitType.Warrior
                            };
                        }
                    case UnitType.Tank:
                        {
                            return new Unit()
                            {
                                Id = Id,
                                Damage = 1,
                                CurrentHealth = 600,
                                MaxHealth = 600,
                                Speed = 2,
                                AttackSpeed = 1,
                                UnitType = UnitType.Tank
                            };
                        }
                    default:
                        return null;
                }
            }
        }

    }
}
   



  
