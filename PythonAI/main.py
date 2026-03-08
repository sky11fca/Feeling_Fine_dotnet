from fastapi import FastAPI

import api.review_router
import api.reply_router
import os
from service.satisfaction_service import SatisfactionService

# os.environ["HF_HUB_ENABLE_HF_TRANSFER"] = "1"

# If SatisfactionService is blocking, remove this global instance
# and instantiate it inside the specific router or use lazy loading within the class.
# sentiment_service = SatisfactionService()

app = FastAPI()

app.include_router(api.review_router.router)
app.include_router(api.reply_router.router)
