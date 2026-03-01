from pydantic import BaseModel


class ReviewModel(BaseModel):
    raw_text: str
    submitted_on: str




