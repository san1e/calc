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
        private double lastResult = 0;

        public MainWindow()
        {
            InitializeComponent();
            Output.Text = "0";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isEqualPressed)
            {
                Output.Text = "0"; // Если равно было нажато, сбросить результат при нажатии цифровой кнопки
                isEqualPressed = false; // Сброс флага равно
            }

            SendToInput(((Button)sender).Content.ToString());
        }

        private void ButtonOper_Click(object sender, RoutedEventArgs e)
        {
            if (isEqualPressed)
            {
                isEqualPressed = false; // Сброс флага равно
                awaitingPowerInput = false; // Сброс флага ожидания ввода степени
                lastResult = double.Parse(Output.Text); // Сохранение текущего результата перед продолжением операции
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
            else if ("*/+-".Contains(content))
            {
                if (Output.Text.Length > 0 && char.IsDigit(Output.Text[Output.Text.Length - 1]))
                {
                    // If the last character is a digit, append the operator
                    Output.Text = $"{Output.Text}{content}";
                }
                else if (Output.Text.Length > 1 && "*/+-".Contains(Output.Text[Output.Text.Length - 1]))
                {
                    // If the last character is an operator, remove it and append the new one
                    Output.Text = $"{Output.Text.Substring(0, Output.Text.Length - 1)}{content}";
                }
            }
            else if (content == ",")
            {
                // Check if the previous character is a digit or another operator
                if (Output.Text.Length == 0 || char.IsDigit(Output.Text[Output.Text.Length - 1]) || "*/+-".Contains(Output.Text[Output.Text.Length - 1]))
                {
                    // If so, append the comma
                    Output.Text = $"{Output.Text},";
                }
            }
            else
            {
                Output.Text = $"{Output.Text}{content}";
            }

            // Reset awaitingPowerInput after adding power symbol
            awaitingPowerInput = false;

            // Reset isEqualPressed flag after adding a number or operator
            isEqualPressed = false;
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

                // Check if the last character is an operator and append zero if needed
                if ("*/+-".Contains(expression[expression.Length - 1]))
                {
                    expression += "0";
                }

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
                            lastResult = Math.Pow(baseNum, exponent);
                            Output.Text = lastResult.ToString();
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
                    lastResult = double.Parse(new DataTable().Compute(expression, null).ToString());
                    Output.Text = lastResult.ToString();
                }
            }
            catch (Exception ex)
            {
                Output.Text = "Error";
            }
        }

        private void ButtonE_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Text.Length > 0 && char.IsDigit(Output.Text[Output.Text.Length - 1]) && Output.Text != "0")
            {
                Output.Text += "+";
            }

            SendToInput("2,718");
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
        }


        private void ButtonPi_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Text.Length > 0 && char.IsDigit(Output.Text[Output.Text.Length - 1]) && Output.Text != "0")
            {
                Output.Text += "+";
            }

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
