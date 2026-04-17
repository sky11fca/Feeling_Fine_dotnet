import ollama
import json
from typing import List
from model.review_model import ReviewModel

class StatisticsService:
    def __init__(self):
        self.model = "gemma:2b"

    def get_ai_statistics(self, reviews: List[ReviewModel]):
        # Prepare the review data for the prompt
        reviews_data = []
        for r in reviews:
            reviews_data.append({
                "text": r.raw_text,
                "rating": r.rating,
                "client_id": r.client_id
            })

        system_instruction = (
            "You are a data analyst. Analyze the provided list of reviews and return a JSON object with 3 statistics: "
            "1. 'ratings': Count of reviews per star rating (e.g., '1.0 Stars', '5.0 Stars'). "
            "2. 'clients': Count of reviews per client_id (use truncated IDs). "
            "3. 'sentiments': Count of reviews per sentiment (Positive, Negative, Neutral). "
            "Return ONLY a valid JSON object with the following structure: "
            "{\"ratings\": {\"labels\": [], \"data\": []}, \"clients\": {\"labels\": [], \"data\": []}, \"sentiments\": {\"labels\": [], \"data\": []}}"
        )

        user_content = f"Analyze these reviews: {json.dumps(reviews_data)}"

        messages = [
            {"role": "system", "content": system_instruction},
            {"role": "user", "content": user_content}
        ]

        try:
            response = ollama.chat(
                model=self.model,
                messages=messages,
                format="json", # Ensure Ollama returns JSON
                options={"temperature": 0.1}
            )

            content = response["message"]["content"].strip()
            return json.loads(content)
        except Exception as e:
            print(f"Error calling Ollama: {e}")
            # Fallback to empty structure if AI fails
            return {
                "ratings": {"labels": [], "data": []},
                "clients": {"labels": [], "data": []},
                "sentiments": {"labels": [], "data": []}
            }

statistics_service = StatisticsService()
