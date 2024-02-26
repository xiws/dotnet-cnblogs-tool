using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MetaWeblogClient;
using Polly;

namespace Dotnetcnblog.Utils
{
    public class PostBlogHelper
    {
        public static Client BlogClient;

        public static void Init(BlogConnectionInfo info)
        {
            BlogClient = new Client(info);
        }
        
        public static string PostBlog(string title, string content)
        {
            var categories= new Collection<string>() { "技术文档" };
            var policy = Policy.Handle<Exception>().Retry(3,
                (exception, retryCount) =>
                {
                    Console.WriteLine("发布失败，正在重试 {0}，异常：{1}", retryCount, exception.Message);
                });
            try
            {
                var postId = policy.Execute(() =>
                {
                    
                    var result = BlogClient.NewPost(title,content,categories,false,DateTime.Now);
                    return result;
                });

                return postId;
            }
            catch (Exception e)
            {
                Console.WriteLine("上传失败,异常：{0}", e.Message);
                throw;
            }
        }
    }
}