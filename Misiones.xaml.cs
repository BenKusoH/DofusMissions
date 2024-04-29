using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;


namespace DofusMissions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class BooleanToImageSourceConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? new BitmapImage(new Uri("pack://application:,,,/seguir.png", UriKind.Absolute)) : new BitmapImage(new Uri("pack://application:,,,/no_seguir.png", UriKind.Absolute));
            }
            return new BitmapImage(new Uri("pack://application:,,,/no_seguir.png", UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    public class BooleanToColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Brushes.Gray : Brushes.White;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    public class BooleanToFontStyleConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? FontStyles.Italic : FontStyles.Normal;
            }
            return FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    public partial class Misiones : Window
    {
        private Icono icono;
        public ObservableCollection<TaskItem> Tasks { get; set; } = new ObservableCollection<TaskItem>();

        public Misiones()
        {
            InitializeComponent();

            this.icono = new Icono();
            this.icono.Hide();

            // Accessing the Properties.Settings object
            double missionsWindowLeft = Settings.Default.MissionsWindowLeft;
            double missionsWindowTop = Settings.Default.MissionsWindowTop;
            double missionsWindowHeight = Settings.Default.MissionsWindowHeight;
            double missionsWindowWidth = Settings.Default.MissionsWindowWidth;

            // Use these values to set your window's position and size, if needed
            Left = missionsWindowLeft;
            Top = missionsWindowTop;
            Width = missionsWindowWidth;
            Height = missionsWindowHeight;

            // Deserialize the JSON string to ItemsControl items
            var missionsJson = Settings.Default.LastMissions;
            if (!string.IsNullOrEmpty(missionsJson))
            {
                Tasks = JsonConvert.DeserializeObject<ObservableCollection<TaskItem>>(missionsJson);
            }
            if (Tasks.Count < 1)
            {
                Tasks.Add(new TaskItem());
            }

                notasList.ItemsSource = Tasks;

        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragWindow(sender, e);
        }

        private void icono_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            TaskItem task = (TaskItem)rect.DataContext;
            task.ToggleStyle();
            task.IsFollowing = !task.IsFollowing;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Tasks.Add(new TaskItem());
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            TaskItem task = item.DataContext as TaskItem;
            Tasks.Remove(task);
            if (Tasks.Count < 1)
            {
                Tasks.Add(new TaskItem());
            }
        }

        private void minimizar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            icono.show(this);
            this.Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.icono.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.MissionsWindowLeft = Left;
            Settings.Default.MissionsWindowTop = Top;
            Settings.Default.MissionsWindowWidth = Width;
            Settings.Default.MissionsWindowHeight = Height;

            // Serialize the ItemsControl items to JSON
            var missionsJson = JsonConvert.SerializeObject(Tasks);
            Settings.Default.LastMissions = missionsJson;
            Settings.Default.Save();
        }

        private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
        {


        }

        private void richTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
            {
                richTextBox.Focus();
                richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
            }
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }


        private void richTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent the RichTextBox from handling the Enter key
                if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                {
                    AddButton_Click(sender, e);
                }
                else
                {
                    Paragraph currentParagraph = richTextBox.CaretPosition.Paragraph;
                    FlowDocument flowDocument = richTextBox.Document;
                    Paragraph newParagraph = new Paragraph();
                    flowDocument.Blocks.InsertAfter(currentParagraph, newParagraph);
                    richTextBox.CaretPosition = newParagraph.ContentStart;
                }
            }

            if (e.Key == Key.Back)
            {
                
                var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

                if (range.Text == "\r\n")
                {
                    // Get the index of the current item
                    var item = richTextBox.DataContext;
                    int currentIndex = notasList.Items.IndexOf(item);

                    // Focus the previous item
                    if (currentIndex > 0)
                    {
                        var container = notasList.ItemContainerGenerator.ContainerFromIndex(currentIndex - 1) as FrameworkElement;
                        if (container != null)
                        {
                            var richTextBoxInPreviousItem = FindVisualChild<RichTextBox>(container);
                            if (richTextBoxInPreviousItem != null)
                            {
                                richTextBoxInPreviousItem.Focus();
                                richTextBoxInPreviousItem.CaretPosition = richTextBoxInPreviousItem.Document.ContentEnd;
                            }
                        }
                    }

                    // Delete the task
                    if (item != null)
                    {
                        if (Tasks.Count > 1)
                            Tasks.Remove((TaskItem)item); // Assuming Tasks is an ObservableCollection<Task>
                        e.Handled = true;
                    }
                }
            }
        }
    }



    public class TaskItem : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        private string text = "";
        public string Text
        {
            get { return text; }
            set { text = value; NotifyPropertyChanged("Text"); }
        }

        private bool isItalic = false;
        public bool IsItalic
        {
            get { return isItalic; }
            set { isItalic = value; NotifyPropertyChanged("IsItalic"); }
        }

        private Brush textColor = Brushes.White;
        public Brush TextColor
        {
            get { return textColor; }
            set { textColor = value; NotifyPropertyChanged("TextColor"); }
        }

        public void ToggleStyle()
        {
            IsItalic = !IsItalic;
            TextColor = IsItalic ? Brushes.Gray : Brushes.White;
        }



        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool isFollowing = false;
        public bool IsFollowing
        {
            get { return isFollowing; }
            set { isFollowing = value; NotifyPropertyChanged("IsFollowing"); }
        }

    }
}