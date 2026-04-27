import pytest
from model.review_model import ReviewModel
from service.statistics_service import StatisticsService

def test_get_ai_statistics_empty():
    service = StatisticsService()
    stats = service.get_ai_statistics([])
    assert stats["ratings"]["labels"] == []
    assert stats["clients"]["labels"] == []
    assert stats["sentiments"]["labels"] == []

def test_get_ai_statistics_with_data():
    service = StatisticsService()
    reviews = [
        ReviewModel(raw_text="Good", submitted_on="2023-01-01", rating=5.0, client_id="c1", rating_type="Type A", sentiment_label="POSITIVE"),
        ReviewModel(raw_text="Bad", submitted_on="2023-01-02", rating=1.0, client_id="c1", rating_type="Type B", sentiment_label="NEGATIVE"),
        ReviewModel(raw_text="Meh", submitted_on="2023-01-03", rating=3.0, client_id="c2", rating_type="Type A", sentiment_label="NEUTRAL"),
        ReviewModel(raw_text="None", submitted_on="2023-01-04"),
    ]
    stats = service.get_ai_statistics(reviews)
    
    assert "Type A" in stats["ratings"]["labels"]
    assert "Type B" in stats["ratings"]["labels"]
    assert "General" in stats["ratings"]["labels"]
    
    assert "c1" in stats["clients"]["labels"]
    assert "c2" in stats["clients"]["labels"]
    assert "Anonymous" in stats["clients"]["labels"]
    
    assert "Positive" in stats["sentiments"]["labels"]
    assert "Negative" in stats["sentiments"]["labels"]
    assert "Neutral" in stats["sentiments"]["labels"]

def test_sentiment_label_mapping():
    service = StatisticsService()
    reviews = [
        ReviewModel(raw_text="1", submitted_on="now", sentiment_label="VERY POSITIVE"),
        ReviewModel(raw_text="2", submitted_on="now", sentiment_label="KIND OF NEGATIVE"),
        ReviewModel(raw_text="3", submitted_on="now", sentiment_label="SOMETHING ELSE"),
    ]
    stats = service.get_ai_statistics(reviews)
    labels = stats["sentiments"]["labels"]
    assert "Positive" in labels
    assert "Negative" in labels
    assert "Neutral" in labels
