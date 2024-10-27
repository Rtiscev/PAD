using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MS_FFMpeg.Services
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

        public async Task<List<string>> ListAllFilesAsync()
        {
            using var cursor = await _gridFS.FindAsync(filterEmpty);
            var fileNames = cursor.ToList().Select(x => x.Id.ToString()).ToList();
            return fileNames;
        }
        public async Task<ObjectId> UploadFile(string path, string fileName)
        {
            using var s = File.OpenRead(path);
            var t = Task.Run<ObjectId>(async () =>
            {
                return await _gridFS.UploadFromStreamAsync(fileName, s);
            });

            return t.Result;
        }
        public async Task<string> DownloadFileAsync(ObjectId fileId, string outputFilePath)
        {
            // find the file
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", fileId);
            var fileInfo = await _gridFS.Find(filter).FirstOrDefaultAsync();

            if (File.Exists(outputFilePath + "/" + fileInfo.Filename))
            {
                Console.WriteLine("File alreay exists:" + outputFilePath + "/" + fileInfo.Filename);
            }
            else
            {
                Console.WriteLine("File doesnt exist:" + outputFilePath);
                // create the file
                using var stream = System.IO.File.Create(outputFilePath + "/" + fileInfo.Filename);
                // Download file from GridFS
                await _gridFS.DownloadToStreamAsync(fileId, stream);
            }

            return fileInfo.Filename;
        }
        public async Task<FileData> GetByteArrayFromFile(string fileId)
        {
            ObjectId objectId = new(fileId);

            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId); // Use "_id" directly
            var fileInfo = await _gridFS.Find(filter).FirstOrDefaultAsync();

            var metaData = fileInfo.Metadata.AsBsonDocument;
            string? contentType = "empty";
            if (metaData.Contains("_contentType"))
                contentType = metaData["_contentType"].ToString();

            Console.WriteLine(contentType);

            return new(contentType, await _gridFS.DownloadAsBytesAsync(objectId));
        }
    }
}
