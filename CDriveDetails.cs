/**Author:Rayon Morgan
 * Email:rayonaray@hotmail.com
 * **/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Security;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RMPurge
{
    public class CDriveDetails
    {
        public static IEnumerable<DriveInfo> ScanDrives()
        {
            //DriveInfo array containing list of all found drives. 
            DriveInfo[] listOfDrive;
            try
            {
                listOfDrive = DriveInfo.GetDrives();
            }
            catch(IOException ie)
            {
                listOfDrive = null;
            }
            catch(UnauthorizedAccessException uae)
            {
                listOfDrive = null;
            }

            return listOfDrive;
        }//

        //method: access and return drive details
        public static Dictionary<String,String> GetDriveDetails<T>(T driveLetter)
        {
            DriveInfo driveDetails = new DriveInfo(driveLetter.ToString());
                //Dictionary: holds all data to display on drive
            Dictionary<String,String> dtnDriveDetails = new Dictionary<String,String>();
                /**dictionary list of driveInfo */
            dtnDriveDetails.Add("Drive", String.Format("Drive: {0} ", driveDetails.Name));
            try
            {
                dtnDriveDetails.Add("VolumeLabel", String.Format("Volume Label: {0}", driveDetails.VolumeLabel));               
            }
            catch(IOException e)
            {
                dtnDriveDetails.Add("VolumeLabel", String.Format("Volume Label: {0}", "Unkown"));
            }
            catch(SecurityException se)
            {
                dtnDriveDetails.Add("VolumeLabel", String.Format("Volume Label: {0}", "Access Restricted"));
            }
            catch(UnauthorizedAccessException uae)
            {
                dtnDriveDetails.Add("VolumeLabel", String.Format("Volume Label: {0}", "Access Restricted"));
            }
            //---------------
            try
            {
                dtnDriveDetails.Add("DriveType", String.Format("Drive Type: {0}", driveDetails.DriveType));
            }
            catch(IOException e)
            {
                dtnDriveDetails.Add("DriveType", String.Format("Drive Type: {0}", "Unknown"));
            }
            //----------------
            try
            {
                dtnDriveDetails.Add("DriveFormat", String.Format("File System: {0}",driveDetails.DriveFormat));
            }
            catch(IOException e)
            {
                dtnDriveDetails.Add("DriveFormat", String.Format("File System: {0}", "Unknown"));
            }
            catch(UnauthorizedAccessException uae)
            {
                dtnDriveDetails.Add("DriveFormat", String.Format("File System: {0}", "Access Restricted"));
            }
            //----------------
            try
            {
                dtnDriveDetails.Add("TotalSize", String.Format("Total Size: {0}{1}", BytesTo(driveDetails.TotalSize).First().Value, BytesTo(driveDetails.TotalSize).First().Key));
            }
            catch(IOException)
            {
                dtnDriveDetails.Add("TotalSize", String.Format("Total Size: {0}", "Unknown"));
            }
            catch(UnauthorizedAccessException uae)
            {
                dtnDriveDetails.Add("TotalSize", String.Format("Total Size: {0}", "Access Restricted"));
            }
            //-------------------
            try
            {
                dtnDriveDetails.Add("Free Space", String.Format("Free Space: {0}{1}",BytesTo(driveDetails.TotalFreeSpace).First().Value, BytesTo(driveDetails.TotalFreeSpace).First().Key));
            }
            catch(IOException)
            {
                dtnDriveDetails.Add("Free Space", String.Format("Free Space: {0}", "Unknown"));
            }
            catch(UnauthorizedAccessException uae)
            {
                dtnDriveDetails.Add("Free Space", String.Format("Free Space: {0}", "Access Restricted"));
            }
            
            return dtnDriveDetails;
        }//

        //Convert bytes to KB, MB, GB or TB: returns dictionary key value data
        public static Dictionary<string,float> BytesTo(long? inBytes)
        {
            Dictionary<string,float> bConversion = new Dictionary<string,float>();
            float runningTotal = 0.0f;
            float baseDivisor = 1024.0f;
            StringBuilder bUnit = new StringBuilder();

            if (inBytes < 0 || inBytes == null)
            {
                bConversion.Add("B", 0f); // return 0 byes if pass value null or 0
            }
            else
            {//id:1
                if (inBytes <= Convert.ToInt32(baseDivisor))
                {
                    bConversion.Add("B", (float)inBytes.Value); //return the in value as byes if below 1024 inclusive
                }
                else
                {//id:2
                    runningTotal = inBytes.GetValueOrDefault() / baseDivisor;   //convert value to KB
                    bUnit.Insert(0, "KB");
                    if(runningTotal >= Convert.ToInt32(baseDivisor)) //check if value in Kilobytes range
                    {//id:3
                        runningTotal = runningTotal / baseDivisor;  //convert to Megabyes
                        bUnit.Clear();  //clear all data from the string
                        bUnit.Insert(0, "MB");
                        if(runningTotal >= Convert.ToInt32(baseDivisor)) //Check if value in Megabytes range
                        {//id:4
                            runningTotal = runningTotal / baseDivisor;  //convert to Gigabyesbytes
                            bUnit.Clear();  //clear all data from the string
                            bUnit.Insert(0, "GB");
                            if(runningTotal >= Convert.ToInt32(baseDivisor))
                            {
                                runningTotal = runningTotal / baseDivisor;
                                bUnit.Clear();  //clear all data from the string
                                bUnit.Insert(0, "TB");
                            }
                        }//id:4
                    }//id:3
                    //set the final base unit and base value after conversion: round off value to x decimal places
                    bConversion.Add(bUnit.ToString(), (float)Math.Round((double)runningTotal,1));
                }//id:2
            }//outer else: id:1

            return bConversion;
        }//
        
            //Accepts Drive Letter and attempt to recover from List of issues
        public static void FileRecover(DirectoryInfo directoryList, ref Stack<string> subFolderList)
        {//id:0.12
           
           
            //TODO: disable all listbox selection, and buttons 

            GlobalUpdate customEvent = new GlobalUpdate(); //declare a GlobalUpdate variable: Used to update form data globally through events.

                //used to access all black less files and extensions. 
            CBlackList blackListFiles = new CBlackList();
            try
            {//id:1.1
                    //declare Directory variable with passed drive letter
                
                    //get list of of files and directories in path 
                FileSystemInfo[] ieDirectoryList = directoryList.GetFileSystemInfos();
                //FileSystemInfo[] ieDirectoryListd = Directory.EnumerateFileSystemEntries(driveLetter);
               
                    //variable use to track if it's first time a file will be processed, as to change attribute details only once
                int CountChangeFileAttribute = 1;   

                foreach(var i in ieDirectoryList)   //traverse files and directories
                {//id: if 1.2
                     /**processes file and determine if it mathces black list file: If it does change the file extension 
                      * and move file to a central location for deletion.*/
                    foreach(var blackList in blackListFiles.getBlackList()) //traverse blackList and return each value in single sequence 
                    {//id:3.3
                       
                        //TODO: processes file attribute
                        
                        //check the type of drive processing and perform accordinly
                       switch( (new DriveInfo(directoryList.Root.ToString())).DriveType.ToString() )
                       {//id:switch 8.1
                           case "Removable":
                                //check if file is hidden orystem file
                               if ((i.Attributes & FileAttributes.Hidden).ToString() == FileAttributes.Hidden.ToString() && (i.Attributes & FileAttributes.System).ToString() == FileAttributes.System.ToString())
                               {                         
                                   //TODO: occurences when system and hidden
                               }
                               else 
                               {//id:else 1.2
                                   
                                   StringBuilder inName;    //create temporary variable to hold current file name

                                  

                                   if(CountChangeFileAttribute == 1) //check if first time current file is processed
                                   {
                                       CountChangeFileAttribute++;  //increment the counter by 1

                                       //Check if file attribute is folder and add it to the subFolder Stack
                                       if ((i.Attributes & FileAttributes.Directory).ToString() == FileAttributes.Directory.ToString())
                                       {
                                           //MessageBox.Show();
                                           subFolderList.Push(i.FullName);
                                       }
                                      

                                       try
                                       {
                                           //unhide files and make them visible
                                           File.SetAttributes(i.FullName, FileAttributes.Normal);
                                       }
                                       catch(FileNotFoundException fnfe)
                                       {
                                           //MessageBox.Show(fnfe.Message);
                                       }
                                   }

                                   if (i.Name.LastIndexOf(".") > 0) //check the last full stop in text it at a location greater than 0
                                   {
                                       inName = new StringBuilder(i.Name.Remove(i.Name.LastIndexOf("."))); //remove string previous to last full stop
                                   }
                                   else
                                   {
                                       inName = new StringBuilder(i.Name);  //assign unchanged value to stringBuilder
                                   }
                                  
                                   
                                   if (inName.ToString() == blackList || i.Extension == blackList) //check if traverse file ieDirectoryList stored in inMae matches the block list
                                   {
                                       
                                       //modify suspected file and change extension
                                       File.Move(i.FullName, Path.ChangeExtension(i.FullName, ".rmpurge"));
                                       customEvent.raiseHeaderDetails(i.FullName); //                                      
                                     
                                       break;    //break the inner loop and return to the outer loop for processing
                                       
                                       //TODO: Fires event to  update fixed list
                                   }

                                   inName = null; //despose of the StringBuilder variable to free up it's memory
                                   

                               }//id:else 1.2
                           break;
                          
                       }//id:switch 8.1
                     
                    }//id3.3
                    //call method to raised processing status and give other details.
                    //customEvent.ProcessStatus(string.Format("{0} {1}", i.Name, i.Attributes));
                    customEvent.ProcessStatus(string.Format("{0}", i.Name));
                    CountChangeFileAttribute = 1; //reset filetracker counter to 1
                }//id:if 1.2
            }//id:01
            catch(IOException e)
            {
                //call method to raised processing and display exception found
                customEvent.ProcessStatus(string.Format("Error: {0}",e.Message));
                //MessageBox.Show("Error thrown here");
            }   
            
            //TODO: Raised on event here to enable all buttons and listbox
        }//id:0.12
    }
}
