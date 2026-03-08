import ollama
from model.client_model import ClientModel


class ReviewService:
    def __init__(self):
        self.model = "gemma:2b"

    def generate_review(self, client_model: ClientModel):
        # 1. Define the system instructions
        system_instruction = (
            "You are a helpful assistant for a small business owner. "
            "Draft a polite and professional reply to the customer review. "
            "Keep it under 3 sentences."
        )

        # 2. Build the message in Gemma's expected chat format
        messages = [
            {
                "role": "system",
                "content": system_instruction
            },
            {
                "role": "user",
                "content": f"Review: {client_model.raw_text}\n"
                           f"Sentiment: {client_model.sentiment}\n"
                           f"Customer Name: {client_model.customer_name}\n"

            }
        ]

        print(messages)

        response = ollama.chat(
            model=self.model,
            messages=messages,
            options={
                "temperature": 0.7,
                "top_k": 50,
                "top_p": 0.95,
                "num_predict": 1024,
            }
        )

        print(response)

        content = response["message"]["content"].strip()

        return content


review_service = ReviewService()
