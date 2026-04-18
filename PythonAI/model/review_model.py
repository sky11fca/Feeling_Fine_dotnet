from pydantic import BaseModel
from typing import Optional


class ReviewModel(BaseModel):
    raw_text: str
    submitted_on: str
    rating: Optional[float] = 0.0
    client_id: Optional[str] = ""
    rating_type: Optional[str] = ""
