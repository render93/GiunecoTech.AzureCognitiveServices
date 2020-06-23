using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;

namespace ImageAnalizator
{
    class Program
    {
        private const string AnalyzeUrlImage = "http://www.giuneco.tech/wp-content/uploads/2018/04/banner_sottile1.jpg";
        private const string Endpoint = "YOUR-ENDPOINT-HERE";
        private const string ApiKey = "YOUR-API-KEY-HERE";
        
        static async Task Main(string[] args)
        {
            try
            {
                // Create a client
                var client = Authenticate(Endpoint, ApiKey);
                // Analyze an image to get features and other properties.
                await AnalyzeImageUrl(client, AnalyzeUrlImage);
                // await AnalyzeImageLocal(client);

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            var client =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                    {Endpoint = endpoint};
            
            return client;
        }

        private static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            var features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };
            
            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            var results = await client.AnalyzeImageAsync(imageUrl, features);
            
            // Summarizes the image content.
            Console.WriteLine("Summary:");
            foreach (var caption in results.Description.Captions)
            {
                var json = JsonConvert.SerializeObject(results, Formatting.Indented);
                Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
            }
            Console.WriteLine();
        }
        
        private static async Task AnalyzeImageLocal(ComputerVisionClient client)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - LOCAL");
            Console.WriteLine();
            
            var features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            var imagesToAnalyze = Directory.GetFiles("images", "*.*", SearchOption.TopDirectoryOnly)
                .Where(w => w.EndsWith(".jpg") || w.EndsWith(".png")).ToList();
            
            if(!imagesToAnalyze.Any())
            {
                Console.WriteLine($"No images found in {Environment.CurrentDirectory}/images");
                return;
            }

            foreach (var image in imagesToAnalyze)
            {
                Console.WriteLine($"Analyzing the image {Path.GetFileName(image)}...");
                Console.WriteLine();
                // Analyze the URL image 
                await using var fs = new FileStream($@"{image}", FileMode.Open);
                var results = await client.AnalyzeImageInStreamAsync(fs, features);
                
                // Summarizes the image content.
                Console.WriteLine("Summary:");
                foreach (var caption in results.Description.Captions)
                {
                    Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
                }
                Console.WriteLine();
            }
        }
    }
}