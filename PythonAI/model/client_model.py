from pydantic import BaseModel


class ClientModel(BaseModel):
    raw_text: str
    customer_name: str
    sentiment: str
