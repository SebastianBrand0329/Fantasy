using System.Text;
using System.Text.Json;

namespace Fantasy.Frontend.Repositories;

public class Repository : IRepository
{
	private readonly HttpClient _httpClient;

	private JsonSerializerOptions _jsonDefaulyOptions => new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
	};

	public Repository(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
	{
		var responseHttp = await _httpClient.GetAsync(url);
		if (responseHttp.IsSuccessStatusCode)
		{
			var response = await UnserializedAnswerAsync<T>(responseHttp);
			return new HttpResponseWrapper<T>(response, false, responseHttp);
		}
		return new HttpResponseWrapper<T>(default, true, responseHttp);
	}

	public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
	{
		var messageJson = JsonSerializer.Serialize(model);
		var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
		var responseHttp = await _httpClient.PostAsync(url, messageContent);
		return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
	}

	public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model)
	{
		var messageJson = JsonSerializer.Serialize(model);
		var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
		var responseHttp = await _httpClient.PostAsync(url, messageContent);
		if (responseHttp.IsSuccessStatusCode)
		{
			var response = await UnserializedAnswerAsync<TActionResponse>(responseHttp);
			return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
		}
		return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
	}

	public async Task<HttpResponseWrapper<object>> DeleteAsync<T>(string url)
	{
		var responseHttp = await _httpClient.DeleteAsync(url);
		return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
	}

	public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
	{
		var messageJson = JsonSerializer.Serialize(model);
		var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
		var responseHttp = await _httpClient.PutAsync(url, messageContent);
		return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
	}

	public async Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string url, T model)
	{
		var messageJson = JsonSerializer.Serialize(model);
		var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
		var responseHttp = await _httpClient.PutAsync(url, messageContent);
		if (responseHttp.IsSuccessStatusCode)
		{
			var response = await UnserializedAnswerAsync<TActionResponse>(responseHttp);
			return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
		}
		return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
	}

	private async Task<T> UnserializedAnswerAsync<T>(HttpResponseMessage responseHttp)
	{
		var response = await responseHttp.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<T>(response, _jsonDefaulyOptions)!;
	}
}