using AppStoreManagementSystem.Domain;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using AppStoreManagementSystem.Domain.Features.App;
using AppStoreManagementSystem.Domain.Features.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppManagementSystem.App.Services;

public class ApiService
{
    // instead of creating HtppClient http = new HttpClient();
    // here, gonna use the IHttpClientFactory and injection
    private readonly IHttpClientFactory _httpclientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public ApiService(IHttpClientFactory httpclientFactory, IConfiguration configuration)
    {
        _httpclientFactory = httpclientFactory;
        _configuration = configuration;
        _baseUrl = _configuration.GetValue<string>("BackendApiUrl")!;
    }

    public async Task<Result<AppListResponseModel>> GetApps(AppListRequestModel request)
    {
        var httpClient = _httpclientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_baseUrl);
        string url = $"{ApiEndpoints.AppList}?pageNo={request.PageNo}&pageSize={request.PageSize}";
        // var response = await httpClient.GetAsync(url);

        //  var result = await response.Content.ReadFromJsonAsync<Result<AppListResponseModel>>();

        Console.WriteLine(httpClient.BaseAddress + url);
        //var response = await httpClient.GetAsync(url);

        //Console.WriteLine(response.StatusCode);

        //var text = await response.Content.ReadAsStringAsync();
        //Console.WriteLine(text);

        //response.EnsureSuccessStatusCode();

        //var result = await response.Content.ReadFromJsonAsync<Result<AppListResponseModel>>();

        //return result!;
        var response = await httpClient.GetAsync(url);

        Console.WriteLine($"Status = {response.StatusCode}");

        var text = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response = {text}");

        // Don't call EnsureSuccessStatusCode yet
        // response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(text);
        }

        var result = await response.Content.ReadFromJsonAsync<Result<AppListResponseModel>>();

        return result!;

    }

    public async Task<Result<List<AppCategoryListModel>>> GetCategories()
    {
        var httpClient = _httpclientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var response = await httpClient.GetAsync(ApiEndpoints.AppCategories);
        var text = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(text);
        }

        var result = await response.Content.ReadFromJsonAsync<Result<List<AppCategoryListModel>>>();

        return result!;
    }

    public async Task<Result<int>> CreateApp(AppCreateRequestModel request)
    {
        var httpClient = _httpclientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var response = await httpClient.PostAsJsonAsync(ApiEndpoints.CreateApp, request);
        var text = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(text);
        }

        var result = await response.Content.ReadFromJsonAsync<Result<int>>();

        return result!;
    }
}

public class ApiEndpoints
{
    public const string AppList = "api/app";
    public const string AppDetail = "api/app/{appId}";
    public const string CreateApp = "api/app";
    public const string AppCategories = "api/app/categories";
}

