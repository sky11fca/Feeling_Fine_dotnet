import os
from ollama import Client


class SatisfactionService:
    def __init__(self):
        # Feel free to change this to 'mistral' or 'mixtral' depending on what you pulled
        self.model_name = "gemma:2b" 
        
        # Use host.docker.internal to break out of the container and hit the host machine.
        # We wrap it in os.getenv so you can override it in production.
        ollama_url = os.getenv("OLLAMA_HOST", "http://ollama:11434")
        self.client = Client(host=ollama_url)

    def analyze(self, text: str):
        prompt = f"""
        Analyze the sentiment of the following text.
        Respond with EXACTLY ONE word: "POSITIVE", "NEGATIVE", or "NEUTRAL".
        Do not include any explanations, context, or punctuation.

        Text: "{text}"

        Sentiment:
        """
        
        response = self.client.chat(
            model=self.model_name,
            messages=[{'role': 'user', 'content': prompt}],
            options={"temperature": 0.0}  # Force deterministic, less chatty output
        )
        
        raw_content = response['message']['content'].upper()
        
        # Robust parsing: actively search for the keyword in the response
        if "POSITIVE" in raw_content:
            sentiment = "POSITIVE"
        elif "NEGATIVE" in raw_content:
            sentiment = "NEGATIVE"
        else:
            sentiment = "NEUTRAL"  # Fallback if it outputs something entirely unexpected
        
        # Emulate the HuggingFace pipeline return format to avoid breaking downstream code
        return {"label": sentiment, "score": 1.0}


satisfaction_service = SatisfactionService()
