﻿using System;
using System.Drawing;
using System.IO;
using System.Text;
using Dotnetcnblog.Command;
using Dotnetcnblog.Utils;
using McMaster.Extensions.CommandLineUtils;
using MetaWeblogClient;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace Dotnetcnblog
{
    [Command(Name = "dotnet-cnblog", Description = "dotNet 博客园工具")]
    [Subcommand(typeof(CommandReset))]
    [Subcommand(typeof(CommandSetConfig))]
    [Subcommand(typeof(CommandProcessFile))]
    [Subcommand(typeof(CommandUploadImg))]
    class Program
    {
        private const string CfgFileName = "dotnet-cnblog.config.json";

        private static int Main(string[] args)
        {
            PrintTitle();
            if (Init())
            {
                return CommandLineApplication.Execute<Program>(args);
            }
            else
            {
                ConsoleHelper.PrintError("您还未设置配置，将引导你设置！");
                var setConfig=new CommandSetConfig();
                setConfig.Execute(CommandContextStore.Get());
                return 0;
            }
        }

        /// <summary>
        /// 一级命令执行
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }

        /// <summary>
        /// 打印标题
        /// </summary>
        static void PrintTitle()
        {
            Console.WriteAscii("dotNet Cnblogs Tool", Color.FromArgb(244, 212, 255));
            Console.Write("作者：", Color.FromArgb(90, 212, 255));
            Console.WriteLine("晓晨Master", Color.FromArgb(200, 212, 255));
            Console.Write("问题反馈：", Color.FromArgb(90, 212, 255));
            Console.WriteLine("https://github.com/stulzq/dotnet-cnblogs-tool/issues ", Color.FromArgb(200, 212, 255));
            Console.WriteLine("");
        }

        /// <summary>
        /// 初始化，加载配置
        /// </summary>
        /// <returns></returns>
        static bool Init()
        {
            var context=new CommandContext();

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            context.AppConfigFilePath = Path.Combine(docPath,"dotnet-cnblog", CfgFileName);

            if (!File.Exists(context.AppConfigFilePath))
            {
                CommandContextStore.Set(context);
                return false;
            }

            var config = JsonConvert.DeserializeObject<BlogConnectionInfo>(File.ReadAllText(context.AppConfigFilePath));
            config.Password =
                Encoding.UTF8.GetString(TeaHelper.Decrypt(Convert.FromBase64String(config.Password), context.EncryptKey));
            context.ConnectionInfo = config;
            ImageUploadHelper.Init(config);
            PostBlogHelper.Init(config);
            CommandContextStore.Set(context);
            return true;
        }
    }
}