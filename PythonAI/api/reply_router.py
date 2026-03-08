from fastapi import APIRouter, HTTPException, status
from fastapi.concurrency import run_in_threadpool
from pydantic import BaseModel

from model.client_model import ClientModel
from service.review_service import review_service


router = APIRouter()


@router.post("/ai/reply/")
async def generate_a_reply(payload: ClientModel):
    try:
        # Run blocking inference in a threadpool to prevent blocking the event loop
        reply = await run_in_threadpool(review_service.generate_review, payload)
        return {"reply": reply}
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to generate reply: {str(e)}"
        )
