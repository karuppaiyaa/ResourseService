using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
using ResourceStorageService.Application.Commands;
using ResourceStorageService.Application.Queries;
using System.Net;

namespace ResourceStorageService.Application.Services
{
    public class S3Services
    {
        private readonly IConfiguration _configuration;
        public virtual string BucketName { get; set; }
        public virtual long ResourceLimit { get; set; }
        public virtual long PartSize { get; set; }
        public virtual double UrlExpiry { get; set; }
        public virtual AmazonS3Client AmazonS3Client { get; set; }
        public S3Services(IConfiguration configuration)
        {
            _configuration = configuration;
            AmazonS3Client = new AmazonS3Client(_configuration["AWS:AccessKey"].ToString(), _configuration["AWS:SecretKey"].ToString(), RegionEndpoint.GetBySystemName(_configuration["Region"]));// RegionEndpoint.APSouth1;
            BucketName = _configuration["AWS:BucketName"];
            ResourceLimit = Convert.ToInt32(_configuration["AWS:FileLimit"]) * (long)Math.Pow(2, 20);
            PartSize = Convert.ToInt32(_configuration["AWS:FileLimit"]) * (long)Math.Pow(2, 20);
            UrlExpiry = Convert.ToDouble(_configuration["AWS:UrlExpiry"]);
        }
        
        public async Task<string> UploadResource(UploadResourceCommand request)
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(AmazonS3Client, BucketName))
            {
                var putbucket = new PutBucketRequest
                {
                    BucketName = BucketName,
                    UseClientRegion = true
                };
                await AmazonS3Client.PutBucketAsync(putbucket);
            }

            if (request.Resource.Length > ResourceLimit)
            {
                var resourceTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = BucketName,
                    InputStream = request.Resource.OpenReadStream(),
                    PartSize = PartSize,
                    Key = request.ResourcePath,
                    ContentType = request.Resource.ContentType
                };

                await new TransferUtility(AmazonS3Client).UploadAsync(resourceTransferUtilityRequest);
                return await Task.Run(() => "200");
            }
            else
            {
                var uploadData = new PutObjectRequest
                {
                    BucketName = BucketName,
                    InputStream = request.Resource.OpenReadStream(),
                    Key = request.ResourcePath,
                    ContentType = request.Resource.ContentType
                };

                PutObjectResponse response = await AmazonS3Client.PutObjectAsync(uploadData);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return await Task.Run(() => "200");
                }
                else
                {
                    return await Task.Run(() => "500");
                }
            }
        }

        public async Task<(byte[] resourceBytes, string contentType, string resourceDownloadName, DateTimeOffset? lastModified)> DownloadResource(DownloadResourceQuery request)
        {
            byte[] resourceBytes = null;
            string contentType = "";
            string resourceDownloadName;
            DateTimeOffset? lastModified = DateTime.UtcNow;
            request.ResourcePath = Uri.UnescapeDataString(request.ResourcePath);
            resourceDownloadName = request.ResourcePath.Split("/").Last();

            if (await AmazonS3Util.DoesS3BucketExistV2Async(AmazonS3Client, BucketName))
            {
                GetObjectRequest resource = new()
                {
                    BucketName = BucketName,
                    Key = request.ResourcePath
                };

                GetObjectResponse response = await AmazonS3Client.GetObjectAsync(resource);

                using (var reader = new MemoryStream())
                {
                    response.ResponseStream.CopyTo(reader);
                    contentType = response.Headers["Content-Type"];
                    resourceBytes = reader.ToArray();
                }
            }
            return (resourceBytes, contentType, resourceDownloadName, lastModified);
        }

        public async Task<List<(byte[] resourceBytes, string contentType, string resourceName)>> DownloadMultipleResources(DownloadMultipleResourcesQuery request)
        {
            byte[] resourceBytes = null;
            string contentType = "";
            List<(byte[] resourceBytes, string contentType, string resourceName)> resources = new();

            if (await AmazonS3Util.DoesS3BucketExistV2Async(AmazonS3Client, BucketName))
            {
                var s3ObjectKeys = await GetAllKeys(request.ResourcePaths);
                s3ObjectKeys = s3ObjectKeys.Distinct().ToList();

                foreach (var key in s3ObjectKeys)
                {
                    GetObjectRequest resource = new()
                    {
                        BucketName = BucketName,
                        Key = key
                    };

                    GetObjectResponse response = await AmazonS3Client.GetObjectAsync(resource);

                    using (var reader = new MemoryStream())
                    {
                        response.ResponseStream.CopyTo(reader);
                        contentType = response.Headers["Content-Type"];
                        resourceBytes = reader.ToArray();
                    }

                    var index = key.LastIndexOf("/");
                    resources.Add((resourceBytes, contentType, key[(index + 1)..]));
                }
            }
            return resources;
        }

        public async Task<string> DeleteResource(DeleteResourceCommand request)
        {
            if (await AmazonS3Util.DoesS3BucketExistV2Async(AmazonS3Client, BucketName))
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = request.ResourcePath
                };

                DeleteObjectResponse response = await AmazonS3Client.DeleteObjectAsync(deleteObjectRequest);
                if (response.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    return await Task.Run(() => "200");
                }
            }
            return await Task.Run(() => "500");

        }

        public async Task<string> DeleteMultipleResources(List<string> paths)
        {
            if (await AmazonS3Util.DoesS3BucketExistV2Async(AmazonS3Client, BucketName))
            {
                var s3ObjectKeys = await GetAllKeys(paths);
                s3ObjectKeys = s3ObjectKeys.Distinct().ToList();
                var deleteObjectsRequest = new DeleteObjectsRequest
                {
                    BucketName = BucketName
                };

                foreach (var key in s3ObjectKeys)
                {
                    deleteObjectsRequest.AddKey(key);

                }
                DeleteObjectsResponse response = await AmazonS3Client.DeleteObjectsAsync(deleteObjectsRequest);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return await Task.Run(() => "200");
                }
            }
            return await Task.Run(() => "500");
        }

        private async Task<List<string>> GetAllKeys(List<string> resourcePath)
        {
            List<string> s3objectpaths = new();
            foreach (var path in resourcePath)
            {
                if (path.Split(@"/").Last().Contains("."))
                {
                    s3objectpaths.Add(path);
                }
                else
                {
                    ListObjectsRequest listObjectsRequest = new()
                    {
                        BucketName = BucketName,
                        Prefix = path
                    };

                    ListObjectsResponse listObjectsResponse = await AmazonS3Client.ListObjectsAsync(listObjectsRequest);

                    if (listObjectsResponse.S3Objects != null)
                    {
                        s3objectpaths.AddRange(listObjectsResponse.S3Objects.Select(a => a.Key).ToList());
                    }
                }
            }
            return s3objectpaths;
        }

        public async Task<GetResourcePathDto> GetFullUrl(string path)
        {
            path = Uri.UnescapeDataString(path);
            GetResourcePathDto getResourcePath = new();
            if (await AmazonS3Util.DoesS3BucketExistV2Async(AmazonS3Client, BucketName))
            {
                if (await ResourceExists(path))
                {
                    double cacheExpiry = UrlExpiry * 60;
                    string cacheControl = $"max-age={cacheExpiry}";
                    DateTime expiration = DateTime.UtcNow.AddMinutes(UrlExpiry);
                    string expires = expiration.ToString("R");
                    var expiryUrlRequest = new GetPreSignedUrlRequest()
                    {
                        BucketName = BucketName,
                        Key = path,
                        Expires = expiration,
                        ResponseHeaderOverrides = new()
                        {
                            CacheControl = cacheControl,
                            Expires = expires
                        }
                    };
                    getResourcePath.ResourcePath = AmazonS3Client.GetPreSignedURL(expiryUrlRequest);
                    getResourcePath.ExpiresAt = expiration;
                }
                else
                {
                    getResourcePath.ResourcePath = string.Empty;
                }
            }
            return getResourcePath;
        }

        private async Task<bool> ResourceExists(string key)
        {
            try
            {
                await AmazonS3Client.GetObjectMetadataAsync(BucketName, key);
                return true;
            }
            catch (AmazonS3Exception)
            {
                return false;
            }
        }
    }
}
