using CrawlerBook.Dtos.MeTruyenChuVN;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text;

namespace CrawlerBook
{
    public class MeTruyenChuVN
    {
        public List<BookRequest> _lstBookRequest { get; set; }

        public MeTruyenChuVN()
        {
            string filePath = Path.Combine("Data", "MeTruyenChuVN", "books.json");
            string json = File.ReadAllText(filePath);
            var bookRequests = JsonConvert.DeserializeObject<Dictionary<string, List<BookRequest>>>(json);
            _lstBookRequest = bookRequests["bookRequests"];
        }

        public async Task<List<Book>> GetAndReadBookListAsync()
        {
            List<Book> books = new List<Book>();
            Book book = null;
            ApiResponse apiResponse = null;
            byte[] data = null;
            string decodedString, responseBody;
            HttpResponseMessage response;
            foreach (var bookrq in _lstBookRequest)
            {
                book = new Book();
                book.name = bookrq.name;
                book.urlReaderBook = bookrq.urlReaderBook;
                book.chapters = new List<Chapter>();
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        response = await client.GetAsync(bookrq.urlChapterOfBook);
                        response.EnsureSuccessStatusCode();

                        responseBody = await response.Content.ReadAsStringAsync();

                        apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                        if (apiResponse != null && apiResponse.success == true)
                        {
                            data = Convert.FromBase64String(apiResponse.data);
                            decodedString = Encoding.UTF8.GetString(data);
                            book.chapters = JsonConvert.DeserializeObject<List<Chapter>>(decodedString);
                        }
                        else
                        {
                            Console.WriteLine($"Error: {apiResponse?.message ?? "Unknown error"}");
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine($"Request error: {e.Message}");
                    }
                    catch (JsonException e)
                    {
                        Console.WriteLine($"JSON Parsing error: {e.Message}");
                    }
                }
                books.Add(book);
            }

            return books;
        }

        public async Task<string> SaveChapterHtmlToText(string folderPath, Book book, Chapter slug)
        {
            string filePathSave = Path.Combine(folderPath, slug.slug + ".txt");
            string url = book.urlReaderBook + slug.slug;

            using (HttpClient client = new HttpClient())
            {
                try
                {

                    string directoryPath = Path.GetDirectoryName(filePathSave);

                    if (Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    if (File.Exists(filePathSave))
                    {
                        return filePathSave;
                    }

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(responseBody);

                    var articleNode = htmlDoc.DocumentNode.SelectSingleNode("//article");
                    var paragraphs = articleNode.SelectNodes(".//p");

                    if (paragraphs != null)
                    {
                        Console.WriteLine("Crawling chapter : " + slug.slug);
                        using (StreamWriter writer = new StreamWriter(filePathSave))
                        {
                            foreach (var p in paragraphs)
                            {
                                writer.WriteLine(p.InnerText);
                            }
                        }
                        Console.WriteLine("Crawling success");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }

            return filePathSave;
        }

        public async Task RewriteChapter(string folderPath, string filePathRewrite)
        {
            string apiKey = "AIzaSyDBB4ielxkcTS8WRkyllool8OWLlEv3R6E";
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key={apiKey}";
            GemeniResponse gResponse;
            string textRewrite = File.ReadAllText(filePathRewrite);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = "viết lại nội dung bên dưới với câu từ chuẩn chỉ phong cách tiên hiệp" }
                        }
                    },
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = textRewrite }
                        }
                    }
                }
            };

            string filePathRewriteSave = filePathRewrite.Replace(".txt", "_rewrite.txt");

            using (HttpClient client = new HttpClient())
            {
                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseString = await response.Content.ReadAsStringAsync();
                gResponse = JsonConvert.DeserializeObject<GemeniResponse>(responseString);
            }

            if(!File.Exists(filePathRewriteSave))
            {
                using (StreamWriter writer = new StreamWriter(filePathRewriteSave))
                {
                    writer.WriteLine(gResponse.Candidates[0].Content.Parts[0].Text);
                }
                Console.WriteLine("(" + Path.GetFileName(filePathRewriteSave) + ") rewrite chapter success");
            }
            else
            {
                Console.WriteLine("(" + Path.GetFileName(filePathRewriteSave) + ") rewrite chapter existed");
            }
        }

        public async void Crawling()
        {
            List<Book> lstBook = await GetAndReadBookListAsync();
            string filePathSave = string.Empty;
            foreach (var book in lstBook)
            {
                Console.WriteLine("Crawling book : " + book.name);
                foreach (var chapter in book.chapters)
                {
                    filePathSave = await SaveChapterHtmlToText(Path.Combine("OutputFiles", book.name), book, chapter);
                    await RewriteChapter(Path.Combine("OutputFiles", book.name), filePathSave);
                }
            }
        }
    }
}
