using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using OrderReader.Core.DataModels;
using OrderReader.Core.Interfaces;

namespace OrderReaderUI.DependencyProperties;

/// <summary>
/// A class that handles dragging and dropping files on the applications UI elements
/// </summary>
public class DropFilesBehaviourExtension
{
    public static INotificationService NotificationService { get; set; } = null!;

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(DropFilesBehaviourExtension), new FrameworkPropertyMetadata(default(bool), OnPropChanged)
        {
            BindsTwoWayByDefault = false
        });

    private static void OnPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement fe)
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
        // because it appears that TextBox by default prevents Drag on preview...
        
        // Check if a file is being dropped from the Explorer
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        // Or if it is an Outlook attachment
        else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            e.Effects = DragDropEffects.Copy;
        // Or none of the above
        else
            e.Effects = DragDropEffects.None;

        e.Handled = true;
    }

    private static async void OnDrop(object sender, DragEventArgs e)
    {
        var dataContext = ((FrameworkElement)sender).DataContext;

        if (dataContext is not IFilesDropped filesDropped)
        {
            if (dataContext is not null)
                Trace.TraceError($"Binding error, '{dataContext.GetType().Name}' doesn't implement '{nameof(IFilesDropped)}'");
            return;
        }

        try
        {
            string[]? allFiles;
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                // If this is a regular file drop from the Explorer
                allFiles = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (allFiles is not null)
                    filesDropped.OnFilesDropped(allFiles);
            }
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            {
                // If this is an Outlook attachment being dropped, we need to do a bit more work
                // Step 1: First we need to get the file name of the attachment and build a fill
                // path name so that we can store it in a temporary folder

                // Create a string that will later contain the file path
                string theFile;

                // Set up to obtain the FileGroupDescriptor and extract file name
                await using (var theStream = (Stream?)e.Data.GetData("FileGroupDescriptor"))
                {
                    var fileGroupDescriptor = new byte[512];
                    await theStream?.ReadAsync(fileGroupDescriptor, 0, 512)!;

                    // Used to build the file name from the FileGroupDescriptor block
                    var fileName = new StringBuilder("");
                    // This trick gets the file name of the passed attached file
                    for (var i = 76; fileGroupDescriptor[i] != 0; i++)
                    {
                        fileName.Append(Convert.ToChar(fileGroupDescriptor[i]));
                    }

                    theStream.Close();

                    var path = Settings.TempFilesPath;
                    // Create the full file path
                    theFile = Path.Combine(path, fileName.ToString());
                }

                // Step 2: Now we need to get the actual raw data for the attached file and copy
                // it into that path in the temp folder so that we can work on it

                // Get the actual raw file into memory
                using (var ms = (MemoryStream?)e.Data.GetData("FileContents", true))
                {
                    // Allocate enough bytes to hold the raw data
                    if (ms != null)
                    {
                        var fileBytes = new byte[ms.Length];

                        // Set the starting position at the first byte and read in the raw data
                        ms.Position = 0;
                        _ = ms.Read(fileBytes, 0, (int)ms.Length);

                        // Create a file and save the raw file data to it
                        await using var fs = new FileStream(theFile, FileMode.Create);
                        fs.Write(fileBytes, 0, fileBytes.Length);
                        // Make sure to close the file once saved
                        fs.Close();
                    }
                }

                var tempFile = new FileInfo(theFile);

                // Let's make sure we actually created the file
                if (!tempFile.Exists) return;
                // Create an array of strings that we'll pass on for processing
                allFiles = new[] { tempFile.FullName };

                // Process the files
                filesDropped.OnFilesDropped(allFiles);

                // Delete files after processing
                // WARNING: This will need some attention later because we may want to process files asynchronously
                // if the files are still being processed, we may want to move deleting them to the processing function
                try
                {
                    tempFile.Delete();
                }
                catch (Exception ex)
                {
                    await NotificationService.ShowMessage("Error deleting a file", ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine("Error in DragDrop function: " + ex.Message);
            // Don't use Dialog Message here because either Outlook or Explorer are waiting!
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