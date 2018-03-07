using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ProjectHub.Web.Services
{
    /// <summary>
    /// 负责将业务数据序列化到本地 JSON 文件。
    /// </summary>
    public class JsonDataStore : IJsonDataStore
    {
        private const string DefaultDataDirectory = "App_Data";

        private static readonly object SyncRoot = new object();
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private readonly string _dataDirectory;
        private readonly ILogger<JsonDataStore> _logger;

        public JsonDataStore(
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            ILogger<JsonDataStore> logger)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            _logger = logger;

            var configuredDirectory = configuration["ProjectHub:DataDirectory"];
            var relativeDirectory = string.IsNullOrWhiteSpace(configuredDirectory)
                ? DefaultDataDirectory
                : configuredDirectory;

            _dataDirectory = Path.IsPathRooted(relativeDirectory)
                ? relativeDirectory
                : Path.Combine(hostingEnvironment.ContentRootPath, relativeDirectory);

            Directory.CreateDirectory(_dataDirectory);
        }

        public T Read<T>(string fileName, Func<T> createDefault)
        {
            if (createDefault == null)
            {
                throw new ArgumentNullException(nameof(createDefault));
            }

            var path = GetPath(fileName);

            lock (SyncRoot)
            {
                if (!File.Exists(path))
                {
                    var defaultValue = createDefault();
                    WriteFile(path, defaultValue);
                    return defaultValue;
                }

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return createDefault();
                }

                try
                {
                    var value = JsonConvert.DeserializeObject<T>(json);
                    return value == null ? createDefault() : value;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "数据文件 {FileName} 格式不正确，无法读取。", fileName);
                    throw new InvalidOperationException("数据文件格式不正确：" + fileName, ex);
                }
            }
        }

        public void Write<T>(string fileName, T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            lock (SyncRoot)
            {
                WriteFile(GetPath(fileName), data);
            }
        }

        private static void WriteFile<T>(string path, T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json, Utf8NoBom);
        }

        private string GetPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("数据文件名不能为空。", nameof(fileName));
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("数据文件名包含非法字符。", nameof(fileName));
            }

            return Path.Combine(_dataDirectory, fileName);
        }
    }
}
