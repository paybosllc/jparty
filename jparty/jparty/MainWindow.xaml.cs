
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace jparty
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Color m_blueish = Color.FromArgb(0xFF, 0x40, 0x40, 0xFF);
        private Color m_yellowish = Color.FromArgb(0xFF, 0xFC, 0xAE, 0x1E);
        private readonly AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            m_AppWindow = GetAppWindowForCurrentWindow();

            SetupTitleBar();

            Application.Current.Resources["ToggleButtonBackgroundChecked"] = new SolidColorBrush(m_yellowish);
            Application.Current.Resources["ToggleButtonBackgroundCheckedPointerOver"] = new SolidColorBrush(m_yellowish);
            Application.Current.Resources["ToggleButtonBackgroundCheckedPressed"] = new SolidColorBrush(m_yellowish);

            Grid jpartyBoard = GenerateGrid();
            LayoutRoot.Children.Add(jpartyBoard);
        }

        private void SetupTitleBar()
        {
            m_AppWindow.Title = "JPARTY";
            m_AppWindow.TitleBar.ForegroundColor = m_yellowish;
            m_AppWindow.TitleBar.BackgroundColor = m_blueish;
            m_AppWindow.TitleBar.ButtonForegroundColor = m_yellowish;
            m_AppWindow.TitleBar.ButtonBackgroundColor = m_blueish;
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        public Grid GenerateGrid() // int rows, int cols, int[,] map)
        {
            int scoreA = 0;
            int scoreB = 0;

            int rows = 7;
            int cols = 5;
            string[] titles = new string[] { "10-Letter Words", "The First Christmas", "Around The World Christmas", "Christmas Carols", "Holiday Hollywood" };


            Grid grid = new();
            grid.Background = new SolidColorBrush(m_blueish);

            // 1.Prepare RowDefinitions
            for (int i = 0; i < rows + 1; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                grid.RowDefinitions.Add(row);
            }

            // 2.Prepare ColumnDefinitions
            for (int j = 0; j < cols; j++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(column);
            }

            // set the titles
            for (int j = 0; j < cols; j++)
            {
                TextBlock textBlock = new()
                {
                    Text = titles[j],
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 36,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.OldLace),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                grid.Children.Add(textBlock);
                Grid.SetColumn(textBlock, j);
                Grid.SetRow(textBlock, 0); // Set row too!
            }

            // 3.Add each item and set row and column.
            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Button button = new()
                    {
                        Content = (i * 200).ToString(),
                        FontSize = 60,
                        FontWeight = FontWeights.Bold,
                        BorderThickness = new Thickness(0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 0xFC, 0xAE, 0x1E)),
                        Background = new SolidColorBrush(Color.FromArgb(255, 64, 64, 255))
                    };

                    grid.Children.Add(button);
                    Grid.SetColumn(button, j);
                    Grid.SetRow(button, i); // Set row too!

                    StackPanel stackPanel = new()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Orientation = Orientation.Horizontal,
                        Visibility = Visibility.Collapsed
                    };

                    ToggleButton tbA = new()
                    {
                        Content = "A",
                        FontSize = 40,
                        FontWeight = FontWeights.Bold,
                        Tag = i,
                        Width = 80,
                        Margin = new Thickness(4)
                    };

                    ToggleButton tbB = new()
                    {
                        Content = "B",
                        FontSize = 40,
                        FontWeight = FontWeights.Bold,
                        Tag = i,
                        Width = 80,
                        Margin = new Thickness(4),
                    };

                    stackPanel.Children.Add(tbA);
                    stackPanel.Children.Add(tbB);

                    grid.Children.Add(stackPanel);
                    Grid.SetColumn(stackPanel, j);
                    Grid.SetRow(stackPanel, i); // Set row too!

                    button.Click += (e, args) =>
                    {
                        button.Visibility = Visibility.Collapsed;
                        stackPanel.Visibility = Visibility.Visible;
                    };

                    tbA.Click += (e, args) =>
                    {
                        ToggleButton toggle = e as ToggleButton;
                        int score = int.Parse(tbA.Tag.ToString()) * 200;
                        scoreA += (toggle.IsChecked == true) ? score : -score;

                        TextBlock textblockA = FindChild<TextBlock>(grid, "TextBlockScoreA");
                        if (textblockA != null)
                        {
                            textblockA.Text = scoreA.ToString();
                        }

                        tbB.IsEnabled = !(toggle.IsChecked == true);
                    };

                    tbB.Click += (e, args) =>
                    {
                        ToggleButton toggle = e as ToggleButton;
                        int score = int.Parse(tbB.Tag.ToString()) * 200;
                        scoreB += (toggle.IsChecked == true) ? score : -score;

                        TextBlock textblockB = FindChild<TextBlock>(grid, "TextBlockScoreB");
                        if (textblockB != null)
                        {
                            textblockB.Text = scoreB.ToString();
                        }

                        tbA.IsEnabled = !(toggle.IsChecked == true);
                    };
                }
            }

            TextBlock textBlockATitle = new()
            {
                Text = "Score A",
                FontSize = 60,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.OldLace),
            };

            Grid.SetColumn(textBlockATitle, 0);
            Grid.SetRow(textBlockATitle, rows); // Set row too!

            TextBlock textBlockA = new()
            {
                Text = scoreA.ToString(),
                FontSize = 60,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.OldLace),
                Name = "TextBlockScoreA"
            };

            Grid.SetColumn(textBlockA, 1);
            Grid.SetRow(textBlockA, rows); // Set row too!

            TextBlock textBlockBTitle = new()
            {
                Text = "Score B",
                FontSize = 60,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.OldLace),
            };

            Grid.SetColumn(textBlockBTitle, cols - 2);
            Grid.SetRow(textBlockBTitle, rows); // Set row too!

            TextBlock textBlockB = new()
            {
                Text = scoreB.ToString(),
                FontSize = 60,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.OldLace),
                Name = "TextBlockScoreB"
            };

            Grid.SetColumn(textBlockB, cols - 1);
            Grid.SetRow(textBlockB, rows); // Set row too!

            grid.Children.Add(textBlockATitle);
            grid.Children.Add(textBlockBTitle);

            grid.Children.Add(textBlockA);
            grid.Children.Add(textBlockB);

            return grid;
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
        where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }

                    // Need this in case the element we want is nested
                    // in another element of the same type
                    foundChild = FindChild<T>(child, childName);
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}




