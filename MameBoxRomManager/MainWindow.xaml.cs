using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MameBoxRomManager
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Utilities functions
        //Open the folder browser
        private string openFolderBrowser(string titleBar = "Please select a folder", string defaultFolder = @"C:\")
        {
            string returnFolder = "";
            var dlg = new CommonOpenFileDialog();
            dlg.Title = titleBar;
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = defaultFolder;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = defaultFolder;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                return folder;
            }
            return returnFolder;
        }

        //Open the file browser
        private string openFileBrowser(string titleBar = "Please select a folder", string defaultFolder = @"C:\")
        {
            string returnFolder = "";
            var dlg = new CommonOpenFileDialog();
            dlg.Title = titleBar;
            dlg.IsFolderPicker = false;
            dlg.InitialDirectory = defaultFolder;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = defaultFolder;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                return folder;
            }
            return returnFolder;
        }

        //UI Functions
        //Select fullset directory
        private void btn_browseFullset_Click(object sender, RoutedEventArgs e)
        {
            tb_fullsetDirectory.Text = openFolderBrowser();
        }

        //Select mamebox directory
        private void btn_browseMameBox_Click(object sender, RoutedEventArgs e)
        {
            tb_mameboxDir.Text = openFolderBrowser();
        }
    }
}
