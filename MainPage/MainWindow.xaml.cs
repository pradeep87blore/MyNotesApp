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
        bool bSortNewestFirst = true;

        private ObservableCollection<NotesContent> listForDataBinding = new ObservableCollection<NotesContent>();

        public ObservableCollection<NotesContent> ListForDataBinding { get => listForDataBinding; set => listForDataBinding = value; }

        NotesContent selectedNotes = null;

        LoginWindow loginWindow = null;

        TwitterUserInfo twitterUserInfo = null;

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
            PerformLogin();

            InitializeUI();

            Logic.Initializer.InitializeHelpers(textBox_userId.Text);

            var prevNotes = NotesHandler.FetchNotes(textBox_userId.Text);

            LoadPreviousNotes(prevNotes);
        }

        private void PerformLogin()
        {
            // Open this in a modal form so that the background processes are blocked till the user logs in
            loginWindow = new LoginWindow();
            var bLoginSuccess = loginWindow.ShowDialog();
            twitterUserInfo = loginWindow.GetTwitterUserInfo();

            if(twitterUserInfo == null)
            {
                var res = MessageBox.Show("Failed to login in to Twitter. Please retry", "Login Failed", MessageBoxButton.OKCancel);
                if(res == MessageBoxResult.OK)
                {
                    PerformLogin();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            Console.WriteLine("Login with Twitter successful");

            textBox_userId.Text = twitterUserInfo.Screen_name; // Update the UI with the logged in user's Twitter screen name
        }

        private void InitializeUI()
        {
            comboBox_sortOrder.IsEditable = false;
            var comboboxItem_newestFirst = new ComboBoxItem();
            comboboxItem_newestFirst.Content = Constants.NEWEST_FIRST;

            var comboboxItem_oldestFirst = new ComboBoxItem();
            comboboxItem_oldestFirst.Content = Constants.OLDEST_FIRST;

            comboBox_sortOrder.Items.Add(comboboxItem_newestFirst);
            comboBox_sortOrder.Items.Add(comboboxItem_oldestFirst);

            comboBox_sortOrder.SelectedItem = comboboxItem_newestFirst;
        }

        private void LoadPreviousNotes(List<NotesContent> prevNotes)
        {
            listBox_previousNotes.ItemsSource = ListForDataBinding;

            foreach (var notes in prevNotes)
            {
                InsertNotesIntoListbox(notes);
            }
        }

        private void Button_addNotes_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textBox_notesText.Text))
            {
                MessageBox.Show("Please enter some text to save to notes");
                return;
            }
            NotesContent notes = new NotesContent(null);
            notes.NotesText = textBox_notesText.Text;
            notes.UserId = textBox_userId.Text;
            notes.NotesTitle = textBox_notesTitle.Text;
            notes.TimeStamp = DateTime.Now.ToString();

            if (ValidFileSelected())
            {
                notes.FilePath = textBox_selectedFilePath.Text;
            }

            if (NotesHandler.AddNotes(notes).Result)
            {
                Logger.AddLog("Successfully added new notes");

                InsertNotesIntoListbox(notes);
            }
            else
            {
                MessageBox.Show("Failed to add logs, please retry after some time");
            }
        }

        private void InsertNotesIntoListbox(NotesContent notes)
        {
            if (bSortNewestFirst)
            {
                ListForDataBinding.Insert(0, notes);
            }
            else
            {
                ListForDataBinding.Add(notes);
            }
        }

        private bool ValidFileSelected()
        {
            string filePath = textBox_selectedFilePath.Text;

            return !(String.IsNullOrEmpty(filePath) || (!File.Exists(filePath))); // return false if file path is empty or if the file doesnt exist

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) // Open file dialog to select a file
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF)| *.BMP; *.JPG; *.GIF";
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

        private void ComboBox_sortOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox_sortOrder.SelectedItem;
            if(selectedItem.Content == Constants.NEWEST_FIRST)
            {
                if (bSortNewestFirst) // Already sorted by newest first order
                    return;

                bSortNewestFirst = true;

                ReverseListItems();
            }
            else
            {
                if (!bSortNewestFirst) // Already sorted by newest first order
                    return;

                bSortNewestFirst = false;

                ReverseListItems();

            }
        }

        private void ReverseListItems()
        {
            ListForDataBinding = new ObservableCollection<NotesContent>(ListForDataBinding.Reverse());

            listBox_previousNotes.ItemsSource = ListForDataBinding;
        }

        private void Button_deleteNotes(object sender, RoutedEventArgs e)
        {
           
        }

        bool bStartEditNotes = false;
        
        private void Button_updateNotes(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel imagePanel = (StackPanel)btn.Content;
            Image img = (Image)imagePanel.Children[0];

            if(bStartEditNotes == true)
            {

            }
            else
            {

            }
            
            var updated = (NotesContent)listBox_previousNotes.SelectedItem; // Fetch and store the values that were originally set
        }

        private void ListBox_previousNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedNotes = new NotesContent((NotesContent)listBox_previousNotes.SelectedItem); // Fetch and store the values that were originally set
        }

        private void Button_selectImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_selectImage_Click_1(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
        }
    }
}
