from typing import List
from model.review_model import ReviewModel
from collections import Counter

class StatisticsService:
    def __init__(self):
        # No longer using Ollama for statistics
        pass

    def get_ai_statistics(self, reviews: List[ReviewModel]):
        if not reviews:
            return {
                "ratings": {"labels": [], "data": []},
                "clients": {"labels": [], "data": []},
                "sentiments": {"labels": [], "data": []}
            }

        # 1. Group by Review Type (Category)
        types = [r.rating_type if r.rating_type else "General" for r in reviews]
        type_counts = Counter(types)
        
        # 2. Group by Client ID
        clients = [r.client_id if r.client_id else "Anonymous" for r in reviews]
        client_counts = Counter(clients)
        
        # 3. Group by Sentiment Label
        sentiments = []
        for r in reviews:
            label = r.sentiment_label.strip() if r.sentiment_label else "Neutral"
            l_upper = label.upper()
            if "POS" in l_upper: clean_label = "Positive"
            elif "NEG" in l_upper: clean_label = "Negative"
            else: clean_label = "Neutral"
            sentiments.append(clean_label)
        
        sentiment_counts = Counter(sentiments)

        # Build Response
        return {
            "ratings": {
                "labels": list(type_counts.keys()),
                "data": list(type_counts.values())
            },
            "clients": {
                "labels": list(client_counts.keys()),
                "data": list(client_counts.values())
            },
            "sentiments": {
                "labels": list(sentiment_counts.keys()),
                "data": list(sentiment_counts.values())
            }
        }

statistics_service = StatisticsService()
