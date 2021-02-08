using OrderReader.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace OrderReader
{
    /// <summary>
    /// A class that handles files being dropped on our applications
    /// </summary>
    public class DropFilesBehaviourExtension
    {
        /// <summary>
        /// A dependency property which can be attached to any element and will handle files being dropped onto it
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled", typeof(bool), typeof(DropFilesBehaviourExtension), new FrameworkPropertyMetadata(default(bool), OnPropChanged)
            {
                BindsTwoWayByDefault = false
            });

        public static void OnPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement fe))
                throw new InvalidOperationException();
            if ((bool)e.NewValue)
            {
                fe.AllowDrop = true;
                fe.Drop += OnDrop;
                fe.PreviewDragOver += OnPreviewDragOver;
            }
            else
            {
                fe.AllowDrop = false;
                fe.Drop -= OnDrop;
                fe.PreviewDragOver -= OnPreviewDragOver;
            }
        }

        private static void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            // NOTE: PreviewDragOver subscription is required at least when FrameworkElement is a TextBox
            // because it appears that TextBox by default prevent Drag on preview...

            // for this program, we allow a file to be dropped from Explorer
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            { e.Effects = DragDropEffects.Copy; }
            //    or this tells us if it is an Outlook attachment drop
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            { e.Effects = DragDropEffects.Copy; }
            //    or none of the above
            else
            { e.Effects = DragDropEffects.None; }

            //e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext;
            if (!(dataContext is IFilesDropped filesDropped))
            {
                if (dataContext != null)
                    Trace.TraceError($"Binding error, '{dataContext.GetType().Name}' doesn't implement '{nameof(IFilesDropped)}'.");
                return;
            }

            string[] allFiles = null;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                {
                    allFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                    filesDropped.OnFilesDropped(allFiles);
                }
                else if (e.Data.GetDataPresent("FileGroupDescriptor"))
                {
                    //
                    // the first step here is to get the filename
                    // of the attachment and
                    // build a full-path name so we can store it
                    // in the temporary folder
                    //

                    // create a string that will later contain the file path
                    string theFile;

                    // set up to obtain the FileGroupDescriptor
                    // and extract the file name
                    using (Stream theStream = (Stream)e.Data.GetData("FileGroupDescriptor"))
                    {
                        byte[] fileGroupDescriptor = new byte[512];
                        theStream.Read(fileGroupDescriptor, 0, 512);
                        // used to build the filename from the FileGroupDescriptor block
                        StringBuilder fileName = new StringBuilder("");
                        // this trick gets the filename of the passed attached file
                        for (int i = 76; fileGroupDescriptor[i] != 0; i++)
                        { fileName.Append(Convert.ToChar(fileGroupDescriptor[i])); }
                        theStream.Close();
                        string path = Settings.TempFilesPath;
                        // put the zip file into the temp directory
                        theFile = Path.Combine(path, fileName.ToString());
                    }
                    // create the full-path name

                    //
                    // Second step:  we have the file name.
                    // Now we need to get the actual raw
                    // data for the attached file and copy it to disk so we work on it.
                    //

                    // get the actual raw file into memory
                    using (MemoryStream ms = (MemoryStream)e.Data.GetData("FileContents", true))
                    {
                        // allocate enough bytes to hold the raw data
                        byte[] fileBytes = new byte[ms.Length];
                        // set starting position at first byte and read in the raw data
                        ms.Position = 0;
                        ms.Read(fileBytes, 0, (int)ms.Length);

                        // create a file and save the raw zip file to it
                        using (FileStream fs = new FileStream(theFile, FileMode.Create))
                        {
                            fs.Write(fileBytes, 0, (int)fileBytes.Length);
                            fs.Close();  // close the file
                        }
                    }

                    FileInfo tempFile = new FileInfo(theFile);

                    // always good to make sure we actually created the file
                    if (tempFile.Exists == true)
                    {
                        // create an array of string that we'll pass on for processing
                        allFiles = new string [] { tempFile.FullName };

                        // process the files
                        filesDropped.OnFilesDropped(allFiles);

                        // delete files after processing
                        // WARNING: this will need some attention later because we may want to process files asyncronously
                        // if the files are still being processed, we may want to move deleting them to the processing function
                        try
                        {
                            tempFile.Delete();
                        }
                        catch (Exception ex)
                        {
                            IoC.UI.ShowMessage(new MessageBoxDialogViewModel {
                                Title = "Error deleting a file",
                                Message = ex.Message,
                                ButtonText = "OK"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in DragDrop function: " + ex.Message);

                // don't use MessageBox here - Outlook or Explorer is waiting !
            }
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }
    }
}
