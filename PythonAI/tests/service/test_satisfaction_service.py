import unittest
from unittest.mock import patch, MagicMock

from service.satisfaction_service import SatisfactionService


@patch("service.satisfaction_service.pipeline")
class TestSatisfactionService(unittest.TestCase):
    def test_initialization(self, mock_pipeline):
        """Test that the service initializes the pipeline with the correct model."""
        # Import inside the test to ensure patch is active if module loads here

        SatisfactionService()

        mock_pipeline.assert_called_with(
            "sentiment-analysis", model="siebert/sentiment-roberta-large-english"
        )

    def test_analyze(self, mock_pipeline):
        """Test that analyze returns the correct result from the classifier."""
        # Setup
        mock_classifier = MagicMock()
        expected_result = {"label": "POSITIVE", "score": 0.998}
        # The classifier returns a list, and the service takes the first element
        mock_classifier.return_value = [expected_result]
        mock_pipeline.return_value = mock_classifier

        service = SatisfactionService()
        text = "I am very happy with this service!"

        # Execute
        result = service.analyze(text)

        # Assert
        mock_classifier.assert_called_once_with(text)
        self.assertEqual(result, expected_result)


if __name__ == "__main__":
    unittest.main()
    