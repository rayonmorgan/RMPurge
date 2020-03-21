using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
 

namespace RMPurge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //initialize  window and components
            InitializeComponent();

            Loaded += MainWindow_Loaded;
                //Subscribe to the NotifyProcessTrace event.
            GlobalUpdate.NotifyProcessTrace += this.OnNotifyProcessTraceSub;
                //subscribe to the evtHeaderDetails event
            GlobalUpdate.evtHeaderDetails += this.OnUpdateHeaderDetails;


            //Instruction on How to scan
             
           
        }

        //called after MainWindow loads. 
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillDrives();           
        }

        // populate drive listbox with drive details
        public void FillDrives()
        {
                //get list of drives by calling static method ScanDrives
            IEnumerable<DriveInfo> lDrives = CDriveDetails.ScanDrives();
            //clear listbox if it has current data
            if (lsbDriveName.HasItems)
            {
                lsbDriveName.Items.Clear();                
            }

            //add heading to list box which displays available drives
            lsbDriveName.Items.Add(new TextBlock().Text="Drive List");
            foreach (var iEListDrives in lDrives)
            {
                //add each drive name to the listBox
                lsbDriveName.Items.Add(new ListBoxItem().Content = iEListDrives.Name);
            }  
        }

        //called when selection changed 
        private void lsbDriveName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//id: 1

            Dictionary<String, String> driveDetails = new Dictionary<string,string>();

            if (itcDriveDetails.HasItems) //check if any items is currently in the Itemcontrol
            {
                itcDriveDetails.Items.Clear(); //clear current list items  
                itcProcessTrace.Items.Clear(); //clear Item Content control that displays processing data
                                 
            }

                //get list of Drive information 
            if (lsbDriveName.HasItems)
            {
                //get a list of drives available on the current system.
                 driveDetails = CDriveDetails.GetDriveDetails<String>(lsbDriveName.SelectedValue.ToString());
                 
                foreach (var i in driveDetails) //traverse dictionary and store each traversed items in i
                 {
                     itcDriveDetails.Items.Add(i.Value);
                 }
            }
            //CDriveDetails.FileRecover(lsbDriveName.SelectedValue.ToString());         
            
        }//id:1
        
        // 
        private void Button_Click(object sender, RoutedEventArgs e)
        {//id:0.1
            FillDrives();   
        }//id:0.1

        //Attempt to Recover files hidden by the shortcut virus
        private void BtnFix_Click(object sender, RoutedEventArgs e)
        {
            if(lsbDriveName.SelectedIndex >= 0) //check if an item was selected
            {//id:if 2.3
                itcProcessTrace.Items.Clear(); //clear Item Content control that displays processing data
                Stack<string> subFoldersList = new Stack<string>(); //contains stack list of subfolders empty initially
                   
                    //declared variable gets directory listing to the drive selected initially
                DirectoryInfo directoryList = new DirectoryInfo(lsbDriveName.SelectedValue.ToString());
               

                CDriveDetails.FileRecover(directoryList,ref subFoldersList);   //make initial call to method to recover files pass drive letter and an empyy stack
                do
                {
                    if (subFoldersList.Count > 0) //check if the sub directory stack has data from previous call
                    {                                               
                       //recall the FileRecover Method passing the stack: subFolderList and popping the top item and using it as the root directory
                        CDriveDetails.FileRecover((new DirectoryInfo(subFoldersList.Pop())),ref subFoldersList);
                      
                    }
                } while (subFoldersList.Count > 0); //process loop until the stack becomes zero or less
                    
            }//id:if 2.3
        }

        //event to update display of running process.
        public void OnNotifyProcessTraceSub(object source, NotifyArgs args)
        {//id:method 2.2


            TextBlock txblContent = new TextBlock();
            txblContent.Text = string.Format("\n{0}", args.outValue + "...");
            txblContent.TextWrapping = TextWrapping.Wrap;

            if (itcProcessTrace.HasItems) //check if the the display panel contains any current items
            {           
                           
                if(itcProcessTrace.Items.Count%2 == 0)
                {
                    //txblContent.Background = Brushes.Aquamarine;
                }
                else{
                    txblContent.Background = Brushes.FloralWhite;
                }

                itcProcessTrace.Items.Add(txblContent);
            
            }
            else
            {
                TextBox txbProcessDiplay = new TextBox();    //create textBox object
                txbProcessDiplay.Text = string.Format("\t--- Files Found ---\n\n");
                //txbProcessDiplay.Text = txbProcessDiplay.Text + args.outValue;
                txbProcessDiplay.TextWrapping = TextWrapping.Wrap;
                txbProcessDiplay.Name = "displayProcess";
                
                itcProcessTrace.Items.Add(txbProcessDiplay);               
                
                txblContent.Background = Brushes.FloralWhite;
               
                itcProcessTrace.Items.Add(txblContent);

            }           
        }//end: id:method 2.2

        public void OnUpdateHeaderDetails(object sender,HeaderDetailsArgs e)
        {
           
            ListBoxItem lsbiHeaderDetails = new ListBoxItem();  //create a listBoxItem
            lsbiHeaderDetails.Content = e.outValue; // set the listBoxItem Content
             
            
        }

   

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
        
    }//MainWindow Class
}//namespace
