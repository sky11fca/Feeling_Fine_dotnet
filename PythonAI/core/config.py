import os

from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    hf_token: str = os.getenv("HF_TOKEN", "")

    class Config:
        env_file = ".env"


settings = Settings()
