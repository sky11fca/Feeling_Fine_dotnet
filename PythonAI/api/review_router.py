from fastapi import APIRouter

from model.review_model import ReviewModel
from service.satisfaction_service import satisfaction_service

router = APIRouter()


@router.post("/ai/review/")
async def analyse_review(payload: ReviewModel):
    prediction = satisfaction_service.analyze(payload.raw_text)
    return prediction
