using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static GUI.GameLogicService;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>

    
    public class UnitBlock
    {
        public Rectangle Body { get; set; }
        public Label HpLabel { get; set; }
        public Unit Unit { get; set; }

        public void move(double position, double startPosition = 217)
        {
            Body.Margin = new Thickness(startPosition + position, Body.Margin.Top, Body.Margin.Right, Body.Margin.Bottom);
            HpLabel.Margin = new Thickness(startPosition + position, HpLabel.Margin.Top, HpLabel.Margin.Right, HpLabel.Margin.Bottom);
        }

        public bool getHit(double damage)
        {
            this.Unit.CurrentHealth -= damage;
            this.HpLabel.Content = Math.Round(Unit.CurrentHealth) + " / " + Unit.MaxHealth;
            return this.Unit.CurrentHealth <= 0;
        }
    }
    public partial class GameWindow : Window
    {
        double BASE_SPEED = 1000.0 / 600;
        int newUnitId;
        int tickCounter;
        double firstBaseHealth = 5000;
        double secondBaseHealth = 5000;

        public List<UnitBlock> leftUnitBlocks = new List<UnitBlock>();
        public List<UnitBlock> rightUnitBlocks = new List<UnitBlock>();

        public GameWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public void spawnUnit(UnitType unitType, int unitId, bool team)
        {
            var unit = GameLogicService.UnitFactory.GetUnit(unitType, unitId);

            var body = WPFHelperMethods.WPFHelpers.BuildUnitBody(unit,
                new Thickness(team ? Lane.Margin.Left - 100 : Lane.Margin.Left + 70 + Lane.Width, 0, 0, 160));

            var labelHeight = 250 + 40 * (team ? leftUnitBlocks.Count() : rightUnitBlocks.Count());

            var hpLabel = WPFHelperMethods.WPFHelpers.buildUnitHpLabel(unit, 
                new Thickness(team ? Lane.Margin.Left - 100 - 15: Lane.Margin.Left + 70 + Lane.Width - 15,
                0, 0, labelHeight));

            var unitBlock = new UnitBlock()
            {
                Body = body,
                HpLabel = hpLabel,
                Unit = unit
            };

            if (team)
            {
                leftUnitBlocks.Add(unitBlock);
            } else
            {
                rightUnitBlocks.Add(unitBlock);
            }

            RegisterName(body.Name, body);
            RegisterName(hpLabel.Name, hpLabel);

            GameGrid.Children.Add(hpLabel);
            GameGrid.Children.Add(body);
        }

        public void removeUnitBloc (UnitBlock unitBlock)
        {
            unitBlock.HpLabel.Visibility = Visibility.Collapsed;
            unitBlock.Body.Visibility = Visibility.Collapsed;
            UnregisterName(unitBlock.HpLabel.Name);
            UnregisterName(unitBlock.Body.Name);
            rightUnitBlocks.Remove(unitBlock);
            leftUnitBlocks.Remove(unitBlock);
        }

        public bool attackBase(bool whichBase, double damage)
        {
            if (whichBase)
            {
                firstBaseHealth -= damage;
                FirstBaseHealthLabel.Content = Math.Round(firstBaseHealth) + " / " + "5000";
                if (firstBaseHealth <= 0)
                {
                    return true;
                }
            } else
            {
                secondBaseHealth -= damage;
                SecondBaseHealthLabel.Content = Math.Round(secondBaseHealth) + " / " + "5000";
                if (secondBaseHealth <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            this.tickCounter++;

            if (tickCounter % 2 == 0)
            {
                // from left to right

            }
            else
            {
                // from right to left

            }
            leftUnitBlocks.ForEach(ub =>
            {
                if (ub.Body.Margin.Left >= Lane.Margin.Left + Lane.Width - 30)
                {
                    attackBase(false, ub.Unit.Damage);
                }
                else
                {
                    var closeRightUnitBlocks = rightUnitBlocks.Where(rub => rub.Body.Margin.Left - 50 <= ub.Body.Margin.Left + 30);
                    if (closeRightUnitBlocks.Count() == 0)
                        ub.move(BASE_SPEED * ub.Unit.Speed);
                    else
                    {
                        if (closeRightUnitBlocks.First().getHit(ub.Unit.Damage))
                        {
                            removeUnitBloc(closeRightUnitBlocks.First());
                        }
                    };
                }
            });

            rightUnitBlocks.ForEach(ub =>
            {
                if (ub.Body.Margin.Left <= Lane.Margin.Left)
                {
                    attackBase(true, ub.Unit.Damage);
                }
                else
                {
                    var closeLeftUnitBlocks = leftUnitBlocks.Where(lub => lub.Body.Margin.Left + 50 + 30 >= ub.Body.Margin.Left);
                    if (closeLeftUnitBlocks.Count() == 0)
                        ub.move(-BASE_SPEED * ub.Unit.Speed);
                    else
                    {
                        if (closeLeftUnitBlocks.First().getHit(ub.Unit.Damage))
                        {
                            removeUnitBloc(closeLeftUnitBlocks.First());
                        }
                    };
                }
            });


        }

        private void DeployWarriorButton_Click(object sender, RoutedEventArgs e)
        {
            char s = (char)12;
            App.Send(App.socket, (s.ToString() + (char)3 + (char)UnitType.Warrior).PadRight(128));
        }

        private void DeployTankButton_Click(object sender, RoutedEventArgs e)
        {
            char s = (char)12;
            App.Send(App.socket, (s.ToString() + (char)3 + (char)UnitType.Tank).PadRight(128));
        }

        private void DeployAssasinButton_Click(object sender, RoutedEventArgs e)
        {
            char s = (char)12;
            App.Send(App.socket, (s.ToString() + (char)3 + (char)UnitType.Assasin).PadRight(128));
        }

    }

}
