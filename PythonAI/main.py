from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

import api.review_router
import api.reply_router
import os
from service.satisfaction_service import SatisfactionService

# os.environ["HF_HUB_ENABLE_HF_TRANSFER"] = "1"

# If SatisfactionService is blocking, remove this global instance
# and instantiate it inside the specific router or use lazy loading within the class.
# sentiment_service = SatisfactionService()

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(api.review_router.router)
app.include_router(api.reply_router.router)
