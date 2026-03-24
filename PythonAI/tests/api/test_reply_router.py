import pytest
from fastapi.testclient import TestClient
from main import app
from unittest.mock import patch

client = TestClient(app)

@pytest.fixture
def mock_review_service():
    with patch("api.reply_router.review_service.generate_review") as mock:
        yield mock

def test_generate_a_reply_success(mock_review_service):
    """Test successful reply generation."""
    mock_review_service.return_value = "Thank you for your feedback!"
    
    response = client.post(
        "/ai/reply/",
        json={"raw_text": "Great service!", "customer_name": "Alice", "sentiment": "POSITIVE"}
    )
    
    assert response.status_code == 200
    assert response.json() == {"reply": "Thank you for your feedback!"}
    mock_review_service.assert_called_once()
    
def test_generate_a_reply_failure(mock_review_service):
    """Test reply generation failure handles exception."""
    mock_review_service.side_effect = Exception("Service unavailable")
    
    response = client.post(
        "/ai/reply/",
        json={"raw_text": "Great service!", "customer_name": "Alice", "sentiment": "POSITIVE"}
    )
    
    assert response.status_code == 500
    assert "Failed to generate reply: Service unavailable" in response.json()["detail"]
