using DotnetApi.Application.Abstractions;
using FluentValidation;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace DotnetApi.Application.Reviews.Queries;

public class GetReviewQueryHandler(IReviewRepository repository, HttpClient httpClient, IConfiguration configuration) : IRequestHandler<GetReviewQuery, List<ReviewDto>>
{
    public async Task<List<ReviewDto>> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Query();

        if (!request.BusinessId.Equals(Guid.Empty))
        {
            query = query.Where(x => x.BusinessId.Equals(request.BusinessId));
        }

        if (!string.IsNullOrWhiteSpace(request.RawText))
        {
            query = query.Where(x => x.RawText.Contains(request.RawText));
        }

        if (!string.IsNullOrWhiteSpace(request.SubmitedOn))
        {
            query = query.Where(x => x.SubmittedOn.Contains(request.SubmitedOn));
        }

        var entities = query.ToList();
        
        var pythonAiUrl = configuration["PythonAiSettings:ReviewUrl"];

        var tasks = entities.Select(async x =>
        {
            SentimentAnalysisResult? sentiment = null;
            try
            {
                var response = await httpClient.PostAsJsonAsync(pythonAiUrl, new { raw_text = x.RawText, submitted_on = x.SubmittedOn }, cancellationToken);
                sentiment = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<SentimentAnalysisResult>(cancellationToken: cancellationToken) 
                    : null;
            }
            catch (HttpRequestException)
            {
                // Fallback to null sentiment if the Python AI service is unreachable
            }

            return new ReviewDto(x.Id, x.ClientId, x.Rating, x.RatingType.ToString(), x.RawText, x.SubmittedOn, sentiment?.Label ?? "Unknown", sentiment?.Score ?? 0);
        });

        var reviews = await Task.WhenAll(tasks);

        return reviews.ToList();
    }
    
}