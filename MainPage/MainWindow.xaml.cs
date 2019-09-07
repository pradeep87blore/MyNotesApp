using Common;
using Logic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private ObservableCollection<NotesContent> listForDataBinding = new ObservableCollection<NotesContent>();

        public ObservableCollection<NotesContent> ListForDataBinding { get => listForDataBinding; set => listForDataBinding = value; }

        public MainWindow()
        {
            //this.DataContext = this;
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            

            Logic.Initializer.InitializeHelpers(textBox_userId.Text);

            var prevNotes = NotesHandler.FetchNotes(textBox_userId.Text);

            UpdatePreviousNotes(prevNotes);
        }

        private void UpdatePreviousNotes(List<NotesContent> prevNotes)
        {
            listBox_previousNotes.ItemsSource = ListForDataBinding;

            foreach (var item in prevNotes)
            {
                ListForDataBinding.Add(item);
            }
            
        }

        private void Button_addNotes_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textBox_notesText.Text))
            {
                MessageBox.Show("Please enter some text to save to notes");
                return;
            }
            NotesContent notes = new NotesContent();
            notes.NotesText = textBox_notesText.Text;
            notes.UserId = textBox_userId.Text;

            if (ValidFileSelected())
            {
                notes.FilePath = textBox_selectedFilePath.Text;
            }

            if (NotesHandler.AddNotes(notes).Result)
            {
                Logger.AddLog("Successfully added new notes");
            }
            else
            {
                MessageBox.Show("Failed to add logs, please retry after some time");
            }
        }

        private bool ValidFileSelected()
        {
            string filePath = textBox_selectedFilePath.Text;

            return !(String.IsNullOrEmpty(filePath) || (!File.Exists(filePath))); // return false if file path is empty or if the file doesnt exist

        }

        private void Button_Click(object sender, RoutedEventArgs e) // Open file dialog to select a file
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            textBox_selectedFilePath.Text = String.Empty;
            if (openFileDialog.ShowDialog() == true)
            {
                if (IsFileSizeOK(openFileDialog.FileName))
                {
                    textBox_selectedFilePath.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("File size is too much, please select a file of size less than " + Constants.MAX_FILE_SIZE_KB.ToString() + " KB");
                    return;
                }
            }
        }

        private bool IsFileSizeOK(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            Decimal FileSize = Decimal.Divide(fileInfo.Length, 1024);

            return (FileSize <= Constants.MAX_FILE_SIZE_KB);
        }
    }
}
