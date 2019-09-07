using Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainPage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Logic.Initializer.InitializeHelpers();
        }

        private void Button_addNotes_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(textBox_notesText.Text))
            {
                MessageBox.Show("Please enter some text to save to notes");
                return;
            }
            NotesContent notes = new NotesContent();            
            notes._notesText = textBox_notesText.Text;
            notes._userId = textBox_userId.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Open file dialog to select a file
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (IsFileSizeOK(openFileDialog.FileName))
                {
                    textBox_selectedFilePath.Text = openFileDialog.FileName;
                }
                else
                {

                }
            }
        }

        private bool IsFileSizeOK(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
