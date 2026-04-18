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
                "client_id": r.client_id,
                "type": r.rating_type
            })

        system_instruction = (
            "You are a professional data analyst. Analyze the provided list of reviews and generate aggregated statistics for a dashboard. "
            "CRITICAL: Labels in each category must be UNIQUE. You must SUM the counts for identical categories. "
            "1. 'ratings': Group and count by the 'type' field (the review category/metric). "
            "2. 'clients': Group and count the frequency of each 'client_id'. "
            "3. 'sentiments': Analyze the 'text' and count occurrences of 'Positive', 'Negative', and 'Neutral' sentiments. "
            "Return ONLY a valid JSON object. No duplicates in labels. "
            "Structure: {\"ratings\": {\"labels\": [], \"data\": []}, \"clients\": {\"labels\": [], \"data\": []}, \"sentiments\": {\"labels\": [], \"data\": []}}"
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
            raw_stats = json.loads(content)

            # Post-process to fix LLM hallucinations (floats, duplicates, bogus sentiments)
            clean_stats = {
                "ratings": {"labels": [], "data": []},
                "clients": {"labels": [], "data": []},
                "sentiments": {"labels": [], "data": []}
            }

            # Helper to merge duplicates and fix floats
            def clean_category(source_dict, target_dict, is_sentiment=False):
                merged = {}
                labels = source_dict.get("labels", [])
                data = source_dict.get("data", [])
                
                for i in range(min(len(labels), len(data))):
                    label = str(labels[i]).strip()
                    
                    try:
                        val = int(float(data[i])) # Convert float like 1.0 to int 1
                    except (ValueError, TypeError):
                        val = 0 # Fallback if LLM outputs text instead of number
                    
                    if is_sentiment:
                        # Fix bogus sentiments
                        l_upper = label.upper()
                        if "POS" in l_upper: label = "Positive"
                        elif "NEG" in l_upper: label = "Negative"
                        else: label = "Neutral"
                        
                    merged[label] = merged.get(label, 0) + val
                    
                target_dict["labels"] = list(merged.keys())
                target_dict["data"] = list(merged.values())

            clean_category(raw_stats.get("ratings", {}), clean_stats["ratings"])
            clean_category(raw_stats.get("clients", {}), clean_stats["clients"])
            clean_category(raw_stats.get("sentiments", {}), clean_stats["sentiments"], is_sentiment=True)

            return clean_stats
        except Exception as e:
            print(f"Error calling Ollama: {e}")
            # Fallback to empty structure if AI fails
            return {
                "ratings": {"labels": [], "data": []},
                "clients": {"labels": [], "data": []},
                "sentiments": {"labels": [], "data": []}
            }

statistics_service = StatisticsService()
