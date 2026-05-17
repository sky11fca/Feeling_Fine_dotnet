from fastapi import APIRouter, HTTPException
from typing import List
import logging
from model.review_model import ReviewModel
from service.statistics_service import statistics_service

router = APIRouter()
logger = logging.getLogger(__name__)

@router.post("/statistics/")
async def get_statistics(reviews: List[ReviewModel]):
    try:
        if not reviews:
            return {
                "ratings": {"labels": [], "data": []},
                "clients": {"labels": [], "data": []},
                "sentiments": {"labels": [], "data": []}
            }

        # Call AI service to generate statistics
        ai_stats = statistics_service.get_ai_statistics(reviews)
        return ai_stats
    except Exception as e:
        logger.error(f"Error generating statistics: {str(e)}")
        raise HTTPException(status_code=500, detail="Internal server error while generating statistics")
