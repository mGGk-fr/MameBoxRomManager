using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MameBoxRomManager
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Database db;
        public ObservableCollection<Game> Games { get; set; }
        private bool firstStart = true;
        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            this.DataContext = this;
            Games = new ObservableCollection<Game>();
            Games = db.fillGameList();
            this.tb_fullsetDirectory.Text = db.getSetting("fullsetDir");
            this.tb_mameboxDir.Text = db.getSetting("mameboxDir");
            this.tb_listXMLFile.Text = db.getSetting("xmlFileDir");
            pg_main.Maximum = Games.Count;
            pg_main.Value = 0;


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

        private void parseXMLFile(string xmlDirectory)
        {

        }

        //UI Functions
        //Select fullset directory
        private void btn_browseFullset_Click(object sender, RoutedEventArgs e)
        {
            tb_fullsetDirectory.Text = openFolderBrowser();
            this.db.setSetting("fullsetDir", tb_fullsetDirectory.Text);
        }

        //Select mamebox directory
        private void btn_browseMameBox_Click(object sender, RoutedEventArgs e)
        {
            tb_mameboxDir.Text = openFolderBrowser();
            this.db.setSetting("mameboxDir", tb_mameboxDir.Text);
        }

        //Select XML File
        private void btn_browselistXML_Click(object sender, RoutedEventArgs e)
        {
            tb_listXMLFile.Text = openFileBrowser();
            this.db.setSetting("xmlFileDir", tb_listXMLFile.Text);
        }

        //Building database from XML
        private void btn_buildDB_Click(object sender, RoutedEventArgs e)
        {
            string fileName = tb_listXMLFile.Text;
            string fullsetDir = tb_fullsetDirectory.Text;
            MessageBox.Show("Building database is quiet long, click ok and wait for the bar to be full");
            disableButton();
            Thread thread = new Thread(() => buildDB(fileName, fullsetDir));
            thread.Start();
        }

        //Building XML DB
        private void buildDB(string xmlDir, string fullSetDir)
        {
            string zipName;
            string gameName;
            XmlDocument doc = new XmlDocument();
            if (!File.Exists(xmlDir))
            {
                MessageBox.Show("XML File does not exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            db.executeQuery("DELETE FROM games");
            doc.Load(xmlDir);
            XmlNodeList elemList = doc.GetElementsByTagName("machine");
            foreach (XmlNode node in elemList)
            {
                zipName = node.Attributes["name"].InnerText;
                gameName = node["description"].InnerText;
                if (File.Exists(fullSetDir + "\\" + zipName + ".zip"))
                {
                    db.addGame(zipName, gameName);
                }
                this.Dispatcher.Invoke(() => {
                    pg_tool.Maximum = elemList.Count;
                    pg_tool.Value+=1;
                });
            }
            MessageBox.Show("Build success ! Please restart the application !");
            this.Dispatcher.Invoke(() => {
                btn_buildDB.IsEnabled = true;
                btn_updateArcadeBox.IsEnabled = true;
                btn_SaveAndSync.IsEnabled = true;
            });
        }

        //Button handler for syncing with mamebox
        private void btn_updateArcadeBox_Click(object sender, RoutedEventArgs e)
        {
            string mameBoxDir = tb_mameboxDir.Text;
            disableButton();
            Thread thread = new Thread(() => buildDBWithMameBox(mameBoxDir));
            thread.Start();
        }

        //Building mamebox list
        private void buildDBWithMameBox(string arcadeBoxDir)
        {
            string[] fileEntries = Directory.GetFiles(arcadeBoxDir);
            string zipFile = "";
            int fileCount = Directory.GetFiles(arcadeBoxDir, "*.zip", SearchOption.TopDirectoryOnly).Length;
            db.executeQuery("UPDATE games SET inMamebox = 0");
            foreach (string fileName in fileEntries)
            {
                zipFile = System.IO.Path.GetFileNameWithoutExtension(fileName);
                db.updateGameEntry(zipFile, 1);
                this.Dispatcher.Invoke(() => {
                    pg_tool.Maximum = fileCount;
                    pg_tool.Value += 1;
                });
            }
            MessageBox.Show("Completed, please restart the application.");
            this.Dispatcher.Invoke(() => {
                btn_buildDB.IsEnabled = true;
                btn_updateArcadeBox.IsEnabled = true;
                btn_SaveAndSync.IsEnabled = true;
            });

        }

        //Disable buttons
        private void disableButton()
        {
            btn_buildDB.IsEnabled = false;
            btn_updateArcadeBox.IsEnabled = false;
            btn_SaveAndSync.IsEnabled = false;
        }

        private void tb_Search_KeyUp(object sender, KeyEventArgs e)
        {
            var filtered = Games.Where(Game => Game.GameName.StartsWith(tb_Search.Text,true,null));
            dg_main.ItemsSource = filtered;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!firstStart)
            {
                var cb = (CheckBox)e.OriginalSource;
                Game dataCxtx = (Game)cb.DataContext;
                if (dataCxtx.InMameBox)
                {
                    this.db.updateGameEntry(dataCxtx.ZipFile, 1);
                }
                else
                {
                    this.db.updateGameEntry(dataCxtx.ZipFile, 0);
                }
                
            }
            
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!firstStart)
            {
                var cb = (CheckBox)e.OriginalSource;
                Game dataCxtx = (Game)cb.DataContext;
                if (dataCxtx.InMameBox)
                {
                    this.db.updateGameEntry(dataCxtx.ZipFile, 1);
                }
                else
                {
                    this.db.updateGameEntry(dataCxtx.ZipFile, 0);
                }
            }
        }

        private void dg_main_Loaded(object sender, RoutedEventArgs e)
        {
            firstStart = false;
        }

        private void btn_SaveAndSync_Click(object sender, RoutedEventArgs e)
        {
            disableButton();
            string fsd = tb_fullsetDirectory.Text;
            string mbdir = tb_mameboxDir.Text;
            pg_main.Value = 0;
            Thread thread = new Thread(() => syncMamebox(fsd, mbdir));
            thread.Start();

        }

        private void syncMamebox(string fullsetDir, string mameboxDir)
        {
            foreach (Game cg in Games)
            {
                if (cg.InMameBox)
                {
                    if (!File.Exists(mameboxDir + "\\"+cg.ZipFile+".zip"))
                    {
                        File.Copy(fullsetDir + "\\" + cg.ZipFile + ".zip", mameboxDir + "\\" + cg.ZipFile + ".zip");
                    }
                }
                else
                {
                    if (File.Exists(mameboxDir + "\\" + cg.ZipFile + ".zip"))
                    {
                        File.Delete(mameboxDir + "\\" + cg.ZipFile + ".zip");
                    }
                }
                this.Dispatcher.Invoke(() => {
                    pg_main.Value += 1;
                });
            }
            MessageBox.Show("Sync OK !");
            this.Dispatcher.Invoke(() => {
                btn_buildDB.IsEnabled = true;
                btn_updateArcadeBox.IsEnabled = true;
                btn_SaveAndSync.IsEnabled = true;
            });
        }
    }
}
