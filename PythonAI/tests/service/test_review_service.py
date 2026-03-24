import pytest
from unittest.mock import patch

from service.review_service import ReviewService
from model.client_model import ClientModel


@pytest.fixture
def mock_ollama():
    with patch("service.review_service.ollama.chat") as mock:
        yield mock


def test_generate_review(mock_ollama):
    """Test that the review service correctly generates a review using Ollama."""
    service = ReviewService()
    client_model = ClientModel(
        raw_text="The food was amazing!", 
        customer_name="John Doe", 
        sentiment="POSITIVE"
    )
    
    expected_response = {"message": {"content": "Thank you, John Doe!"}}
    mock_ollama.return_value = expected_response
    
    result = service.generate_review(client_model)
    
    assert result == "Thank you, John Doe!"
    mock_ollama.assert_called_once()
    kwargs = mock_ollama.call_args.kwargs
    assert kwargs["model"] == "gemma:2b"
    assert "The food was amazing!" in kwargs["messages"][1]["content"]
    assert "POSITIVE" in kwargs["messages"][1]["content"]
    assert "John Doe" in kwargs["messages"][1]["content"]
