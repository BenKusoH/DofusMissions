using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DofusMissions
{
    public partial class Icono : Window
    {
        private Window misiones;
        public Icono()
        {
            InitializeComponent();
            // Accessing the Properties.Settings object
            double iconTop = Settings.Default.IconTop;
            double iconLeft = Settings.Default.IconLeft;

            // Use these values to set your window's position and size, if needed
            Left = iconLeft;
            Top = iconTop;
        }

        public void show(Window misiones)
        {
            this.misiones = misiones;
            this.Show();
        }

        private bool isDragging = false;

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                this.DragMove();
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
            {
                this.misiones.Show();
                this.Hide();
            } else
            {
                isDragging = false;
            }

            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.misiones != null)
            {
                this.misiones.Close();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.IconLeft = Left;
            Settings.Default.IconTop = Top;
            Settings.Default.Save();
        }
    }
}