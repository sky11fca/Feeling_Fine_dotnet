using DotnetApi.Application.Abstractions;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;

namespace DotnetApi.Application.Reviews.Queries;

public class GetReviewQueryHandler(IReviewRepository repository, HttpClient httpClient) : IRequestHandler<GetReviewQuery, List<ReviewDto>>
{
    public async Task<List<ReviewDto>> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        if (request.BusinessId == default)
        {
            throw new ArgumentException("Business ID is required");
        }

        var query = repository.Query().Where(x => x.BusinessId == request.BusinessId);

        if (!string.IsNullOrWhiteSpace(request.RawText) || !string.IsNullOrWhiteSpace(request.SubmitedOn))
        {
            query = query.Where(x =>
                (!string.IsNullOrWhiteSpace(request.RawText) && x.RawText.Contains(request.RawText)) ||
                (!string.IsNullOrWhiteSpace(request.SubmitedOn) && x.SubmitedOn.Contains(request.SubmitedOn)));
        }

        var entities = query.ToList();

        var tasks = entities.Select(async x =>
        {
            var response = await httpClient.PostAsJsonAsync("http://localhost:8000/ai/review", new { raw_text = x.RawText, submitted_on = x.SubmitedOn }, cancellationToken);
            var sentiment = response.IsSuccessStatusCode 
                ? await response.Content.ReadFromJsonAsync<SentimentResult>(cancellationToken: cancellationToken) 
                : null;

            return new ReviewDto(x.Id, x.Rating, x.RatingType.ToString(), x.RawText, x.SubmitedOn, sentiment?.Label ?? "Unknown", sentiment?.Score ?? 0);
        });

        var reviews = await Task.WhenAll(tasks);

        return reviews.ToList();
    }

    private record SentimentResult(string Label, double Score);
}