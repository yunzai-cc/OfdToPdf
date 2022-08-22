using System.Net.Http.Headers;

using System.Security.Cryptography;
using Newtonsoft.Json;
using OfdToPdf.Responses;

namespace OfdToPdf;

public class OfdClient
{
    private readonly string _secretId;
    private readonly string _secretKey; 
    public OfdClient(string secretId,string secretKey)
    {
        _secretId = secretId;
        _secretKey = secretKey;
    }

    public async Task<OfdToPdfResponse?> ToPdfAsync(byte[] bytes)
    {
        return await ConvertToPdf(bytes, null);
    }
    public async Task<OfdToPdfResponse?> ToPdfAsync(string url)
    {
        return await ConvertToPdf(null, url);
    }
    internal async Task<OfdToPdfResponse?> ConvertToPdf(byte[]? file, string? url)
    {
        return await ConvertTo<OfdToPdfResponse>("ofdtopdf", file, url);
    }
    public async Task<OfdToPdfResponse?> ToImageAsync(byte[] bytes)
    {
        return await ConvertToImage(bytes, null);
    }
    public async Task<OfdToPdfResponse?> ToImageAsync(string url)
    {
        return await ConvertToImage(null, url);
    }
    internal async Task<OfdToPdfResponse?> ConvertToImage(byte[]? file, string? url)
    {
        return await ConvertTo<OfdToPdfResponse>("ofdtoimage", file, url);
    }
    internal async Task<T?> ConvertTo<T>(string path,byte[]? file, string? url)
    {
        var baseUrl = $"https://service-aof132i1-1312051063.sh.apigw.tencentcs.com/release/{path}";
        var postDict = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(url))
        {
            postDict.Add("url", url);
        }

        if (file != null)
        {
            postDict.Add("file", Convert.ToBase64String(file));
        }

        using HttpClient client = new HttpClient();

        var source = "market";
        string dt = DateTime.UtcNow.GetDateTimeFormats('r')[0];
        var auth = Auth(dt, source);
        //ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
        client.DefaultRequestHeaders.Add("X-Source", source);
        client.DefaultRequestHeaders.Add("X-Date", dt);
        var ret = await client.PostAsync(baseUrl, new FormUrlEncodedContent(postDict));

        var text = await ret.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(text);
    }

    private string Auth(string dt, string source)
    {
        string signStr = $"x-date: {dt}\n" + $"x-source: {source}";
        string sign = HmacSha1Text(signStr, _secretKey);

        string auth = $"hmac id=\"{_secretId}\", algorithm=\"hmac-sha1\", headers=\"x-date x-source\", signature=\"";
        auth = auth + sign + "\"";
        return auth;
    }

    public static string HmacSha1Text(string encryptText, string encryptKey)
   {
       using var hmacSha1 = new HMACSHA1();
       hmacSha1.Key = System.Text.Encoding.UTF8.GetBytes(encryptKey);

       var dataBuffer = System.Text.Encoding.UTF8.GetBytes(encryptText);
       var hashBytes = hmacSha1.ComputeHash(dataBuffer);
       return Convert.ToBase64String(hashBytes);
   }
}