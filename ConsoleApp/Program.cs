﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using InterfacesLibrary;

namespace ConsoleApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient {BaseAddress = new Uri("http://localhost:5000")};

            var generator = new HttpProxyGenerator.HttpProxyGenerator();

            var proxy = generator.CreateProxy<IDataFetcher>(client);

            for (int i = 0; i < 20; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                var result = await proxy.GetData("ufff", 123);
                Console.WriteLine($"{result} - {sw.ElapsedMilliseconds}");
                sw.Stop();
            }
        }
    }
}