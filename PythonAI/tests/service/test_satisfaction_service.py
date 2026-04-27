import pytest
from unittest.mock import patch, MagicMock
from service.satisfaction_service import SatisfactionService

@pytest.fixture
def mock_ollama_client():
    with patch("service.satisfaction_service.Client") as mock:
        yield mock

def test_initialization(mock_ollama_client):
    """Test that the service initializes the Ollama client."""
    service = SatisfactionService()
    assert service.model_name == "gemma:2b"
    mock_ollama_client.assert_called_once()

def test_analyze_positive(mock_ollama_client):
    """Test that analyze returns POSITIVE for a positive response."""
    # Setup
    mock_client_instance = mock_ollama_client.return_value
    mock_client_instance.chat.return_value = {
        'message': {'content': 'POSITIVE'}
    }

    service = SatisfactionService()
    text = "I am very happy with this service!"

    # Execute
    result = service.analyze(text)

    # Assert
    assert result["label"] == "POSITIVE"
    assert result["score"] == pytest.approx(1.0)

def test_analyze_negative(mock_ollama_client):
    """Test that analyze returns NEGATIVE for a negative response."""
    # Setup
    mock_client_instance = mock_ollama_client.return_value
    mock_client_instance.chat.return_value = {
        'message': {'content': 'NEGATIVE'}
    }

    service = SatisfactionService()
    text = "I am very disappointed."

    # Execute
    result = service.analyze(text)

    # Assert
    assert result["label"] == "NEGATIVE"
    assert result["score"] == pytest.approx(1.0)

def test_analyze_fallback(mock_ollama_client):
    """Test that analyze returns NEUTRAL as fallback."""
    # Setup
    mock_client_instance = mock_ollama_client.return_value
    mock_client_instance.chat.return_value = {
        'message': {'content': 'I am not sure'}
    }

    service = SatisfactionService()
    text = "It is okay."

    # Execute
    result = service.analyze(text)

    # Assert
    assert result["label"] == "NEUTRAL"
    assert result["score"] == pytest.approx(1.0)
