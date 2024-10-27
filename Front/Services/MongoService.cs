using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Front.Services
{
    public class MongoService
    {
        private const string MongoConnectionString = "mongodb://root:example@mongo:27017/";
        private readonly FilterDefinition<GridFSFileInfo> filterEmpty = Builders<GridFSFileInfo>.Filter.Empty;
        private IMongoDatabase _database;
        private GridFSBucket _gridFS;

        public MongoService()
        {
            _database = new MongoClient(MongoConnectionString).GetDatabase("downloadedFiles");
            _gridFS = new GridFSBucket(this._database);
        }

        public async Task ListAllFilesAsync()
        {
            using var cursor = _gridFS.Find(filterEmpty);

            var fileNames = cursor.ToList().Select(x => x.Id);
            foreach (var item in fileNames)
            {
                Console.WriteLine(item);
            }
        }
        public async Task DownloadFileAsync(ObjectId fileId, string outputFilePath)
        {
            using (var stream = System.IO.File.Create(outputFilePath))
            {
                // Download file from GridFS
                await _gridFS.DownloadToStreamAsync(fileId, stream);
                Console.WriteLine($"File downloaded to: {outputFilePath}");
            }
        }
        public async Task<FileData> GetByteArrayFromFile(string fileId)
        {
            ObjectId objectId = new(fileId);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            var fileInfo = await _gridFS.Find(filter).FirstOrDefaultAsync();

            var metaData = fileInfo.Metadata.AsBsonDocument;
            string? contentType = "empty";
            if (metaData.Contains("_contentType"))
                contentType = metaData["_contentType"].ToString();
            Console.WriteLine(contentType);

            return new(contentType, await _gridFS.DownloadAsBytesAsync(objectId));
        }

        public async Task<byte[]> GetByteArrayFromFileSimple(string fileId)
        {
            ObjectId objectId = new(fileId);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            var fileInfo = await _gridFS.Find(filter).FirstOrDefaultAsync();

            return await _gridFS.DownloadAsBytesAsync(objectId);
        }

        public async Task<FileData> GetByteArrayFromFileNEW(string fileId)
        {
            Console.WriteLine("FILE ID:" + fileId);
            ObjectId objectId = new(fileId);
            Console.WriteLine("object id:" + objectId);

            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            var fileInfo = await _gridFS.Find(filter).FirstOrDefaultAsync();

            if (fileInfo == null)
            {
                Console.WriteLine("FILE INFO IS NULL");
            }


            //var metaData = fileInfo.Metadata.AsBsonDocument;
            //string? contentType = "empty";
            //if (metaData.Contains("_contentType"))
            //    contentType = metaData["_contentType"].ToString();

            //Console.WriteLine(contentType);

            return new("", await _gridFS.DownloadAsBytesAsync(objectId));
        }
    }
}
