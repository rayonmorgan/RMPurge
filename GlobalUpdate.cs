using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
 

namespace RMPurge
{
    //class containing properties and functions used to update controls data from anywhere in the program.
    //Use to handle all custom events across the application domain. 
   public class GlobalUpdate
    {//id:2.2
        //event handler delegate for process tracing output
       public delegate void StartProcessTraceEventHandler(object sender,NotifyArgs e);
       // event handler delegate for header details
       public delegate void dlgHeaderDetails(object sender,HeaderDetailsArgs e);
       //public delegate void (object sender, EventArgs e);

       //create event using the delegate event handler StartProcessTrace
       public static event StartProcessTraceEventHandler NotifyProcessTrace;  
       //create  header details event
       public static event dlgHeaderDetails evtHeaderDetails;

       //virtual method to reaise event for process
       protected virtual void OnHeaderDetails(string inValue)
       {
           if (evtHeaderDetails != null) //check if any method is subscribing to the event
           {
               evtHeaderDetails(this, new HeaderDetailsArgs() { outValue = inValue }); //raise the event 
           }
       }

       //call the virtual method OnHeaderDetails to raise the event
       public void raiseHeaderDetails(string inValue)
       {
           OnHeaderDetails(inValue);   //call method to raise event
       }
       

       //create OnEvent Method to raise event for the process output
       protected virtual void OnNotifyProcessTrace(string inValue)
       {
           if (NotifyProcessTrace != null) //check if any method is subscribing to this event
               NotifyProcessTrace(this, new NotifyArgs() { outValue = inValue });    //public the event
       }

       //Provide status updates, and invoke event to alert subscribers. 
       public void ProcessStatus(object invalue)
       {
          // MessageBox.Show("Rasing event");
           OnNotifyProcessTrace((String)invalue); //Raised the NotifyProcessTrace Event
       }
    }//id:2.2
}
