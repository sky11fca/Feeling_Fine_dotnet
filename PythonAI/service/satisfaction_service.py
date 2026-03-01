from transformers import pipeline

from core.config import settings


class SatisfactionService:
    def __init__(self):
        model_name = "siebert/sentiment-roberta-large-english"

        self.classifier = pipeline("sentiment-analysis", model=model_name, token=settings.hf_token)

    def analyze(self, text: str):
        return self.classifier(text)[0]


satisfaction_service = SatisfactionService()
