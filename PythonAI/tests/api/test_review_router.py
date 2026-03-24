import pytest
from fastapi.testclient import TestClient
from main import app
from unittest.mock import patch


client = TestClient(app)

@pytest.fixture
def mock_satisfaction_service():
    with patch("api.review_router.satisfaction_service.analyze") as mock:
        yield mock

def test_analyse_review(mock_satisfaction_service):
    """Test that analyse_review returns the prediction from the service."""
    mock_satisfaction_service.return_value = {"label": "POSITIVE", "score": 0.998}
    
    response = client.post(
        "/ai/review/",
        json={"raw_text": "I am very happy with this service!", "submitted_on": "2023-10-27"}
    )
    
    assert response.status_code == 200
    assert response.json() == {"label": "POSITIVE", "score": 0.998}
    mock_satisfaction_service.assert_called_once_with("I am very happy with this service!")
