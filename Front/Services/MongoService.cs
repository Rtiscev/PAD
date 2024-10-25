using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;

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
    }
}
