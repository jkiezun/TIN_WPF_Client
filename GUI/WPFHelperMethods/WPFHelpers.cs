using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static GUI.GameLogicService;

namespace GUI.WPFHelperMethods
{
     static class WPFHelpers
    {
        public static Window GetWindowByName(Application app, string name)
        {
            var allWindows = Application.Current.Windows;
            Window foundWindow = null;
            foreach (var window in allWindows)
            {
                Window win = window as Window;
                if (win.Name == name)
                {
                    return win;
                };
            }
            return null;
        }

        public static Rectangle BuildUnitBody(Unit unit, Thickness position) {
            switch (unit.UnitType)
            {
                case UnitType.Warrior:
                    {
                        return new Rectangle {
                            Width = 30, 
                Height = 30, Margin = position, 
                Name = "unit" + unit.Id.ToString(), HorizontalAlignment = HorizontalAlignment.Left, 
                Fill = new SolidColorBrush(System.Windows.Media.Colors.Red)
            };

                    }
                case UnitType.Tank:
                    {
                        return new Rectangle
                        {
                            Width = 30,
                            Height = 20,
                            Margin = position,
                            Name = "unit" + unit.Id.ToString(),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(System.Windows.Media.Colors.Silver)
                        };
                    }
                case UnitType.Assasin:
                    {
                        return new Rectangle
                        {
                            Width = 30,
                            Height = 40,
                            Margin = position,
                            Name = "unit" + unit.Id.ToString(),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Fill = new SolidColorBrush(System.Windows.Media.Colors.Blue)
                        };
                    }
                default:
                    return null;
            }
        }

        public static Label buildUnitHpLabel (Unit unit, Thickness position)
        {
            return new Label()
            {
                Width = 80,
                Height  = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                Name = "unit" + unit.Id.ToString() + "HpLabel",
                Margin = position,
                Content = unit.CurrentHealth + " / " + unit.MaxHealth
            };
        }
    }
}
