using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Common.AzureBlobUtility.Models;
using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.AzureBlobUtility.Helper
{
    public class AzureBlobHelper
    {
        private BlobContainerClient _blobContainerClient;
        private BlobContainerClient _blobAdditionalContainerClient;
        private BlobServiceClient _blobServiceClient;
        private string _containerName;

        #region Constructor

        public AzureBlobHelper(string accountName, string accountKey, string containerName, string additionalContainerName = null)
        {
            var builder = new StringBuilder();

            builder.Append("DefaultEndpointsProtocol=https;");
            builder.Append("AccountName=" + accountName + ";");
            builder.Append("AccountKey=" + accountKey + ";");
            builder.Append("EndpointSuffix=core.windows.net");

            var storageConnStr = builder.ToString();

            _blobContainerClient = new BlobContainerClient(storageConnStr, containerName);
            if (!string.IsNullOrEmpty(additionalContainerName))
            {
                _blobAdditionalContainerClient = new BlobContainerClient(storageConnStr, additionalContainerName);
            }
            _blobServiceClient = new BlobServiceClient(storageConnStr);
            _containerName = containerName;
        }

        #endregion

        #region Upload Blob to Azure Storage Container

        public void UploadBlobToAzureStorageContainer(string image, string filePath, ref string fileName, ImageProperties imageProperties = null)
        {
            var blobName = !filePath.Contains("/")
                         ? string.Join(string.Empty, filePath, image.GetBase64Extension())
                         : filePath;

            // Get response for Container if not exists
            var createResponse = _blobContainerClient.CreateIfNotExists();

            // Checking Container already created
            if (createResponse != null &&
                createResponse.GetRawResponse().Status == 201)
            {
                // Setting Private Access to created Container
                _blobContainerClient.SetAccessPolicy(PublicAccessType.None);
            }

            // Get BlobClient by BlobName
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            // Delete BlobItem
            DeleteBlobItem(filePath);

            // Upload BlobItem
            UploadBlobItem(image, blobClient, imageProperties);

            fileName = blobName;
        }

        public void UploadBlobToAzureStorageContainer(string filePath, int rotateAngle, ref string base64String)
        {
            // Get BlobItem from Azure Storage Container based on BlobName
            var blobItem = _blobContainerClient.GetBlobs(BlobTraits.None, BlobStates.None, filePath).FirstOrDefault();

            // Checkng whether BlobItem exists
            if (blobItem != null)
            {
                // Get BlobClient by BlobName
                var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);

                // Get BlobContent Array
                var blobContentArray = GetBlobContentArray(blobClient);

                // Get BlobFile Extension
                var fileExtn = GetBlobFileExtension(blobClient);

                // Get RotatedImage Data
                var byteData = GetRotatedImageData(blobContentArray, fileExtn, rotateAngle);

                // Get Base64 String
                base64String = GetBase64String(fileExtn, byteData);

                // Get ImageProperties MetaData
                var imageProps = GetMetaDatas(blobClient, false, true);

                // Upload BlobItem
                UploadBlobItem(base64String, blobClient, imageProps);
            }
        }

        #endregion

        #region Get Blob from Azure Storage Container

        public void GetBlobFromAzureStorageContainer(string filePath, ref string base64String, out ImageProperties imageProps, bool isWebApp = false)
        {
            var builder = new StringBuilder();

            // Get BlobItem from AzureStorageContainer based on BlobName
            var blobItem = _blobContainerClient.GetBlobs(BlobTraits.None, BlobStates.None, filePath).FirstOrDefault();

            // Checkng whether BlobItem exists
            if (blobItem != null)
            {
                // Get BlobClient by BlobName
                var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);

                // Get BlobFile Extension
                var fileExtn = GetBlobFileExtension(blobClient);

                // Get BlobContent Array
                var blobContentArray = GetBlobContentArray(blobClient);

                // Get Base64 String
                if (isWebApp)
                {
                    base64String = GetBase64String(fileExtn, blobContentArray, isWebApp);
                }
                else
                {
                    base64String = GetBase64String(fileExtn, blobContentArray);
                }

                // Get ImageProperties MetaData
                imageProps = GetMetaDatas(blobClient, isWebApp);
            }
            else
            {
                base64String = "";

                imageProps = null;
            }
        }

        #endregion

        #region Get Blob from Additional Azure Storage Container

        public void GetBlobFromAzureStorageAdditionalContainer(string filePath, ref string base64String, out ImageProperties imageProps, bool isWebApp = false, bool isMetaDataRequired = true)
        {
            var builder = new StringBuilder();

            // Get BlobItem from  Additional AzureStorageContainer based on BlobName
            var blobItem = _blobAdditionalContainerClient.GetBlobs(BlobTraits.None, BlobStates.None, filePath).FirstOrDefault();

            // Checkng whether BlobItem exists
            if (blobItem != null)
            {
                // Get BlobClient by BlobName
                var blobClient = _blobAdditionalContainerClient.GetBlobClient(blobItem.Name);

                // Get BlobFile Extension
                var fileExtn = GetBlobFileExtension(blobClient);

                // Get BlobContent Array
                var blobContentArray = GetBlobContentArray(blobClient);

                // Get Base64 String
                if (isWebApp)
                {
                    base64String = GetBase64String(fileExtn, blobContentArray, isWebApp);
                }
                else
                {
                    base64String = GetBase64String(fileExtn, blobContentArray);
                }

                imageProps = null;

                if (isMetaDataRequired)
                {
                    // Get ImageProperties MetaData
                    imageProps = GetMetaDatas(blobClient, isWebApp);
                }
            }
            else
            {
                base64String = "";

                imageProps = null;
            }
        }

        #endregion

        #region Transfer Blob in Azure Storage Container

        public void TransferBlobsInAzureStorageContainer(long inspectionId,
                                                         long sourceImageId, string sourceFilePath,
                                                         long destinationImageId, string destinationFilePath,
                                                         out string sourceBase64String, out string destinationBase64String,
                                                         out string sourceNewFilePath, out string destinationNewFilePath)
        {
            sourceBase64String = string.Empty;
            destinationBase64String = string.Empty;
            sourceNewFilePath = string.Empty;
            destinationNewFilePath = string.Empty;

            if (!string.IsNullOrEmpty(sourceFilePath) &&
                !string.IsNullOrEmpty(destinationFilePath))
            {
                // Get BlobClient by BlobName
                BlobClient sourceblobClient = _blobContainerClient.GetBlobClient(sourceFilePath);
                BlobClient destinationBlobClient = _blobContainerClient.GetBlobClient(destinationFilePath);

                // Get BlobFile Extension
                string sourceFileExtn = GetBlobFileExtension(sourceblobClient);
                string destinationFileExtn = GetBlobFileExtension(destinationBlobClient);

                // Get BlobContent Array
                byte[] sourceBlobContentArray = GetBlobContentArray(sourceblobClient);
                byte[] destinationBlobContentArray = GetBlobContentArray(destinationBlobClient);

                // Get Base64 String
                sourceBase64String = GetBase64String(sourceFileExtn, sourceBlobContentArray);
                destinationBase64String = GetBase64String(destinationFileExtn, destinationBlobContentArray);

                // Get ImageProperties MetaData
                ImageProperties sourceImageProperties = GetMetaDatas(sourceblobClient, false, true);
                ImageProperties destinationImageProperties = GetMetaDatas(destinationBlobClient, false, true);

                // Upload BlobItem
                if (!string.IsNullOrEmpty(sourceFilePath))
                {
                    var destinationFileName = string.Empty;
                    var destinationFileInfo = sourceFilePath.Split("/")[2];
                    destinationNewFilePath = string.Join("/", inspectionId, destinationImageId, destinationFileInfo);
                    UploadBlobToAzureStorageContainer(sourceBase64String, destinationNewFilePath, ref destinationFileName, sourceImageProperties);
                }
                if (!string.IsNullOrEmpty(destinationFilePath))
                {
                    var sourceFileName = string.Empty;
                    var sourceFileInfo = destinationFilePath.Split("/")[2];
                    sourceNewFilePath = string.Join("/", inspectionId, sourceImageId, sourceFileInfo);
                    UploadBlobToAzureStorageContainer(destinationBase64String, sourceNewFilePath, ref sourceFileName, destinationImageProperties);
                }

                // Delete BlobItem
                DeleteBlobItem(sourceFilePath);
                DeleteBlobItem(destinationFilePath);
            }
            else if (!string.IsNullOrEmpty(sourceFilePath) &&
                     string.IsNullOrEmpty(destinationFilePath))
            {
                // Get BlobClient by BlobName
                BlobClient sourceblobClient = _blobContainerClient.GetBlobClient(sourceFilePath);

                // Get BlobFile Extension
                string sourceFileExtn = GetBlobFileExtension(sourceblobClient);

                // Get BlobContent Array
                byte[] sourceBlobContentArray = GetBlobContentArray(sourceblobClient);

                // Get Base64 String
                sourceBase64String = GetBase64String(sourceFileExtn, sourceBlobContentArray);

                // Get ImageProperties MetaData
                ImageProperties sourceImageProperties = GetMetaDatas(sourceblobClient);

                // Upload BlobItem
                if (!string.IsNullOrEmpty(sourceFilePath))
                {
                    var fileInfo = sourceFilePath.Split("/")[2];
                    destinationNewFilePath = string.Join("/", inspectionId, destinationImageId, fileInfo);
                    var fileName = string.Empty;
                    UploadBlobToAzureStorageContainer(sourceBase64String, destinationNewFilePath, ref fileName, sourceImageProperties);
                }

                // Delete BlobItem
                DeleteBlobItem(sourceFilePath);
            }
        }

        #endregion

        #region Upload File to Azure Storage Container

        public void UploadFileToAzureStorageContainer(string filePath, byte[] fileContents)
        {
            // Get response for Container if not exists
            var createResponse = _blobContainerClient.CreateIfNotExists();

            // Checking Container already created
            if (createResponse != null &&
                createResponse.GetRawResponse().Status == 201)
            {
                // Setting Private Access to created Container
                _blobContainerClient.SetAccessPolicy(PublicAccessType.None);
            }

            // Get BlobClient by BlobName
            var blobClient = _blobContainerClient.GetBlobClient(filePath);

            // Delete BlobItem
            DeleteBlobItem(filePath);

            // Upload BlobItem
            UploadBlobItem(blobClient, fileContents);
        }

        #endregion

        #region Delete Blobs in Azure Storage Container

        public async void DeleteBlobsInAzureStorageContainer(long inspectionId)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobItems = blobContainerClient.GetBlobsAsync(prefix: inspectionId.ToString());
            await foreach (BlobItem blobItem in blobItems)
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteIfExistsAsync();
            }
        }

        #endregion

        #region Private Methods

        #region Delete BlobItem

        private void DeleteBlobItem(string filePath)
        {
            // Get BlobItem from AzureStorageContainer by filePath
            var blobItem = _blobContainerClient.GetBlobs(BlobTraits.None, BlobStates.None, filePath).FirstOrDefault();

            // Checking whether Blob Item exists
            if (blobItem != null)
            {
                // Delete Blob if already exists
                _blobContainerClient.DeleteBlobIfExists(blobItem.Name);
            }
        }

        #endregion

        #region Upload BlobItem

        private void UploadBlobItem(string base64Image, BlobClient blobClient, ImageProperties imageProperties = null)
        {
            var base64String = string.Empty;

            // Get EncodedImage as StringArray
            var encodedImage = base64Image.Split(',');

            // Checking StringArray length greater than 1
            if (encodedImage.Length > 1)
            {
                // Get Base64 String
                base64String = encodedImage[1];
            }

            // Get DecodedImage as ByteArray
            var decodedImage = Convert.FromBase64String(base64String);

            // Create MemoryStream for DecodedImage
            using (var fileStream = new MemoryStream(decodedImage))
            {
                // Upload Blob with specific content type
                blobClient.Upload(fileStream, true);

                // Checking ImageProperties exists
                if (imageProperties != null)
                {
                    // Add ImageProperties to MetaData
                    blobClient.SetMetadata(AddMetaDatas(imageProperties));
                }
            }
        }


        private void UploadBlobItem(BlobClient blobClient, byte[] fileContents)
        {
            // Create MemoryStream for File
            using (var fileStream = new MemoryStream(fileContents))
            {
                // Upload Blob with specific content type
                blobClient.Upload(fileStream, true);
            }
        }

        #endregion

        #region Get Base64 String

        private string GetBase64String(string fileExtn, byte[] byteData, bool isWebApp = false)
        {
            var builder = new StringBuilder();

            builder.Clear();
            if (!isWebApp)
            {
                builder.Append("data:image/");
                builder.Append(fileExtn);
                builder.Append(";base64,");
            }
            builder.Append(Convert.ToBase64String(byteData));

            return builder.ToString();
        }

        #endregion

        #region Get BlobFile Extension

        private string GetBlobFileExtension(BlobClient blobClient)
        {
            string fileExtn = string.Empty;

            if (blobClient.Uri.Segments.Length > 0)
            {
                // Get Blob File Extension
                var fileContents = blobClient.Uri.Segments[blobClient.Uri.Segments.Length - 1].Split(".");
                fileExtn = fileContents[1];
            }

            return fileExtn;

            //string fileExtn = string.Empty;

            //// Get FileName of Uploaded Blob
            //var fileName = blobClient.Uri.Segments.Length < 4
            //             ? blobClient.Uri.Segments[2].Split(".")
            //             : blobClient.Uri.Segments[4].Split(".");

            //// Checking StringArray length greater than 1
            //if (fileName.Length > 1)
            //{
            //    // Get Blob File Extension
            //    fileExtn = fileName[1];
            //}

            //return fileExtn;
        }

        #endregion

        #region Get BlobContent Array

        private byte[] GetBlobContentArray(BlobClient blobClient)
        {
            // Get DownloadResponse of Blob
            var blobDownloadResponse = blobClient.DownloadContent();

            // Get BlobContent Array
            var blobContentArray = blobDownloadResponse.Value.Content.ToArray();

            return blobContentArray;
        }

        #endregion

        #region Add ImageProperties MetaDatas

        private Dictionary<string, string> AddMetaDatas(ImageProperties imageProperties)
        {
            var metaDatas = new Dictionary<string, string>();

            // Get CapturedTime
            //var capturedTime = DateTime.UtcNow.ToString("dd/MM/yyyyTHH:mm:ss").Replace("-", "/");

            // Adding MetaDatas
            metaDatas.Add("Lattitude", imageProperties.Lattitude);
            metaDatas.Add("Longitude", imageProperties.Longitude);
            metaDatas.Add("CapturedTime", imageProperties.CapturedTime);
            //metaDatas.Add("CapturedTime", capturedTime);
            metaDatas.Add("IsFakeLocation", imageProperties.IsFakeLocation.ToString());
            metaDatas.Add("Dimension", imageProperties.Dimension);
            metaDatas.Add("Address", imageProperties.Address);

            return metaDatas;
        }

        private string GetAESTDate(string date, bool? ImageRotate = null)
        {
            DateTime localDate;
            if (DateTime.TryParseExact(date, "dd-MM-yyyy hh:mm:ss tt", null, DateTimeStyles.None, out localDate))
            {
                return date;
            }
            //Image Rotate == null ==> Conversion happen for showing date in clientside
            //ImageRotate !=null => Get the data without any conversion (save in cloud meta data)
            if (ImageRotate == null)
            {
                DateTime.TryParseExact(date, "dd/MM/yyyyTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out localDate);
                TimeSpan t = new TimeSpan(0, 11, 00, 0);
                localDate = localDate.Add(t);
                var dateLocal = localDate.ToString("dd-MM-yyyy hh:mm:ss tt");
                return dateLocal.ToString();
            }
            return date;
        }

        #endregion

        #region Get ImageProperties MetaDatas

        private ImageProperties GetMetaDatas(BlobClient blobClient, bool isWebApp = false, bool isRaw = false)
        {
            var imageProperties = new ImageProperties();

            // Get ImageProperties
            var properties = blobClient.GetProperties().Value;

            foreach (var item in properties.Metadata)
            {
                switch (item.Key)
                {
                    // Get Lattitue value
                    case "Lattitude":
                        imageProperties.Lattitude = item.Value;
                        break;

                    // Get Longitude Value
                    case "Longitude":
                        imageProperties.Longitude = item.Value;
                        break;

                    // Get CapturedTime Value
                    case "CapturedTime":
                        if (isRaw)
                        {
                            imageProperties.CapturedTime = item.Value;
                        }
                        else
                        {
                            var capturedTime = item.Value.Replace("T", " ").Split(" ");
                            var resultCapturedTime = "";
                            if (capturedTime.Length > 0)
                            {
                                var date = string.Join("-", capturedTime[0].Split('/').Reverse());
                                var time = capturedTime[1];
                                resultCapturedTime = string.Join("", date, "T", time);
                            }
                            imageProperties.CapturedTime = !isWebApp
                                                         ? resultCapturedTime
                                                         : item.Value;
                        }
                        break;

                    // Get IsFakeLocation Value
                    case "IsFakeLocation":
                        imageProperties.IsFakeLocation = bool.Parse(item.Value);
                        break;

                    // Get Dimension Value
                    case "Dimension":
                        imageProperties.Dimension = item.Value;
                        break;

                    // Get Address Value
                    case "Address":
                        if (isRaw)
                        {
                            imageProperties.Address = item.Value;
                        }
                        else
                        {
                            Span<byte> buffer = new Span<byte>(new byte[item.Value.Length]);
                            var isEncode = Convert.TryFromBase64String(item.Value, buffer, out int _);

                            if (isEncode)
                            {
                                var base64EncodedBytes = System.Convert.FromBase64String(item.Value);
                                imageProperties.Address = Encoding.UTF8.GetString(base64EncodedBytes);
                            }
                            else
                            {
                                imageProperties.Address = item.Value;
                            }
                        }
                        break;
                }
            }

            return imageProperties;
        }

        #endregion

        #region Get RotatedImage Data

        private byte[] GetRotatedImageData(byte[] imageDownloadBytes, string fileExtn, int rotateFlip)
        {
            Image img = (Bitmap)((new ImageConverter()).ConvertFrom(imageDownloadBytes));

            var rot = RotateFlipType.RotateNoneFlipNone;

            if (rotateFlip == 90 || rotateFlip == -270)
            {
                rot = RotateFlipType.Rotate90FlipNone;
            }
            if (rotateFlip == 180 || rotateFlip == -180)
            {
                rot = RotateFlipType.Rotate180FlipNone;
            }
            if (rotateFlip == 270 || rotateFlip == -90)
            {
                rot = RotateFlipType.Rotate270FlipNone;
            }
            if (rotateFlip == 360 || rotateFlip == 0 || rotateFlip == -360)
            {
                rot = RotateFlipType.RotateNoneFlipNone;
            }

            img.RotateFlip(rot);

            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);

                return ms.ToArray();
            }
        }

        #endregion

        #endregion
    }
}
