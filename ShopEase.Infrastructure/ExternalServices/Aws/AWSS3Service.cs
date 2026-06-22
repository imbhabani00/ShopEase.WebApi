using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.ExternalServices.Aws
{
    public interface IAWSS3Service
    {
        Task<string> GetDocumentUrl(string filepath, bool isPublic = false);
        Task<string> UploadDocumentAsync(Stream fileStream, string fileName, string folder = "uploads");
        Task<bool> DeleteDocumentAsync(string filepath);
        Task<List<string>> GetAllDocumentsAsync(string folder = "uploads");
    }
    public class AWSS3Service
    {
        private readonly string _awsAccessKey;
        private readonly string _awsSecretKey;
        private readonly string _awsBucketName;
        private readonly string _awsRegion;
        private readonly int _signedUrlExpiresIn;

        public AWSS3Service(IConfiguration configuration)
        {
            _awsAccessKey = configuration["AwsSettings:AWSPrivateAccessKey"];
            _awsSecretKey = configuration["AwsSettings:AWSPrivateSecretKey"];
            _awsBucketName = configuration["AwsSettings:AWSBucketName"];
            _awsRegion = configuration["AwsSettings:S3EndpointRegion"];
            _signedUrlExpiresIn = int.TryParse(configuration["AwsSettings:AWSSignedURLExpiresIn"], out var exp)
                                    ? exp : 240;
        }

        #region GetDocumentUrl
        public async Task<string> GetDocumentUrl(string filepath, bool isPublic = false)
        {
            var awsAccessKey = _awsAccessKey;
            var awsSecretKey = _awsSecretKey;
            var awsBucketName = _awsBucketName;

            var client = new AmazonS3Client(
                awsAccessKey,
                awsSecretKey,
                RegionEndpoint.GetBySystemName(_awsRegion));

            if (isPublic)
            {
                return $"https://{awsBucketName}.s3.{_awsRegion}.amazonaws.com/{filepath}";
            }

            var signedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = awsBucketName,
                Key = filepath,
                Expires = DateTime.UtcNow.AddMinutes(_signedUrlExpiresIn),
                Verb = HttpVerb.GET
            };

            string signedUrl = await Task.FromResult(client.GetPreSignedURL(signedUrlRequest));
            return signedUrl;
        }
        #endregion

        #region UploadDocument
        public async Task<string> UploadDocumentAsync(Stream fileStream, string fileName, string folder = "uploads")
        {
            var client = new AmazonS3Client(
                _awsAccessKey,
                _awsSecretKey,
                RegionEndpoint.GetBySystemName(_awsRegion));

            var key = $"{folder}/{Guid.NewGuid()}_{fileName}";

            var uploadRequest = new PutObjectRequest
            {
                BucketName = _awsBucketName,
                Key = key,
                InputStream = fileStream,
                AutoCloseStream = true
            };

            await client.PutObjectAsync(uploadRequest);
            return key;
        }
        #endregion

        #region DeleteDocument
        public async Task<bool> DeleteDocumentAsync(string filepath)
        {
            var client = new AmazonS3Client(
                _awsAccessKey,
                _awsSecretKey,
                RegionEndpoint.GetBySystemName(_awsRegion));

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _awsBucketName,
                Key = filepath
            };

            var response = await client.DeleteObjectAsync(deleteRequest);
            return (int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300;
        }
        #endregion

        #region GetAllDocuments
        public async Task<List<string>> GetAllDocumentsAsync(string folder = "uploads")
        {
            var client = new AmazonS3Client(
                _awsAccessKey,
                _awsSecretKey,
                RegionEndpoint.GetBySystemName(_awsRegion));

            var listRequest = new ListObjectsV2Request
            {
                BucketName = _awsBucketName,
                Prefix = folder
            };

            var response = await client.ListObjectsV2Async(listRequest);
            return response.S3Objects.Select(o => o.Key).ToList();
        }
        #endregion
    }
}