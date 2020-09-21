using System;
using System.Text.Json;
using Azure;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace AzureSearch.SDK.Quickstart.v11

{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceName = "lynx-searchengine-jsancho";
            string indexName = "hotels-quickstart-v11";
            string apiKey = "6C88A8613367F73135756A4CCFE1C24C";

            // Create a SearchIndexClient to send create/delete index commands
            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            SearchIndexClient idxclient = new SearchIndexClient(serviceEndpoint, credential);

            // Create a SearchClient to load and query documents
            SearchClient srchclient = new SearchClient(serviceEndpoint, indexName, credential);

            // Delete index if it exists
            Console.WriteLine("{0}", "Deleting index...\n");
            DeleteIndexIfExists(indexName, idxclient);

            var searchClientOptions = new SearchClientOptions
            {
                Serializer = new JsonObjectSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
            };

            var builder = new FieldBuilder
            {
                Serializer = searchClientOptions.Serializer
            };

            var index = new SearchIndex(indexName)
            {
                Fields = builder.Build(typeof(Hotel))
            };

            Console.WriteLine("{0}", "Creating index...\n");
            idxclient.CreateIndex(index);

            // Load documents (using a subset of fields for brevity)
            IndexDocumentsBatch<Hotel> batch = IndexDocumentsBatch.Create(
                IndexDocumentsAction.Upload(new Hotel { Id = "78", Name = "Upload Inn", Category = "hotel", Rate = 279, Updated = new DateTime(2018, 3, 1, 7, 0, 0) }),
                IndexDocumentsAction.Upload(new Hotel { Id = "54", Name = "Breakpoint by the Sea", Category = "motel", Rate = 162, Updated = new DateTime(2015, 9, 12, 7, 0, 0) }),
                IndexDocumentsAction.Upload(new Hotel { Id = "39", Name = "Debug Motel", Category = "motel", Rate = 159, Updated = new DateTime(2016, 11, 11, 7, 0, 0) }),
                IndexDocumentsAction.Upload(new Hotel { Id = "48", Name = "NuGet Hotel", Category = "hotel", Rate = 238, Updated = new DateTime(2016, 5, 30, 7, 0, 0) }),
                IndexDocumentsAction.Upload(new Hotel { Id = "12", Name = "Renovated Ranch", Category = "motel", Rate = 149, Updated = new DateTime(2020, 1, 24, 7, 0, 0) }));

            IndexDocumentsOptions idxoptions = new IndexDocumentsOptions { ThrowOnAnyError = true };

            Console.WriteLine("{0}", "Loading index...\n");
            srchclient.IndexDocuments(batch, idxoptions);

            // Wait 2 secondsfor indexing to complete before starting queries (for demo and console-app purposes only)
            Console.WriteLine("Waiting for indexing...\n");
            System.Threading.Thread.Sleep(2000);

            // Call the RunQueries method to invoke a series of queries
            Console.WriteLine("Starting queries...\n");
            RunQueries(srchclient);

            // End the program
            Console.WriteLine("{0}", "Complete. Press any key to end this program...\n");
            Console.ReadKey();
        }

        // Delete the hotels-v11 index to reuse its name
        private static void DeleteIndexIfExists(string indexName, SearchIndexClient idxclient)
        {
            idxclient.GetIndexNames();
            {
                idxclient.DeleteIndex(indexName);
            }
        }

        // Run queries, use WriteDocuments to print output
        private static void RunQueries(SearchClient srchclient)
        {
            SearchOptions options;
            SearchResults<Hotel> response;

            Console.WriteLine("Query #1: Search on the term 'motel' and list the relevance score for each match...\n");

            options = new SearchOptions()
            {
                Filter = "",
                OrderBy = { "" }
            };

            response = srchclient.Search<Hotel>("motel", options);
            WriteDocuments(response);

            Console.WriteLine("Query #2: Find hotels where 'type' equals hotel...\n");

            options = new SearchOptions()
            {
                Filter = "hotelCategory eq 'hotel'",
            };

            response = srchclient.Search<Hotel>("*", options);
            WriteDocuments(response);

            Console.WriteLine("Query #3: Filter on rates less than $200 and sort by when the hotel was last updated...\n");

            options = new SearchOptions()
            {
                Filter = "baseRate lt 200",
                OrderBy = { "lastRenovationDate desc" }
            };

            response = srchclient.Search<Hotel>("*", options);
            WriteDocuments(response);
        }

        // Write search results to console
        private static void WriteDocuments(SearchResults<Hotel> searchResults)
        {
            foreach (SearchResult<Hotel> response in searchResults.GetResults())
            {
                Hotel doc = response.Document;
                var score = response.Score;
                Console.WriteLine($"Name: {doc.Name}, Type: {doc.Category}, Rate: {doc.Rate}, Last-update: {doc.Updated}, Score: {score}");
            }

            Console.WriteLine();
        }
    }
}
