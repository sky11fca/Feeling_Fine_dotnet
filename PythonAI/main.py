from fastapi import FastAPI

import api.review_router
from service.satisfaction_service import SatisfactionService

sentiment_service = SatisfactionService()

app = FastAPI()

app.include_router(api.review_router.router)


