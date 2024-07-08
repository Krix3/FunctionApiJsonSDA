using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class KlineData
{
    public long KlineOpenTime { get; set; }
    public string OpenPrice { get; set; }
    public string HighPrice { get; set; }
    public string LowPrice { get; set; }
    public string ClosePrice { get; set; }
    public string Volume { get; set; }
    public long KlineCloseTime { get; set; }
    public string QuoteAssetVolume { get; set; }
    public int NumberOfTrades { get; set; }
    public string TakerBuyBaseAssetVolume { get; set; }
    public string TakerBuyQuoteAssetVolume { get; set; }
}

class Program
{
    private static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        var builder = new UriBuilder("https://api.binance.com/api/v3/klines");
        var query = "?symbol=BTCUSDT&interval=1d";
        builder.Query = query;

        var uri = builder.Uri;

        try
        {
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var klineDataList = JsonSerializer.Deserialize<object[][]>(responseBody);

            var klineDataObjects = new KlineData[klineDataList.Length];

            for (int i = 0; i < klineDataList.Length; i++)
            {
                klineDataObjects[i] = new KlineData
                {
                    KlineOpenTime = Convert.ToInt64(klineDataList[i][0]),
                    OpenPrice = klineDataList[i][1].ToString(),
                    HighPrice = klineDataList[i][2].ToString(),
                    LowPrice = klineDataList[i][3].ToString(),
                    ClosePrice = klineDataList[i][4].ToString(),
                    Volume = klineDataList[i][5].ToString(),
                    KlineCloseTime = Convert.ToInt64(klineDataList[i][6]),
                    QuoteAssetVolume = klineDataList[i][7].ToString(),
                    NumberOfTrades = Convert.ToInt32(klineDataList[i][8]),
                    TakerBuyBaseAssetVolume = klineDataList[i][9].ToString(),
                    TakerBuyQuoteAssetVolume = klineDataList[i][10].ToString(),
                };
            }

            var jsonString = JsonSerializer.Serialize(klineDataObjects, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync("klineData.json", jsonString);

            Console.WriteLine("Данные успешно сериализованы в klineData.json");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Запрос завершился ошибкой: {e.Message}");
        }
    }
}

