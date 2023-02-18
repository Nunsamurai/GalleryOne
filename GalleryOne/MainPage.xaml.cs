using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;


namespace GalleryOne
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            boxGallery.SelectionChanged += BoxGallery_SelectionChanged;

            // Register the DataRequested event handler
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        

        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
{
    // Get the selected image
    Image selectedImage = (Image)boxGallery.SelectedItem;
    if (selectedImage == null) return;

    // Create a DataPackage object and set the image as a thumbnail and a bitmap or a stream
    DataPackage dataPackage = args.Request.Data;
    dataPackage.Properties.Title = "Shared image";
    dataPackage.Properties.Description = "An image shared from GalleryOne app";
}

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a file picker object and set its properties
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");

            // Show the file picker and wait for the user to select a file
            StorageFile file = await filePicker.PickSingleFileAsync();

            // If the user selected a file, display it in the app
            if (file != null)
            {
                // Open a stream for the selected file
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    // Set the image source to the selected file stream
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);

                    // Add the image to the gallery
                    Image image = new Image();
                    image.Source = bitmapImage;
                    boxGallery.Items.Add(image);
                }
            }
        }



        private void BoxGallery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item in the ListBox
            Image selectedImage = (Image)boxGallery.SelectedItem;

            if (selectedImage != null)
            {
                // Set the source of the displayImage control to the selected image
                displayImage.Source = selectedImage.Source;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected image
            Image selectedImage = (Image)boxGallery.SelectedItem;
            if (selectedImage == null) return;

            // Create a file save picker object and set its properties
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("JPEG Image", new List<string>() { ".jpg" });
            savePicker.FileTypeChoices.Add("PNG Image", new List<string>() { ".png" });
            savePicker.SuggestedFileName = "image";

            // sow the file save picker and wait for the user to select a file
            StorageFile file = await savePicker.PickSaveFileAsync();

            // If the user selected a file, save the image to it
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapImage bitmapImage = (BitmapImage)selectedImage.Source;
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmapImage.PixelWidth, (uint)bitmapImage.PixelHeight, 96, 96, ConvertPixelData(bitmapImage));
                    await encoder.FlushAsync();
                }
            }
        }

        private byte[] ConvertPixelData(BitmapImage bitmapImage)
{
    byte[] pixelData;
    WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
    using (Stream stream = writeableBitmap.PixelBuffer.AsStream())
    {
        pixelData = new byte[stream.Length];
        stream.Read(pixelData, 0, pixelData.Length);
    }
    return pixelData;
}




        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Image image = (Image)button.DataContext;
            boxGallery.Items.Remove(image);
        }
        


    }
}










    




