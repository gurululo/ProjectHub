using System;

namespace ProjectHub.Web.Services
{
    /// <summary>
    /// 以 JSON 文件为基础的数据读写接口。
    /// </summary>
    public interface IJsonDataStore
    {
        T Read<T>(string fileName, Func<T> createDefault);

        void Write<T>(string fileName, T data);
    }
}
