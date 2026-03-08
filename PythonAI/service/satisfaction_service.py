from transformers import pipeline


class SatisfactionService:
    def __init__(self):
        self.model_name = "siebert/sentiment-roberta-large-english"
        self._classifier = None

    @property
    def classifier(self):
        if self._classifier is None:
            print(f"Loading sentiment model '{self.model_name}'...")
            self._classifier = pipeline("sentiment-analysis", model=self.model_name)
        return self._classifier

    def analyze(self, text: str):
        return self.classifier(text)[0]


satisfaction_service = SatisfactionService()
