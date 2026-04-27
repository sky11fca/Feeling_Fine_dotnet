import pytest
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_get_statistics_empty():
    response = client.post("/ai/statistics/", json=[])
    assert response.status_code == 200
    assert response.json()["ratings"]["labels"] == []

def test_get_statistics_with_data():
    reviews = [
        {
            "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "client_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "rating": 5,
            "rating_type": "Good",
            "raw_text": "Excellent service",
            "submitted_on": "2023-10-27",
            "sentiment_label": "Positive",
            "sentiment_accuracy": 0.99
        }
    ]
    response = client.post("/ai/statistics/", json=reviews)
    assert response.status_code == 200
    assert "ratings" in response.json()
    assert len(response.json()["ratings"]["labels"]) > 0
