using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Splat.ModeDetection;

namespace Avalonia_Monogame_Dock_Template.Services.ContentGenerator
{
    public class DynamicUserControlGeneratorService
    {
        /// <summary>
        /// Vygeneruje UserControl, ktorý obsahuje ovládacie prvky pre všetky vlastnosti modelu,
        /// ktoré sú označené atribútom DynamicInputAttribute.
        /// </summary>
        /// <param name="model">Inštancia modelu, z ktorého sa majú prečítať vlastnosti.</param>
        /// <returns>UserControl s dynamicky vygenerovaným obsahom.</returns>
        public UserControl GenerateUserControlForModel(object model)
        {
            // Hlavný kontajner – StackPanel, v ktorom budú vložené jednotlivé riadky.
            var wrapPanel = new WrapPanel
            {
                Margin = Thickness.Parse("15"),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            };

            // Získame vlastnosti modelu, ktoré majú atribút DynamicInputAttribute.
            var properties = model.GetType().GetProperties()
                                  .Where(prop => Attribute.IsDefined(prop, typeof(DynamicInputAttribute)));

            foreach (var prop in properties)
            {
                var attribute = prop.GetCustomAttribute<DynamicInputAttribute>();

                // Vytvoríme kontajner pre jeden riadok – použijeme Grid so 2 stĺpcami (Label a input).
                var stackPanel = new StackPanel()
                {
                    Width = 210,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Vytvoríme Label s názvom vlastnosti.
                var label = new TextBlock
                {
                    Text = prop.Name,
                    TextAlignment = Avalonia.Media.TextAlignment.Start,
                    FontSize =  10,
                    Opacity = 0.5,
                    Width = 200,
                    Margin = new Thickness(5)
                };

                Control inputControl = null;

                // Podľa typu vlastnosti vytvoríme príslušný ovládací prvok.
                if (prop.PropertyType == typeof(string))
                {
                    if (attribute.IsReadonly)
                    {
                        if (attribute.Binding is not null)
                        {
                            var textBox = new TextBlock
                            {
                                FontSize = 11,
                                Width = 200,
                                Margin = new Thickness(5),
                            };

                            // Vytvoríme TwoWay binding na vlastnosť modelu.
                            textBox.Bind(TextBox.TextProperty, new Binding(attribute.Binding)
                            {
                                Mode = BindingMode.TwoWay
                            });
                            inputControl = textBox;
                        }
                        else
                        {
                            var textBox = new TextBlock
                            {
                                FontSize = 10,
                                Width = 200,
                                Margin = new Thickness(5),
                                DataContext = model  // Nastavíme DataContext na model pre zjednodušenie bindingu.
                                                     // Prípadne môžete nastaviť Watermark ako informáciu o min/max dĺžke.
                            };

                            // Vytvoríme TwoWay binding na vlastnosť modelu.
                            textBox.Bind(TextBox.TextProperty, new Binding(prop.Name)
                            {
                                Mode = BindingMode.TwoWay
                            });
                            inputControl = textBox;
                        }
                    }
                    else
                    {
                        if (attribute.Binding is not null)
                        {
                            var textBox = new TextBox
                            {
                                FontSize = 11,
                                Width = 200,
                                Margin = new Thickness(5),
                            };

                            // Vytvoríme TwoWay binding na vlastnosť modelu.
                            textBox.Bind(TextBox.TextProperty, new Binding(attribute.Binding)
                            {
                                Mode = BindingMode.TwoWay
                            });
                            inputControl = textBox;
                        }
                        else
                        {
                            var textBox = new TextBox
                            {
                                FontSize = 10,
                                Width = 200,
                                Margin = new Thickness(5),
                                DataContext = model  // Nastavíme DataContext na model pre zjednodušenie bindingu.
                                                     // Prípadne môžete nastaviť Watermark ako informáciu o min/max dĺžke.
                            };

                            // Vytvoríme TwoWay binding na vlastnosť modelu.
                            textBox.Bind(TextBox.TextProperty, new Binding(prop.Name)
                            {
                                Mode = BindingMode.TwoWay
                            });
                            inputControl = textBox;
                        }
                    }
                }
                else if (prop.PropertyType == typeof(int) ||
                         prop.PropertyType == typeof(float) ||
                         prop.PropertyType == typeof(double))
                {
                    // Použijeme NumericUpDown – predpokladáme, že ho máte k dispozícii.
                    var numericUpDown = new NumericUpDown
                    {
                        Margin = new Thickness(5),
                        FontSize = 10,
                        Width = 200,
                        Minimum = (decimal)attribute.Min,
                        Maximum = (decimal)attribute.Max,
                        DataContext = model  // Nastavíme DataContext na model pre zjednodušenie bindingu.
                    };

                    // Vytvoríme TwoWay binding na vlastnosť modelu.
                    numericUpDown.Bind(NumericUpDown.ValueProperty, new Binding(prop.Name)
                    {
                        Mode = BindingMode.TwoWay
                    });
                    inputControl = numericUpDown;
                }
                else
                {
                    // Iné typy v tomto príklade nepodporujeme – preskočíme ich.
                    continue;
                }

                // Pridáme Label a ovládací prvok do gridu.
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(inputControl);

                // Pridáme grid (riadok) do hlavného stackPanelu.
                wrapPanel.Children.Add(stackPanel);
            }

            // Vytvoríme UserControl a nastavíme jeho Content na náš dynamicky vygenerovaný stackPanel.
            var userControl = new UserControl
            {
                Content = wrapPanel
            };

            return userControl;
        }
    }
}
