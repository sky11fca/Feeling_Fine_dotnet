import os

from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    hf_token: str = os.getenv("HF_TOKEN", "")
    ollama_host: str = os.getenv("OLLAMA_HOST", f"https://{'ollama'}:11434")

    class Config:
        env_file = ".env"


settings = Settings()
