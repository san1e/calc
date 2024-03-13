using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FluentWpfChromes;
using NCalc;

namespace calc
{
    public partial class MainWindow : Window
    {
        private bool awaitingPowerInput = false;
        private bool isEqualPressed = false;
        public MainWindow()
        {
            InitializeComponent();
            Output.Text = "0";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isEqualPressed)
            {
                Output.Text = ""; // Очищаем поле вывода, если нажато равно в предыдущем вычислении
                isEqualPressed = false; // Сбрасываем флаг
            }
            SendToInput(((Button)sender).Content.ToString());
        }

        private void SendToInput(string content)
        {
            if (Output.Text == "0")
            {
                Output.Text = "";
            }

            // Check for operator before adding "^"
            if (content == "^" && (Output.Text.Length == 0 || !char.IsDigit(Output.Text[Output.Text.Length - 1]) && !"*/+-".Contains(Output.Text[Output.Text.Length - 1])))
            {
                Output.Text = $"{Output.Text}0";
            }

            Output.Text = $"{Output.Text}{content}";
        }
        private void BtnCE_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Text.Length > 0)
            {
                int lastIndex = Output.Text.LastIndexOfAny(new char[] { '+', '-', '*', '/' });
                if (lastIndex >= 0)
                {
                    Output.Text = Output.Text.Substring(0, lastIndex + 1);
                }
                else
                {
                    Output.Text = "0";
                }
            }
        }
        private void MenuIm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AdditionalPanel.Visibility == Visibility.Collapsed)
            {
                ColumnDefinition newColumn = new ColumnDefinition();
                AdditionalPanel.Visibility = Visibility.Visible;
                Main.ColumnDefinitions.Add(newColumn);
                Width = 370;
                Height = 450;
            }
            else
            {
                if (Main.ColumnDefinitions.Count > 4)
                    Main.ColumnDefinitions.RemoveAt(4);
                AdditionalPanel.Visibility = Visibility.Collapsed;
                Width = 300;
                Height = 450;
            }
        }
        private void BtnC_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "0";
        }
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Text.Length > 0)
            {
                Output.Text = Output.Text.Substring(0, Output.Text.Length - 1);
            }
        }
        private void BtnRes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isEqualPressed = true; // Устанавливаем флаг при нажатии равно
                string expression = Output.Text.Replace(',', '.').Replace('x', '*').Replace('÷', '/');
                if (expression.Contains("^"))
                {
                    string[] parts = expression.Split('^');
                    if (parts.Length == 2)
                    {
                        double baseNum;
                        double exponent;
                        if (double.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out baseNum) &&
                            double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out exponent))
                        {
                            double result = Math.Pow(baseNum, exponent);
                            Output.Text = result.ToString();
                        }
                        else
                        {
                            Output.Text = "Error";
                        }
                    }
                    else
                    {
                        Output.Text = "Error";
                    }
                }
                else
                {
                    string result = new DataTable().Compute(expression, null).ToString();
                    Output.Text = result;
                }
            }
            catch (Exception ex)
            {
                Output.Text = "Error";
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                double number = double.Parse(Output.Text);
                double squareRoot = Math.Sqrt(number);
                Output.Text = squareRoot.ToString();
            }
            catch (Exception ex)
            {
                Output.Text = "Error";
            }
        }
        private void ButtonPow_Click(object sender, RoutedEventArgs e) 
        {
            SendToInput("^");
            awaitingPowerInput = true;
        }

        private void ButtonPi_Click(object sender, RoutedEventArgs e)
        {
            SendToInput("3,14");
        }

        private void ButtonLn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double num = double.Parse(Output.Text);
                double result = Math.Log(num);
                Output.Text = result.ToString();
            }
            catch (Exception ex)
            {
                Output.Text = "Error";
            }
        }
    }
}
