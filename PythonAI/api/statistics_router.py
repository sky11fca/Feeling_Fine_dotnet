from fastapi import APIRouter
from typing import List
from model.review_model import ReviewModel
from service.statistics_service import statistics_service

router = APIRouter()

@router.post("/ai/statistics/")
async def get_statistics(reviews: List[ReviewModel]):
    if not reviews:
        return {
            "ratings": {"labels": [], "data": []},
            "clients": {"labels": [], "data": []},
            "sentiments": {"labels": [], "data": []}
        }

    # Call AI service to generate statistics
    ai_stats = statistics_service.get_ai_statistics(reviews)
    return ai_stats
